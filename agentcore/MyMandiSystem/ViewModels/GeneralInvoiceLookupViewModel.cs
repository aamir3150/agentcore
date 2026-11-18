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

public class GeneralInvoiceRowDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public string TypeDisplay => InvoiceType switch
    {
        InvoiceType.GenPurchase => "Purchase Invoice",
        InvoiceType.GenSale => "Sale Invoice",
        InvoiceType.GenPurchaseReturn => "Purchase Return",
        InvoiceType.GenSaleReturn => "Sale Return",
        _ => InvoiceType.ToString()
    };
    public string PartyName { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal NetAmount { get; set; }
    public bool IsPosted { get; set; }
    public string StatusDisplay => IsPosted ? "Posted" : "Draft";
}

public partial class GeneralInvoiceLookupViewModel : ObservableObject
{
    private readonly IInvoiceService _invoiceService;
    private List<GeneralInvoiceRowDto> _allInvoices = new();

    [ObservableProperty] private string _searchQuery = "";
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isDateRange = false;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    [ObservableProperty] private ObservableCollection<string> _typeOptions = new() { "All General", "Purchase Invoices", "Sale Invoices", "Purchase Returns", "Sale Returns" };
    [ObservableProperty] private string _selectedTypeOption = "All General";

    [ObservableProperty] private ObservableCollection<GeneralInvoiceRowDto> _filteredInvoices = new();
    [ObservableProperty] private GeneralInvoiceRowDto? _selectedInvoice;

    public Action? RequestClose { get; set; }
    public bool IsConfirmed { get; private set; } = false;

    public GeneralInvoiceLookupViewModel(IInvoiceService invoiceService)
    {
        _invoiceService = invoiceService;
        _ = LoadInvoicesAsync();
    }

    public void SetTypeFilter(InvoiceType type)
    {
        SelectedTypeOption = type switch
        {
            InvoiceType.GenPurchase => "Purchase Invoices",
            InvoiceType.GenSale => "Sale Invoices",
            InvoiceType.GenPurchaseReturn => "Purchase Returns",
            InvoiceType.GenSaleReturn => "Sale Returns",
            _ => "All General"
        };
    }

    private async Task LoadInvoicesAsync()
    {
        try
        {
            DateTime? from = IsDateRange ? FromDate : null;
            DateTime? to = IsDateRange ? ToDate : null;

            var rawInvoices = await _invoiceService.SearchInvoicesAsync(null, null, from, to);
            var generalInvoices = rawInvoices.Where(i => 
                i.InvoiceType == InvoiceType.GenPurchase ||
                i.InvoiceType == InvoiceType.GenSale ||
                i.InvoiceType == InvoiceType.GenPurchaseReturn ||
                i.InvoiceType == InvoiceType.GenSaleReturn);

            _allInvoices = generalInvoices.Select(i => new GeneralInvoiceRowDto
            {
                Id = i.Id,
                InvoiceNo = i.InvoiceNo,
                InvoiceDate = i.InvoiceDate,
                InvoiceType = i.InvoiceType,
                PartyName = i.Party?.Name ?? "N/A",
                SubTotal = i.SubTotal,
                DiscountAmount = i.DiscountAmount,
                TaxAmount = i.TaxAmount,
                NetAmount = i.NetAmount,
                IsPosted = i.IsPosted
            }).ToList();

            ApplyFilters();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[GeneralInvoiceLookup] {ex.Message}");
        }
    }

    private void ApplyFilters()
    {
        var filtered = _allInvoices.AsEnumerable();

        if (SelectedTypeOption == "Purchase Invoices")
            filtered = filtered.Where(x => x.InvoiceType == InvoiceType.GenPurchase);
        else if (SelectedTypeOption == "Sale Invoices")
            filtered = filtered.Where(x => x.InvoiceType == InvoiceType.GenSale);
        else if (SelectedTypeOption == "Purchase Returns")
            filtered = filtered.Where(x => x.InvoiceType == InvoiceType.GenPurchaseReturn);
        else if (SelectedTypeOption == "Sale Returns")
            filtered = filtered.Where(x => x.InvoiceType == InvoiceType.GenSaleReturn);

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            string q = SearchQuery.Trim().ToLower();
            filtered = filtered.Where(x => x.InvoiceNo.ToLower().Contains(q) || x.PartyName.ToLower().Contains(q));
        }

        FilteredInvoices = new ObservableCollection<GeneralInvoiceRowDto>(filtered);
        SelectedInvoice = FilteredInvoices.FirstOrDefault();
    }

    partial void OnSearchQueryChanged(string value) => ApplyFilters();
    partial void OnSelectedTypeOptionChanged(string value) => ApplyFilters();
    partial void OnIsAllDatesChanged(bool value)
    {
        if (value)
        {
            IsDateRange = false;
            _ = LoadInvoicesAsync();
        }
    }

    partial void OnIsDateRangeChanged(bool value)
    {
        if (value)
        {
            IsAllDates = false;
            _ = LoadInvoicesAsync();
        }
    }

    partial void OnFromDateChanged(DateTime value)
    {
        if (IsDateRange) _ = LoadInvoicesAsync();
    }

    partial void OnToDateChanged(DateTime value)
    {
        if (IsDateRange) _ = LoadInvoicesAsync();
    }

    [RelayCommand]
    private void Confirm()
    {
        if (SelectedInvoice != null)
        {
            IsConfirmed = true;
            RequestClose?.Invoke();
        }
    }

    [RelayCommand]
    private void Cancel()
    {
        IsConfirmed = false;
        RequestClose?.Invoke();
    }
}
