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
using Microsoft.Extensions.DependencyInjection;

namespace MyMandiSystem.ViewModels;

public partial class CashPaymentVoucherViewModel : ObservableObject
{
    [ObservableProperty]
    private string _voucherNo = "1";

    [ObservableProperty]
    private DateTime _voucherDate = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<string> _cropSeasons = new();

    [ObservableProperty]
    private string _cropSeason = string.Empty;

    [ObservableProperty]
    private string _branchName = "Main Branch";

    [ObservableProperty]
    private string _searchVoucherNo = string.Empty;

    [ObservableProperty]
    private bool _isExistingVoucher = false;

    [ObservableProperty]
    private int _currentVoucherId = 0;

    [ObservableProperty]
    private string _cashInHand = "0";

    [ObservableProperty]
    private string _accountNo = string.Empty;

    [ObservableProperty]
    private string _accountName = string.Empty;

    // Account Balance & Party Real-Time Info
    [ObservableProperty]
    private string _accountBalance = "0";

    [ObservableProperty]
    private string _accountBalanceStatus = string.Empty;

    [ObservableProperty]
    private bool _hasSelectedAccount = false;

    [ObservableProperty]
    private string _selectedAccountUrduName = string.Empty;

    [ObservableProperty]
    private string _selectedAccountCity = string.Empty;

    [ObservableProperty]
    private string _selectedAccountPhone = string.Empty;

    // Options
    [ObservableProperty]
    private bool _sendSms = true;

    [ObservableProperty]
    private bool _showThumb = true;

    [ObservableProperty]
    private bool _autoPrint = true;

    // Account Search & Selection Modal
    [ObservableProperty] private bool _isAccountLookupOpen;
    [ObservableProperty] private string _lookupSearchQuery = string.Empty;
    [ObservableProperty] private ObservableCollection<Account> _filteredAccounts = new();
    [ObservableProperty] private Account? _selectedAccount;
    private List<Account> _allAccounts = new();

    [ObservableProperty]
    private string _narration = string.Empty;

    [ObservableProperty]
    private decimal _amount;

    [ObservableProperty]
    private string _narrationUrdu = string.Empty;

    [ObservableProperty]
    private decimal _totalAmount;

    // ═══ WHT & Special Deduction Mode (AgenSOFT FrmCashWHT Exact Logic) ═══
    [ObservableProperty] private bool _isWhtMode;
    [ObservableProperty] private string _creditVoucherNo = string.Empty;
    [ObservableProperty] private decimal _whtRatio;
    [ObservableProperty] private decimal _whtAmount;
    [ObservableProperty] private bool _isWhtMinus = true;
    [ObservableProperty] private bool _isWhtPlus = false;
    [ObservableProperty] private decimal _freight;
    [ObservableProperty] private bool _isFreightMinus = false;
    [ObservableProperty] private bool _isFreightPlus = true;
    [ObservableProperty] private decimal _netCashPaid;

    partial void OnAmountChanged(decimal value) => RecalculateWht();
    partial void OnWhtRatioChanged(decimal value) => RecalculateWht();
    partial void OnWhtAmountChanged(decimal value) => RecalculateNetCash();
    partial void OnIsWhtMinusChanged(bool value)
    {
        if (value && IsWhtPlus) IsWhtPlus = false;
        RecalculateNetCash();
    }
    partial void OnIsWhtPlusChanged(bool value)
    {
        if (value && IsWhtMinus) IsWhtMinus = false;
        RecalculateNetCash();
    }
    partial void OnFreightChanged(decimal value) => RecalculateNetCash();
    partial void OnIsFreightMinusChanged(bool value)
    {
        if (value && IsFreightPlus) IsFreightPlus = false;
        RecalculateNetCash();
    }
    partial void OnIsFreightPlusChanged(bool value)
    {
        if (value && IsFreightMinus) IsFreightMinus = false;
        RecalculateNetCash();
    }
    public string FormTitle => IsWhtMode ? "Cash Payment Voucher (Withholding Tax)" : "Debit Voucher (Cash Payment)";
    public string FormSubtitle => IsWhtMode ? "Record party payments subject to Withholding Tax and auto-generate linked Credit Voucher (Account 2303)" : "Record and disburse cash payments to vendor and expense accounts";

    partial void OnIsWhtModeChanged(bool value)
    {
        OnPropertyChanged(nameof(FormTitle));
        OnPropertyChanged(nameof(FormSubtitle));
        if (value && string.IsNullOrEmpty(CreditVoucherNo))
        {
            _ = LoadCreditVoucherNoAsync();
        }
        RecalculateNetCash();
    }

    private async Task LoadCreditVoucherNoAsync()
    {
        try
        {
            var year = await _systemService.GetCurrentYearAsync();
            if (year != null)
            {
                CreditVoucherNo = await _voucherService.GetNextVoucherNoAsync(Core.Enums.VoucherType.CashReceiving, year.Id);
            }
        }
        catch
        {
            CreditVoucherNo = "1";
        }
    }

    private void RecalculateWht()
    {
        if (IsWhtMode && WhtRatio > 0 && Amount > 0)
        {
            WhtAmount = Math.Round((Amount * WhtRatio) / 100m, 2);
        }
        RecalculateNetCash();
    }

    private void RecalculateNetCash()
    {
        if (!IsWhtMode)
        {
            NetCashPaid = TotalAmount > 0 ? TotalAmount : Amount;
            return;
        }

        decimal baseAmt = Amount > 0 ? Amount : TotalAmount;
        decimal net = baseAmt;

        if (WhtAmount > 0)
        {
            if (IsWhtMinus)
                net -= WhtAmount;
            else
                net += WhtAmount;
        }

        if (Freight > 0)
        {
            if (IsFreightMinus)
                net -= Freight;
            else
                net += Freight;
        }

        NetCashPaid = Math.Max(0, net);
    }

    // Denominations (نوٹ شماری)
    [ObservableProperty] private int _count5000;
    [ObservableProperty] private int _count1000;
    [ObservableProperty] private int _count500;
    [ObservableProperty] private int _count100;
    [ObservableProperty] private int _count50;
    [ObservableProperty] private int _count20;
    [ObservableProperty] private int _count10;
    [ObservableProperty] private int _count5;
    [ObservableProperty] private int _count2;
    [ObservableProperty] private int _count1;
    [ObservableProperty] private decimal _denominationTotal;

    public ObservableCollection<VoucherDetailViewModel> Details { get; } = new();

    private readonly IVoucherService _voucherService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;
    private readonly MyMandiSystem.Core.Printing.IPrintService _printService;
    private readonly MyMandiSystem.Core.Printing.IPrintModelFactory _printModelFactory;
    private List<CropSeason> _loadedSeasons = new();

    public CashPaymentVoucherViewModel(
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

    [RelayCommand]
    private void Print()
    {
        if (!Details.Any())
        {
            System.Windows.MessageBox.Show("Please add at least one line item before printing.", "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            return;
        }

        var voucher = new Voucher
        {
            VoucherNo = VoucherNo,
            VoucherDate = VoucherDate,
            VoucherType = Core.Enums.VoucherType.CashPayment,
            TotalDebit = TotalAmount,
            TotalCredit = TotalAmount,
            NarrationUrdu = NarrationUrdu,
            Details = Details.Select(d => new VoucherDetail
            {
                AccountId = d.AccountId,
                Narration = d.Narration,
                Debit = d.Debit,
                Credit = 0,
                Account = new Account { AccountNo = d.AccountNo, Name = d.AccountName }
            }).ToList()
        };

        var slip = _printModelFactory.BuildVoucherSlip(voucher);
        _printService.Preview(slip);
    }

    [RelayCommand]
    private void ManageCropSeasons()
    {
        var window = new Views.MasterDataWindow(4);
        window.DataContext = App.ServiceProvider?.GetRequiredService<MasterDataViewModel>();
        window.Closed += async (s, e) =>
        {
            var seasonsList = await _systemService.GetActiveSeasonsAsync();
            _loadedSeasons = seasonsList.ToList();
            var prevSelected = CropSeason;
            CropSeasons = new ObservableCollection<string>(_loadedSeasons.Select(s => s.Name));
            if (CropSeasons.Contains(prevSelected))
                CropSeason = prevSelected;
            else if (CropSeasons.Any())
                CropSeason = _loadedSeasons.FirstOrDefault(s => s.IsActive)?.Name ?? CropSeasons.First();
        };
        window.ShowDialog();
    }

    private static bool IsCashAccount(Account? a)
    {
        if (a == null) return false;
        if (a.AccountNo == "1001") return true;
        if (!string.IsNullOrEmpty(a.Name) && (a.Name.Contains("Cash in Hand", StringComparison.OrdinalIgnoreCase) || a.Name == "General Cash Account")) return true;
        if (!string.IsNullOrEmpty(a.UrduName) && a.UrduName.Contains("روکڑ")) return true;
        return false;
    }

    private static bool IsCashAccount(string? accountNo)
    {
        if (string.IsNullOrWhiteSpace(accountNo)) return false;
        return accountNo.Trim() == "1001";
    }

    private async void LoadInitialData()
    {
        var seasonsList = await _systemService.GetActiveSeasonsAsync();
        _loadedSeasons = seasonsList.ToList();
        CropSeasons = new ObservableCollection<string>(_loadedSeasons.Select(s => s.Name));
        if (CropSeasons.Any())
        {
            CropSeason = _loadedSeasons.FirstOrDefault(s => s.IsActive)?.Name ?? CropSeasons.First();
        }

        var year = await _systemService.GetCurrentYearAsync();
        if (year != null)
        {
            VoucherNo = await _voucherService.GetNextVoucherNoAsync(Core.Enums.VoucherType.CashPayment, year.Id);
            if (IsWhtMode)
            {
                CreditVoucherNo = await _voucherService.GetNextVoucherNoAsync(Core.Enums.VoucherType.CashReceiving, year.Id);
            }
        }

        try
        {
            var accounts = (await _accountService.GetAllAccountsAsync()).ToList();
            _allAccounts = accounts.Where(a => !a.IsDeleted && !IsCashAccount(a)).ToList();
            FilteredAccounts = new ObservableCollection<Account>(_allAccounts);

            var cashAccounts = accounts.Where(a => !a.IsDeleted && (a.Name.Contains("Cash") || a.AccountNo == "1001"));
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

    partial void OnCount5000Changed(int value) => CalculateDenominationTotal();
    partial void OnCount1000Changed(int value) => CalculateDenominationTotal();
    partial void OnCount500Changed(int value) => CalculateDenominationTotal();
    partial void OnCount100Changed(int value) => CalculateDenominationTotal();
    partial void OnCount50Changed(int value) => CalculateDenominationTotal();
    partial void OnCount20Changed(int value) => CalculateDenominationTotal();
    partial void OnCount10Changed(int value) => CalculateDenominationTotal();
    partial void OnCount5Changed(int value) => CalculateDenominationTotal();
    partial void OnCount2Changed(int value) => CalculateDenominationTotal();
    partial void OnCount1Changed(int value) => CalculateDenominationTotal();

    private void CalculateDenominationTotal()
    {
        DenominationTotal = (Count5000 * 5000) + (Count1000 * 1000) + (Count500 * 500) + 
                            (Count100 * 100) + (Count50 * 50) + (Count20 * 20) + 
                            (Count10 * 10) + (Count5 * 5) + (Count2 * 2) + (Count1 * 1);
    }

    [RelayCommand]
    private void ApplyDenominationToAmount()
    {
        if (DenominationTotal > 0)
        {
            Amount = DenominationTotal;
        }
    }

    partial void OnLookupSearchQueryChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            FilteredAccounts = new ObservableCollection<Account>(_allAccounts);
        }
        else
        {
            var q = value.Trim().ToLower();
            var matches = _allAccounts.Where(a => 
                (a.AccountNo != null && a.AccountNo.ToLower().Contains(q)) ||
                (a.Name != null && a.Name.ToLower().Contains(q)) ||
                (a.UrduName != null && a.UrduName.Contains(q)) ||
                (a.AccountGroup != null && a.AccountGroup.Name.ToLower().Contains(q)));
            FilteredAccounts = new ObservableCollection<Account>(matches);
        }
        SelectedAccount = FilteredAccounts.FirstOrDefault();
    }

    partial void OnAccountNoChanged(string value)
    {
        if (string.IsNullOrEmpty(value)) 
        {
            AccountName = string.Empty;
            ResetSelectedAccountInfo();
            return;
        }

        // Use service to lookup account
        LookupAccountByNo(value);
    }

    private async void LookupAccountByNo(string no)
    {
        if (IsCashAccount(no))
        {
            AccountName = "⚠️ Cash in Hand cannot be selected as party";
            ResetSelectedAccountInfo();
            return;
        }

        var account = await _accountService.GetAccountByNoAsync(no);
        if (account != null)
        {
            if (IsCashAccount(account))
            {
                AccountName = "⚠️ Cash in Hand cannot be selected as party";
                ResetSelectedAccountInfo();
                return;
            }
            AccountName = account.Name;
            await UpdateAccountBalanceInfo(account);
        }
        else
        {
            AccountName = "Account Not Found";
            ResetSelectedAccountInfo();
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
            SelectedAccountPhone = string.Empty;
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
        SelectedAccountPhone = string.Empty;
    }

    [RelayCommand]
    private async Task AddDetail()
    {
        if (string.IsNullOrWhiteSpace(AccountNo))
        {
            System.Windows.MessageBox.Show("Please enter an account code (A/c Code) first.", "Account Required", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (IsCashAccount(AccountNo))
        {
            System.Windows.MessageBox.Show(
                "Cash in Hand (1001) cannot be used as destination account in Payment Voucher.\nPlease select payee or expense account.", 
                "Invalid Account", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (Amount <= 0)
        {
            System.Windows.MessageBox.Show("Please enter amount greater than zero.", "Amount Required", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        var account = await _accountService.GetAccountByNoAsync(AccountNo.Trim());
        if (account == null)
        {
            System.Windows.MessageBox.Show($"Account code '{AccountNo}' was not found in the system.", "Account Not Found", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        if (IsCashAccount(account))
        {
            System.Windows.MessageBox.Show(
                "Cash in Hand (1001) cannot be used as destination account in Payment Voucher.\nPlease select payee or expense account.", 
                "Invalid Account", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        Details.Add(new VoucherDetailViewModel
        {
            AccountId = account.Id,
            AccountNo = account.AccountNo,
            AccountName = account.Name,
            Narration = IsWhtMode && WhtAmount > 0 
                ? $"{Narration} (Gross: Rs. {Amount:N0}, WHT: Rs. {WhtAmount:N0}, Net: Rs. {NetCashPaid:N0})".Trim() 
                : Narration,
            Debit = Amount // Payments DEBIT the target account
        });

        if (IsWhtMode)
        {
            if (WhtAmount > 0)
            {
                var whtAccount = await GetOrCreateWhtAccountAsync();
                Details.Add(new VoucherDetailViewModel
                {
                    AccountId = whtAccount.Id,
                    AccountNo = whtAccount.AccountNo,
                    AccountName = whtAccount.Name,
                    Narration = $"WHT @ {WhtRatio}% on {account.Name} (CPV #{VoucherNo})",
                    Credit = IsWhtMinus ? WhtAmount : 0,
                    Debit = IsWhtMinus ? 0 : WhtAmount
                });
            }

            if (Freight > 0)
            {
                var freightAccount = await GetOrCreateFreightAccountAsync();
                Details.Add(new VoucherDetailViewModel
                {
                    AccountId = freightAccount.Id,
                    AccountNo = freightAccount.AccountNo,
                    AccountName = freightAccount.Name,
                    Narration = $"Freight on {account.Name} (CPV #{VoucherNo})",
                    Credit = IsFreightMinus ? Freight : 0,
                    Debit = IsFreightMinus ? 0 : Freight
                });
            }
        }

        CalculateTotal();
        
        // Clear inputs
        AccountNo = string.Empty;
        AccountName = string.Empty;
        Narration = string.Empty;
        Amount = 0;
        WhtRatio = 0;
        WhtAmount = 0;
        Freight = 0;
    }

    private async Task<Account> GetOrCreateWhtAccountAsync()
    {
        var wht = await _accountService.GetAccountByNoAsync("2303");
        if (wht != null) return wht;

        var all = await _accountService.GetAllAccountsAsync();
        var existing = all.FirstOrDefault(a => a.Name.Contains("WH-Tax") || a.Name.Contains("WHT") || a.Name.Contains("Withholding"));
        if (existing != null) return existing;

        var groups = await _accountService.GetAccountGroupsAsync();
        var group = groups.FirstOrDefault(g => g.Name.Contains("Liability") || g.Name.Contains("Payable")) ?? groups.FirstOrDefault();

        var newAcc = new Account
        {
            AccountNo = "2303",
            Name = "WH-Tax on Cash Payments",
            UrduName = "ودہولڈنگ ٹیکس برائے نقد ادائیگی",
            AccountType = AccountType.Liability,
            AccountGroupId = group?.Id ?? 1,
            IsSystemAccount = true
        };
        await _accountService.AddAccountAsync(newAcc);
        return newAcc;
    }

    private async Task<Account> GetOrCreateFreightAccountAsync()
    {
        var fr = await _accountService.GetAccountByNoAsync("5004");
        if (fr != null) return fr;

        var all = await _accountService.GetAllAccountsAsync();
        var existing = all.FirstOrDefault(a => a.Name.Contains("Freight") || a.Name.Contains("کرایہ"));
        if (existing != null) return existing;

        var groups = await _accountService.GetAccountGroupsAsync();
        var group = groups.FirstOrDefault(g => g.Name.Contains("Expense")) ?? groups.FirstOrDefault();

        var newAcc = new Account
        {
            AccountNo = "5004",
            Name = "Freight Charges",
            UrduName = "کرایہ برائے مال / باربرداری",
            AccountType = AccountType.Expense,
            AccountGroupId = group?.Id ?? 1,
            IsSystemAccount = true
        };
        await _accountService.AddAccountAsync(newAcc);
        return newAcc;
    }

    [RelayCommand]
    private void RemoveDetail(VoucherDetailViewModel detail)
    {
        if (detail != null && Details.Contains(detail))
        {
            Details.Remove(detail);
            CalculateTotal();
        }
    }

    private void CalculateTotal()
    {
        TotalAmount = Details.Where(d => d.Debit > 0).Sum(d => d.Debit);
        decimal credits = Details.Where(d => d.Credit > 0).Sum(d => d.Credit);
        NetCashPaid = Math.Max(0, TotalAmount - credits);
    }

    [RelayCommand]
    private void OpenAccountLookup(string? initialQuery = null)
    {
        var query = initialQuery ?? (!string.IsNullOrWhiteSpace(AccountName) && AccountName != "Account Not Found" ? AccountName : string.Empty);
        LookupSearchQuery = query;
        if (string.IsNullOrWhiteSpace(query))
        {
            FilteredAccounts = new ObservableCollection<Account>(_allAccounts);
        }
        else
        {
            OnLookupSearchQueryChanged(query);
        }
        SelectedAccount = FilteredAccounts.FirstOrDefault();
        IsAccountLookupOpen = true;
    }

    [RelayCommand]
    private async Task SelectAccountFromLookup(Account? account)
    {
        var target = account ?? SelectedAccount;
        if (target != null)
        {
            if (IsCashAccount(target))
            {
                System.Windows.MessageBox.Show(
                    "Cash in Hand (1001) cannot be selected for cash payment.", 
                    "Invalid Account", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Warning);
                return;
            }
            AccountNo = target.AccountNo;
            AccountName = target.Name;
            await UpdateAccountBalanceInfo(target);
            IsAccountLookupOpen = false;
        }
    }

    [RelayCommand]
    private void CloseAccountLookup()
    {
        IsAccountLookupOpen = false;
    }

    [RelayCommand]
    private async Task SearchVoucher()
    {
        var targetNo = !string.IsNullOrWhiteSpace(SearchVoucherNo) ? SearchVoucherNo : VoucherNo;
        if (string.IsNullOrWhiteSpace(targetNo))
        {
            System.Windows.MessageBox.Show("Please enter a voucher number to search.", "Voucher Number Required", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        var year = await _systemService.GetCurrentYearAsync();
        var voucher = await _voucherService.GetVoucherByNoAsync(Core.Enums.VoucherType.CashPayment, targetNo, year?.Id ?? 0);
        if (voucher != null)
        {
            LoadVoucherIntoForm(voucher);
        }
        else
        {
            System.Windows.MessageBox.Show($"Voucher #{targetNo} not found in the system.", "Voucher Not Found", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task PreviousVoucher()
    {
        var year = await _systemService.GetCurrentYearAsync();
        var voucher = await _voucherService.GetPreviousVoucherAsync(Core.Enums.VoucherType.CashPayment, CurrentVoucherId, year?.Id ?? 0);
        if (voucher != null)
        {
            LoadVoucherIntoForm(voucher);
        }
        else
        {
            System.Windows.MessageBox.Show("No previous voucher found.", "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task NextVoucher()
    {
        var year = await _systemService.GetCurrentYearAsync();
        var voucher = await _voucherService.GetNextVoucherAsync(Core.Enums.VoucherType.CashPayment, CurrentVoucherId, year?.Id ?? 0);
        if (voucher != null)
        {
            LoadVoucherIntoForm(voucher);
        }
        else
        {
            System.Windows.MessageBox.Show("No next voucher found.", "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task DeleteVoucher()
    {
        if (!IsExistingVoucher || CurrentVoucherId <= 0)
        {
            System.Windows.MessageBox.Show("Please load a saved voucher first to delete.", "Information", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        var confirm = System.Windows.MessageBox.Show(
            $"Are you sure you want to delete Voucher #{VoucherNo}?\nThis will remove it from the system and ledger.",
            "Confirm Delete",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (confirm == System.Windows.MessageBoxResult.Yes)
        {
            await _voucherService.DeleteVoucherAsync(CurrentVoucherId);
            WeakReferenceMessenger.Default.Send(new MyMandiSystem.Core.Messages.RefreshDashboardMessage());
            System.Windows.MessageBox.Show("Voucher deleted successfully.", "Success", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            Clear();
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
        Count5000 = voucher.Count5000;
        Count1000 = voucher.Count1000;
        Count500 = voucher.Count500;
        Count100 = voucher.Count100;
        Count50 = voucher.Count50;
        Count20 = voucher.Count20;
        Count10 = voucher.Count10;
        Count5 = voucher.Count5;
        Count2 = voucher.Count2;
        Count1 = voucher.Count1;
        CalculateDenominationTotal();

        if (voucher.CropSeason != null)
        {
            CropSeason = voucher.CropSeason.Name;
        }

        Details.Clear();
        foreach (var d in voucher.Details)
        {
            Details.Add(new VoucherDetailViewModel
            {
                AccountId = d.AccountId,
                AccountNo = d.Account?.AccountNo ?? string.Empty,
                AccountName = d.Account?.Name ?? string.Empty,
                Narration = d.Narration ?? string.Empty,
                Debit = d.Debit
            });
        }

        CalculateTotal();

        // Reset entry line
        AccountNo = string.Empty;
        AccountName = string.Empty;
        Narration = string.Empty;
        Amount = 0;
        ResetSelectedAccountInfo();
    }

    [RelayCommand]
    private async Task LookupAccount()
    {
        if (string.IsNullOrWhiteSpace(AccountNo))
        {
            OpenAccountLookup(AccountName);
            return;
        }

        if (IsCashAccount(AccountNo))
        {
            AccountName = "⚠️ Cash in Hand cannot be selected as party";
            ResetSelectedAccountInfo();
            System.Windows.MessageBox.Show(
                "Cash in Hand (1001) cannot be used as destination account in Payment Voucher.\nPlease select payee or expense account.", 
                "Invalid Account", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        var account = await _accountService.GetAccountByNoAsync(AccountNo.Trim());
        if (account != null)
        {
            if (IsCashAccount(account))
            {
                AccountName = "⚠️ Cash in Hand cannot be selected as party";
                ResetSelectedAccountInfo();
                System.Windows.MessageBox.Show(
                    "Cash in Hand (1001) cannot be used as destination account in Payment Voucher.\nPlease select payee or expense account.", 
                    "Invalid Account", 
                    System.Windows.MessageBoxButton.OK, 
                    System.Windows.MessageBoxImage.Warning);
                return;
            }
            AccountName = account.Name;
            await UpdateAccountBalanceInfo(account);
        }
        else
        {
            // If code not matched, open search modal with the typed input so user can search immediately
            OpenAccountLookup(AccountNo);
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        // Auto-add line item if user entered data but forgot to click Add
        if (Amount > 0 && !string.IsNullOrWhiteSpace(AccountNo))
        {
            await AddDetail();
        }

        if (Details.Any(d => d.AccountNo == "1001" || (d.AccountName != null && d.AccountName.Contains("Cash in Hand", StringComparison.OrdinalIgnoreCase))))
        {
            System.Windows.MessageBox.Show(
                "Voucher contains Cash in Hand (1001) account which is not valid. Please delete it from the grid.", 
                "Invalid Entry", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Error);
            return;
        }

        if (Details.Count == 0)
        {
            System.Windows.MessageBox.Show(
                "Please add at least one line item before saving.", 
                "Line Entry Required", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            var year = await _systemService.GetCurrentYearAsync();
            if (year == null)
            {
                System.Windows.MessageBox.Show("Financial year not found.", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
                return;
            }

            var selectedSeason = _loadedSeasons.FirstOrDefault(s => s.Name == CropSeason);

            var voucher = new Voucher
            {
                Id = IsExistingVoucher ? CurrentVoucherId : 0,
                VoucherNo = VoucherNo,
                VoucherDate = VoucherDate,
                FinancialYearId = year.Id,
                CropSeasonId = selectedSeason?.Id,
                CropSeason = selectedSeason,
                TotalDebit = TotalAmount,
                TotalCredit = TotalAmount,
                VoucherType = Core.Enums.VoucherType.CashPayment,
                NarrationUrdu = NarrationUrdu,
                Count5000 = Count5000,
                Count1000 = Count1000,
                Count500 = Count500,
                Count100 = Count100,
                Count50 = Count50,
                Count20 = Count20,
                Count10 = Count10,
                Count5 = Count5,
                Count2 = Count2,
                Count1 = Count1
            };

            foreach (var detail in Details)
            {
                voucher.Details.Add(new VoucherDetail
                {
                    AccountId = detail.AccountId,
                    AccountNo = detail.AccountNo,
                    Narration = detail.Narration,
                    Debit = detail.Debit,
                    Credit = detail.Credit,
                    Account = new Account
                    {
                        AccountNo = detail.AccountNo,
                        Name = detail.AccountName
                    }
                });
            }

            await _voucherService.SaveVoucherAsync(voucher);
            VoucherNo = voucher.VoucherNo;
            
            // Auto-post to Ledger
            await _voucherService.PostVoucherAsync(voucher.Id);

            if (AutoPrint)
            {
                var slip = _printModelFactory.BuildVoucherSlip(voucher);
                _printService.Preview(slip);
            }
            else
            {
                var printPrompt = System.Windows.MessageBox.Show(
                    "Cash Payment Voucher saved successfully.\nDo you want to print the voucher slip?", 
                    "Voucher Saved", 
                    System.Windows.MessageBoxButton.YesNo, 
                    System.Windows.MessageBoxImage.Question);
                if (printPrompt == System.Windows.MessageBoxResult.Yes)
                {
                    var slip = _printModelFactory.BuildVoucherSlip(voucher);
                    _printService.Preview(slip);
                }
            }

            WeakReferenceMessenger.Default.Send(new MyMandiSystem.Core.Messages.RefreshDashboardMessage());

            var whtMsg = IsWhtMode && WhtAmount > 0 ? $"\n(WHT Deduction: Rs. {WhtAmount:N0}, Linked CRV #{CreditVoucherNo})" : "";
            var displayPaid = NetCashPaid > 0 ? NetCashPaid : TotalAmount;
            System.Windows.MessageBox.Show(
                $"Cash Payment Voucher #{voucher.VoucherNo} saved and posted to General Ledger successfully!\nNet amount of Rs. {displayPaid:N0} disbursed from Cash in Hand.{whtMsg}", 
                "Success", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Information);

            Clear();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Cash Payment Voucher Save");
        }
    }

    [RelayCommand]
    private void Clear()
    {
        Details.Clear();
        TotalAmount = 0;
        NarrationUrdu = string.Empty;
        Count5000 = 0;
        Count1000 = 0;
        Count500 = 0;
        Count100 = 0;
        Count50 = 0;
        Count20 = 0;
        Count10 = 0;
        Count5 = 0;
        Count2 = 0;
        Count1 = 0;
        DenominationTotal = 0;
        AccountNo = string.Empty;
        AccountName = string.Empty;
        Narration = string.Empty;
        Amount = 0;
        WhtRatio = 0;
        WhtAmount = 0;
        Freight = 0;
        NetCashPaid = 0;
        CreditVoucherNo = string.Empty;
        SearchVoucherNo = string.Empty;
        IsExistingVoucher = false;
        CurrentVoucherId = 0;
        ResetSelectedAccountInfo();
        LoadInitialData();
    }

    [RelayCommand]
    private void Close(Window? window)
    {
        window?.Close();
    }
}
