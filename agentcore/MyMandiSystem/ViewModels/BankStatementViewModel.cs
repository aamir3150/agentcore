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

public enum BankReportMode
{
    Statement,
    Position,
    ChequesRegister,
    UnclearedCheques,
    LostCheques,
    BouncedCheques
}

public partial class BankStatementViewModel : ObservableObject
{
    private readonly IReportService _reportService;
    private readonly IAccountService _accountService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private BankReportMode _mode = BankReportMode.Statement;
    [ObservableProperty] private string _windowTitle = "Bank Statement";
    [ObservableProperty] private string _headerTitle = "Bank Statement";
    [ObservableProperty] private string _headerSubtitle = "Bank Account Statement & Transaction History";

    // Date Range
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    // Account Selection
    [ObservableProperty] private string _accountNo = "";
    [ObservableProperty] private string _accountName = "";
    [ObservableProperty] private Account? _selectedAccount;
    [ObservableProperty] private ObservableCollection<Account> _bankAccounts = new();

    // Options
    [ObservableProperty] private bool _includeAllVouchers = true; // "Include Journal, Debit & Credit Vouchers also"
    [ObservableProperty] private bool _urduPrint = false;

    // Lookup Modal
    [ObservableProperty] private bool _isAccountLookupOpen = false;
    [ObservableProperty] private string _lookupSearchQuery = "";
    [ObservableProperty] private ObservableCollection<Account> _filteredAccounts = new();

    public Action? RequestClose { get; set; }

    public void SetMode(BankReportMode mode)
    {
        Mode = mode;
        switch (mode)
        {
            case BankReportMode.Statement:
                WindowTitle = "Bank Statement";
                HeaderTitle = "Bank Statement";
                HeaderSubtitle = "Bank Account Statement & Transaction History";
                break;
            case BankReportMode.Position:
                WindowTitle = "Bank Position";
                HeaderTitle = "Bank Position";
                HeaderSubtitle = "Bank Position Summary (Total Balance, Cleared & Uncleared Cheques)";
                break;
            case BankReportMode.ChequesRegister:
                WindowTitle = "Bank Cheques Register";
                HeaderTitle = "Bank Cheques Register";
                HeaderSubtitle = "Detailed Register of Issued & Deposited Cheques";
                break;
            case BankReportMode.UnclearedCheques:
                WindowTitle = "Uncleared Cheques";
                HeaderTitle = "Uncleared Cheques";
                HeaderSubtitle = "Uncleared Bank Cheques Report";
                break;
            case BankReportMode.LostCheques:
                WindowTitle = "Lost Cheques";
                HeaderTitle = "Lost Cheques";
                HeaderSubtitle = "Lost & Cancelled Bank Cheques Register";
                break;
            case BankReportMode.BouncedCheques:
                WindowTitle = "Bounced Cheques";
                HeaderTitle = "Bounced Cheques";
                HeaderSubtitle = "Bounced & Dishonoured Bank Cheques Report";
                break;
        }
    }

    public BankStatementViewModel(IReportService reportService, IAccountService accountService, ISystemService systemService)
    {
        _reportService = reportService;
        _accountService = accountService;
        _systemService = systemService;

        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        var allAccounts = await _accountService.GetAllAccountsAsync();
        // Bank accounts usually have head asset/bank or specific prefix, or include all accounts
        var banks = allAccounts.Where(a => 
            a.AccountGroup?.Name?.Contains("Bank", StringComparison.OrdinalIgnoreCase) == true ||
            a.Name.Contains("Bank", StringComparison.OrdinalIgnoreCase) ||
            a.AccountNo.StartsWith("01-02") || a.AccountNo.StartsWith("01-01")
        ).ToList();

        if (!banks.Any())
        {
            banks = allAccounts.ToList();
        }

        BankAccounts = new ObservableCollection<Account>(banks);
        FilteredAccounts = new ObservableCollection<Account>(allAccounts);

        // Auto select first bank if available
        if (BankAccounts.Any())
        {
            SelectedAccount = BankAccounts.First();
            AccountNo = SelectedAccount.AccountNo;
            AccountName = SelectedAccount.Name;
        }

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

        var match = FilteredAccounts.FirstOrDefault(a => a.AccountNo.Equals(value.Trim(), StringComparison.OrdinalIgnoreCase));
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
            FilteredAccounts = new ObservableCollection<Account>(BankAccounts);
        }
        else
        {
            var query = value.ToLower().Trim();
            var matches = BankAccounts.Where(a => 
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
        FilteredAccounts = new ObservableCollection<Account>(BankAccounts);
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
            System.Windows.MessageBox.Show("Please select a Bank Account (Account No) first.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (SelectedAccount == null)
        {
            var match = FilteredAccounts.FirstOrDefault(a => a.AccountNo.Equals(AccountNo.Trim(), StringComparison.OrdinalIgnoreCase));
            if (match == null)
            {
                System.Windows.MessageBox.Show($"Account '{AccountNo}' not found.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            SelectedAccount = match;
            AccountName = match.Name;
        }

        var data = await _reportService.GetBankStatementReportAsync(
            SelectedAccount.Id, 
            FromDate.Date, 
            ToDate.Date.AddDays(1).AddSeconds(-1), 
            IncludeAllVouchers);

        var reportWindow = new ReportWindow(
            reportPath: "Reports/BankStatementReport.rdlc",
            dataSourceName: "DS_BankStatement",
            data: data
        );
        reportWindow.Title = $"Bank Statement - {SelectedAccount.AccountNo} ({SelectedAccount.Name})";
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
