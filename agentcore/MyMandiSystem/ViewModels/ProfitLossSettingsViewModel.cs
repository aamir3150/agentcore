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

public partial class ProfitLossSettingsViewModel : ObservableObject
{
    private readonly IAccountService _accountService;
    private List<AccountReportSettingDto> _allAccounts = new();

    [ObservableProperty] private ObservableCollection<AccountReportSettingDto> _filteredAccounts = new();
    [ObservableProperty] private AccountReportSettingDto? _selectedAccount;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _accountTypes = new() { "All", "Revenue", "Cost of Sales / Purchases", "Expenses", "Assets", "Liabilities", "Capital / Equity" };
    [ObservableProperty] private string _selectedAccountType = "All";
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _statusMessage = string.Empty;

    // Metrics summary badges (AgenSoft Parity)
    [ObservableProperty] private int _totalAccountsCount = 0;
    [ObservableProperty] private int _includedPLCount = 0;
    [ObservableProperty] private int _bothPLAndBSCount = 0;

    public Action? RequestClose { get; set; }

    public ProfitLossSettingsViewModel(IAccountService accountService)
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
            StatusMessage = "Loading Chart of Accounts report configuration...";
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
            FriendlyErrorHandler.ShowError(ex, "Load P&L Settings");
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSearchTextChanged(string value) => ApplyFilter();
    partial void OnSelectedAccountTypeChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var query = _allAccounts.AsEnumerable();

        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var txt = SearchText.Trim().ToLower();
            query = query.Where(a => (a.AccountNo != null && a.AccountNo.ToLower().Contains(txt)) 
                                  || (a.AccountName != null && a.AccountName.ToLower().Contains(txt))
                                  || (a.AccountType != null && a.AccountType.ToLower().Contains(txt)));
        }

        if (SelectedAccountType != "All")
        {
            if (SelectedAccountType.Contains("Revenue"))
            {
                query = query.Where(a => a.AccountType.Contains("Revenue", StringComparison.OrdinalIgnoreCase) 
                                      || a.AccountType.Contains("Income", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountNo.StartsWith("4"));
            }
            else if (SelectedAccountType.Contains("Cost of Sales"))
            {
                query = query.Where(a => a.AccountType.Contains("Purchase", StringComparison.OrdinalIgnoreCase) 
                                      || a.AccountType.Contains("Cost", StringComparison.OrdinalIgnoreCase));
            }
            else if (SelectedAccountType.Contains("Expense"))
            {
                query = query.Where(a => a.AccountType.Contains("Expense", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountNo.StartsWith("5"));
            }
            else if (SelectedAccountType.Contains("Asset"))
            {
                query = query.Where(a => a.AccountType.Contains("Asset", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountNo.StartsWith("1"));
            }
            else if (SelectedAccountType.Contains("Liabilit"))
            {
                query = query.Where(a => a.AccountType.Contains("Liabilit", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountNo.StartsWith("2"));
            }
            else if (SelectedAccountType.Contains("Capital"))
            {
                query = query.Where(a => a.AccountType.Contains("Capital", StringComparison.OrdinalIgnoreCase) 
                                      || a.AccountType.Contains("Equity", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountNo.StartsWith("3"));
            }
        }

        FilteredAccounts = new ObservableCollection<AccountReportSettingDto>(query);
        UpdateCounts();
    }

    private void UpdateCounts()
    {
        IncludedPLCount = _allAccounts.Count(a => a.IsPLIncluded);
        BothPLAndBSCount = _allAccounts.Count(a => a.IsPLBoth);
    }

    // AgenSoft CmdAuto Parity
    [RelayCommand]
    public void AutoConfigure()
    {
        int modified = 0;
        foreach (var a in _allAccounts)
        {
            var typeLow = (a.AccountType ?? "").ToLower();
            var no = a.AccountNo ?? "";

            // Revenue / Income / Purchases / Expenses are P&L items
            if (typeLow.Contains("rev") || typeLow.Contains("inc") || typeLow.Contains("exp") || 
                typeLow.Contains("pur") || typeLow.Contains("cost") || no.StartsWith("4") || no.StartsWith("5"))
            {
                if (a.PLFlag != "P" && a.PLFlag != "B")
                {
                    a.PLFlag = "P";
                    modified++;
                }
            }
            else
            {
                // Balance Sheet items (Assets, Liabilities, Capital)
                if (a.PLFlag == "P")
                {
                    a.PLFlag = "";
                    modified++;
                }
            }
        }

        UpdateCounts();
        ApplyFilter();
        StatusMessage = $"Auto configured {modified} account mapping(s) based on standard Chart of Accounts classification. Click 'Save (F2)' to persist.";
        MessageBox.Show($"Auto configuration completed.\n\nCategorized standard Revenue, Cost of Goods Sold, and Expenses into Trading and Profit & Loss.\n\nClick 'Save (F2)' to save settings in Chart of Accounts.", "Auto Configuration (CmdAuto)", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    public void SelectAllPL()
    {
        foreach (var a in FilteredAccounts)
        {
            a.IsPLIncluded = true;
        }
        UpdateCounts();
        StatusMessage = $"Selected all {FilteredAccounts.Count} filtered account(s) for Profit & Loss Statement.";
    }

    [RelayCommand]
    public void UnselectAllPL()
    {
        foreach (var a in FilteredAccounts)
        {
            a.IsPLIncluded = false;
        }
        UpdateCounts();
        StatusMessage = $"Unselected all {FilteredAccounts.Count} filtered account(s) from Profit & Loss Statement.";
    }

    [RelayCommand]
    public void SetFilteredAsBoth()
    {
        foreach (var a in FilteredAccounts)
        {
            a.IsPLBoth = true;
        }
        UpdateCounts();
        StatusMessage = $"Marked {FilteredAccounts.Count} filtered account(s) for Both P&L and Balance Sheet (PLFlag = 'B').";
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Saving Profit & Loss report settings to ChartOfAccounts...";
            await _accountService.UpdateProfitLossSettingsAsync(_allAccounts);
            StatusMessage = "Profit & Loss report settings saved successfully in database.";
            MessageBox.Show("Profit & Loss report settings saved successfully in database (ChartOfAccounts.PLFlag updated).", 
                "Saved Successfully (F2)", MessageBoxButton.OK, MessageBoxImage.Information);
            RequestClose?.Invoke();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save failed: {ex.Message}";
            FriendlyErrorHandler.ShowError(ex, "Save P&L Settings");
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
