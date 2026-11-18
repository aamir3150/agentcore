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

public class PestroInvoiceRowDto
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public DateTime InvoiceDate { get; set; }
    public InvoiceType InvoiceType { get; set; }
    public BatchMode BatchMode { get; set; }
    public string TypeDisplay => InvoiceType == InvoiceType.PestroPurchase 
        ? "Purchase (خریداری)" 
        : (BatchMode == BatchMode.Auto ? "Sale Auto Batch (فروخت آٹو)" : "Sale Manual Batch (فروخت مینول)");
    public string PartyName { get; set; } = string.Empty;
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public bool IsPosted { get; set; }
    public string StatusDisplay => IsPosted ? "Posted" : "Draft";
}

public partial class PestroInvoiceLookupViewModel : ObservableObject
{
    private readonly IPestroService _pestroService;
    private List<PestroInvoiceRowDto> _allInvoices = new();

    [ObservableProperty] private string _searchQuery = "";
    [ObservableProperty] private bool _isAllDates = true;
    [ObservableProperty] private bool _isDateRange = false;
    [ObservableProperty] private DateTime _fromDate = new DateTime(DateTime.Today.Year, 7, 1);
    [ObservableProperty] private DateTime _toDate = DateTime.Today;

    [ObservableProperty] private ObservableCollection<string> _typeOptions = new() { "All Pestro", "Purchase Invoices", "Sale Auto Batch", "Sale Manual Batch" };
    [ObservableProperty] private string _selectedTypeOption = "All Pestro";

    [ObservableProperty] private ObservableCollection<PestroInvoiceRowDto> _filteredInvoices = new();
    [ObservableProperty] private PestroInvoiceRowDto? _selectedInvoice;

    public Action? RequestClose { get; set; }
    public bool IsConfirmed { get; private set; } = false;

    public PestroInvoiceLookupViewModel(IPestroService pestroService)
    {
        _pestroService = pestroService;
        _ = LoadInvoicesAsync();
    }

    public void SetTypeFilter(InvoiceType type, BatchMode batchMode = BatchMode.Auto)
    {
        if (type == InvoiceType.PestroPurchase)
            SelectedTypeOption = "Purchase Invoices";
        else if (type == InvoiceType.PestroSale && batchMode == BatchMode.Auto)
            SelectedTypeOption = "Sale Auto Batch";
        else if (type == InvoiceType.PestroSale && batchMode == BatchMode.Manual)
            SelectedTypeOption = "Sale Manual Batch";
    }

    public async Task LoadInvoicesAsync()
    {
        var invoices = await _pestroService.GetAllPestroInvoicesAsync();
        _allInvoices = invoices.Select(i => new PestroInvoiceRowDto
        {
            Id = i.Id,
            InvoiceNo = i.InvoiceNo,
            InvoiceDate = i.InvoiceDate,
            InvoiceType = i.InvoiceType,
            BatchMode = i.BatchMode,
            PartyName = i.Party?.Name ?? "",
            SubTotal = i.SubTotal,
            DiscountAmount = i.DiscountAmount,
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
            query = query.Where(x => x.InvoiceType == InvoiceType.PestroPurchase);
        }
        else if (SelectedTypeOption == "Sale Auto Batch")
        {
            query = query.Where(x => x.InvoiceType == InvoiceType.PestroSale && x.BatchMode == BatchMode.Auto);
        }
        else if (SelectedTypeOption == "Sale Manual Batch")
        {
            query = query.Where(x => x.InvoiceType == InvoiceType.PestroSale && x.BatchMode == BatchMode.Manual);
        }

        if (IsDateRange)
        {
            query = query.Where(x => x.InvoiceDate.Date >= FromDate.Date && x.InvoiceDate.Date <= ToDate.Date);
        }

        if (!string.IsNullOrWhiteSpace(SearchQuery))
        {
            var q = SearchQuery.Trim().ToLower();
            query = query.Where(x =>
                x.InvoiceNo.ToLower().Contains(q) ||
                x.PartyName.ToLower().Contains(q) ||
                x.NetAmount.ToString().Contains(q));
        }

        FilteredInvoices = new ObservableCollection<PestroInvoiceRowDto>(query);
        SelectedInvoice = FilteredInvoices.FirstOrDefault();
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
