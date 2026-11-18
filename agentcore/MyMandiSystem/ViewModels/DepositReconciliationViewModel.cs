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

public partial class BankDepositReconciliationRowViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public long VoucherNo { get; set; }
    public DateTime VoucherDate { get; set; }
    public string SlipNo { get; set; } = string.Empty;
    public DateTime? DepositedDate { get; set; }
    public string BankAccountNo { get; set; } = string.Empty;
    public string BankAccountName { get; set; } = string.Empty;
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
    public bool IsCheque { get; set; }
    public string? ChequeNo { get; set; }

    [ObservableProperty] private DateTime? _reconcileDate;
    [ObservableProperty] private bool _isReconciled;

    public string VoucherDateFormatted => VoucherDate.ToString("dd/MM/yyyy");
    public string DepositedDateFormatted => DepositedDate?.ToString("dd/MM/yyyy") ?? "-";
    public string ReconcileDateFormatted => ReconcileDate?.ToString("dd/MM/yyyy") ?? "-";
    public string DepositType => IsCheque ? "Cheque Deposit" : "Cash Slip";
    public string StatusBadge => IsReconciled ? "Reconciled" : "Un-Reconciled";

    partial void OnReconcileDateChanged(DateTime? value)
    {
        IsReconciled = value.HasValue;
    }
}

public partial class DepositReconciliationViewModel : ObservableObject
{
    private readonly IBankingService _bankingService;
    private readonly IAccountService _accountService;

    [ObservableProperty] private ObservableCollection<BankAccount> _bankAccounts = new();
    [ObservableProperty] private BankAccount? _selectedBankAccount;

    [ObservableProperty] private string _bankAccountNo = string.Empty;
    [ObservableProperty] private string _bankAccountName = string.Empty;

    // Filters (AgenSoft Radio Buttons)
    [ObservableProperty] private bool _isUnReconciledFilter = true;
    [ObservableProperty] private bool _isReconciledFilter = false;
    [ObservableProperty] private bool _isAllFilter = false;

    // Balances (AgenSoft txtConciledBalance & txtUnConciledBalance)
    [ObservableProperty] private decimal _reconciledBalance = 0;
    [ObservableProperty] private decimal _unReconciledBalance = 0;

    [ObservableProperty] private ObservableCollection<BankDepositReconciliationRowViewModel> _deposits = new();
    [ObservableProperty] private bool _isSelectAll = false;

    [ObservableProperty] private DateTime _batchReconcileDate = DateTime.Today;

    [ObservableProperty] private int _totalDepositsCount = 0;
    [ObservableProperty] private int _selectedDepositsCount = 0;

    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public event Action? RequestClose;

    public DepositReconciliationViewModel(IBankingService bankingService, IAccountService accountService)
    {
        _bankingService = bankingService;
        _accountService = accountService;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadBankAccountsAsync();
    }

    public async Task LoadBankAccountsAsync()
    {
        try
        {
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
    }

    partial void OnSelectedBankAccountChanged(BankAccount? value)
    {
        if (value != null)
        {
            BankAccountNo = value.Account?.AccountNo ?? value.AccountNumber ?? string.Empty;
            BankAccountName = value.Account?.Name ?? $"{value.BankName} - {value.BranchName}";
            _ = LoadDepositsAsync();
        }
    }

    partial void OnIsUnReconciledFilterChanged(bool value)
    {
        if (value) _ = LoadDepositsAsync();
    }

    partial void OnIsReconciledFilterChanged(bool value)
    {
        if (value) _ = LoadDepositsAsync();
    }

    partial void OnIsAllFilterChanged(bool value)
    {
        if (value) _ = LoadDepositsAsync();
    }

    partial void OnIsSelectAllChanged(bool value)
    {
        foreach (var item in Deposits)
        {
            item.IsSelected = value;
        }
        UpdateSelectionCount();
    }

    [RelayCommand]
    public async Task LoadDepositsAsync()
    {
        if (string.IsNullOrWhiteSpace(BankAccountNo))
        {
            Deposits.Clear();
            ReconciledBalance = 0;
            UnReconciledBalance = 0;
            TotalDepositsCount = 0;
            SelectedDepositsCount = 0;
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Loading deposit reconciliation records...";

            bool? filter = null;
            if (IsUnReconciledFilter) filter = false;
            else if (IsReconciledFilter) filter = true;

            var items = (await _bankingService.GetDepositsForReconciliationAsync(BankAccountNo, filter)).ToList();
            var balances = await _bankingService.GetBankDepositsReconciliationBalancesAsync(BankAccountNo);

            ReconciledBalance = balances.ReconciledBalance;
            UnReconciledBalance = balances.UnreconciledBalance;

            var rowList = new ObservableCollection<BankDepositReconciliationRowViewModel>();
            foreach (var item in items)
            {
                var row = new BankDepositReconciliationRowViewModel
                {
                    VoucherNo = item.VoucherNo,
                    VoucherDate = item.VoucherDate,
                    SlipNo = item.SlipNo,
                    DepositedDate = item.DepositedDate,
                    BankAccountNo = item.BankAccountNo,
                    BankAccountName = item.BankAccountName,
                    AccountNo = item.AccountNo,
                    AccountName = item.AccountName,
                    Amount = item.Amount,
                    Narration = item.Narration,
                    ReconcileDate = item.ReconcileDate,
                    IsCheque = item.IsCheque,
                    ChequeNo = item.ChequeNo,
                    IsReconciled = item.IsReconciled,
                    IsSelected = false
                };
                row.PropertyChanged += (s, e) => 
                { 
                    if (e.PropertyName == nameof(BankDepositReconciliationRowViewModel.IsSelected)) 
                        UpdateSelectionCount(); 
                };
                rowList.Add(row);
            }

            Deposits = rowList;
            TotalDepositsCount = rowList.Count;
            IsSelectAll = false;
            UpdateSelectionCount();

            if (items.Count == 0)
            {
                StatusMessage = "Sorry! No Record Found!";
            }
            else
            {
                StatusMessage = $"Loaded {items.Count} deposit record(s).";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
            MessageBox.Show($"Error loading deposits: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void UpdateSelectionCount()
    {
        SelectedDepositsCount = Deposits.Count(c => c.IsSelected);
    }

    [RelayCommand]
    public void ApplyBatchDate()
    {
        var selected = Deposits.Where(c => c.IsSelected).ToList();
        if (!selected.Any())
        {
            MessageBox.Show("Please select at least one deposit record to apply reconciliation date.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        foreach (var item in selected)
        {
            item.ReconcileDate = BatchReconcileDate;
        }

        StatusMessage = $"Applied date {BatchReconcileDate:dd/MM/yyyy} to {selected.Count} selected record(s). Click 'Save (F2)' to persist.";
    }

    [RelayCommand]
    public void ClearSelectedDates()
    {
        var selected = Deposits.Where(c => c.IsSelected).ToList();
        if (!selected.Any())
        {
            MessageBox.Show("Please select at least one deposit record to un-reconcile.", "Selection Required", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        foreach (var item in selected)
        {
            item.ReconcileDate = null;
        }

        StatusMessage = $"Cleared reconciliation date for {selected.Count} record(s). Click 'Save (F2)' to persist.";
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(BankAccountNo))
        {
            MessageBox.Show("Please select a bank account first.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var selected = Deposits.Where(c => c.IsSelected).ToList();
        if (!selected.Any())
        {
            // If none checked, save all items whose ReconcileDate was modified
            selected = Deposits.ToList();
        }

        if (!selected.Any())
        {
            MessageBox.Show("No records available to save.", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Saving deposit reconciliations...";

            var updates = selected.Select(c => (c.VoucherNo, BankAccountNo, c.ReconcileDate)).ToList();
            await _bankingService.BatchSaveDepositReconciliationAsync(updates);

            MessageBox.Show($"Successfully updated {updates.Count} deposit record(s).", "Reconciliation Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadDepositsAsync();
        }
        catch (Exception ex)
        {
            StatusMessage = $"Save failed: {ex.Message}";
            MessageBox.Show($"Error saving deposit reconciliation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task ClearAsync()
    {
        IsUnReconciledFilter = true;
        BatchReconcileDate = DateTime.Today;
        await LoadDepositsAsync();
    }

    [RelayCommand]
    public void Close()
    {
        RequestClose?.Invoke();
    }
}

