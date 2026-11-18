using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.ViewModels;

public partial class BrokerageMultiInvoiceLookupViewModel : ObservableObject
{
    private readonly IBrokerageService _brokerageService;
    private List<BrokerageMultiInvoiceSummaryDto> _allInvoices = new();

    [ObservableProperty] private string _searchQuery = "";
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isDateRange = false;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    [ObservableProperty] private ObservableCollection<BrokerageMultiInvoiceSummaryDto> _filteredInvoices = new();
    [ObservableProperty] private BrokerageMultiInvoiceSummaryDto? _selectedInvoice;

    public Action? RequestClose { get; set; }
    public bool IsConfirmed { get; private set; } = false;

    public BrokerageMultiInvoiceLookupViewModel(IBrokerageService brokerageService)
    {
        _brokerageService = brokerageService;
        _ = LoadInvoicesAsync();
    }

    private async Task LoadInvoicesAsync()
    {
        try
        {
            _allInvoices = await _brokerageService.GetAllMultiInvoicesAsync();
            ApplyFilter();
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Failed to load multi invoices: {ex.Message}", "Error", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Error);
        }
    }

    partial void OnSearchQueryChanged(string value) => ApplyFilter();
    partial void OnIsAllDatesChanged(bool value) => ApplyFilter();
    partial void OnIsDateRangeChanged(bool value) => ApplyFilter();
    partial void OnFromDateChanged(DateTime value) => ApplyFilter();
    partial void OnToDateChanged(DateTime value) => ApplyFilter();

    private void ApplyFilter()
    {
        IEnumerable<BrokerageMultiInvoiceSummaryDto> result = _allInvoices;

        if (IsDateRange)
        {
            result = result.Where(i => i.Date.Date >= FromDate.Date && i.Date.Date <= ToDate.Date);
        }

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var q = SearchQuery.Trim().ToLowerInvariant();
            result = result.Where(i =>
                i.MultiId.ToLowerInvariant().Contains(q) ||
                i.PurchaseId.ToLowerInvariant().Contains(q) ||
                i.SaleId.ToLowerInvariant().Contains(q) ||
                i.SupplierName.ToLowerInvariant().Contains(q) ||
                i.CustomerName.ToLowerInvariant().Contains(q));
        }

        FilteredInvoices = new ObservableCollection<BrokerageMultiInvoiceSummaryDto>(result);
        if (FilteredInvoices.Any())
        {
            SelectedInvoice = FilteredInvoices.First();
        }
        else
        {
            SelectedInvoice = null;
        }
    }

    [RelayCommand]
    private void Confirm()
    {
        if (SelectedInvoice != null)
        {
            IsConfirmed = true;
            RequestClose?.Invoke();
        }
        else
        {
            System.Windows.MessageBox.Show("Please select a multi invoice from the list.", "Selection Required", System.Windows.MessageBoxButton.OK, System.Windows.MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        IsConfirmed = false;
        RequestClose?.Invoke();
    }
}

