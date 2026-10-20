using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Core.Enums;
using MyMandiSystem.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;
using Microsoft.Extensions.DependencyInjection;

namespace MyMandiSystem.ViewModels;

public partial class CashReceivingVoucherViewModel : ObservableObject
{
    [ObservableProperty]
    private string _voucherNo = "164";

    [ObservableProperty]
    private DateTime _voucherDate = DateTime.Today;

    [ObservableProperty]
    private ObservableCollection<string> _cropSeasons = new();

    [ObservableProperty]
    private string _cropSeason = string.Empty;

    [ObservableProperty]
    private string _branchName = "Main Branch / ہیڈ آفس";

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

    // Denominations
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

    public CashReceivingVoucherViewModel(
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
            System.Windows.MessageBox.Show("واؤچر پرنٹ کرنے کے لیے کم از کم ایک انٹری شامل ہونی چاہیے!", "اطلاع", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
            return;
        }

        var voucher = new Voucher
        {
            VoucherNo = VoucherNo,
            VoucherDate = VoucherDate,
            VoucherType = Core.Enums.VoucherType.CashReceiving,
            TotalDebit = TotalAmount,
            TotalCredit = TotalAmount,
            NarrationUrdu = NarrationUrdu,
            Details = Details.Select(d => new VoucherDetail
            {
                AccountId = d.AccountId,
                Narration = d.Narration,
                Credit = d.Credit,
                Debit = 0,
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
            VoucherNo = await _voucherService.GetNextVoucherNoAsync(Core.Enums.VoucherType.CashReceiving, year.Id);
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
            AccountName = "⚠️ نقد / روکڑ کھاتہ وصولی کے لیے منتخب نہیں ہو سکتا";
            ResetSelectedAccountInfo();
            return;
        }

        var account = await _accountService.GetAccountByNoAsync(no);
        if (account != null)
        {
            if (IsCashAccount(account))
            {
                AccountName = "⚠️ نقد / روکڑ کھاتہ وصولی کے لیے منتخب نہیں ہو سکتا";
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
                AccountBalanceStatus = "واجب الوصول (لینا ہے)";
            }
            else if (bal < 0)
            {
                AccountBalance = $"{Math.Abs(bal):N0} Cr";
                AccountBalanceStatus = "واجب الادا (دینا ہے)";
            }
            else
            {
                AccountBalance = "0 Nil";
                AccountBalanceStatus = "حساب برابر ہے";
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
    private async System.Threading.Tasks.Task AddDetail()
    {
        if (string.IsNullOrWhiteSpace(AccountNo))
        {
            System.Windows.MessageBox.Show("براہ کرم پہلے اکاؤنٹ کوڈ (A/c Code) درج کریں۔", "اکاؤنٹ درکار ہے", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (IsCashAccount(AccountNo))
        {
            System.Windows.MessageBox.Show(
                "کیش ان ہینڈ (1001 - Cash in Hand) سے کیش ان ہینڈ میں وصولی کا اندراج نہیں ہو سکتا!\nکیونکہ رقم خود بخود روکڑ میں جمع ہو رہی ہے، یہاں صرف ادائیگی کرنے والے گاہک یا پارٹی کا کھاتہ درج کریں۔", 
                "ناقابلِ قبول کھاتہ", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        if (Amount <= 0)
        {
            System.Windows.MessageBox.Show("براہ کرم رقم (Amount) درج کریں۔ رقم 0 سے زیادہ ہونی چاہیے۔", "رقم درکار ہے", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        var account = await _accountService.GetAccountByNoAsync(AccountNo.Trim());
        if (account == null)
        {
            System.Windows.MessageBox.Show($"اکاؤنٹ کوڈ '{AccountNo}' سسٹم میں موجود نہیں ہے!", "اکاؤنٹ نہیں ملا", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
            return;
        }

        if (IsCashAccount(account))
        {
            System.Windows.MessageBox.Show(
                "کیش ان ہینڈ (1001 - Cash in Hand) سے کیش ان ہینڈ میں وصولی کا اندراج نہیں ہو سکتا!\nیہاں صرف ادائیگی کرنے والے گاہک یا پارٹی کا کھاتہ درج کریں۔", 
                "ناقابلِ قبول کھاتہ", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        Details.Add(new VoucherDetailViewModel
        {
            AccountId = account.Id,
            AccountNo = account.AccountNo,
            AccountName = account.Name,
            Narration = Narration,
            Credit = Amount
        });

        CalculateTotal();
        
        // Clear inputs
        AccountNo = string.Empty;
        AccountName = string.Empty;
        Narration = string.Empty;
        Amount = 0;
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
        TotalAmount = Details.Sum(d => d.Credit);
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
                    "کیش ان ہینڈ (1001 - Cash in Hand) اکاؤنٹ وصولی میں منتخب نہیں کیا جا سکتا۔", 
                    "ناقابلِ قبول کھاتہ", 
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
            System.Windows.MessageBox.Show("براہ کرم تلاش کرنے کے لیے واؤچر نمبر درج کریں!", "واؤچر نمبر درکار ہے", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        var year = await _systemService.GetCurrentYearAsync();
        var voucher = await _voucherService.GetVoucherByNoAsync(Core.Enums.VoucherType.CashReceiving, targetNo, year?.Id ?? 0);
        if (voucher != null)
        {
            LoadVoucherIntoForm(voucher);
        }
        else
        {
            System.Windows.MessageBox.Show($"واؤچر نمبر '{targetNo}' سسٹم میں نہیں ملا!", "واؤچر نہیں ملا", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task PreviousVoucher()
    {
        var year = await _systemService.GetCurrentYearAsync();
        var voucher = await _voucherService.GetPreviousVoucherAsync(Core.Enums.VoucherType.CashReceiving, CurrentVoucherId, year?.Id ?? 0);
        if (voucher != null)
        {
            LoadVoucherIntoForm(voucher);
        }
        else
        {
            System.Windows.MessageBox.Show("پچھلا کوئی واؤچر موجود نہیں ہے!", "اطلاع", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task NextVoucher()
    {
        var year = await _systemService.GetCurrentYearAsync();
        var voucher = await _voucherService.GetNextVoucherAsync(Core.Enums.VoucherType.CashReceiving, CurrentVoucherId, year?.Id ?? 0);
        if (voucher != null)
        {
            LoadVoucherIntoForm(voucher);
        }
        else
        {
            System.Windows.MessageBox.Show("اگلا کوئی واؤچر موجود نہیں ہے!", "اطلاع", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
        }
    }

    [RelayCommand]
    private async Task DeleteVoucher()
    {
        if (!IsExistingVoucher || CurrentVoucherId <= 0)
        {
            System.Windows.MessageBox.Show("حذف کرنے کے لیے پہلے کوئی محفوظ شدہ واؤچر لوڈ کریں۔", "اطلاع", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
            return;
        }

        var confirm = System.Windows.MessageBox.Show(
            $"کیا آپ واقعی واؤچر نمبر '{VoucherNo}' کو سسٹم اور لیجر سے حذف (Delete) کرنا چاہتے ہیں؟",
            "واؤچر حذف کی تصدیق",
            System.Windows.MessageBoxButton.YesNo,
            System.Windows.MessageBoxImage.Warning);

        if (confirm == System.Windows.MessageBoxResult.Yes)
        {
            await _voucherService.DeleteVoucherAsync(CurrentVoucherId);
            WeakReferenceMessenger.Default.Send(new MyMandiSystem.Core.Messages.RefreshDashboardMessage());
            System.Windows.MessageBox.Show("واؤچر کامیابی سے حذف کر دیا گیا ہے۔", "کامیابی", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Information);
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
                Credit = d.Credit
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
    private async System.Threading.Tasks.Task LookupAccount()
    {
        if (string.IsNullOrWhiteSpace(AccountNo))
        {
            OpenAccountLookup(AccountName);
            return;
        }

        if (IsCashAccount(AccountNo))
        {
            AccountName = "⚠️ نقد / روکڑ کھاتہ وصولی کے لیے منتخب نہیں ہو سکتا";
            ResetSelectedAccountInfo();
            System.Windows.MessageBox.Show(
                "کیش ان ہینڈ (1001 - Cash in Hand) سے کیش ان ہینڈ میں وصولی نہیں ہو سکتی!\nبراہ کرم ادائیگی کرنے والی پارٹی یا گاہک کا کھاتہ درج کریں۔", 
                "ناقابلِ قبول کھاتہ", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        var account = await _accountService.GetAccountByNoAsync(AccountNo.Trim());
        if (account != null)
        {
            if (IsCashAccount(account))
            {
                AccountName = "⚠️ نقد / روکڑ کھاتہ وصولی کے لیے منتخب نہیں ہو سکتا";
                ResetSelectedAccountInfo();
                System.Windows.MessageBox.Show(
                    "کیش ان ہینڈ (1001 - Cash in Hand) سے کیش ان ہینڈ میں وصولی نہیں ہو سکتی!\nبراہ کرم ادائیگی کرنے والی پارٹی یا گاہک کا کھاتہ درج کریں۔", 
                    "ناقابلِ قبول کھاتہ", 
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
    private async System.Threading.Tasks.Task Save()
    {
        // Auto-add line item if user entered data but forgot to click Add
        if (Amount > 0 && !string.IsNullOrWhiteSpace(AccountNo))
        {
            await AddDetail();
        }

        if (Details.Any(d => d.AccountNo == "1001" || (d.AccountName != null && d.AccountName.Contains("Cash in Hand", StringComparison.OrdinalIgnoreCase))))
        {
            System.Windows.MessageBox.Show(
                "واؤچر میں کیش ان ہینڈ (1001) اکاؤنٹ کی انٹری شامل ہے جو کہ درست نہیں ہے۔ براہ کرم اسے گرڈ سے ڈیلیٹ کریں۔", 
                "غلط اندراج", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Error);
            return;
        }

        if (Details.Count == 0)
        {
            System.Windows.MessageBox.Show(
                "واؤچر محفوظ کرنے سے پہلے کم از کم ایک انٹری شامل کریں۔ آپ اوپر رقم اور اکاؤنٹ لکھ کر '+ Add' کا بٹن دبا سکتے ہیں یا Enter پریس کر سکتے ہیں۔", 
                "انٹری درکار ہے", 
                System.Windows.MessageBoxButton.OK, 
                System.Windows.MessageBoxImage.Warning);
            return;
        }

        try
        {
            var year = await _systemService.GetCurrentYearAsync();
            if (year == null)
            {
                System.Windows.MessageBox.Show("مالی سال (Financial Year) نہیں مل سکا!", "خرابی", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
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
                TotalCredit = TotalAmount,
                TotalDebit = TotalAmount,
                VoucherType = Core.Enums.VoucherType.CashReceiving,
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
                    Narration = detail.Narration,
                    Credit = detail.Credit
                });
            }

            await _voucherService.SaveVoucherAsync(voucher);
            
            // Auto-post to Ledger
            await _voucherService.PostVoucherAsync(voucher.Id);

            if (AutoPrint)
            {
                var slip = _printModelFactory.BuildVoucherSlip(voucher.Id);
                _printService.Preview(slip);
            }
            else
            {
                var printPrompt = System.Windows.MessageBox.Show(
                    "Cash Receiving Voucher کامیابی سے محفوظ ہو گیا ہے۔\nکیا آپ اس کا پرنٹ نکالنا چاہتے ہیں؟", 
                    "واؤچر محفوظ ہو گیا", 
                    System.Windows.MessageBoxButton.YesNo, 
                    System.Windows.MessageBoxImage.Question);
                if (printPrompt == System.Windows.MessageBoxResult.Yes)
                {
                    var slip = _printModelFactory.BuildVoucherSlip(voucher.Id);
                    _printService.Preview(slip);
                }
            }

            // Notify Dashboard to refresh metrics live
            WeakReferenceMessenger.Default.Send(new MyMandiSystem.Core.Messages.RefreshDashboardMessage());

            // Reset form
            Clear();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Cash Receiving Voucher Save");
        }
    }

    [RelayCommand]
    private void Clear()
    {
        CurrentVoucherId = 0;
        IsExistingVoucher = false;
        SearchVoucherNo = string.Empty;
        Details.Clear();
        TotalAmount = 0;
        AccountNo = string.Empty;
        AccountName = string.Empty;
        Narration = string.Empty;
        Amount = 0;
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
        ResetSelectedAccountInfo();
        LoadInitialData();
    }

    [RelayCommand]
    private void Close(System.Windows.Window window)
    {
        if (window != null)
        {
            window.Close();
        }
    }
}

