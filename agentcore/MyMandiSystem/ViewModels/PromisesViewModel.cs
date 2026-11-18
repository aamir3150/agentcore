using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class PromisesViewModel : ObservableObject
{
    private readonly IPromiseService _promiseService;
    private readonly IPartyService _partyService;

    [ObservableProperty] private string _windowTitle = "Promises Management - لین دین کے وعدے";
    [ObservableProperty] private ObservableCollection<Promise> _allPromises = new();
    [ObservableProperty] private ObservableCollection<Promise> _filteredPromises = new();
    [ObservableProperty] private ObservableCollection<Party> _parties = new();

    [ObservableProperty] private Promise? _selectedPromise;
    [ObservableProperty] private Promise _currentPromise = new();
    [ObservableProperty] private bool _isFormVisible;

    // Promise Form Type
    [ObservableProperty] private bool _isReceivingPromise = true;
    [ObservableProperty] private bool _isPaymentPromise;

    // Type Filters
    [ObservableProperty] private bool _isTypeAll = true;
    [ObservableProperty] private bool _isTypeReceiving;
    [ObservableProperty] private bool _isTypePayment;

    // Status Filters
    [ObservableProperty] private bool _isStatusAll = true;
    [ObservableProperty] private bool _isStatusPending;
    [ObservableProperty] private bool _isStatusFulfilled;
    [ObservableProperty] private bool _isStatusOverdue;

    [ObservableProperty] private string _searchText = string.Empty;

    // Totals
    [ObservableProperty] private decimal _totalReceivable;
    [ObservableProperty] private decimal _totalPayable;
    [ObservableProperty] private decimal _totalPending;

    public Action? RequestClose { get; set; }

    public PromisesViewModel(IPromiseService promiseService, IPartyService partyService)
    {
        _promiseService = promiseService;
        _partyService = partyService;
        _ = LoadDataAsync();
    }

    [RelayCommand]
    private async Task LoadDataAsync()
    {
        try
        {
            var parties = await _partyService.GetAllPartiesAsync();
            Parties = new ObservableCollection<Party>(parties);

            var promises = await _promiseService.GetAllPromisesAsync();
            AllPromises = new ObservableCollection<Promise>(promises);
            ApplyFilters();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Load Promises");
        }
    }

    partial void OnIsReceivingPromiseChanged(bool value)
    {
        if (value) IsPaymentPromise = false;
    }

    partial void OnIsPaymentPromiseChanged(bool value)
    {
        if (value) IsReceivingPromise = false;
    }

    partial void OnIsTypeAllChanged(bool value) { if (value) ApplyFilters(); }
    partial void OnIsTypeReceivingChanged(bool value) { if (value) ApplyFilters(); }
    partial void OnIsTypePaymentChanged(bool value) { if (value) ApplyFilters(); }

    partial void OnIsStatusAllChanged(bool value) { if (value) ApplyFilters(); }
    partial void OnIsStatusPendingChanged(bool value) { if (value) ApplyFilters(); }
    partial void OnIsStatusFulfilledChanged(bool value) { if (value) ApplyFilters(); }
    partial void OnIsStatusOverdueChanged(bool value) { if (value) ApplyFilters(); }

    partial void OnSearchTextChanged(string value) => ApplyFilters();

    private void ApplyFilters()
    {
        var query = AllPromises.AsEnumerable();

        // Type filter
        if (IsTypeReceiving)
            query = query.Where(p => !p.IsPayment);
        else if (IsTypePayment)
            query = query.Where(p => p.IsPayment);

        // Status filter
        if (IsStatusPending)
            query = query.Where(p => !p.IsFulfilled);
        else if (IsStatusFulfilled)
            query = query.Where(p => p.IsFulfilled);
        else if (IsStatusOverdue)
            query = query.Where(p => p.IsOverdue);

        // Search text
        if (!string.IsNullOrWhiteSpace(SearchText))
        {
            var s = SearchText.Trim().ToLower();
            query = query.Where(p =>
                (p.Party != null && !string.IsNullOrEmpty(p.Party.Name) && p.Party.Name.ToLower().Contains(s)) ||
                (p.Party != null && !string.IsNullOrEmpty(p.Party.PartyNo) && p.Party.PartyNo.ToLower().Contains(s)) ||
                (!string.IsNullOrEmpty(p.Reason) && p.Reason.ToLower().Contains(s)) ||
                p.PromiseNo.ToString().Contains(s)
            );
        }

        var list = query.ToList();
        FilteredPromises = new ObservableCollection<Promise>(list);

        TotalReceivable = AllPromises.Where(p => !p.IsPayment && !p.IsFulfilled).Sum(p => p.Amount);
        TotalPayable = AllPromises.Where(p => p.IsPayment && !p.IsFulfilled).Sum(p => p.Amount);
        TotalPending = AllPromises.Where(p => !p.IsFulfilled).Sum(p => p.Amount);
    }

    [RelayCommand]
    private void AddPromise()
    {
        CurrentPromise = new Promise
        {
            PromiseDate = DateTime.Today,
            DueDate = DateTime.Today.AddDays(7),
            IsPayment = false,
            Status = PromiseStatus.Pending
        };
        IsReceivingPromise = true;
        IsPaymentPromise = false;
        IsFormVisible = true;
    }

    [RelayCommand]
    private void EditPromise(Promise? promise)
    {
        if (promise == null) return;
        CurrentPromise = new Promise
        {
            Id = promise.Id,
            PromiseNo = promise.PromiseNo,
            PartyId = promise.PartyId,
            Party = promise.Party,
            PromiseDate = promise.PromiseDate,
            DueDate = promise.DueDate,
            Amount = promise.Amount,
            Reason = promise.Reason,
            IsPayment = promise.IsPayment,
            Status = promise.Status
        };
        IsReceivingPromise = !promise.IsPayment;
        IsPaymentPromise = promise.IsPayment;
        IsFormVisible = true;
    }

    [RelayCommand]
    private async Task SavePromise()
    {
        if (CurrentPromise.PartyId <= 0)
        {
            MessageBox.Show("Please select a Party / برائے مہربانی پارٹی کا انتخاب کریں", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CurrentPromise.Amount <= 0)
        {
            MessageBox.Show("Please enter a valid Amount / برائے مہربانی درست رقم درج کریں", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            CurrentPromise.IsPayment = IsPaymentPromise;
            if (CurrentPromise.Id == 0 && CurrentPromise.PromiseNo == 0)
            {
                await _promiseService.AddPromiseAsync(CurrentPromise);
            }
            else
            {
                await _promiseService.UpdatePromiseAsync(CurrentPromise);
            }

            IsFormVisible = false;
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Save Promise");
        }
    }

    [RelayCommand]
    private void CancelPromise() => IsFormVisible = false;

    [RelayCommand]
    private async Task DeletePromise(Promise? promise)
    {
        if (promise == null) return;
        if (MessageBox.Show($"Are you sure to remove promise #{promise.PromiseNo} for {promise.Party?.Name}? / کیا آپ واقعی یہ وعدہ حذف کرنا چاہتے ہیں؟", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Warning) == MessageBoxResult.Yes)
        {
            try
            {
                await _promiseService.DeletePromiseAsync(promise.Id != 0 ? promise.Id : promise.PromiseNo);
                await LoadDataAsync();
            }
            catch (Exception ex)
            {
                FriendlyErrorHandler.ShowError(ex, "Delete Promise");
            }
        }
    }

    [RelayCommand]
    private async Task ToggleFulfilled(Promise? promise)
    {
        if (promise == null) return;
        try
        {
            await _promiseService.ToggleFulfilledAsync(promise.Id != 0 ? promise.Id : promise.PromiseNo);
            await LoadDataAsync();
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Toggle Promise Status");
        }
    }

    [RelayCommand]
    private void Close() => RequestClose?.Invoke();
}
