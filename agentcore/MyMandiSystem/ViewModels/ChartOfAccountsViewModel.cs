using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Enums;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class ChartOfAccountsViewModel : ObservableObject
{
    private readonly IAccountService _accountService;

    [ObservableProperty] private ObservableCollection<AccountGroup> _accountGroups = new();
    [ObservableProperty] private ObservableCollection<Account> _accounts = new();
    [ObservableProperty] private ObservableCollection<Account> _filteredAccounts = new();
    
    [ObservableProperty] private AccountGroup? _selectedGroup;
    [ObservableProperty] private Account? _selectedAccount;

    // Group Form Properties
    [ObservableProperty] private bool _isGroupFormVisible;
    [ObservableProperty] private AccountGroup _currentGroup = new();
    public Array AccountTypes => Enum.GetValues(typeof(AccountType));

    // Account Form Properties
    [ObservableProperty] private bool _isAccountFormVisible;
    [ObservableProperty] private Account _currentAccount = new();
    
    public ChartOfAccountsViewModel(IAccountService accountService)
    {
        _accountService = accountService;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        var groups = await _accountService.GetAccountGroupsAsync();
        AccountGroups = new ObservableCollection<AccountGroup>(groups.Where(g => !g.IsDeleted));

        var accounts = await _accountService.GetAllAccountsAsync();
        Accounts = new ObservableCollection<Account>(accounts.Where(a => !a.IsDeleted));

        FilterAccounts();
    }

    partial void OnSelectedGroupChanged(AccountGroup? value)
    {
        FilterAccounts();
        IsAccountFormVisible = false;
        IsGroupFormVisible = false;
    }

    private void FilterAccounts()
    {
        if (SelectedGroup == null)
        {
            FilteredAccounts = new ObservableCollection<Account>();
        }
        else
        {
            FilteredAccounts = new ObservableCollection<Account>(Accounts.Where(a => a.AccountGroupId == SelectedGroup.Id));
        }
    }

    // --- Group Operations ---

    [RelayCommand]
    private void AddNewGroup()
    {
        CurrentGroup = new AccountGroup();
        IsGroupFormVisible = true;
        IsAccountFormVisible = false;
    }

    [RelayCommand]
    private void EditGroup(AccountGroup group)
    {
        if (group == null) return;
        CurrentGroup = new AccountGroup
        {
            Id = group.Id,
            Name = group.Name,
            GroupType = group.GroupType
        };
        IsGroupFormVisible = true;
        IsAccountFormVisible = false;
    }

    [RelayCommand]
    private async Task SaveGroup()
    {
        if (string.IsNullOrWhiteSpace(CurrentGroup.Name))
        {
            System.Windows.MessageBox.Show("Group Name is required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            if (CurrentGroup.Id == 0)
            {
                await _accountService.AddAccountGroupAsync(CurrentGroup);
            }
            else
            {
                var existing = await _accountService.GetAccountGroupsAsync();
                var dbGroup = existing.FirstOrDefault(g => g.Id == CurrentGroup.Id);
                if (dbGroup != null)
                {
                    dbGroup.Name = CurrentGroup.Name;
                    dbGroup.GroupType = CurrentGroup.GroupType;
                    await _accountService.UpdateAccountGroupAsync(dbGroup);
                }
            }

            IsGroupFormVisible = false;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error saving group: {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    private void CancelGroupForm()
    {
        IsGroupFormVisible = false;
    }

    [RelayCommand]
    private async Task DeleteGroup(AccountGroup group)
    {
        if (group == null) return;
        
        var result = System.Windows.MessageBox.Show($"Are you sure you want to delete group '{group.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            await _accountService.DeleteAccountGroupAsync(group.Id);
            await LoadDataAsync();
        }
    }

    // --- Account Operations ---

    [RelayCommand]
    private void AddNewAccount()
    {
        CurrentAccount = new Account
        {
            AccountGroupId = SelectedGroup?.Id ?? (AccountGroups.FirstOrDefault()?.Id ?? 0),
            AccountType = SelectedGroup?.GroupType ?? AccountType.Asset,
            IsSystemAccount = false,
            IsActive = true
        };
        IsAccountFormVisible = true;
        IsGroupFormVisible = false;
    }

    [RelayCommand]
    private void EditAccount(Account account)
    {
        if (account == null) return;
        if (account.IsSystemAccount)
        {
            System.Windows.MessageBox.Show("System accounts cannot be modified manually.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        CurrentAccount = new Account
        {
            Id = account.Id,
            AccountNo = account.AccountNo,
            Name = account.Name,
            UrduName = account.UrduName,
            AccountType = account.AccountType,
            AccountGroupId = account.AccountGroupId,
            OpeningBalance = account.OpeningBalance,
            CurrentBalance = account.CurrentBalance,
            IsSystemAccount = account.IsSystemAccount,
            IsActive = account.IsActive
        };
        IsAccountFormVisible = true;
        IsGroupFormVisible = false;
    }

    [RelayCommand]
    private async Task SaveAccount()
    {
        if (string.IsNullOrWhiteSpace(CurrentAccount.AccountNo) || string.IsNullOrWhiteSpace(CurrentAccount.Name))
        {
            System.Windows.MessageBox.Show("Account No and Name are required.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            if (CurrentAccount.Id == 0)
            {
                // Verify AccountNo uniqueness (simplified for speed)
                var existingList = await _accountService.GetAllAccountsAsync();
                if (existingList.Any(a => !a.IsDeleted && a.AccountNo == CurrentAccount.AccountNo))
                {
                    System.Windows.MessageBox.Show("This Account Number already exists.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                
                // Inherit type from group
                var grp = AccountGroups.FirstOrDefault(g => g.Id == CurrentAccount.AccountGroupId);
                if (grp != null) CurrentAccount.AccountType = grp.GroupType;

                await _accountService.AddAccountAsync(CurrentAccount);
            }
            else
            {
                var existing = await _accountService.GetAccountByNoAsync(CurrentAccount.AccountNo);
                if (existing != null && existing.Id == CurrentAccount.Id)
                {
                    existing.Name = CurrentAccount.Name;
                    existing.UrduName = CurrentAccount.UrduName;
                    existing.AccountGroupId = CurrentAccount.AccountGroupId;
                    
                    var grp = AccountGroups.FirstOrDefault(g => g.Id == CurrentAccount.AccountGroupId);
                    if (grp != null) existing.AccountType = grp.GroupType;

                    existing.OpeningBalance = CurrentAccount.OpeningBalance;
                    existing.IsActive = CurrentAccount.IsActive;

                    await _accountService.UpdateAccountAsync(existing);
                }
            }

            IsAccountFormVisible = false;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error saving account: {ex.Message}", "Error");
        }
    }

    [RelayCommand]
    private void CancelAccountForm()
    {
        IsAccountFormVisible = false;
    }

    [RelayCommand]
    private async Task DeleteAccount(Account account)
    {
        if (account == null) return;
        if (account.IsSystemAccount)
        {
            System.Windows.MessageBox.Show("System accounts cannot be deleted.", "Warning", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var balance = await _accountService.GetAccountBalanceAsync(account.Id);
        if (balance != 0)
        {
            System.Windows.MessageBox.Show($"Cannot delete account because it has a non-zero balance: {balance:N2}", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = System.Windows.MessageBox.Show($"Are you sure you want to delete account '{account.Name}'?", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            await _accountService.DeleteAccountAsync(account.Id);
            await LoadDataAsync();
        }
    }
}
