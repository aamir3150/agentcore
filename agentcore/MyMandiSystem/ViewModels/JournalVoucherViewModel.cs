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
using CommunityToolkit.Mvvm.Messaging;

namespace MyMandiSystem.ViewModels;

public partial class JournalVoucherViewModel : ObservableObject
{
    private readonly IVoucherService _voucherService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;
    private readonly MyMandiSystem.Core.Printing.IPrintService _printService;
    private readonly MyMandiSystem.Core.Printing.IPrintModelFactory _printModelFactory;

    [ObservableProperty] private string _voucherNo = "1";
    [ObservableProperty] private DateTime _voucherDate = DateTime.Today;
    [ObservableProperty] private CropSeason? _selectedSeason;
    [ObservableProperty] private ObservableCollection<CropSeason> _seasons = new();
    [ObservableProperty] private string _branchName = "Main Branch";
    [ObservableProperty] private string _searchVoucherNo = string.Empty;
    [ObservableProperty] private bool _isExistingVoucher = false;
    [ObservableProperty] private int _currentVoucherId = 0;

    // Real-Time Balances (AgenSoft Parity)
    [ObservableProperty] private string _cashInHand = "0";
    [ObservableProperty] private string _accountBalance = "0";
    [ObservableProperty] private string _accountBalanceStatus = string.Empty;
    [ObservableProperty] private bool _hasSelectedAccount = false;
    [ObservableProperty] private string _selectedAccountUrduName = string.Empty;
    [ObservableProperty] private string _selectedAccountCity = string.Empty;

    // Entry Fields
    [ObservableProperty] private string _accountNo = string.Empty;
    [ObservableProperty] private string _accountName = string.Empty;
    [ObservableProperty] private decimal _debit;
    [ObservableProperty] private decimal _credit;
    [ObservableProperty] private string _narration = string.Empty;
    [ObservableProperty] private string _narrationUrdu = string.Empty;

    // Grid & Totals
    [ObservableProperty] private ObservableCollection<VoucherDetailViewModel> _details = new();
    [ObservableProperty] private decimal _totalDebit;
    [ObservableProperty] private decimal _totalCredit;
    [ObservableProperty] private decimal _difference;

    // Account Lookup Modal (F1)
    [ObservableProperty] private bool _isAccountLookupOpen;
    [ObservableProperty] private string _lookupSearchQuery = string.Empty;
    [ObservableProperty] private ObservableCollection<Account> _filteredAccounts = new();
    [ObservableProperty] private Account? _selectedAccount;
    private List<Account> _allAccounts = new();

    private int? _currentAccountId;

    public JournalVoucherViewModel(
        IVoucherService voucherService, 
        IAccountService accountService,
        ISystemService systemService,
        MyMandiSystem.Core.Printing.IPrintService printService,
        MyMandiSystem.Core.Printing.IPrintModelFactory printModelFactory)
    {
        _voucherService = voucherService;
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
            SelectedSeason = Seasons.FirstOrDefault(s => s.IsActive);
            
            var year = await _systemService.GetCurrentYearAsync();
            if (year != null)
            {
                VoucherNo = await _voucherService.GetNextVoucherNoAsync(VoucherType.JournalVoucher, year.Id);
            }

            _allAccounts = (await _accountService.GetAllAccountsAsync()).Where(a => !a.IsDeleted).ToList();
            FilteredAccounts = new ObservableCollection<Account>(_allAccounts);

            await LoadCashInHandAsync();
        }
        catch
        {
            // Resilient initialization
        }
    }

    private async Task LoadCashInHandAsync()
    {
        try
        {
            var allAccounts = await _accountService.GetAllAccountsAsync();
            var cashAccounts = allAccounts.Where(a => a.AccountNo == "1001" || a.Name.Contains("Cash") || a.Name.Contains("روکڑ") || a.Name == "General Cash Account" || a.AccountNo == "101").ToList();
            decimal bal = 0m;
            foreach (var ca in cashAccounts)
            {
                bal += await _accountService.GetAccountBalanceAsync(ca.Id);
            }
            CashInHand = bal.ToString("N0");
        }
        catch
        {
            CashInHand = "0";
        }
    }

    partial void OnAccountNoChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AccountName = string.Empty;
            _currentAccountId = null;
            ResetSelectedAccountInfo();
            return;
        }

        LookupAccountByNo(value.Trim());
    }

    private async void LookupAccountByNo(string no)
    {
        var account = await _accountService.GetAccountByNoAsync(no);
        if (AccountNo?.Trim() == no)
        {
            if (account != null)
            {
                AccountName = account.Name;
                _currentAccountId = account.Id;
                await UpdateAccountBalanceInfo(account);
            }
            else
            {
                AccountName = string.Empty;
                _currentAccountId = null;
                ResetSelectedAccountInfo();
            }
        }
    }

    private async Task UpdateAccountBalanceInfo(Account account)
    {
        try
        {
            var bal = await _accountService.GetAccountBalanceAsync(account.Id);
            if (bal > 0)
            {
                AccountBalance = $"{bal:N0} Dr";
                AccountBalanceStatus = "Debit (Dr)";
            }
            else if (bal < 0)
            {
                AccountBalance = $"{Math.Abs(bal):N0} Cr";
                AccountBalanceStatus = "Credit (Cr)";
            }
            else
            {
                AccountBalance = "0 Nil";
                AccountBalanceStatus = "Balanced (Nil)";
            }
            HasSelectedAccount = true;
            SelectedAccountUrduName = account.UrduName ?? string.Empty;
            SelectedAccountCity = account.AccountGroup?.Name ?? string.Empty;
        }
        catch
        {
            AccountBalance = "0";
            AccountBalanceStatus = string.Empty;
        }
    }

    private void ResetSelectedAccountInfo()
    {
        AccountBalance = "0";
        AccountBalanceStatus = string.Empty;
        HasSelectedAccount = false;
        SelectedAccountUrduName = string.Empty;
        SelectedAccountCity = string.Empty;
    }

    [RelayCommand]
    public void OpenAccountLookup(string? initialFilter = null)
    {
        LookupSearchQuery = initialFilter ?? string.Empty;
        ApplyAccountFilter();
        IsAccountLookupOpen = true;
    }

    [RelayCommand]
    private void CloseAccountLookup()
    {
        IsAccountLookupOpen = false;
        LookupSearchQuery = string.Empty;
    }

    partial void OnLookupSearchQueryChanged(string value)
    {
        ApplyAccountFilter();
    }

    private void ApplyAccountFilter()
    {
        if (string.IsNullOrWhiteSpace(LookupSearchQuery))
        {
            FilteredAccounts = new ObservableCollection<Account>(_allAccounts);
        }
        else
        {
            var q = LookupSearchQuery.Trim().ToLower();
            var matches = _allAccounts.Where(a =>
                (a.AccountNo != null && a.AccountNo.ToLower().Contains(q)) ||
                (a.Name != null && a.Name.ToLower().Contains(q)) ||
                (a.UrduName != null && a.UrduName.Contains(q)) ||
                (a.AccountGroup != null && a.AccountGroup.Name.ToLower().Contains(q))
            ).ToList();
            FilteredAccounts = new ObservableCollection<Account>(matches);
        }

        if (FilteredAccounts.Any())
        {
            SelectedAccount = FilteredAccounts.First();
        }
        else
        {
            SelectedAccount = null;
        }
    }

    [RelayCommand]
    public async Task SelectAccountFromLookup(Account? selected)
    {
        var target = selected ?? SelectedAccount;
        if (target != null)
        {
            AccountNo = target.AccountNo;
            AccountName = target.Name;
            _currentAccountId = target.Id;
            await UpdateAccountBalanceInfo(target);
            IsAccountLookupOpen = false;
        }
    }

    [RelayCommand]
    public void AddDetail()
    {
        if (string.IsNullOrWhiteSpace(AccountNo) || string.IsNullOrWhiteSpace(AccountName))
        {
            System.Windows.MessageBox.Show("Please select an Account (Account No and Name are required).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Debit == 0 && Credit == 0)
        {
            System.Windows.MessageBox.Show("Please enter either Debit or Credit amount.", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (Debit > 0 && Credit > 0)
        {
            System.Windows.MessageBox.Show("Debit and Credit amounts cannot both be entered on the same line.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (Debit >= 100_000_000_000_000m)
        {
            System.Windows.MessageBox.Show("The debit amount must be less than 100 trillion.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        if (Credit >= 100_000_000_000_000m)
        {
            System.Windows.MessageBox.Show("The credit amount must be less than 100 trillion.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        Details.Add(new VoucherDetailViewModel
        {
            AccountId = _currentAccountId ?? 0,
            AccountNo = AccountNo.Trim(),
            AccountName = AccountName.Trim(),
            Narration = Narration?.Trim() ?? string.Empty,
            Debit = Debit,
            Credit = Credit
        });

        UpdateTotals();
        ClearEntryFields();
    }

    [RelayCommand]
    private void RemoveDetail(VoucherDetailViewModel detail)
    {
        if (detail != null)
        {
            Details.Remove(detail);
            UpdateTotals();
        }
    }

    private void UpdateTotals()
    {
        TotalDebit = Details.Sum(d => d.Debit);
        TotalCredit = Details.Sum(d => d.Credit);
        Difference = Math.Abs(TotalDebit - TotalCredit);
    }

    private void ClearEntryFields()
    {
        AccountNo = string.Empty;
        AccountName = string.Empty;
        Debit = 0;
        Credit = 0;
        Narration = string.Empty;
        _currentAccountId = null;
        ResetSelectedAccountInfo();
    }

    [RelayCommand]
    public async Task SearchVoucher()
    {
        var targetNo = !string.IsNullOrWhiteSpace(SearchVoucherNo) ? SearchVoucherNo : VoucherNo;
        if (string.IsNullOrWhiteSpace(targetNo))
        {
            System.Windows.MessageBox.Show("Please enter a voucher number to search.", "Search Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var year = await _systemService.GetCurrentYearAsync();
        var voucher = await _voucherService.GetVoucherByNoAsync(VoucherType.JournalVoucher, targetNo, year?.Id ?? 0);
        if (voucher != null)
        {
            LoadVoucherIntoForm(voucher);
        }
        else
        {
            System.Windows.MessageBox.Show($"Journal Voucher #{targetNo} not found in the system.", "Voucher Not Found", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    public async Task PreviousVoucher()
    {
        var year = await _systemService.GetCurrentYearAsync();
        var voucher = await _voucherService.GetPreviousVoucherAsync(VoucherType.JournalVoucher, CurrentVoucherId, year?.Id ?? 0);
        if (voucher != null)
        {
            LoadVoucherIntoForm(voucher);
        }
        else
        {
            System.Windows.MessageBox.Show("No previous Journal Voucher found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    public async Task NextVoucher()
    {
        var year = await _systemService.GetCurrentYearAsync();
        var voucher = await _voucherService.GetNextVoucherAsync(VoucherType.JournalVoucher, CurrentVoucherId, year?.Id ?? 0);
        if (voucher != null)
        {
            LoadVoucherIntoForm(voucher);
        }
        else
        {
            System.Windows.MessageBox.Show("No next Journal Voucher found.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    public async Task DeleteVoucher()
    {
        if (!IsExistingVoucher || CurrentVoucherId <= 0)
        {
            System.Windows.MessageBox.Show("Please load a saved voucher first to delete.", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirm = System.Windows.MessageBox.Show(
            $"Are you sure you want to delete Journal Voucher #{VoucherNo}?\nThis will remove it from the system and ledger.",
            "Confirm Delete",
            MessageBoxButton.YesNo,
            MessageBoxImage.Warning);

        if (confirm == MessageBoxResult.Yes)
        {
            await _voucherService.DeleteVoucherAsync(CurrentVoucherId);
            WeakReferenceMessenger.Default.Send(new MyMandiSystem.Core.Messages.RefreshDashboardMessage());
            System.Windows.MessageBox.Show("Journal Voucher deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            ClearAll();
        }
    }

    private void LoadVoucherIntoForm(Voucher voucher)
    {
        CurrentVoucherId = voucher.Id;
        IsExistingVoucher = true;
        VoucherNo = voucher.VoucherNo;
        SearchVoucherNo = voucher.VoucherNo;
        VoucherDate = voucher.VoucherDate;
        NarrationUrdu = voucher.NarrationUrdu ?? string.Empty;

        if (voucher.CropSeason != null)
        {
            SelectedSeason = Seasons.FirstOrDefault(s => s.Id == voucher.CropSeason.Id || s.Name == voucher.CropSeason.Name);
        }

        Details.Clear();
        foreach (var d in voucher.Details)
        {
            var accNo = !string.IsNullOrWhiteSpace(d.Account?.AccountNo) ? d.Account.AccountNo : (d.AccountNo ?? string.Empty);
            var accName = !string.IsNullOrWhiteSpace(d.Account?.Name) ? d.Account.Name : accNo;
            Details.Add(new VoucherDetailViewModel
            {
                AccountId = d.AccountId,
                AccountNo = accNo,
                AccountName = accName,
                Narration = d.Narration ?? string.Empty,
                Debit = d.Debit,
                Credit = d.Credit
            });
        }
        UpdateTotals();
    }

    [RelayCommand]
    public async Task Save()
    {
        if (Details.Count < 2)
        {
            System.Windows.MessageBox.Show("A valid Journal Voucher requires at least 2 entries (Double Entry).", "Information", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!Details.Any(d => d.Debit > 0) || !Details.Any(d => d.Credit > 0))
        {
            System.Windows.MessageBox.Show("Journal Voucher must contain at least one Debit entry and one Credit entry.", "Double Entry Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (TotalDebit != TotalCredit || TotalDebit <= 0)
        {
            System.Windows.MessageBox.Show($"Total Debit ({TotalDebit:N0}) and Total Credit ({TotalCredit:N0}) must be equal and greater than zero!\nDifference: {Difference:N0}", "Double Entry Balance Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (TotalDebit >= 100_000_000_000_000m)
        {
            System.Windows.MessageBox.Show("The debit amount must be less than 100 trillion.", "Limit Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var year = await _systemService.GetCurrentYearAsync();
            var voucher = new Voucher
            {
                Id = IsExistingVoucher ? CurrentVoucherId : 0,
                VoucherNo = VoucherNo,
                VoucherType = VoucherType.JournalVoucher,
                VoucherDate = VoucherDate,
                FinancialYearId = year?.Id ?? 1,
                CropSeasonId = (SelectedSeason != null && SelectedSeason.Id > 0) ? SelectedSeason.Id : null,
                CropSeason = SelectedSeason,
                NarrationUrdu = NarrationUrdu,
                TotalDebit = TotalDebit,
                TotalCredit = TotalCredit,
                Details = Details.Select(d => new VoucherDetail
                {
                    AccountId = d.AccountId,
                    AccountNo = d.AccountNo,
                    Debit = d.Debit,
                    Credit = d.Credit,
                    Narration = d.Narration,
                    Account = new Account
                    {
                        AccountNo = d.AccountNo,
                        Name = d.AccountName
                    }
                }).ToList()
            };

            await _voucherService.SaveVoucherAsync(voucher);
            VoucherNo = voucher.VoucherNo;
            await _voucherService.PostVoucherAsync(voucher.Id);
            WeakReferenceMessenger.Default.Send(new MyMandiSystem.Core.Messages.RefreshDashboardMessage());

            var printPrompt = System.Windows.MessageBox.Show(
                "Journal Voucher saved successfully.\nDo you want to print the voucher slip?", 
                "Print Voucher", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (printPrompt == MessageBoxResult.Yes)
            {
                var slip = _printModelFactory.BuildVoucherSlip(voucher);
                _printService.Preview(slip);
            }

            System.Windows.MessageBox.Show(
                $"Journal Voucher #{voucher.VoucherNo} saved and posted to General Ledger successfully!\nTotal Balanced Amount: Rs. {TotalDebit:N0} (Debit = Credit).", 
                "Success", 
                MessageBoxButton.OK, 
                MessageBoxImage.Information);

            ClearAll();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Journal Voucher Save");
        }
    }

    [RelayCommand]
    public void Print()
    {
        if (!Details.Any())
        {
            System.Windows.MessageBox.Show("Please add at least one line entry before printing.", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var voucher = new Voucher
        {
            VoucherNo = VoucherNo,
            VoucherType = VoucherType.JournalVoucher,
            VoucherDate = VoucherDate,
            TotalDebit = TotalDebit,
            TotalCredit = TotalCredit,
            NarrationEnglish = Details.FirstOrDefault()?.Narration,
            NarrationUrdu = NarrationUrdu,
            Details = Details.Select(d => new VoucherDetail
            {
                AccountId = d.AccountId,
                Debit = d.Debit,
                Credit = d.Credit,
                Narration = d.Narration,
                Account = new Account { AccountNo = d.AccountNo, Name = d.AccountName }
            }).ToList()
        };

        var slip = _printModelFactory.BuildVoucherSlip(voucher);
        _printService.Preview(slip);
    }

    [RelayCommand]
    public void ClearAll()
    {
        CurrentVoucherId = 0;
        IsExistingVoucher = false;
        SearchVoucherNo = string.Empty;
        Details.Clear();
        NarrationUrdu = string.Empty;
        UpdateTotals();
        ClearEntryFields();
        LoadInitialData();
    }

    [RelayCommand]
    private void Close(Window? window)
    {
        window?.Close();
    }
}
