using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Views;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public enum MarkupCalculatorMode
{
    BankMarkup,
    PartyMarkup
}

public partial class MarkupCalculatorViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private MarkupCalculatorMode _mode = MarkupCalculatorMode.BankMarkup;
    [ObservableProperty] private string _windowTitle = "Bank Markup Calculator";
    [ObservableProperty] private string _headerTitle = "Markup Calculator";
    [ObservableProperty] private string _headerSubtitle = "مارک اپ کیلکولیٹر (سود / منافع تخمینہ رپورٹ)";

    // Account / Party
    [ObservableProperty] private string _accountNo = "";
    [ObservableProperty] private string _accountName = "";
    [ObservableProperty] private Account? _selectedAccount;
    [ObservableProperty] private ObservableCollection<Account> _accounts = new();
    [ObservableProperty] private ObservableCollection<Account> _filteredAccounts = new();

    // Date Range
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Rates & Calculation
    [ObservableProperty] private decimal _annualRate = 12.0m; // Default 12%
    [ObservableProperty] private int _totalDays = 365;
    [ObservableProperty] private decimal _principalBalance = 0m;
    [ObservableProperty] private decimal _calculatedMarkup = 0m;
    [ObservableProperty] private decimal _totalWithMarkup = 0m;

    // Lookup Modal
    [ObservableProperty] private bool _isAccountLookupOpen = false;
    [ObservableProperty] private string _lookupSearchQuery = "";

    public Action? RequestClose { get; set; }

    public MarkupCalculatorViewModel(IReportService reportService, IAccountService accountService, ISystemService systemService)
    {
        _reportService = reportService;
        _accountService = accountService;
        _systemService = systemService;

        _ = InitializeDataAsync();
    }

    public async Task SetModeAsync(MarkupCalculatorMode mode)
    {
        Mode = mode;
        if (mode == MarkupCalculatorMode.BankMarkup)
        {
            WindowTitle = "Bank Markup Calculator - بینک مارک اپ کیلکولیٹر";
            HeaderTitle = "Bank Markup Calculator";
            HeaderSubtitle = "بینک مارک اپ تخمینہ (بینک بیلنس پر منافع / مارک اپ)";
        }
        else
        {
            WindowTitle = "Party Markup Calculator - پارٹی مارک اپ کیلکولیٹر";
            HeaderTitle = "Party Markup Calculator";
            HeaderSubtitle = "پارٹی مارک اپ تخمینہ (کھاتہ دار کے بیلنس پر مارک اپ)";
        }
        await InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        var allAccounts = await _accountService.GetAllAccountsAsync();
        if (Mode == MarkupCalculatorMode.BankMarkup)
        {
            var banks = allAccounts.Where(a => 
                a.AccountGroup?.Name?.Contains("Bank", StringComparison.OrdinalIgnoreCase) == true ||
                a.Name.Contains("Bank", StringComparison.OrdinalIgnoreCase) ||
                a.AccountNo.StartsWith("01-02") || a.AccountNo.StartsWith("01-01")
            ).ToList();
            Accounts = new ObservableCollection<Account>(banks.Any() ? banks : allAccounts);
        }
        else
        {
            // Parties
            var parties = allAccounts.Where(a => a.AccountType == AccountType.Asset || a.AccountType == AccountType.Liability).ToList();
            Accounts = new ObservableCollection<Account>(parties.Any() ? parties : allAccounts);
        }

        FilteredAccounts = new ObservableCollection<Account>(Accounts);
        if (Accounts.Any())
        {
            SelectedAccount = Accounts.First();
            AccountNo = SelectedAccount.AccountNo;
            AccountName = SelectedAccount.Name;
        }

        var year = await _systemService.GetCurrentYearAsync();
        if (year != null)
        {
            FromDate = year.StartDate;
            ToDate = DateTime.Today < year.EndDate ? DateTime.Today : year.EndDate;
        }

        RecalculateDays();
    }

    partial void OnFromDateChanged(DateTime value) => RecalculateDays();
    partial void OnToDateChanged(DateTime value) => RecalculateDays();
    partial void OnAnnualRateChanged(decimal value) => RecalculateMarkup();
    partial void OnPrincipalBalanceChanged(decimal value) => RecalculateMarkup();

    private void RecalculateDays()
    {
        if (ToDate >= FromDate)
        {
            TotalDays = (int)(ToDate - FromDate).TotalDays + 1;
        }
        else
        {
            TotalDays = 0;
        }
        RecalculateMarkup();
    }

    private void RecalculateMarkup()
    {
        if (TotalDays > 0 && AnnualRate > 0 && PrincipalBalance > 0)
        {
            // Daily markup formula: (Balance * Rate / 100) * (Days / 365)
            CalculatedMarkup = Math.Round((PrincipalBalance * (AnnualRate / 100m)) * ((decimal)TotalDays / 365m), 2);
            TotalWithMarkup = PrincipalBalance + CalculatedMarkup;
        }
        else
        {
            CalculatedMarkup = 0m;
            TotalWithMarkup = PrincipalBalance;
        }
    }

    partial void OnAccountNoChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            AccountName = "";
            SelectedAccount = null;
            PrincipalBalance = 0m;
            return;
        }

        var match = FilteredAccounts.FirstOrDefault(a => a.AccountNo.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
        if (match != null)
        {
            SelectedAccount = match;
            AccountName = match.Name;
            PrincipalBalance = Math.Abs(match.CurrentBalance);
        }
    }

    [RelayCommand]
    private void OpenAccountLookup()
    {
        LookupSearchQuery = "";
        FilteredAccounts = new ObservableCollection<Account>(Accounts);
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
            PrincipalBalance = Math.Abs(account.CurrentBalance);
        }
        IsAccountLookupOpen = false;
    }

    [RelayCommand]
    private void CloseAccountLookup() => IsAccountLookupOpen = false;

    [RelayCommand]
    private async Task PreviewAsync()
    {
        if (SelectedAccount == null)
        {
            MessageBox.Show("براہ کرم کھاتہ منتخب کریں!", "معلومات", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var data = await _reportService.GetLedgerReportAsync(SelectedAccount.Id, FromDate.Date, ToDate.Date.AddDays(1).AddSeconds(-1));
        var reportWindow = new ReportWindow(
            reportPath: "Reports/LedgerReport.rdlc",
            dataSourceName: "DS_Ledger",
            data: data
        );
        reportWindow.Title = $"{HeaderTitle} - {SelectedAccount.AccountNo} ({SelectedAccount.Name}) [Rate: {AnnualRate}%, Markup: Rs. {CalculatedMarkup:N2}]";
        reportWindow.ShowDialog();
    }

    [RelayCommand]
    private async Task PrintAsync() => await PreviewAsync();

    [RelayCommand]
    private void Close() => RequestClose?.Invoke();
}
