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

public partial class BankChequeReconciliationRowViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public long VoucherNo { get; set; }
    public DateTime VoucherDate { get; set; }
    public string ChequeNo { get; set; } = string.Empty;
    public DateTime? ChequeDate { get; set; }
    public string BankAccountNo { get; set; } = string.Empty;
    public string BankAccountName { get; set; } = string.Empty;
    public string PartyAccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;

    [ObservableProperty] private DateTime? _reconcileDate;
    [ObservableProperty] private bool _isReconciled;
    public bool IsLost { get; set; }

    public string VoucherDateFormatted => VoucherDate.ToString("dd/MM/yyyy");
    public string ChequeDateFormatted => ChequeDate?.ToString("dd/MM/yyyy") ?? "-";
    public string ReconcileDateFormatted => ReconcileDate?.ToString("dd/MM/yyyy") ?? "-";
    public string StatusBadge => IsReconciled ? "Reconciled" : "Un-Reconciled";

    partial void OnReconcileDateChanged(DateTime? value)
    {
        IsReconciled = value.HasValue;
    }
}

public partial class BankChequesReconciliationViewModel : ObservableObject
{
    private readonly IBankingService _bankingService;
    private readonly IAccountService _accountService;

    [ObservableProperty] private ObservableCollection<BankAccount> _bankAccounts = new();
    [ObservableProperty] private BankAccount? _selectedBankAccount;

    [ObservableProperty] private string _bankAccountNo = string.Empty;
    [ObservableProperty] private string _bankAccountName = string.Empty;

    [ObservableProperty] private bool _isUnReconciledFilter = true;
    [ObservableProperty] private bool _isReconciledFilter = false;
    [ObservableProperty] private bool _isAllFilter = false;

    [ObservableProperty] private decimal _reconciledBalance = 0;
    [ObservableProperty] private decimal _unReconciledBalance = 0;

    [ObservableProperty] private ObservableCollection<BankChequeReconciliationRowViewModel> _cheques = new();
    [ObservableProperty] private bool _isSelectAll = false;

    [ObservableProperty] private DateTime _batchReconcileDate = DateTime.Today;

    [ObservableProperty] private int _totalChequesCount = 0;
    [ObservableProperty] private int _selectedChequesCount = 0;

    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public event Action? RequestClose;

    public BankChequesReconciliationViewModel(IBankingService bankingService, IAccountService accountService)
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
            _ = LoadChequesAsync();
        }
    }

    partial void OnIsUnReconciledFilterChanged(bool value)
    {
        if (value) _ = LoadChequesAsync();
    }

    partial void OnIsReconciledFilterChanged(bool value)
    {
        if (value) _ = LoadChequesAsync();
    }

    partial void OnIsAllFilterChanged(bool value)
    {
        if (value) _ = LoadChequesAsync();
    }

    partial void OnIsSelectAllChanged(bool value)
    {
        foreach (var item in Cheques)
        {
            item.IsSelected = value;
        }
        UpdateSelectionCount();
    }

    [RelayCommand]
    public async Task LoadChequesAsync()
    {
        if (string.IsNullOrWhiteSpace(BankAccountNo))
        {
            Cheques.Clear();
            ReconciledBalance = 0;
            UnReconciledBalance = 0;
            TotalChequesCount = 0;
            SelectedChequesCount = 0;
            return;
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Loading reconciliation records...";

            bool? filter = null;
            if (IsUnReconciledFilter) filter = false;
            else if (IsReconciledFilter) filter = true;

            var items = (await _bankingService.GetChequesForReconciliationAsync(BankAccountNo, filter)).ToList();
            var balances = await _bankingService.GetBankChequesReconciliationBalancesAsync(BankAccountNo);

            ReconciledBalance = balances.ReconciledBalance;
            UnReconciledBalance = balances.UnreconciledBalance;

            var rowList = new ObservableCollection<BankChequeReconciliationRowViewModel>();
            foreach (var item in items)
            {
                var row = new BankChequeReconciliationRowViewModel
                {
                    VoucherNo = item.VoucherNo,
                    VoucherDate = item.VoucherDate,
                    ChequeNo = item.ChequeNo,
                    ChequeDate = item.ChequeDate,
                    BankAccountNo = item.BankAccountNo,
                    BankAccountName = item.BankAccountName,
                    PartyAccountNo = item.PartyAccountNo,
                    AccountName = item.AccountName,
                    Amount = item.Amount,
                    Narration = item.Narration,
                    ReconcileDate = item.ReconcileDate,
                    IsReconciled = item.IsReconciled,
                    IsLost = item.IsLost,
                    IsSelected = false
                };
                row.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(BankChequeReconciliationRowViewModel.IsSelected)) UpdateSelectionCount(); };
                rowList.Add(row);
            }

            Cheques = rowList;
            TotalChequesCount = rowList.Count;
            IsSelectAll = false;
            UpdateSelectionCount();

            if (items.Count == 0)
            {
                StatusMessage = "Sorry! No Record Found!";
            }
            else
            {
                StatusMessage = $"Loaded {items.Count} cheque record(s).";
            }
        }
        catch (Exception ex)
        {
            StatusMessage = $"Error: {ex.Message}";
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public void ApplyBatchDateToSelected()
    {
        var selected = Cheques.Where(x => x.IsSelected).ToList();
        if (!selected.Any())
        {
            MessageBox.Show("Please select at least one cheque to set reconciliation date.", "Bank Cheques Reconciliation", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        foreach (var item in selected)
        {
            if (item.IsLost)
            {
                MessageBox.Show($"Cheque #{item.ChequeNo} has been lost, so it cannot be Reconciled.", "Validation Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
                continue;
            }

            if (item.ChequeDate.HasValue && BatchReconcileDate < item.ChequeDate.Value.Date)
            {
                MessageBox.Show($"Reconcile Date cannot be less than Cheque Date for Cheque #{item.ChequeNo}.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                continue;
            }

            item.ReconcileDate = BatchReconcileDate;
        }

        StatusMessage = $"Applied reconciliation date to {selected.Count} selected cheque(s).";
    }

    [RelayCommand]
    public void ClearSelectedReconcileDate()
    {
        var selected = Cheques.Where(x => x.IsSelected).ToList();
        if (!selected.Any())
        {
            MessageBox.Show("Please select at least one cheque to clear reconciliation.", "Bank Cheques Reconciliation", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        foreach (var item in selected)
        {
            item.ReconcileDate = null;
        }

        StatusMessage = $"Cleared reconciliation for {selected.Count} cheque(s).";
    }

    [RelayCommand]
    public async Task SaveReconciliationAsync()
    {
        if (!Cheques.Any())
        {
            MessageBox.Show("No cheques to reconcile.", "Bank Cheques Reconciliation", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        // Validate
        foreach (var item in Cheques)
        {
            if (item.ReconcileDate.HasValue)
            {
                if (item.IsLost)
                {
                    MessageBox.Show($"Cheque #{item.ChequeNo} has been lost, so it cannot be Reconciled.", "Validation Alert", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
                if (item.ChequeDate.HasValue && item.ReconcileDate.Value.Date < item.ChequeDate.Value.Date)
                {
                    MessageBox.Show($"Reconcile Date cannot be less than Cheque Date for Cheque #{item.ChequeNo}.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
                    return;
                }
            }
        }

        try
        {
            IsLoading = true;
            StatusMessage = "Saving reconciliation changes...";

            var updates = Cheques.Select(c => (c.VoucherNo, c.BankAccountNo, c.ReconcileDate)).ToList();
            await _bankingService.BatchSaveChequeReconciliationAsync(updates);

            MessageBox.Show("Operations Applied successfuly !!!", "Bank Cheques Reconciliation", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadChequesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving reconciliation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task ClearAsync()
    {
        if (BankAccounts.Any())
        {
            SelectedBankAccount = BankAccounts.First();
        }
        IsUnReconciledFilter = true;
        IsReconciledFilter = false;
        IsAllFilter = false;
        BatchReconcileDate = DateTime.Today;
        IsSelectAll = false;
        StatusMessage = "Filters reset.";
        await LoadChequesAsync();
    }

    [RelayCommand]
    public void Close()
    {
        RequestClose?.Invoke();
    }

    private void UpdateSelectionCount()
    {
        SelectedChequesCount = Cheques.Count(x => x.IsSelected);
    }
}

