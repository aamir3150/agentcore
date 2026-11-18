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

public class BrokerageInvoiceRowDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public string TypeDisplay => InvoiceType == InvoiceType.BrkPurchase ? "Purchase" : "Sale";
    public string PartyName { get; set; } = string.Empty;
    public string VehicleNo { get; set; } = string.Empty;
    public int TotalBags { get; set; }
    public decimal TotalWeight { get; set; }
    public decimal GrossAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal NetAmount { get; set; }
    public bool IsPosted { get; set; }
    public string StatusDisplay => IsPosted ? "Posted" : "Draft";
}

public partial class BrokerageInvoiceLookupViewModel : ObservableObject
{
    private readonly IBrokerageService _brokerageService;
    private List<BrokerageInvoiceRowDto> _allInvoices = new();

    [ObservableProperty] private string _searchQuery = "";
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isDateRange = false;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    [ObservableProperty] private ObservableCollection<string> _typeOptions = new() { "All Invoices", "Purchase Invoices", "Sale Invoices" };
    [ObservableProperty] private string _selectedTypeOption = "All Invoices";

    [ObservableProperty] private ObservableCollection<BrokerageInvoiceRowDto> _filteredInvoices = new();
    [ObservableProperty] private BrokerageInvoiceRowDto? _selectedInvoice;

    public Action? RequestClose { get; set; }
    public bool IsConfirmed { get; private set; } = false;

    public BrokerageInvoiceLookupViewModel(IBrokerageService brokerageService)
    {
        _brokerageService = brokerageService;
        _ = LoadInvoicesAsync();
    }

    public void SetTypeFilter(InvoiceType type)
    {
        if (type == InvoiceType.BrkPurchase)
            SelectedTypeOption = "Purchase Invoices";
        else if (type == InvoiceType.BrkSale)
            SelectedTypeOption = "Sale Invoices";
    }

    public async Task LoadInvoicesAsync()
    {
        var invoices = await _brokerageService.GetAllBrokerageInvoicesAsync();
        _allInvoices = invoices.Select(i => new BrokerageInvoiceRowDto
        {
            Id = i.Id,
            InvoiceNo = i.InvoiceNo,
            InvoiceDate = i.InvoiceDate,
            InvoiceType = i.InvoiceType,
            PartyName = i.Party?.Name ?? "",
            VehicleNo = i.VehicleNo ?? "",
            TotalBags = i.Details.Sum(d => d.Bags),
            TotalWeight = i.Details.Sum(d => d.NetWeight),
            GrossAmount = i.GrossAmount,
            CommissionAmount = i.CommissionAmount,
            NetAmount = i.NetAmount,
            IsPosted = i.IsPosted
        }).OrderByDescending(x => x.InvoiceDate).ThenByDescending(x => x.Id).ToList();

        ApplyFilter();
    }

    partial void OnSearchQueryChanged(string value) => ApplyFilter();
    partial void OnIsAllDatesChanged(bool value) => ApplyFilter();
    partial void OnIsDateRangeChanged(bool value) => ApplyFilter();
    partial void OnFromDateChanged(DateTime value) => ApplyFilter();
    partial void OnToDateChanged(DateTime value) => ApplyFilter();
    partial void OnSelectedTypeOptionChanged(string value) => ApplyFilter();

    private void ApplyFilter()
    {
        var query = _allInvoices.AsEnumerable();

        if (SelectedTypeOption == "Purchase Invoices")
        {
            query = query.Where(x => x.InvoiceType == InvoiceType.BrkPurchase);
        }
        else if (SelectedTypeOption == "Sale Invoices")
        {
            query = query.Where(x => x.InvoiceType == InvoiceType.BrkSale);
        }

        if (IsDateRange)
        {
            var from = FromDate.Date;
            var to = ToDate.Date.AddDays(1).AddSeconds(-1);
            query = query.Where(x => x.InvoiceDate >= from && x.InvoiceDate <= to);
        }

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var term = SearchQuery.Trim().ToLower();
            query = query.Where(x => 
                x.InvoiceNo.ToLower().Contains(term) ||
                x.PartyName.ToLower().Contains(term) ||
                x.VehicleNo.ToLower().Contains(term));
        }

        FilteredInvoices = new ObservableCollection<BrokerageInvoiceRowDto>(query);
    }

    [RelayCommand]
    private void Select()
    {
        if (SelectedInvoice != null)
        {
            IsConfirmed = true;
            RequestClose?.Invoke();
        }
    }

    [RelayCommand]
    private void Close()
    {
        IsConfirmed = false;
        RequestClose?.Invoke();
    }
}
