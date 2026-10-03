using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Core.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class CashPaymentVoucherViewModel : ObservableObject
{
    private readonly IVoucherService _voucherService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private string _voucherNo = string.Empty;
    [ObservableProperty] private DateTime _voucherDate = DateTime.Today;
    [ObservableProperty] private CropSeason? _selectedSeason;
    [ObservableProperty] private ObservableCollection<CropSeason> _seasons = new();
    
    [ObservableProperty] private string _accountNo = string.Empty;
    [ObservableProperty] private string _accountName = string.Empty;
    [ObservableProperty] private string _narration = string.Empty;
    [ObservableProperty] private decimal _amount;
    [ObservableProperty] private string _narrationUrdu = string.Empty;
    [ObservableProperty] private decimal _totalAmount;

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

    public ObservableCollection<VoucherDetailViewModel> Details { get; } = new();

    private int? _currentAccountId;

    public CashPaymentVoucherViewModel(
        IVoucherService voucherService, 
        IAccountService accountService, 
        ISystemService systemService)
    {
        _voucherService = voucherService;
        _accountService = accountService;
        _systemService = systemService;
        
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
            VoucherNo = await _voucherService.GetNextVoucherNoAsync(VoucherType.CashPayment, year.Id);
        }
    }

    [RelayCommand]
    private async Task LookupAccount()
    {
        if (string.IsNullOrWhiteSpace(AccountNo)) return;

        var account = await _accountService.GetAccountByNoAsync(AccountNo);
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
        if (_currentAccountId == null || Amount <= 0) return;

        Details.Add(new VoucherDetailViewModel
        {
            AccountId = _currentAccountId.Value,
            AccountNo = AccountNo,
            AccountName = AccountName,
            Narration = Narration,
            Debit = Amount // Payments DEBIT the target account
        });

        CalculateTotal();
        ClearEntryFields();
    }

    private void CalculateTotal()
    {
        TotalAmount = Details.Sum(d => d.Debit);
    }

    private void ClearEntryFields()
    {
        AccountNo = string.Empty;
        AccountName = string.Empty;
        Narration = string.Empty;
        Amount = 0;
        _currentAccountId = null;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (!Details.Any()) return;

        var year = await _systemService.GetCurrentYearAsync();
        if (year == null) return;

        try
        {
            var voucher = new Voucher
            {
                VoucherNo = VoucherNo,
                VoucherType = VoucherType.CashPayment,
                VoucherDate = VoucherDate,
                FinancialYearId = year.Id,
                CropSeasonId = SelectedSeason?.Id ?? 0,
                TotalDebit = TotalAmount,
                TotalCredit = TotalAmount,
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
                Count1 = Count1,
                Details = Details.Select(d => new VoucherDetail
                {
                    AccountId = d.AccountId,
                    Debit = d.Debit,
                    Credit = 0,
                    Narration = d.Narration
                }).ToList()
            };

            await _voucherService.SaveVoucherAsync(voucher);
            await _voucherService.PostVoucherAsync(voucher.Id);
            
            System.Windows.MessageBox.Show("Cash Payment Voucher saved and posted successfully!", "Success");
            ClearAll();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error saving voucher: {ex.Message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void ClearAll()
    {
        Details.Clear();
        TotalAmount = 0;
        NarrationUrdu = string.Empty;
        ClearEntryFields();
        LoadInitialData();
    }

    [RelayCommand]
    private void Close(Window window)
    {
        window?.Close();
    }
}
