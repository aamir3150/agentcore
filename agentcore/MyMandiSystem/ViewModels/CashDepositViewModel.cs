using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Core.Enums;
using MyMandiSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class CashDepositViewModel : ObservableObject
{
    private readonly IBankingService _bankingService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;
    private readonly MyMandiSystem.Core.Printing.IPrintService _printService;
    private readonly MyMandiSystem.Core.Printing.IPrintModelFactory _printModelFactory;

    // Header & Navigation
    [ObservableProperty] private string _voucherNo = "1";
    [ObservableProperty] private DateTime _voucherDate = DateTime.Today;
    [ObservableProperty] private string _branchName = "Main Branch";
    [ObservableProperty] private CropSeason? _selectedSeason;
    [ObservableProperty] private ObservableCollection<CropSeason> _seasons = new();
    [ObservableProperty] private string _searchVoucherNo = string.Empty;
    [ObservableProperty] private bool _isExistingVoucher = false;

    // Real-Time Balances (AgenSoft Parity: TxtCashInHand & TxtAccountBal)
    [ObservableProperty] private string _cashInHand = "0";
    [ObservableProperty] private string _bankBalance = "0";
    [ObservableProperty] private string _bankBalanceStatus = string.Empty;

    // Bank Account Selection (AgenSoft txtBankAccountno & txtBankAccountName)
    [ObservableProperty] private string _bankAccountNo = string.Empty;
    [ObservableProperty] private string _bankAccountName = string.Empty;
    [ObservableProperty] private ObservableCollection<BankAccount> _bankAccounts = new();
    [ObservableProperty] private BankAccount? _selectedBank;
    private int _bankAccountId = 0;

    // Cash Deposit Particulars (AgenSoft frmCashDeposit Controls)
    [ObservableProperty] private string _slipNo = string.Empty;
    [ObservableProperty] private DateTime _depositedDate = DateTime.Today;
    [ObservableProperty] private string _depositedBy = string.Empty;

    // Checkboxes (AgenSoft Flags)
    [ObservableProperty] private bool _isReconciled = false;

    // Amount & Narration
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private string _amountInWords = string.Empty;
    [ObservableProperty] private string _narration = string.Empty;
    [ObservableProperty] private string _narrationUrdu = string.Empty;

    // F1 Bank Lookup Modal
    [ObservableProperty] private bool _isAccountLookupOpen = false;
    [ObservableProperty] private string _lookupTitle = "Select Bank Account";
    [ObservableProperty] private string _lookupSearchQuery = string.Empty;
    [ObservableProperty] private ObservableCollection<Account> _filteredAccounts = new();
    [ObservableProperty] private Account? _selectedLookupAccount;
    private List<Account> _allAccounts = new();

    // Search Bank Cash Deposits (AgenSoft SchBankDeposits Parity)
    [ObservableProperty] private bool _isSearchListOpen = false;
    [ObservableProperty] private string _searchListQuery = string.Empty;
    [ObservableProperty] private ObservableCollection<ChequeDepositDto> _searchListResults = new();
    [ObservableProperty] private ChequeDepositDto? _selectedSearchItem;

    public Action? RequestClose { get; set; }

    public CashDepositViewModel(
        IBankingService bankingService,
        IAccountService accountService,
        ISystemService systemService,
        MyMandiSystem.Core.Printing.IPrintService printService,
        MyMandiSystem.Core.Printing.IPrintModelFactory printModelFactory)
    {
        _bankingService = bankingService;
        _accountService = accountService;
        _systemService = systemService;
        _printService = printService;
        _printModelFactory = printModelFactory;

        LoadInitialData();
    }

    public async void LoadInitialData()
    {
        try
        {
            var seasonsList = await _systemService.GetActiveSeasonsAsync();
            Seasons = new ObservableCollection<CropSeason>(seasonsList);
            SelectedSeason = Seasons.FirstOrDefault(s => s.IsActive) ?? Seasons.FirstOrDefault();

            _allAccounts = (await _accountService.GetAllAccountsAsync()).Where(a => !a.IsDeleted).ToList();

            var bankList = await _bankingService.GetAllBankAccountsAsync();
            BankAccounts = new ObservableCollection<BankAccount>(bankList);
            if (BankAccounts.Any())
            {
                SelectedBank = BankAccounts.First();
            }

            await LoadCashInHandAsync();
            await NewVoucherAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading initial data: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private async Task LoadCashInHandAsync()
    {
        try
        {
            var cashAc = _allAccounts.FirstOrDefault(a => a.AccountNo == "1001" || a.Name.Contains("Cash") || a.Name.Contains("روکڑ"));
            if (cashAc != null)
            {
                var bal = await _accountService.GetAccountBalanceAsync(cashAc.Id);
                CashInHand = bal.ToString("N0");
            }
        }
        catch { }
    }

    partial void OnSelectedBankChanged(BankAccount? value)
    {
        if (value != null)
        {
            _bankAccountId = value.Id;
            BankAccountNo = value.Account?.AccountNo ?? value.AccountNumber;
            BankAccountName = value.BankName ?? value.Account?.Name ?? string.Empty;
            _ = UpdateBankBalanceAsync(value.AccountId);
        }
        else
        {
            _bankAccountId = 0;
            BankAccountNo = string.Empty;
            BankAccountName = string.Empty;
            BankBalance = "0";
            BankBalanceStatus = string.Empty;
        }
    }

    public async Task UpdateBankBalanceAsync(int accountId)
    {
        try
        {
            var bal = await _accountService.GetAccountBalanceAsync(accountId);
            BankBalance = Math.Abs(bal).ToString("N0");
            BankBalanceStatus = bal >= 0 ? "Dr" : "Cr";
        }
        catch
        {
            BankBalance = "0";
            BankBalanceStatus = string.Empty;
        }
    }

    partial void OnAmountChanged(decimal value)
    {
        if (value > 0)
        {
            AmountInWords = ConvertAmountToWords((long)value) + " Rupees Only";
        }
        else
        {
            AmountInWords = string.Empty;
        }
    }

    private string ConvertAmountToWords(long number)
    {
        if (number == 0) return "Zero";
        if (number < 0) return "Minus " + ConvertAmountToWords(Math.Abs(number));

        string words = "";

        if ((number / 10000000) > 0)
        {
            words += ConvertAmountToWords(number / 10000000) + " Crore ";
            number %= 10000000;
        }

        if ((number / 100000) > 0)
        {
            words += ConvertAmountToWords(number / 100000) + " Lakh ";
            number %= 100000;
        }

        if ((number / 1000) > 0)
        {
            words += ConvertAmountToWords(number / 1000) + " Thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += ConvertAmountToWords(number / 100) + " Hundred ";
            number %= 100;
        }

        if (number > 0)
        {
            var unitsMap = new[] { "Zero", "One", "Two", "Three", "Four", "Five", "Six", "Seven", "Eight", "Nine", "Ten", "Eleven", "Twelve", "Thirteen", "Fourteen", "Fifteen", "Sixteen", "Seventeen", "Eighteen", "Nineteen" };
            var tensMap = new[] { "Zero", "Ten", "Twenty", "Thirty", "Forty", "Fifty", "Sixty", "Seventy", "Eighty", "Ninety" };

            if (number < 20)
                words += unitsMap[number];
            else
            {
                words += tensMap[number / 10];
                if ((number % 10) > 0)
                    words += " " + unitsMap[number % 10];
            }
        }

        return words.Trim();
    }

    [RelayCommand]
    public async Task NewVoucherAsync()
    {
        var nextNo = await _bankingService.GetNextChequeDepositVoucherNoAsync();
        VoucherNo = nextNo.ToString();
        VoucherDate = DateTime.Today;
        IsExistingVoucher = false;

        SlipNo = string.Empty;
        DepositedDate = DateTime.Today;
        DepositedBy = string.Empty;

        IsReconciled = false;

        Amount = 0;
        AmountInWords = string.Empty;
        Narration = string.Empty;
        NarrationUrdu = string.Empty;

        await LoadCashInHandAsync();
        if (SelectedBank != null)
        {
            await UpdateBankBalanceAsync(SelectedBank.AccountId);
        }
    }

    [RelayCommand]
    public void OpenBankLookup()
    {
        LookupTitle = "Select Bank Account";
        LookupSearchQuery = string.Empty;
        var bankAcNos = BankAccounts.Select(b => b.Account?.AccountNo ?? b.AccountNumber).ToHashSet();
        FilteredAccounts = new ObservableCollection<Account>(
            _allAccounts.Where(a => bankAcNos.Contains(a.AccountNo) || a.Name.Contains("Bank") || a.Name.Contains("بینک")));
        IsAccountLookupOpen = true;
    }

    partial void OnLookupSearchQueryChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            var bankAcNos = BankAccounts.Select(b => b.Account?.AccountNo ?? b.AccountNumber).ToHashSet();
            FilteredAccounts = new ObservableCollection<Account>(
                _allAccounts.Where(a => bankAcNos.Contains(a.AccountNo) || a.Name.Contains("Bank") || a.Name.Contains("بینک")));
        }
        else
        {
            var q = value.ToLower().Trim();
            var baseList = _allAccounts.Where(a => a.Name.Contains("Bank") || a.Name.Contains("بینک"));
            FilteredAccounts = new ObservableCollection<Account>(
                baseList.Where(a => (a.AccountNo != null && a.AccountNo.ToLower().Contains(q)) ||
                                    (a.Name != null && a.Name.ToLower().Contains(q)) ||
                                    (a.UrduName != null && a.UrduName.Contains(q))));
        }
    }

    [RelayCommand]
    public void SelectLookupAccount(Account? account)
    {
        if (account == null) return;

        var matchingBank = BankAccounts.FirstOrDefault(b => b.AccountId == account.Id || b.AccountNumber == account.AccountNo);
        if (matchingBank != null)
        {
            SelectedBank = matchingBank;
        }
        else
        {
            BankAccountNo = account.AccountNo;
            BankAccountName = account.Name;
            _ = UpdateBankBalanceAsync(account.Id);
        }

        IsAccountLookupOpen = false;
    }

    [RelayCommand]
    public void CloseLookup()
    {
        IsAccountLookupOpen = false;
    }

    public void OnBankAccountNoEntered()
    {
        if (string.IsNullOrWhiteSpace(BankAccountNo)) return;

        var matchingBank = BankAccounts.FirstOrDefault(b => (b.Account != null && b.Account.AccountNo == BankAccountNo.Trim()) || b.AccountNumber == BankAccountNo.Trim());
        if (matchingBank != null)
        {
            SelectedBank = matchingBank;
        }
        else
        {
            var ac = _allAccounts.FirstOrDefault(a => a.AccountNo == BankAccountNo.Trim());
            if (ac != null)
            {
                SelectLookupAccount(ac);
            }
            else
            {
                OpenBankLookup();
                LookupSearchQuery = BankAccountNo.Trim();
            }
        }
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (!long.TryParse(VoucherNo, out var vNo) || vNo <= 0)
        {
            MessageBox.Show("Please enter a valid numeric voucher number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrEmpty(BankAccountNo))
        {
            MessageBox.Show("Please select a bank account.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Amount <= 0)
        {
            MessageBox.Show("Please enter deposit amount greater than zero.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var dto = new ChequeDepositDto
            {
                VoucherNo = vNo,
                VoucherDate = VoucherDate,
                IsCheque = false,
                ChequeNo = string.Empty,
                ChequeDate = VoucherDate,
                SlipNo = SlipNo.Trim(),
                DepositedDate = DepositedDate,
                IsReconciled = IsReconciled,
                ReconcileDate = IsReconciled ? DepositedDate : null,
                BankAccountId = _bankAccountId,
                BankAccountNo = BankAccountNo,
                BankAccountName = BankAccountName,
                PartyAccountId = null,
                PartyAccountNo = "1001",
                PartyAccountName = "Cash in Hand",
                Amount = Amount,
                DepositedBy = DepositedBy,
                ReceivedBy = string.Empty,
                Narration = Narration,
                NarrationUrdu = NarrationUrdu,
                IsDepositConfirmed = true,
                SendSms = false,
                CropSeasonId = SelectedSeason?.Id,
                CropSeason = SelectedSeason?.Name ?? string.Empty,
                BranchName = BranchName
            };

            await _bankingService.SaveChequeDepositAsync(dto);
            MessageBox.Show($"Bank cash deposit voucher #{vNo} saved successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            await LoadCashInHandAsync();
            if (SelectedBank != null)
            {
                await UpdateBankBalanceAsync(SelectedBank.AccountId);
            }
            await NewVoucherAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving cash deposit: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public async Task LoadVoucherAsync(long vNo)
    {
        try
        {
            var dto = await _bankingService.LoadChequeDepositByVoucherNoAsync(vNo);
            if (dto == null)
            {
                MessageBox.Show($"Voucher #{vNo} not found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            VoucherNo = dto.VoucherNo.ToString();
            VoucherDate = dto.VoucherDate;
            IsExistingVoucher = true;

            var matchingBank = BankAccounts.FirstOrDefault(b => (b.Account != null && b.Account.AccountNo == dto.BankAccountNo) || b.AccountNumber == dto.BankAccountNo || b.Id == dto.BankAccountId);
            if (matchingBank != null)
            {
                SelectedBank = matchingBank;
            }
            else
            {
                BankAccountNo = dto.BankAccountNo;
                BankAccountName = dto.BankAccountName;
            }

            SlipNo = dto.SlipNo;
            DepositedDate = dto.DepositedDate;
            DepositedBy = dto.DepositedBy;

            IsReconciled = dto.IsReconciled;

            Amount = dto.Amount;
            Narration = dto.Narration;
            NarrationUrdu = dto.NarrationUrdu;

            if (!string.IsNullOrEmpty(dto.CropSeason))
            {
                SelectedSeason = Seasons.FirstOrDefault(s => s.Name == dto.CropSeason) ?? SelectedSeason;
            }

            await LoadCashInHandAsync();
            if (SelectedBank != null)
            {
                await UpdateBankBalanceAsync(SelectedBank.AccountId);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading voucher #{vNo}: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public async Task SearchVoucherAsync()
    {
        if (long.TryParse(SearchVoucherNo, out var vNo))
        {
            await LoadVoucherAsync(vNo);
            SearchVoucherNo = string.Empty;
        }
        else
        {
            MessageBox.Show("Please enter a valid numeric voucher number.", "Notice", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    public async Task PreviousVoucherAsync()
    {
        if (long.TryParse(VoucherNo, out var current) && current > 1)
        {
            await LoadVoucherAsync(current - 1);
        }
    }

    [RelayCommand]
    public async Task NextVoucherAsync()
    {
        if (long.TryParse(VoucherNo, out var current))
        {
            await LoadVoucherAsync(current + 1);
        }
    }

    [RelayCommand]
    public async Task DeleteVoucherAsync()
    {
        if (!IsExistingVoucher || !long.TryParse(VoucherNo, out var vNo))
        {
            MessageBox.Show("Please load an existing voucher first before deleting.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirm = MessageBox.Show($"Are you sure you want to delete Cash Deposit Voucher #{vNo}?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm == MessageBoxResult.Yes)
        {
            try
            {
                await _bankingService.DeleteChequeDepositAsync(vNo);
                MessageBox.Show($"Voucher #{vNo} deleted successfully.", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                await NewVoucherAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting voucher: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    public async Task OpenSearchModalAsync()
    {
        IsSearchListOpen = true;
        SearchListQuery = string.Empty;
        var list = await _bankingService.SearchChequeDepositsAsync(isCheque: false);
        SearchListResults = new ObservableCollection<ChequeDepositDto>(list);
    }

    partial void OnSearchListQueryChanged(string value)
    {
        _ = FilterSearchListAsync(value);
    }

    private async Task FilterSearchListAsync(string query)
    {
        var list = await _bankingService.SearchChequeDepositsAsync(query, isCheque: false);
        SearchListResults = new ObservableCollection<ChequeDepositDto>(list);
    }

    [RelayCommand]
    public async Task SelectSearchItemAsync(ChequeDepositDto? item)
    {
        if (item == null) return;
        IsSearchListOpen = false;
        await LoadVoucherAsync(item.VoucherNo);
    }

    [RelayCommand]
    public void CloseSearchModal()
    {
        IsSearchListOpen = false;
    }

    [RelayCommand]
    public void Print()
    {
        try
        {
            MessageBox.Show($"Deposit Slip for Cash Voucher #{VoucherNo} sent to printer / preview.", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Print failed: {ex.Message}", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public void Close()
    {
        RequestClose?.Invoke();
    }
}

