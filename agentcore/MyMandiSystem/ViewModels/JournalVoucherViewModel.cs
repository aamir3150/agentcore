using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Core.Enums;
using MyMandiSystem.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class JournalVoucherViewModel : ObservableObject
{
    private readonly IVoucherService _voucherService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private string _voucherNo = string.Empty;
    [ObservableProperty] private DateTime _voucherDate = DateTime.Now;
    [ObservableProperty] private CropSeason? _selectedSeason;
    [ObservableProperty] private ObservableCollection<CropSeason> _seasons = new();
    
    [ObservableProperty] private string _accountNo = string.Empty;
    [ObservableProperty] private string _accountName = string.Empty;
    [ObservableProperty] private decimal _debit;
    [ObservableProperty] private decimal _credit;
    [ObservableProperty] private string _narration = string.Empty;
    [ObservableProperty] private string _narrationUrdu = string.Empty;

    [ObservableProperty] private ObservableCollection<VoucherDetailViewModel> _details = new();
    [ObservableProperty] private decimal _totalDebit;
    [ObservableProperty] private decimal _totalCredit;
    [ObservableProperty] private decimal _difference;

    private int? _currentAccountId;
    private readonly MyMandiSystem.Core.Printing.IPrintService _printService;
    private readonly MyMandiSystem.Core.Printing.IPrintModelFactory _printModelFactory;

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

    private async void LoadInitialData()
    {
        var seasonsList = await _systemService.GetActiveSeasonsAsync();
        Seasons = new ObservableCollection<CropSeason>(seasonsList);
        SelectedSeason = Seasons.FirstOrDefault(s => s.IsActive);
        var year = await _systemService.GetCurrentYearAsync();
        if (year != null)
        {
            VoucherNo = await _voucherService.GetNextVoucherNoAsync(VoucherType.JournalVoucher, year.Id);
        }
    }

    partial void OnAccountNoChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AccountName = string.Empty;
            _currentAccountId = null;
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
            }
            else
            {
                AccountName = string.Empty;
                _currentAccountId = null;
            }
        }
    }

    [RelayCommand]
    private async Task LookupAccount()
    {
        if (string.IsNullOrWhiteSpace(AccountNo)) return;

        var account = await _accountService.GetAccountByNoAsync(AccountNo.Trim());
        if (account != null)
        {
            AccountName = account.Name;
            _currentAccountId = account.Id;
        }
        else
        {
            AccountName = "Account Not Found";
            _currentAccountId = null;
        }
    }

    [RelayCommand]
    private void AddDetail()
    {
        if (_currentAccountId == null || (Debit == 0 && Credit == 0)) return;
        if (Debit > 0 && Credit > 0)
        {
            System.Windows.MessageBox.Show("A single line cannot have both Debit and Credit.", "Validation Error");
            return;
        }

        Details.Add(new VoucherDetailViewModel
        {
            AccountId = _currentAccountId.Value,
            AccountNo = AccountNo,
            AccountName = AccountName,
            Narration = Narration,
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
    }

    [RelayCommand]
    private async Task Save()
    {
        if (!Details.Any()) return;
        if (TotalDebit != TotalCredit)
        {
            System.Windows.MessageBox.Show("Total Debit must equal Total Credit for a Journal Voucher.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var year = await _systemService.GetCurrentYearAsync();
            var voucher = new Voucher
            {
                VoucherNo = VoucherNo,
                VoucherType = VoucherType.JournalVoucher,
                VoucherDate = VoucherDate,
                FinancialYearId = year?.Id ?? 1,
                CropSeasonId = (SelectedSeason != null && SelectedSeason.Id > 0) ? SelectedSeason.Id : null,
                NarrationUrdu = NarrationUrdu,
                TotalDebit = TotalDebit,
                TotalCredit = TotalCredit,
                Details = Details.Select(d => new VoucherDetail
                {
                    AccountId = d.AccountId,
                    Debit = d.Debit,
                    Credit = d.Credit,
                    Narration = d.Narration
                }).ToList()
            };

            await _voucherService.SaveVoucherAsync(voucher);
            await _voucherService.PostVoucherAsync(voucher.Id);

            var printPrompt = System.Windows.MessageBox.Show(
                "Journal Voucher کامیابی سے محفوظ ہو گیا ہے۔\nکیا آپ دستخط کے لیے واؤچر سلپ پرنٹ کرنا چاہتے ہیں؟", 
                "واؤچر پرنٹ کریں؟", 
                MessageBoxButton.YesNo, 
                MessageBoxImage.Question);

            if (printPrompt == MessageBoxResult.Yes)
            {
                var slip = _printModelFactory.BuildVoucherSlip(voucher.Id);
                _printService.Preview(slip);
            }

            ClearAll();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Journal Voucher Save");
        }
    }

    [RelayCommand]
    private void Print()
    {
        if (!Details.Any())
        {
            System.Windows.MessageBox.Show("واؤچر پرنٹ کرنے کے لیے کم از کم ایک انٹری شامل ہونی چاہیے!", "اطلاع", MessageBoxButton.OK, MessageBoxImage.Information);
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
    private void ClearAll()
    {
        Details.Clear();
        NarrationUrdu = string.Empty;
        UpdateTotals();
        ClearEntryFields();
        LoadInitialData();
    }

    [RelayCommand]
    private void Close(Window window)
    {
        window?.Close();
    }
}

