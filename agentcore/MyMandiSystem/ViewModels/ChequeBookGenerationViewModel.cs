using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class ChequeBookGenerationViewModel : ObservableObject
{
    private readonly IBankingService _bankingService;
    private readonly IAccountService _accountService;

    [ObservableProperty] private ObservableCollection<BankAccount> _bankAccounts = new();
    [ObservableProperty] private BankAccount? _selectedBankAccount;

    [ObservableProperty] private string _bankAccountNo = string.Empty;
    [ObservableProperty] private string _bankAccountName = string.Empty;
    [ObservableProperty] private string _bankBranch = string.Empty;

    [ObservableProperty] private string _fromChequeNo = string.Empty;
    [ObservableProperty] private string _toChequeNo = string.Empty;
    [ObservableProperty] private int _totalChequesCount = 0;

    [ObservableProperty] private ObservableCollection<ChequeHistoryDto> _existingCheques = new();
    [ObservableProperty] private ChequeHistoryDto? _selectedCheque;

    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public ChequeBookGenerationViewModel(IBankingService bankingService, IAccountService accountService)
    {
        _bankingService = bankingService;
        _accountService = accountService;
        _ = LoadBankAccountsAsync();
    }

    public async Task LoadBankAccountsAsync()
    {
        try
        {
            IsLoading = true;
            var list = (await _bankingService.GetAllBankAccountsAsync()).ToList();
            BankAccounts = new ObservableCollection<BankAccount>(list);

            if (BankAccounts.Any())
            {
                SelectedBankAccount = BankAccounts.First();
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading bank accounts: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    partial void OnSelectedBankAccountChanged(BankAccount? value)
    {
        if (value != null)
        {
            BankAccountNo = value.Account?.AccountNo ?? value.AccountNumber ?? string.Empty;
            BankAccountName = value.Account?.Name ?? value.BankName;
            BankBranch = value.BranchName ?? string.Empty;
            _ = RefreshExistingChequesAsync();
        }
        else
        {
            BankAccountNo = string.Empty;
            BankAccountName = string.Empty;
            BankBranch = string.Empty;
            ExistingCheques.Clear();
        }
    }

    partial void OnFromChequeNoChanged(string value) => CalculateTotalCheques();
    partial void OnToChequeNoChanged(string value) => CalculateTotalCheques();

    private void CalculateTotalCheques()
    {
        if (long.TryParse(FromChequeNo?.Trim(), out long start) &&
            long.TryParse(ToChequeNo?.Trim(), out long end))
        {
            if (end >= start)
            {
                TotalChequesCount = (int)Math.Min(end - start + 1, 10000);
            }
            else
            {
                TotalChequesCount = 0;
            }
        }
        else
        {
            TotalChequesCount = 0;
        }
    }

    public async Task RefreshExistingChequesAsync()
    {
        if (string.IsNullOrWhiteSpace(BankAccountNo))
        {
            ExistingCheques.Clear();
            return;
        }

        try
        {
            var history = await _bankingService.GetChequeBookHistoryForBankAsync(BankAccountNo);
            ExistingCheques = new ObservableCollection<ChequeHistoryDto>(history);
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error loading cheque history: {ex.Message}";
        }
    }

    [RelayCommand]
    private async Task GenerateChequeBookAsync()
    {
        if (string.IsNullOrWhiteSpace(BankAccountNo))
        {
            MessageBox.Show("Please select or enter a valid Bank Account Number.", "Validation Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(FromChequeNo) || string.IsNullOrWhiteSpace(ToChequeNo))
        {
            MessageBox.Show("Please specify both From Cheque No and To Cheque No.", "Validation Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (!long.TryParse(FromChequeNo.Trim(), out long start) || !long.TryParse(ToChequeNo.Trim(), out long end))
        {
            MessageBox.Show("Invalid cheque number(s). Cheque numbers must be numeric.", "Format Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (end < start)
        {
            MessageBox.Show("To Cheque No cannot be less than From Cheque No.", "Range Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (end - start + 1 > 500)
        {
            MessageBox.Show("Cannot generate more than 500 cheque leaves in a single cheque book.", "Limit Exceeded", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Generating cheque leaves...";

            int count = await _bankingService.GenerateChequeBookLeavesAsync(BankAccountNo, FromChequeNo, ToChequeNo);

            await RefreshExistingChequesAsync();

            MessageBox.Show($"{count} cheque leaf/leaves generated successfully into the cheque book.", "Generation Complete", MessageBoxButton.OK, MessageBoxImage.Information);

            StatusMessage = $"{count} cheque(s) generated successfully.";
            FromChequeNo = string.Empty;
            ToChequeNo = string.Empty;
            TotalChequesCount = 0;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error generating cheque book: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    private void ClearFields()
    {
        FromChequeNo = string.Empty;
        ToChequeNo = string.Empty;
        TotalChequesCount = 0;
        StatusMessage = "Ready";
    }

    [RelayCommand]
    private async Task CancelChequeAsync(ChequeHistoryDto? item)
    {
        var target = item ?? SelectedCheque;
        if (target == null) return;

        if (target.Status == "Issued")
        {
            MessageBox.Show("This cheque has already been issued in a voucher and cannot be cancelled from here.", "Invalid Action", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Are you sure you want to cancel cheque #{target.ChequeNo}?", "Confirm Cancellation", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _bankingService.CancelChequeInHistoryAsync(target.AccountNo, target.ChequeNo, "Cancelled by user");
                await RefreshExistingChequesAsync();
                StatusMessage = $"Cheque #{target.ChequeNo} cancelled.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error cancelling cheque: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private async Task DeleteChequeAsync(ChequeHistoryDto? item)
    {
        var target = item ?? SelectedCheque;
        if (target == null) return;

        if (target.Status == "Issued")
        {
            MessageBox.Show("This cheque has already been issued and cannot be deleted.", "Invalid Action", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var result = MessageBox.Show($"Are you sure you want to permanently delete cheque leaf #{target.ChequeNo} from history?", "Confirm Deletion", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (result == MessageBoxResult.Yes)
        {
            try
            {
                await _bankingService.DeleteChequeFromHistoryAsync(target.AccountNo, target.ChequeNo);
                await RefreshExistingChequesAsync();
                StatusMessage = $"Cheque #{target.ChequeNo} deleted.";
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting cheque: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}

