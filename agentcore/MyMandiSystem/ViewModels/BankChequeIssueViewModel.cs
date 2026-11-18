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

public partial class BankChequeBodyViewModel : ObservableObject
{
    [ObservableProperty] private string _accountNo = string.Empty;
    [ObservableProperty] private string _accountName = string.Empty;
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private string _narration = string.Empty;
}

public partial class BankChequeIssueViewModel : ObservableObject
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

    // Real-Time Balances (AgenSoft Parity)
    [ObservableProperty] private string _cashInHand = "0";
    [ObservableProperty] private string _accountBalance = "0";
    [ObservableProperty] private string _accountBalanceStatus = string.Empty;
    [ObservableProperty] private string _bankBalance = "0";
    [ObservableProperty] private string _bankBalanceStatus = string.Empty;

    // Bank Account Selection (AgenSoft txtBankAccountno & txtBankAccountName)
    [ObservableProperty] private string _bankAccountNo = string.Empty;
    [ObservableProperty] private string _bankAccountName = string.Empty;
    [ObservableProperty] private ObservableCollection<BankAccount> _bankAccounts = new();
    [ObservableProperty] private BankAccount? _selectedBank;
    private int _bankAccountId = 0;

    // Cheque Leaf Selection (Populated from BankChequeBookHistory for selected bank)
    [ObservableProperty] private ObservableCollection<string> _availableChequeNumbers = new();
    [ObservableProperty] private string _selectedChequeNo = string.Empty;
    [ObservableProperty] private string _chequeNo = string.Empty;
    [ObservableProperty] private DateTime _chequeDate = DateTime.Today;
    [ObservableProperty] private bool _isCrossCheque = true;
    [ObservableProperty] private bool _isReconciled = false;
    [ObservableProperty] private DateTime _reconcileDate = DateTime.Today;

    // Primary Payee Account (AgenSoft txtPayTo & txtAccountName)
    [ObservableProperty] private string _partyAccountNo = string.Empty;
    [ObservableProperty] private string _partyAccountName = string.Empty;
    [ObservableProperty] private string _partyUrduName = string.Empty;
    [ObservableProperty] private string _partyCity = string.Empty;
    [ObservableProperty] private bool _hasSelectedParty = false;
    private int? _partyAccountId;

    // Received By (Physical cheque recipient name)
    [ObservableProperty] private string _receivedBy = string.Empty;

    // Amount & Narration
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private string _amountInWords = string.Empty;
    [ObservableProperty] private string _narration = string.Empty;
    [ObservableProperty] private string _narrationUrdu = string.Empty;

    // Withholding Tax (WHT) Options (AgenSoft TxtWHTax, TxtWHTRatio, OptMinus/OptPlus, chkChargToBank, chkChargToPayees)
    [ObservableProperty] private bool _isWhtMode = false;
    [ObservableProperty] private decimal _whtRatio = 0;
    [ObservableProperty] private decimal _whtAmount = 0;
    [ObservableProperty] private bool _isWhtMinus = true; // Minus WHT from payment
    [ObservableProperty] private bool _isWhtPlus = false;  // Add WHT to payment
    [ObservableProperty] private bool _whtToPayee = true;
    [ObservableProperty] private bool _whtToBank = false;
    [ObservableProperty] private decimal _netChequeAmount = 0;

    // Multi-Payee / Split Details (AgenSoft Grid & BankChequesBody)
    [ObservableProperty] private bool _isMultiPayeeMode = false;
    [ObservableProperty] private ObservableCollection<BankChequeBodyViewModel> _splitDetails = new();
    [ObservableProperty] private string _splitAccountNo = string.Empty;
    [ObservableProperty] private string _splitAccountName = string.Empty;
    [ObservableProperty] private decimal _splitAmount;
    [ObservableProperty] private string _splitNarration = string.Empty;
    [ObservableProperty] private decimal _splitTotalAmount = 0;

    // Checkboxes
    [ObservableProperty] private bool _sendSms = false;

    // F1 Account Lookup Modal
    [ObservableProperty] private bool _isAccountLookupOpen = false;
    [ObservableProperty] private bool _isLookupForBank = false;
    [ObservableProperty] private bool _isLookupForSplit = false;
    [ObservableProperty] private string _lookupTitle = "Account Lookup";
    [ObservableProperty] private string _lookupSearchQuery = string.Empty;
    [ObservableProperty] private ObservableCollection<Account> _filteredAccounts = new();
    [ObservableProperty] private Account? _selectedLookupAccount;
    private List<Account> _allAccounts = new();

    // Search Bank Cheques (AgenSoft SchBankCheques Parity)
    [ObservableProperty] private bool _isSearchListOpen = false;
    [ObservableProperty] private string _searchListQuery = string.Empty;
    [ObservableProperty] private ObservableCollection<BankChequeDto> _searchListResults = new();
    [ObservableProperty] private BankChequeDto? _selectedSearchItem;

    public Action? RequestClose { get; set; }

    public BankChequeIssueViewModel(
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

        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        try
        {
            // Load Crop Seasons
            var seasonsList = await _systemService.GetActiveSeasonsAsync();
            Seasons = new ObservableCollection<CropSeason>(seasonsList);
            SelectedSeason = Seasons.FirstOrDefault(s => s.IsActive) ?? Seasons.FirstOrDefault();

            // Load Bank Accounts
            var bList = (await _bankingService.GetAllBankAccountsAsync()).ToList();
            BankAccounts = new ObservableCollection<BankAccount>(bList);

            // Load All Accounts for Lookup
            _allAccounts = (await _accountService.GetAllAccountsAsync()).Where(a => !a.IsDeleted).ToList();
            FilteredAccounts = new ObservableCollection<Account>(_allAccounts);

            // Load Cash in Hand
            await LoadCashInHandAsync();

            // Next Voucher Number
            await NewVoucherAsync();

            if (BankAccounts.Any())
            {
                SelectedBank = BankAccounts.First();
            }
        }
        catch { }
    }

    public async Task LoadCashInHandAsync()
    {
        try
        {
            var cashAccounts = _allAccounts.Where(a => a.AccountNo == "1001" || a.Name.Contains("Cash") || a.Name.Contains("روکڑ"));
            decimal totalCash = 0;
            foreach (var ca in cashAccounts)
            {
                totalCash += await _accountService.GetAccountBalanceAsync(ca.Id);
            }
            CashInHand = totalCash.ToString("N0");
        }
        catch
        {
            CashInHand = "0";
        }
    }

    partial void OnSelectedBankChanged(BankAccount? value)
    {
        if (value != null)
        {
            _bankAccountId = value.Id;
            BankAccountNo = value.Account?.AccountNo ?? value.AccountNumber;
            BankAccountName = value.BankName;
            _ = UpdateBankBalanceAsync(value.AccountId);
            _ = RefreshAvailableChequeNumbersAsync(BankAccountNo);
        }
        else
        {
            _bankAccountId = 0;
            BankAccountNo = string.Empty;
            BankAccountName = string.Empty;
            BankBalance = "0";
            BankBalanceStatus = string.Empty;
            AvailableChequeNumbers.Clear();
        }
    }

    public async Task RefreshAvailableChequeNumbersAsync(string bankAcNo)
    {
        if (string.IsNullOrWhiteSpace(bankAcNo)) return;
        try
        {
            long currentVNo = 0;
            long.TryParse(VoucherNo, out currentVNo);
            var list = await _bankingService.GetAvailableChequeNumbersForBankAsync(bankAcNo, currentVNo);
            AvailableChequeNumbers = new ObservableCollection<string>(list);
            if (AvailableChequeNumbers.Any() && string.IsNullOrWhiteSpace(ChequeNo))
            {
                SelectedChequeNo = AvailableChequeNumbers.First();
            }
        }
        catch { }
    }

    partial void OnSelectedChequeNoChanged(string value)
    {
        if (!string.IsNullOrWhiteSpace(value))
        {
            ChequeNo = value;
        }
    }

    public async Task UpdateBankBalanceAsync(int accountId)
    {
        try
        {
            var bal = await _accountService.GetAccountBalanceAsync(accountId);
            BankBalance = Math.Abs(bal).ToString("N0");
            BankBalanceStatus = bal >= 0 ? "Debit (Dr)" : "Credit (Cr)";
        }
        catch
        {
            BankBalance = "0";
            BankBalanceStatus = string.Empty;
        }
    }

    partial void OnAmountChanged(decimal value)
    {
        CalculateWht();
        if (value > 0)
        {
            AmountInWords = ConvertNumberToWords((long)value) + " Rupees Only";
        }
        else
        {
            AmountInWords = string.Empty;
        }
    }

    partial void OnWhtRatioChanged(decimal value)
    {
        if (Amount > 0 && value >= 0)
        {
            WhtAmount = Math.Round((Amount * value) / 100m, 2);
        }
        CalculateWht();
    }

    partial void OnWhtAmountChanged(decimal value)
    {
        CalculateWht();
    }

    partial void OnIsWhtMinusChanged(bool value)
    {
        IsWhtPlus = !value;
        CalculateWht();
    }

    partial void OnIsWhtPlusChanged(bool value)
    {
        IsWhtMinus = !value;
        CalculateWht();
    }

    private void CalculateWht()
    {
        if (!IsWhtMode || WhtAmount == 0)
        {
            NetChequeAmount = Amount;
            return;
        }

        if (IsWhtMinus)
        {
            NetChequeAmount = Math.Max(0, Amount - WhtAmount);
        }
        else
        {
            NetChequeAmount = Amount + WhtAmount;
        }
    }

    public static string ConvertNumberToWords(long number)
    {
        if (number == 0) return "Zero";
        if (number < 0) return "Minus " + ConvertNumberToWords(Math.Abs(number));

        string words = "";

        if ((number / 10000000) > 0)
        {
            words += ConvertNumberToWords(number / 10000000) + " Crore ";
            number %= 10000000;
        }

        if ((number / 100000) > 0)
        {
            words += ConvertNumberToWords(number / 100000) + " Lakh ";
            number %= 100000;
        }

        if ((number / 1000) > 0)
        {
            words += ConvertNumberToWords(number / 1000) + " Thousand ";
            number %= 1000;
        }

        if ((number / 100) > 0)
        {
            words += ConvertNumberToWords(number / 100) + " Hundred ";
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
        var nextNo = await _bankingService.GetNextBankChequeVoucherNoAsync();
        VoucherNo = nextNo.ToString();
        VoucherDate = DateTime.Today;
        IsExistingVoucher = false;

        PartyAccountNo = string.Empty;
        PartyAccountName = string.Empty;
        PartyUrduName = string.Empty;
        PartyCity = string.Empty;
        HasSelectedParty = false;
        _partyAccountId = null;
        AccountBalance = "0";
        AccountBalanceStatus = string.Empty;

        ChequeNo = string.Empty;
        SelectedChequeNo = string.Empty;
        ChequeDate = DateTime.Today;
        IsCrossCheque = true;
        IsReconciled = false;
        ReconcileDate = DateTime.Today;
        ReceivedBy = string.Empty;

        Amount = 0;
        AmountInWords = string.Empty;
        Narration = string.Empty;
        NarrationUrdu = string.Empty;

        IsWhtMode = false;
        WhtRatio = 0;
        WhtAmount = 0;
        IsWhtMinus = true;
        IsWhtPlus = false;
        WhtToPayee = true;
        WhtToBank = false;
        NetChequeAmount = 0;

        IsMultiPayeeMode = false;
        SplitDetails.Clear();
        SplitAccountNo = string.Empty;
        SplitAccountName = string.Empty;
        SplitAmount = 0;
        SplitNarration = string.Empty;
        SplitTotalAmount = 0;

        SendSms = false;

        await LoadCashInHandAsync();
        if (!string.IsNullOrEmpty(BankAccountNo))
        {
            await RefreshAvailableChequeNumbersAsync(BankAccountNo);
        }
    }

    // Split Details Commands (Multi-Payee Mode)
    [RelayCommand]
    public void AddSplitDetail()
    {
        if (string.IsNullOrWhiteSpace(SplitAccountNo) || SplitAmount <= 0)
        {
            MessageBox.Show("Please specify a valid Account Code and Amount greater than zero.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        SplitDetails.Add(new BankChequeBodyViewModel
        {
            AccountNo = SplitAccountNo.Trim(),
            AccountName = SplitAccountName,
            Amount = SplitAmount,
            Narration = SplitNarration
        });

        SplitTotalAmount = SplitDetails.Sum(s => s.Amount);
        Amount = SplitTotalAmount;

        SplitAccountNo = string.Empty;
        SplitAccountName = string.Empty;
        SplitAmount = 0;
        SplitNarration = string.Empty;
    }

    [RelayCommand]
    public void RemoveSplitDetail(BankChequeBodyViewModel? detail)
    {
        if (detail != null && SplitDetails.Contains(detail))
        {
            SplitDetails.Remove(detail);
            SplitTotalAmount = SplitDetails.Sum(s => s.Amount);
            Amount = SplitTotalAmount;
        }
    }

    // Lookups
    [RelayCommand]
    public void OpenBankLookup()
    {
        IsLookupForBank = true;
        IsLookupForSplit = false;
        LookupTitle = "Select Bank Account";
        LookupSearchQuery = string.Empty;
        var bankAcNos = BankAccounts.Select(b => b.Account?.AccountNo ?? b.AccountNumber).ToHashSet();
        FilteredAccounts = new ObservableCollection<Account>(
            _allAccounts.Where(a => bankAcNos.Contains(a.AccountNo) || a.Name.Contains("Bank") || a.Name.Contains("بینک")));
        IsAccountLookupOpen = true;
    }

    [RelayCommand]
    public void OpenPartyLookup()
    {
        IsLookupForBank = false;
        IsLookupForSplit = false;
        LookupTitle = "Select Payee / Customer / Vendor";
        LookupSearchQuery = string.Empty;
        FilteredAccounts = new ObservableCollection<Account>(_allAccounts);
        IsAccountLookupOpen = true;
    }

    [RelayCommand]
    public void OpenSplitAccountLookup()
    {
        IsLookupForBank = false;
        IsLookupForSplit = true;
        LookupTitle = "Select Split Account";
        LookupSearchQuery = string.Empty;
        FilteredAccounts = new ObservableCollection<Account>(_allAccounts);
        IsAccountLookupOpen = true;
    }

    partial void OnLookupSearchQueryChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            if (IsLookupForBank)
            {
                var bankAcNos = BankAccounts.Select(b => b.Account?.AccountNo ?? b.AccountNumber).ToHashSet();
                FilteredAccounts = new ObservableCollection<Account>(
                    _allAccounts.Where(a => bankAcNos.Contains(a.AccountNo) || a.Name.Contains("Bank") || a.Name.Contains("بینک")));
            }
            else
            {
                FilteredAccounts = new ObservableCollection<Account>(_allAccounts);
            }
        }
        else
        {
            var q = value.ToLower().Trim();
            var baseList = IsLookupForBank 
                ? _allAccounts.Where(a => a.Name.Contains("Bank") || a.Name.Contains("بینک")) 
                : _allAccounts;
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

        if (IsLookupForBank)
        {
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
                _ = RefreshAvailableChequeNumbersAsync(account.AccountNo);
            }
        }
        else if (IsLookupForSplit)
        {
            SplitAccountNo = account.AccountNo;
            SplitAccountName = account.Name;
        }
        else
        {
            _partyAccountId = account.Id;
            PartyAccountNo = account.AccountNo;
            PartyAccountName = account.Name;
            PartyUrduName = account.UrduName ?? string.Empty;
            PartyCity = string.Empty;
            HasSelectedParty = true;
            _ = UpdatePartyBalanceAsync(account.Id);
        }

        IsAccountLookupOpen = false;
    }

    [RelayCommand]
    public void CloseLookup()
    {
        IsAccountLookupOpen = false;
    }

    public async Task UpdatePartyBalanceAsync(int accountId)
    {
        try
        {
            var bal = await _accountService.GetAccountBalanceAsync(accountId);
            AccountBalance = Math.Abs(bal).ToString("N0");
            AccountBalanceStatus = bal >= 0 ? "Debit (Dr)" : "Credit (Cr)";
        }
        catch
        {
            AccountBalance = "0";
            AccountBalanceStatus = string.Empty;
        }
    }

    public void OnPartyAccountNoEntered()
    {
        if (string.IsNullOrWhiteSpace(PartyAccountNo)) return;

        var ac = _allAccounts.FirstOrDefault(a => a.AccountNo == PartyAccountNo.Trim());
        if (ac != null)
        {
            SelectLookupAccount(ac);
        }
        else
        {
            OpenPartyLookup();
            LookupSearchQuery = PartyAccountNo.Trim();
        }
    }

    public void OnSplitAccountNoEntered()
    {
        if (string.IsNullOrWhiteSpace(SplitAccountNo)) return;

        var ac = _allAccounts.FirstOrDefault(a => a.AccountNo == SplitAccountNo.Trim());
        if (ac != null)
        {
            SplitAccountName = ac.Name;
        }
        else
        {
            OpenSplitAccountLookup();
            LookupSearchQuery = SplitAccountNo.Trim();
        }
    }

    // Save & CRUD Actions
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

        if (string.IsNullOrWhiteSpace(ChequeNo))
        {
            MessageBox.Show("Please specify a valid Cheque Number.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Amount <= 0)
        {
            MessageBox.Show("Please enter cheque amount greater than zero.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (IsMultiPayeeMode)
        {
            if (!SplitDetails.Any())
            {
                MessageBox.Show("Please add at least one line entry into the Payee Grid.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
            if (SplitDetails.Sum(s => s.Amount) != Amount)
            {
                MessageBox.Show("Cheque Amount should be equal to the total of Pay To Accounts in Grid.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }
        else
        {
            if (string.IsNullOrEmpty(PartyAccountNo) && !_partyAccountId.HasValue)
            {
                MessageBox.Show("Please specify a valid Pay To / Party account.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }
        }

        try
        {
            var dto = new BankChequeDto
            {
                VoucherNo = vNo,
                VoucherDate = VoucherDate,
                ChequeNo = ChequeNo.Trim(),
                ChequeDate = ChequeDate,
                IsReconciled = IsReconciled,
                ReconcileDate = IsReconciled ? ReconcileDate : null,
                BankAccountId = _bankAccountId,
                BankAccountNo = BankAccountNo,
                BankAccountName = BankAccountName,
                PartyAccountId = _partyAccountId,
                PartyAccountNo = PartyAccountNo,
                PartyAccountName = PartyAccountName,
                Amount = Amount,
                ReceivedBy = ReceivedBy,
                Narration = Narration,
                NarrationUrdu = NarrationUrdu,
                IsCrossCheque = IsCrossCheque,
                WhtTax = IsWhtMode ? WhtAmount : 0,
                WhtRatio = IsWhtMode ? WhtRatio : 0,
                PmFlag = IsWhtPlus ? "+" : "-",
                WhtToBank = WhtToBank,
                WhtToPayee = WhtToPayee,
                SendSms = SendSms,
                CropSeasonId = SelectedSeason?.Id,
                CropSeason = SelectedSeason?.Name ?? string.Empty,
                BranchId = 1,
                BranchName = BranchName,
                SplitDetails = SplitDetails.Select(s => new BankChequeBodyDto
                {
                    AccountNo = s.AccountNo,
                    AccountName = s.AccountName,
                    Amount = s.Amount,
                    Narration = s.Narration
                }).ToList()
            };

            await _bankingService.SaveBankChequeAsync(dto);

            var result = MessageBox.Show(
                $"Bank Cheque Issue Voucher #{VoucherNo} saved successfully!\n\nDo you want to print the cheque / voucher slip?",
                "Voucher Saved",
                MessageBoxButton.YesNo,
                MessageBoxImage.Information);

            if (result == MessageBoxResult.Yes)
            {
                Print();
            }

            await NewVoucherAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving Bank Cheque Voucher:\n{ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public async Task SearchVoucherAsync()
    {
        if (string.IsNullOrWhiteSpace(SearchVoucherNo)) return;
        if (long.TryParse(SearchVoucherNo.Trim(), out var vNo))
        {
            await LoadVoucherAsync(vNo);
        }
        else
        {
            MessageBox.Show("Please enter a valid numeric voucher number.", "Search Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

    public async Task LoadVoucherAsync(long voucherNo)
    {
        try
        {
            var dto = await _bankingService.LoadBankChequeByVoucherNoAsync(voucherNo);
            if (dto == null)
            {
                MessageBox.Show($"Bank Cheque Issue Voucher #{voucherNo} not found!", "Voucher Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
                return;
            }

            VoucherNo = dto.VoucherNo.ToString();
            VoucherDate = dto.VoucherDate;
            IsExistingVoucher = true;

            ChequeNo = dto.ChequeNo;
            SelectedChequeNo = dto.ChequeNo;
            ChequeDate = dto.ChequeDate;
            IsCrossCheque = dto.IsCrossCheque;
            IsReconciled = dto.IsReconciled;
            ReconcileDate = dto.ReconcileDate ?? dto.ChequeDate;

            BankAccountNo = dto.BankAccountNo;
            BankAccountName = dto.BankAccountName;
            var bAc = BankAccounts.FirstOrDefault(b => b.Account?.AccountNo == dto.BankAccountNo || b.AccountNumber == dto.BankAccountNo);
            if (bAc != null)
            {
                SelectedBank = bAc;
            }

            PartyAccountNo = dto.PartyAccountNo;
            PartyAccountName = dto.PartyAccountName;
            var pAc = _allAccounts.FirstOrDefault(a => a.AccountNo == dto.PartyAccountNo);
            if (pAc != null)
            {
                _partyAccountId = pAc.Id;
                PartyUrduName = pAc.UrduName ?? string.Empty;
                PartyCity = string.Empty;
                HasSelectedParty = true;
                await UpdatePartyBalanceAsync(pAc.Id);
            }

            ReceivedBy = dto.ReceivedBy;
            Amount = dto.Amount;
            Narration = dto.Narration;
            NarrationUrdu = dto.NarrationUrdu;

            IsWhtMode = dto.WhtTax > 0;
            WhtAmount = dto.WhtTax;
            WhtRatio = dto.WhtRatio;
            IsWhtPlus = dto.PmFlag == "+";
            IsWhtMinus = !IsWhtPlus;
            WhtToBank = dto.WhtToBank;
            WhtToPayee = dto.WhtToPayee;
            CalculateWht();

            SplitDetails.Clear();
            if (dto.SplitDetails.Any())
            {
                IsMultiPayeeMode = true;
                foreach (var s in dto.SplitDetails)
                {
                    SplitDetails.Add(new BankChequeBodyViewModel
                    {
                        AccountNo = s.AccountNo,
                        AccountName = s.AccountName,
                        Amount = s.Amount,
                        Narration = s.Narration
                    });
                }
                SplitTotalAmount = SplitDetails.Sum(s => s.Amount);
            }
            else
            {
                IsMultiPayeeMode = false;
            }

            if (!string.IsNullOrEmpty(dto.CropSeason))
            {
                SelectedSeason = Seasons.FirstOrDefault(s => s.Name == dto.CropSeason) ?? SelectedSeason;
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error loading voucher:\n{ex.Message}", "Error Loading Voucher", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public async Task DeleteVoucherAsync()
    {
        if (!IsExistingVoucher || !long.TryParse(VoucherNo, out var vNo))
        {
            MessageBox.Show("No saved voucher selected to delete.", "Delete Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show(
            $"Are you sure you want to delete Bank Cheque Issue Voucher #{VoucherNo}?\nThis will remove the entry from both General Ledger and Database.",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _bankingService.DeleteBankChequeAsync(vNo);
                MessageBox.Show($"Voucher #{VoucherNo} deleted successfully!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
                await NewVoucherAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting voucher:\n{ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    public async Task OpenSearchModalAsync()
    {
        IsSearchListOpen = true;
        SearchListQuery = string.Empty;
        var list = await _bankingService.SearchBankChequesAsync();
        SearchListResults = new ObservableCollection<BankChequeDto>(list);
    }

    partial void OnSearchListQueryChanged(string value)
    {
        _ = FilterSearchListAsync(value);
    }

    private async Task FilterSearchListAsync(string query)
    {
        var list = await _bankingService.SearchBankChequesAsync(query);
        SearchListResults = new ObservableCollection<BankChequeDto>(list);
    }

    [RelayCommand]
    public async Task SelectSearchItemAsync(BankChequeDto? item)
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
            var voucher = new Voucher
            {
                VoucherNo = VoucherNo,
                VoucherType = VoucherType.BankPayment,
                VoucherDate = VoucherDate,
                TotalDebit = Amount,
                TotalCredit = Amount,
                NarrationEnglish = $"Bank Cheque Issue (Chq #{ChequeNo}) - {Narration}",
                NarrationUrdu = NarrationUrdu,
                Details = new List<VoucherDetail>
                {
                    new VoucherDetail
                    {
                        AccountId = _partyAccountId ?? 0,
                        Debit = Amount,
                        Credit = 0,
                        Narration = $"Cheque #{ChequeNo} Issued to {PartyAccountName}",
                        Account = new Account { AccountNo = PartyAccountNo, Name = PartyAccountName }
                    },
                    new VoucherDetail
                    {
                        AccountId = SelectedBank?.AccountId ?? 0,
                        Debit = 0,
                        Credit = Amount,
                        Narration = $"Cheque #{ChequeNo} Issued from Bank ({BankAccountName})",
                        Account = SelectedBank?.Account ?? new Account { AccountNo = BankAccountNo, Name = BankAccountName }
                    }
                }
            };

            var slip = _printModelFactory.BuildVoucherSlip(voucher);
            _printService.Preview(slip);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error preparing print preview:\n{ex.Message}", "Print Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    public void PrintCheque()
    {
        // Specialized Cheque Leaf Print preview
        Print();
    }

    [RelayCommand]
    public void Close()
    {
        RequestClose?.Invoke();
    }
}

