using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Core.Enums;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using CommunityToolkit.Mvvm.Messaging;

namespace MyMandiSystem.ViewModels;

public partial class CashReceivingVoucherViewModel : ObservableObject
{
    [ObservableProperty]
    private string _voucherNo = "164";

    [ObservableProperty]
    private DateTime _voucherDate = DateTime.Today;

    [ObservableProperty]
    private string _cropSeason = "Wheat";

    [ObservableProperty]
    private string _cashInHand = "1931472";

    [ObservableProperty]
    private string _accountNo = string.Empty;

    [ObservableProperty]
    private string _accountName = string.Empty;

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

    public ObservableCollection<VoucherDetailViewModel> Details { get; } = new();

    private readonly IVoucherService _voucherService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;

    public CashReceivingVoucherViewModel(IVoucherService voucherService, IAccountService accountService, ISystemService systemService)
    {
        _voucherService = voucherService;
        _accountService = accountService;
        _systemService = systemService;
        
        LoadInitialData();
    }

    private async void LoadInitialData()
    {
        var year = await _systemService.GetCurrentYearAsync();
        if (year != null)
        {
            VoucherNo = await _voucherService.GetNextVoucherNoAsync(Core.Enums.VoucherType.CashReceiving, year.Id);
        }
    }

    partial void OnAccountNoChanged(string value)
    {
        if (string.IsNullOrEmpty(value)) 
        {
            AccountName = string.Empty;
            return;
        }

        // Use service to lookup account
        LookupAccountByNo(value);
    }

    private async void LookupAccountByNo(string no)
    {
        var account = await _accountService.GetAccountByNoAsync(no);
        if (account != null)
        {
            AccountName = account.Name;
        }
        else
        {
            AccountName = "Account Not Found";
        }
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task AddDetail()
    {
        if (string.IsNullOrEmpty(AccountNo) || Amount <= 0) return;

        var account = await _accountService.GetAccountByNoAsync(AccountNo);
        if (account == null) return;

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

    private void CalculateTotal()
    {
        TotalAmount = Details.Sum(d => d.Credit);
    }

    [RelayCommand]
    private void LookupAccount()
    {
        // TODO: Show Account Lookup Dialog
    }

    [RelayCommand]
    private async System.Threading.Tasks.Task Save()
    {
        if (Details.Count == 0) return;

        var year = await _systemService.GetCurrentYearAsync();
        if (year == null) return;

        var voucher = new Voucher
        {
            VoucherNo = VoucherNo,
            VoucherDate = VoucherDate,
            FinancialYearId = year.Id,
            CropSeasonId = CropSeason == "Wheat" ? 1 : 2, // Map to DB IDs
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
        
        // Reset form
        Clear();
        LoadInitialData(); // Refresh Voucher No
    }

    [RelayCommand]
    private void Clear()
    {
        Details.Clear();
        TotalAmount = 0;
        VoucherNo = string.Empty;
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

