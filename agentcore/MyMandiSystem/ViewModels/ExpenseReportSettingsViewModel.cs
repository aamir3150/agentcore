using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class ExpenseReportSettingsViewModel : ObservableObject
{
    private readonly IAccountService _accountService;
    private List<AccountReportSettingDto> _allAccounts = new();

    [ObservableProperty] private ObservableCollection<AccountReportSettingDto> _filteredAccounts = new();
    [ObservableProperty] private AccountReportSettingDto? _selectedAccount;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _accountTypes = new() 
    { 
        "All Accounts", 
        "All Expenses", 
        "Direct Expense", 
        "Indirect Expense", 
        "Administrative Expense", 
        "Selling / Marketing", 
        "Financial Charges" 
    };
    [ObservableProperty] private string _selectedAccountType = "All Expenses";
    [ObservableProperty] private bool _onlyExpenseAccounts = true;
    [ObservableProperty] private decimal _batchExpenseLimit = 0;
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _statusMessage = string.Empty;

    // Metrics summary badges (AgenSoft ExpSettings Parity)
    [ObservableProperty] private int _totalAccountsCount = 0;
    [ObservableProperty] private int _includedExpCount = 0;
    [ObservableProperty] private decimal _totalBudgetLimit = 0;

    public Action? RequestClose { get; set; }

    public ExpenseReportSettingsViewModel(IAccountService accountService)
    {
        _accountService = accountService;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    public async Task LoadDataAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading Expense report configuration...";
            var list = await _accountService.GetAccountReportSettingsAsync();
            _allAccounts = list.ToList();
            TotalAccountsCount = _allAccounts.Count;
            ApplyFilter();
            UpdateCounts();
            StatusMessage = $"Loaded {_allAccounts.Count} account(s) from Chart of Accounts.";
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading accounts: {ex.Message}";
            FriendlyErrorHandler.ShowError(ex, "Load Expense Settings");
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnSelectedAccountTypeChanged(string value) => ApplyFilter();
    partial void OnOnlyExpenseAccountsChanged(bool value) => ApplyFilter();

    private void ApplyFilter()
    {
        var query = _allAccounts.AsEnumerable();

        if (OnlyExpenseAccounts)
        {
            query = query.Where(a => (a.AccountType != null && a.AccountType.Contains("Expense", StringComparison.OrdinalIgnoreCase))
                                  || (a.AccountNo != null && a.AccountNo.StartsWith("5"))
                                  || a.ExpFlag);
        }

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var txt = SearchText.Trim().ToLower();
            query = query.Where(a => (a.AccountNo != null && a.AccountNo.ToLower().Contains(txt)) 
                                  || (a.AccountName != null && a.AccountName.ToLower().Contains(txt))
                                  || (a.AccountType != null && a.AccountType.ToLower().Contains(txt)));
        }

        if (SelectedAccountType != "All Accounts")
        {
            if (SelectedAccountType == "All Expenses")
            {
                query = query.Where(a => (a.AccountType != null && a.AccountType.Contains("Expense", StringComparison.OrdinalIgnoreCase))
                                      || (a.AccountNo != null && a.AccountNo.StartsWith("5")));
            }
            else if (SelectedAccountType == "Direct Expense")
            {
                query = query.Where(a => a.AccountName.Contains("Direct", StringComparison.OrdinalIgnoreCase) 
                                      || a.AccountType.Contains("Direct", StringComparison.OrdinalIgnoreCase));
            }
            else if (SelectedAccountType == "Indirect Expense")
            {
                query = query.Where(a => a.AccountName.Contains("Indirect", StringComparison.OrdinalIgnoreCase) 
                                      || a.AccountType.Contains("Indirect", StringComparison.OrdinalIgnoreCase));
            }
            else if (SelectedAccountType == "Administrative Expense")
            {
                query = query.Where(a => a.AccountName.Contains("Admin", StringComparison.OrdinalIgnoreCase) 
                                      || a.AccountName.Contains("Office", StringComparison.OrdinalIgnoreCase));
            }
            else if (SelectedAccountType == "Selling / Marketing")
            {
                query = query.Where(a => a.AccountName.Contains("Sell", StringComparison.OrdinalIgnoreCase) 
                                      || a.AccountName.Contains("Market", StringComparison.OrdinalIgnoreCase));
            }
            else if (SelectedAccountType == "Financial Charges")
            {
                query = query.Where(a => a.AccountName.Contains("Bank", StringComparison.OrdinalIgnoreCase) 
                                      || a.AccountName.Contains("Markup", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountName.Contains("Tax", StringComparison.OrdinalIgnoreCase));
            }
        }

        FilteredAccounts = new ObservableCollection<AccountReportSettingDto>(query);
        UpdateCounts();
    }

    private void UpdateCounts()
    {
        IncludedExpCount = _allAccounts.Count(a => a.ExpFlag);
        TotalBudgetLimit = _allAccounts.Where(a => a.ExpFlag && a.ExpenseLimit.HasValue).Sum(a => a.ExpenseLimit!.Value);
    }

    // AgenSoft CmdAuto Parity
    [RelayCommand]
    public void AutoConfigure()
    {
        int modified = 0;
        foreach (var a in _allAccounts)
        {
            var typeLow = (a.AccountType ?? "").ToLower();
            var nameLow = (a.AccountName ?? "").ToLower();
            var no = a.AccountNo ?? "";

            // Expense accounts: Type contains Expense or code starts with 5
            if (typeLow.Contains("exp") || nameLow.Contains("expense") || no.StartsWith("5"))
            {
                if (!a.ExpFlag)
                {
                    a.ExpFlag = true;
                    modified++;
                }
            }
            else
            {
                if (a.ExpFlag && (no.StartsWith("1") || no.StartsWith("2") || no.StartsWith("3") || no.StartsWith("4")))
                {
                    a.ExpFlag = false;
                    modified++;
                }
            }
        }

        UpdateCounts();
        ApplyFilter();
        StatusMessage = $"Auto configured {modified} expense account(s). Click 'Save (F2)' to persist.";
        MessageBox.Show($"Auto configuration completed.\n\nClassified all general ledger expense accounts with ExpFlag = 1 for Expense Reports.\n\nClick 'Save (F2)' to save settings in Chart of Accounts.", "Auto Configuration (CmdAuto)", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    public void SelectAllExp()
    {
        foreach (var a in FilteredAccounts)
        {
            a.ExpFlag = true;
        }
        UpdateCounts();
        StatusMessage = $"Selected all {FilteredAccounts.Count} filtered account(s) for Expense Reports.";
    }

    [RelayCommand]
    public void UnselectAllExp()
    {
        foreach (var a in FilteredAccounts)
        {
            a.ExpFlag = false;
        }
        UpdateCounts();
        StatusMessage = $"Unselected all {FilteredAccounts.Count} filtered account(s) from Expense Reports.";
    }

    [RelayCommand]
    public void ApplyBatchLimit()
    {
        if (BatchExpenseLimit <= 0)
        {
            MessageBox.Show("Please enter a valid budget limit greater than zero to apply.", "Limit Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        int count = 0;
        foreach (var a in FilteredAccounts)
        {
            a.ExpenseLimit = BatchExpenseLimit;
            a.ExpFlag = true;
            count++;
        }

        UpdateCounts();
        StatusMessage = $"Applied budget limit of Rs. {BatchExpenseLimit:N2} to {count} account(s). Click 'Save (F2)' to persist.";
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Saving Expense report settings to ChartOfAccounts...";
            await _accountService.UpdateExpenseReportSettingsAsync(_allAccounts);
            StatusMessage = "Expense report settings and budget limits saved successfully in database.";
            MessageBox.Show("Expense report settings and budget limits saved successfully in database (ChartOfAccounts.ExpFlag & ExpenseLimit updated).", 
                "Saved Successfully (F2)", MessageBoxButton.OK, MessageBoxImage.Information);
            RequestClose?.Invoke();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save failed: {ex.Message}";
            FriendlyErrorHandler.ShowError(ex, "Save Expense Settings");
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void Close()
    {
        RequestClose?.Invoke();
    }
}
