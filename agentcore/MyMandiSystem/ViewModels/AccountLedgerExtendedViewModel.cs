using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class AccountLedgerExtendedViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;

    // Filters
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = "All Crops";
    [ObservableProperty] private bool _withSummary = true;
    [ObservableProperty] private bool _showGrouping = false;

    // Account
    [ObservableProperty] private string _accountNo = "";
    [ObservableProperty] private string _accountName = "";
    [ObservableProperty] private Account? _selectedAccount;
    [ObservableProperty] private ObservableCollection<Account> _accountsList = new();

    // Date Options
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isSingleDate = false;
    [ObservableProperty] private bool _isDateRange = false;

    [ObservableProperty] private DateTime _singleDate = DateTime.Today;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Additional Options
    [ObservableProperty] private bool _withoutOpening = false;
    [ObservableProperty] private bool _showFullJVNarration = false;
    [ObservableProperty] private bool _hideInvoiceNarration = false;
    [ObservableProperty] private bool _urduPrint = false;

    // Lookup Modal Visibility
    [ObservableProperty] private bool _isAccountLookupOpen = false;
    [ObservableProperty] private string _lookupSearchQuery = "";
    [ObservableProperty] private ObservableCollection<Account> _filteredAccounts = new();

    public Action? RequestClose { get; set; }

    public AccountLedgerExtendedViewModel(IReportService reportService, IAccountService accountService, ISystemService systemService)
    {
        _reportService = reportService;
        _accountService = accountService;
        _systemService = systemService;

        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        // Load Seasons
        var seasons = new List<string> { "All Crops", "Kharif 2025", "Rabi 2025-26", "Kharif 2026" };
        CropSeasons = new ObservableCollection<string>(seasons);
        SelectedCropSeason = "All Crops";

        // Load Accounts
        var accounts = await _accountService.GetAllAccountsAsync();
        AccountsList = new ObservableCollection<Account>(accounts);
        FilteredAccounts = new ObservableCollection<Account>(accounts);

        // Set financial year dates
        var year = await _systemService.GetCurrentYearAsync();
        if (year != null)
        {
            FromDate = year.StartDate;
            ToDate = DateTime.Today < year.EndDate ? DateTime.Today : year.EndDate;
        }
    }

    partial void OnAccountNoChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AccountName = "";
            SelectedAccount = null;
            return;
        }

        var match = AccountsList.FirstOrDefault(a => a.AccountNo.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            SelectedAccount = match;
            AccountName = match.Name;
        }
    }

    partial void OnLookupSearchQueryChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            FilteredAccounts = new ObservableCollection<Account>(AccountsList);
        }
        else
        {
            var query = value.ToLower().Trim();
            var matches = AccountsList.Where(a => 
                (a.AccountNo != null && a.AccountNo.ToLower().Contains(query)) ||
                (a.Name != null && a.Name.ToLower().Contains(query)) ||
                (a.UrduName != null && a.UrduName.Contains(query)));
            FilteredAccounts = new ObservableCollection<Account>(matches);
        }
    }

    [RelayCommand]
    private void OpenAccountLookup()
    {
        LookupSearchQuery = "";
        FilteredAccounts = new ObservableCollection<Account>(AccountsList);
        IsAccountLookupOpen = true;
    }

    [RelayCommand]
    private void SelectAccountFromLookup(Account? account)
    {
        if (account != null)
        {
            SelectedAccount = account;
            AccountNo = account.AccountNo;
            AccountName = account.Name;
        }
        IsAccountLookupOpen = false;
    }

    [RelayCommand]
    private void CloseAccountLookup()
    {
        IsAccountLookupOpen = false;
    }

    [RelayCommand]
    private async Task PreviewAsync()
    {
        if (SelectedAccount == null && string.IsNullOrWhiteSpace(AccountNo))
        {
            System.Windows.MessageBox.Show("Please select an Account (A/c No.) first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedAccount == null)
        {
            var match = AccountsList.FirstOrDefault(a => a.AccountNo.Equals(AccountNo.Trim(), StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                System.Windows.MessageBox.Show($"Account '{AccountNo}' not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SelectedAccount = match;
            AccountName = match.Name;
        }

        DateTime from = IsAllDates ? DateTime.MinValue : (IsSingleDate ? SingleDate.Date : FromDate.Date);
        DateTime to = IsAllDates ? DateTime.MaxValue : (IsSingleDate ? SingleDate.Date.AddDays(1).AddSeconds(-1) : ToDate.Date.AddDays(1).AddSeconds(-1));

        var data = await _reportService.GetLedgerReportAsync(SelectedAccount.Id, from, to);

        var reportWindow = new ReportWindow(
            reportPath: "Reports/LedgerReport.rdlc",
            dataSourceName: "DS_Ledger",
            data: data
        );
        reportWindow.Title = $"Account Ledger - {SelectedAccount.AccountNo} ({SelectedAccount.Name})";
        reportWindow.ShowDialog();
    }

    [RelayCommand]
    private async Task PrintAsync()
    {
        await PreviewAsync();
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
