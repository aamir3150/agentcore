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

    public JournalVoucherViewModel(
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
            VoucherNo = await _voucherService.GetNextVoucherNoAsync(VoucherType.JournalVoucher, year.Id);
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
            var voucher = new Voucher
            {
                VoucherNo = VoucherNo,
                VoucherType = VoucherType.JournalVoucher,
                VoucherDate = VoucherDate,
                CropSeasonId = SelectedSeason?.Id ?? 0,
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
            System.Windows.MessageBox.Show("Journal Voucher saved successfully!", "Success");
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

