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

public partial class PestroViewModel : ObservableObject
{
    private readonly IPestroService _pestroService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private int _selectedTabIndex;
    
    // Batches
    [ObservableProperty] private ObservableCollection<PestroBatch> _batches = new();
    [ObservableProperty] private PestroBatch _selectedBatch = new();
    
    // Invoices
    [ObservableProperty] private ObservableCollection<PestroInvoice> _invoices = new();
    [ObservableProperty] private PestroInvoice _currentInvoice = new();
    [ObservableProperty] private ObservableCollection<PestroInvoiceDetail> _invoiceDetails = new();

    // Lookups
    [ObservableProperty] private ObservableCollection<Party> _parties = new();
    [ObservableProperty] private ObservableCollection<Product> _products = new();
    [ObservableProperty] private ObservableCollection<PestroBatch> _availableBatches = new();

    public PestroViewModel(
        IPestroService pestroService,
        IPartyService partyService,
        IProductService productService,
        ISystemService systemService)
    {
        _pestroService = pestroService;
        _partyService = partyService;
        _productService = productService;
        _systemService = systemService;
        
        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        await LoadLookupsAsync();
        await LoadBatchesAsync();
        await LoadInvoicesAsync();
    }

    private async Task LoadLookupsAsync()
    {
        Parties = new ObservableCollection<Party>(await _partyService.GetAllPartiesAsync());
        Products = new ObservableCollection<Product>(await _productService.GetAllProductsAsync());
    }

    #region Batch Management
    [RelayCommand]
    private async Task LoadBatchesAsync()
    {
        Batches = new ObservableCollection<PestroBatch>(await _pestroService.GetAllBatchesAsync());
    }

    [RelayCommand]
    private void NewBatch()
    {
        SelectedBatch = new PestroBatch { Status = BatchStatus.Active };
    }

    [RelayCommand]
    private async Task SaveBatchAsync()
    {
        if (SelectedBatch.ProductId == 0 || string.IsNullOrEmpty(SelectedBatch.BatchNo))
        {
            System.Windows.MessageBox.Show("Please select Product and enter Batch No.");
            return;
        }
        await _pestroService.SaveBatchAsync(SelectedBatch);
        await LoadBatchesAsync();
        System.Windows.MessageBox.Show("Batch saved successfully.");
    }
    #endregion

    #region Invoice Management
    [RelayCommand]
    private async Task LoadInvoicesAsync()
    {
        Invoices = new ObservableCollection<PestroInvoice>(await _pestroService.GetAllPestroInvoicesAsync());
    }

    [RelayCommand]
    private async Task NewInvoiceAsync(InvoiceType type)
    {
        var year = await _systemService.GetCurrentYearAsync();
        CurrentInvoice = new PestroInvoice 
        { 
            InvoiceType = type,
            InvoiceDate = DateTime.Today,
            FinancialYearId = year?.Id ?? 0,
            Status = InvoiceStatus.Draft
        };
        InvoiceDetails = new ObservableCollection<PestroInvoiceDetail>();
        AddRow();
    }

    [RelayCommand]
    private void AddRow()
    {
        var detail = new PestroInvoiceDetail { LineNo = InvoiceDetails.Count + 1 };
        InvoiceDetails.Add(detail);
    }

    public async void OnRowProductChanged(PestroInvoiceDetail detail)
    {
        if (detail.ProductId != 0)
        {
            var batches = await _pestroService.GetActiveBatchesAsync(detail.ProductId);
            // In a better UI, this would populate a per-row dropdown or a global list
        }
    }

    [RelayCommand]
    private async Task SaveInvoiceAsync()
    {
        CurrentInvoice.Details = InvoiceDetails.Where(d => d.ProductId != 0).ToList();
        await _pestroService.SavePestroInvoiceAsync(CurrentInvoice);
        await LoadInvoicesAsync();
        System.Windows.MessageBox.Show("Pestro Invoice saved.");
    }

    [RelayCommand]
    private async Task PostInvoiceAsync()
    {
        if (CurrentInvoice.Id != 0)
        {
            await _pestroService.PostPestroInvoiceToGLAsync(CurrentInvoice.Id);
            System.Windows.MessageBox.Show("Pestro Invoice posted and stock updated.");
            await LoadInvoicesAsync();
        }
    }
    #endregion
}
