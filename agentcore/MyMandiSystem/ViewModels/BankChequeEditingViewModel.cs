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

public partial class ChequeEditingItemViewModel : ObservableObject
{
    [ObservableProperty] private bool _isSelected;
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string ChequeNo { get; set; } = string.Empty;

    [ObservableProperty] private DateTime? _cancelDate = DateTime.Today;
    [ObservableProperty] private string? _remarks = string.Empty;
    [ObservableProperty] private bool _isCancelled;

    public string EntryDateFormatted => EntryDate.ToString("dd/MM/yyyy");
    public string CancelDateFormatted => CancelDate?.ToString("dd/MM/yyyy") ?? "-";
    public string StatusBadge => IsCancelled ? "Cancelled" : "Available";
}

public partial class BankChequeEditingViewModel : ObservableObject
{
    private readonly IBankingService _bankingService;
    private readonly IAccountService _accountService;

    [ObservableProperty] private ObservableCollection<BankAccount> _bankAccounts = new();
    [ObservableProperty] private BankAccount? _selectedBankAccount;

    [ObservableProperty] private string _bankAccountNo = string.Empty;
    [ObservableProperty] private string _bankAccountName = string.Empty;

    [ObservableProperty] private bool _isFilterByDate = false;
    [ObservableProperty] private DateTime _entryDate = DateTime.Today;

    [ObservableProperty] private int _selectedTabIndex = 0; // 0 = Cancellation, 1 = Deletion

    [ObservableProperty] private ObservableCollection<ChequeEditingItemViewModel> _cancellationCheques = new();
    [ObservableProperty] private ObservableCollection<ChequeEditingItemViewModel> _deletionCheques = new();

    [ObservableProperty] private bool _isSelectAllCancellation = false;
    [ObservableProperty] private bool _isSelectAllDeletion = false;

    [ObservableProperty] private DateTime _batchCancelDate = DateTime.Today;
    [ObservableProperty] private string _batchRemarks = "Cancelled by User";

    [ObservableProperty] private int _totalCancellationCount = 0;
    [ObservableProperty] private int _selectedCancellationCount = 0;
    [ObservableProperty] private int _totalDeletionCount = 0;
    [ObservableProperty] private int _selectedDeletionCount = 0;

    [ObservableProperty] private bool _isLoading = false;
    [ObservableProperty] private string _statusMessage = string.Empty;

    public event Action? RequestClose;

    public BankChequeEditingViewModel(IBankingService bankingService, IAccountService accountService)
    {
        _bankingService = bankingService;
        _accountService = accountService;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadBankAccountsAsync();
        await LoadChequesAsync();
    }

    public async Task LoadBankAccountsAsync()
    {
        try
        {
            var list = (await _bankingService.GetAllBankAccountsAsync()).ToList();
            BankAccounts = new ObservableCollection<BankAccount>(list);
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
        }
    }

    partial void OnIsSelectAllCancellationChanged(bool value)
    {
        foreach (var item in CancellationCheques)
        {
            item.IsSelected = value;
        }
        UpdateSelectionCounts();
    }

    partial void OnIsSelectAllDeletionChanged(bool value)
    {
        foreach (var item in DeletionCheques)
        {
            item.IsSelected = value;
        }
        UpdateSelectionCounts();
    }

    [RelayCommand]
    public async Task LoadChequesAsync()
    {
        try
        {
            IsLoading = true;
            StatusMessage = "Loading bank cheque records...";

            DateTime? filterDate = IsFilterByDate ? EntryDate.Date : null;
            string? filterAccount = string.IsNullOrWhiteSpace(BankAccountNo) ? null : BankAccountNo.Trim();

            var records = (await _bankingService.GetChequeBookHistoryAsync(filterAccount, filterDate)).ToList();

            var cancelList = new ObservableCollection<ChequeEditingItemViewModel>();
            var delList = new ObservableCollection<ChequeEditingItemViewModel>();

            foreach (var r in records)
            {
                // Tab 0: Cancellation Tab (allows cancelling active cheques or updating remarks)
                var cancelItem = new ChequeEditingItemViewModel
                {
                    AccountNo = r.AccountNo,
                    AccountName = r.AccountName,
                    EntryDate = r.EntryDate,
                    ChequeNo = r.ChequeNo,
                    CancelDate = r.CancelDate ?? DateTime.Today,
                    Remarks = string.IsNullOrWhiteSpace(r.Remarks) ? (r.IsCancelled ? "Cancelled" : "Cheque Cancelled") : r.Remarks,
                    IsCancelled = r.IsCancelled,
                    IsSelected = false
                };
                cancelItem.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(ChequeEditingItemViewModel.IsSelected)) UpdateSelectionCounts(); };
                cancelList.Add(cancelItem);

                // Tab 1: Deletion Tab (Permanently delete unissued, uncancelled cheque leaves)
                if (!r.IsCancelled)
                {
                    var delItem = new ChequeEditingItemViewModel
                    {
                        AccountNo = r.AccountNo,
                        AccountName = r.AccountName,
                        EntryDate = r.EntryDate,
                        ChequeNo = r.ChequeNo,
                        IsCancelled = false,
                        IsSelected = false
                    };
                    delItem.PropertyChanged += (s, e) => { if (e.PropertyName == nameof(ChequeEditingItemViewModel.IsSelected)) UpdateSelectionCounts(); };
                    delList.Add(delItem);
                }
            }

            CancellationCheques = cancelList;
            DeletionCheques = delList;

            TotalCancellationCount = cancelList.Count;
            TotalDeletionCount = delList.Count;
            IsSelectAllCancellation = false;
            IsSelectAllDeletion = false;
            UpdateSelectionCounts();

            if (records.Count == 0)
            {
                StatusMessage = "Sorry! No Record Found!";
            }
            else
            {
                StatusMessage = $"Loaded {records.Count} cheque leaf record(s).";
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
    public void ApplyBatchDetailsToSelected()
    {
        var selected = CancellationCheques.Where(x => x.IsSelected).ToList();
        if (!selected.Any())
        {
            MessageBox.Show("Please select at least one cheque to apply batch details.", "Bank Cheques Editing", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        foreach (var item in selected)
        {
            item.CancelDate = BatchCancelDate;
            item.Remarks = BatchRemarks;
        }

        StatusMessage = $"Applied batch details to {selected.Count} selected cheque(s).";
    }

    [RelayCommand]
    public async Task ApplyActionsAsync()
    {
        bool isCancellationTab = (SelectedTabIndex == 0);

        if (isCancellationTab)
        {
            var selected = CancellationCheques.Where(x => x.IsSelected).ToList();
            if (!selected.Any())
            {
                MessageBox.Show("Please select at least one cheque to apply cancellation.", "Bank Cheques Editing", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "Are you sure to Apply Actions on Selected Cheques?",
                "Bank Cheques Editing",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                int count = 0;
                foreach (var item in selected)
                {
                    DateTime cDate = item.CancelDate ?? DateTime.Today;
                    string rem = string.IsNullOrWhiteSpace(item.Remarks) ? "Cancelled by User" : item.Remarks.Trim();
                    await _bankingService.UpdateChequeCancellationAsync(item.AccountNo, item.ChequeNo, true, cDate, rem);
                    count++;
                }

                MessageBox.Show("Operations Applied successfuly !!!", "Bank Cheques Editing", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadChequesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error applying cancellation: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
        else
        {
            var selected = DeletionCheques.Where(x => x.IsSelected).ToList();
            if (!selected.Any())
            {
                MessageBox.Show("Please select at least one cheque to permanently delete.", "Bank Cheques Editing", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var confirm = MessageBox.Show(
                "* Note: Selected Cheques will be Permanently Deleted.\n\nAre you sure to Apply Actions on Selected Cheques?",
                "Bank Cheques Editing - Confirm Permanent Deletion",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (confirm != MessageBoxResult.Yes) return;

            try
            {
                IsLoading = true;
                int count = 0;
                foreach (var item in selected)
                {
                    await _bankingService.DeleteChequeFromHistoryAsync(item.AccountNo, item.ChequeNo);
                    count++;
                }

                MessageBox.Show("Operations Applied successfuly !!!", "Bank Cheques Editing", MessageBoxButton.OK, MessageBoxImage.Information);
                await LoadChequesAsync();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error deleting cheques: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            finally
            {
                IsLoading = false;
            }
        }
    }

    [RelayCommand]
    public async Task RestoreSelectedChequesAsync()
    {
        var selected = CancellationCheques.Where(x => x.IsSelected && x.IsCancelled).ToList();
        if (!selected.Any())
        {
            MessageBox.Show("Please select at least one currently Cancelled cheque to restore.", "Bank Cheques Editing", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"Are you sure you want to restore {selected.Count} cancelled cheque(s) back to Available status?",
            "Bank Cheques Editing - Restore Cheques",
            MessageBoxButton.YesNo,
            MessageBoxImage.Question);

        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            IsLoading = true;
            foreach (var item in selected)
            {
                await _bankingService.UpdateChequeCancellationAsync(item.AccountNo, item.ChequeNo, false, null, null);
            }

            MessageBox.Show("Operations Applied successfuly !!!", "Bank Cheques Editing", MessageBoxButton.OK, MessageBoxImage.Information);
            await LoadChequesAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error restoring cheques: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoading = false;
        }
    }

    [RelayCommand]
    public async Task ClearAsync()
    {
        SelectedBankAccount = null;
        BankAccountNo = string.Empty;
        BankAccountName = string.Empty;
        IsFilterByDate = false;
        EntryDate = DateTime.Today;
        BatchCancelDate = DateTime.Today;
        BatchRemarks = "Cancelled by User";
        IsSelectAllCancellation = false;
        IsSelectAllDeletion = false;
        StatusMessage = "Form cleared.";
        await LoadChequesAsync();
    }

    [RelayCommand]
    public void Close()
    {
        RequestClose?.Invoke();
    }

    private void UpdateSelectionCounts()
    {
        SelectedCancellationCount = CancellationCheques.Count(x => x.IsSelected);
        SelectedDeletionCount = DeletionCheques.Count(x => x.IsSelected);
    }
}
