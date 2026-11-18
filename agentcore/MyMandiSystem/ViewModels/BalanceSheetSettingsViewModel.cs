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

public partial class BalanceSheetSettingsViewModel : ObservableObject
{
    private readonly IAccountService _accountService;
    private List<AccountReportSettingDto> _allAccounts = new();

    [ObservableProperty] private ObservableCollection<AccountReportSettingDto> _filteredAccounts = new();
    [ObservableProperty] private AccountReportSettingDto? _selectedAccount;
    [ObservableProperty] private string _searchText = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _accountTypes = new() { "All", "Assets", "Liabilities", "Capital / Equity", "Nominal Accounts" };
    [ObservableProperty] private string _selectedAccountType = "All";
    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _statusMessage = string.Empty;

    // Metrics summary badges (AgenSoft BLSettings Parity)
    [ObservableProperty] private int _totalAccountsCount = 0;
    [ObservableProperty] private int _includedBSCount = 0;
    [ObservableProperty] private int _excludedBSCount = 0;

    public Action? RequestClose { get; set; }

    public BalanceSheetSettingsViewModel(IAccountService accountService)
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
            StatusMessage = "Loading Balance Sheet report configuration...";
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
            FriendlyErrorHandler.ShowError(ex, "Load Balance Sheet Settings");
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
            if (SelectedAccountType.Contains("Asset"))
            {
                query = query.Where(a => a.AccountType.Contains("Asset", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountNo.StartsWith("1"));
            }
            else if (SelectedAccountType.Contains("Liabilit"))
            {
                query = query.Where(a => a.AccountType.Contains("Liabilit", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountNo.StartsWith("2"));
            }
            else if (SelectedAccountType.Contains("Capital") || SelectedAccountType.Contains("Equity"))
            {
                query = query.Where(a => a.AccountType.Contains("Capital", StringComparison.OrdinalIgnoreCase) 
                                      || a.AccountType.Contains("Equity", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountNo.StartsWith("3"));
            }
            else if (SelectedAccountType.Contains("Nominal"))
            {
                query = query.Where(a => a.AccountNo.StartsWith("4") || a.AccountNo.StartsWith("5")
                                      || a.AccountType.Contains("Revenue", StringComparison.OrdinalIgnoreCase)
                                      || a.AccountType.Contains("Expense", StringComparison.OrdinalIgnoreCase));
            }
        }

        FilteredAccounts = new ObservableCollection<AccountReportSettingDto>(query);
        UpdateCounts();
    }

    private void UpdateCounts()
    {
        IncludedBSCount = _allAccounts.Count(a => a.BalFlag);
        ExcludedBSCount = _allAccounts.Count(a => !a.BalFlag);
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

            // Balance Sheet accounts: Assets, Liabilities, Capital / Equity
            if (typeLow.Contains("asset") || typeLow.Contains("liab") || typeLow.Contains("cap") || 
                typeLow.Contains("equi") || typeLow.Contains("bank") || typeLow.Contains("cash") ||
                no.StartsWith("1") || no.StartsWith("2") || no.StartsWith("3"))
            {
                if (!a.BalFlag)
                {
                    a.BalFlag = true;
                    modified++;
                }
            }
            else
            {
                // Nominal accounts (Income / Expense) are excluded from direct Balance Sheet (closed to P&L)
                if (a.BalFlag && (no.StartsWith("4") || no.StartsWith("5") || typeLow.Contains("exp") || typeLow.Contains("rev")))
                {
                    a.BalFlag = false;
                    modified++;
                }
            }
        }

        UpdateCounts();
        ApplyFilter();
        StatusMessage = $"Auto configured {modified} account mapping(s) based on standard Balance Sheet classification. Click 'Save (F2)' to persist.";
        MessageBox.Show($"Auto configuration completed.\n\nIncluded all Assets, Liabilities, and Equity accounts in Balance Sheet.\n\nClick 'Save (F2)' to save settings in Chart of Accounts.", "Auto Configuration (CmdAuto)", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    public void SelectAllBS()
    {
        foreach (var a in FilteredAccounts)
        {
            a.BalFlag = true;
        }
        UpdateCounts();
        StatusMessage = $"Selected all {FilteredAccounts.Count} filtered account(s) for Balance Sheet Statement.";
    }

    [RelayCommand]
    public void UnselectAllBS()
    {
        foreach (var a in FilteredAccounts)
        {
            a.BalFlag = false;
        }
        UpdateCounts();
        StatusMessage = $"Unselected all {FilteredAccounts.Count} filtered account(s) from Balance Sheet Statement.";
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Saving Balance Sheet report settings to ChartOfAccounts...";
            await _accountService.UpdateBalanceSheetSettingsAsync(_allAccounts);
            StatusMessage = "Balance Sheet report settings saved successfully in database.";
            MessageBox.Show("Balance Sheet report settings saved successfully in database (ChartOfAccounts.BalFlag updated).", 
                "Saved Successfully (F2)", MessageBoxButton.OK, MessageBoxImage.Information);
            RequestClose?.Invoke();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save failed: {ex.Message}";
            FriendlyErrorHandler.ShowError(ex, "Save Balance Sheet Settings");
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
