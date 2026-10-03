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

public partial class GeneralInvoiceViewModel : ObservableObject
{
    private readonly IInvoiceService _invoiceService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private Invoice _currentInvoice = new();
    [ObservableProperty] private ObservableCollection<InvoiceDetail> _invoiceDetails = new();
    
    // Lookups
    [ObservableProperty] private ObservableCollection<Party> _parties = new();
    [ObservableProperty] private ObservableCollection<Product> _products = new();
    
    [ObservableProperty] private string _title = "General Invoice";
    [ObservableProperty] private InvoiceType _currentType;

    public GeneralInvoiceViewModel(
        IInvoiceService invoiceService,
        IPartyService partyService,
        IProductService productService,
        ISystemService systemService)
    {
        _invoiceService = invoiceService;
        _partyService = partyService;
        _productService = productService;
        _systemService = systemService;
        
        // Default to Purchase for now, can be set via navigation
        _ = InitializeAsync(InvoiceType.GenPurchase);
    }

    public async Task InitializeAsync(InvoiceType type)
    {
        CurrentType = type;
        Title = type switch
        {
            InvoiceType.GenPurchase => "GENERAL PURCHASE INVOICE",
            InvoiceType.GenSale => "GENERAL SALE INVOICE",
            InvoiceType.GenPurchaseReturn => "PURCHASE RETURN (DEBIT NOTE)",
            InvoiceType.GenSaleReturn => "SALE RETURN (CREDIT NOTE)",
            _ => "TRADE TRANSACTION"
        };
        
        // Load Parties (Filter by type)
        var allParties = await _partyService.GetAllPartiesAsync();
        if (type == InvoiceType.GenPurchase || type == InvoiceType.GenPurchaseReturn)
            Parties = new ObservableCollection<Party>(allParties.Where(p => p.PartyType == PartyType.Vendor || p.PartyType == PartyType.Both));
        else
            Parties = new ObservableCollection<Party>(allParties.Where(p => p.PartyType == PartyType.Customer || p.PartyType == PartyType.Both));

        // Load Products
        var allProducts = await _productService.GetAllProductsAsync();
        Products = new ObservableCollection<Product>(allProducts);

        await NewInvoiceAsync();
    }

    [RelayCommand]
    private async Task NewInvoiceAsync()
    {
        var financialYear = await _systemService.GetCurrentYearAsync();
        var season = (await _systemService.GetActiveSeasonsAsync()).FirstOrDefault(s => s.IsActive) ?? new CropSeason();

        CurrentInvoice = new Invoice
        {
            InvoiceType = CurrentType,
            InvoiceDate = DateTime.Today,
            FinancialYearId = financialYear?.Id ?? 0,
            CropSeasonId = season.Id,
            InvoiceNo = await _invoiceService.GetNextInvoiceNoAsync(CurrentType, financialYear?.Id ?? 0),
            Status = InvoiceStatus.Draft
        };
        InvoiceDetails = new ObservableCollection<InvoiceDetail>();
        AddRow();
    }

    [RelayCommand]
    private void AddRow()
    {
        var detail = new InvoiceDetail { LineNo = InvoiceDetails.Count + 1 };
        InvoiceDetails.Add(detail);
    }

    [RelayCommand]
    private void RemoveRow(InvoiceDetail detail)
    {
        if (detail != null)
        {
            InvoiceDetails.Remove(detail);
            CalculateTotals();
        }
    }

    public void OnRowChanged(InvoiceDetail detail)
    {
        detail.Amount = detail.Quantity * detail.Rate;
        CalculateTotals();
    }

    private void CalculateTotals()
    {
        CurrentInvoice.SubTotal = InvoiceDetails.Sum(d => d.Amount);
        
        // Calculations for Percentages
        if (CurrentInvoice.DiscountPercentage > 0)
            CurrentInvoice.DiscountAmount = (CurrentInvoice.SubTotal * CurrentInvoice.DiscountPercentage) / 100;
            
        if (CurrentInvoice.TaxPercentage > 0)
            CurrentInvoice.TaxAmount = (CurrentInvoice.SubTotal * CurrentInvoice.TaxPercentage) / 100;

        CurrentInvoice.NetAmount = CurrentInvoice.SubTotal 
                                 - CurrentInvoice.DiscountAmount 
                                 + CurrentInvoice.TaxAmount 
                                 + CurrentInvoice.FreightCharges 
                                 + CurrentInvoice.LaborCharges 
                                 + CurrentInvoice.MarketCommitteeFee;
        
        // Trigger UI updates for properties
        OnPropertyChanged(nameof(CurrentInvoice));
    }

    [RelayCommand]
    private async Task SaveInvoiceAsync()
    {
        if (CurrentInvoice.PartyId == 0)
        {
            System.Windows.MessageBox.Show("Please select a Party.");
            return;
        }

        if (!InvoiceDetails.Any(d => d.ProductId != 0 && d.Quantity > 0))
        {
            System.Windows.MessageBox.Show("Please add at least one valid product.");
            return;
        }

        try
        {
            CurrentInvoice.Details = InvoiceDetails.Where(d => d.ProductId != 0).ToList();
            await _invoiceService.SaveInvoiceAsync(CurrentInvoice);
            System.Windows.MessageBox.Show($"Invoice {CurrentInvoice.InvoiceNo} saved successfully.");
        }
        catch (Exception ex)
        {
            System.Windows.MessageBox.Show($"Error saving invoice: {ex.Message}");
        }
    }

    [RelayCommand]
    private async Task PostInvoiceAsync()
    {
        if (CurrentInvoice.Id == 0)
        {
            await SaveInvoiceAsync();
        }

        if (CurrentInvoice.Id != 0 && !CurrentInvoice.IsPosted)
        {
            if (System.Windows.MessageBox.Show("Are you sure you want to post this invoice? This will update Ledger and Stock.", "Confirm Posting", MessageBoxButton.YesNo) == MessageBoxResult.Yes)
            {
                try
                {
                    await _invoiceService.PostInvoiceToGLAsync(CurrentInvoice.Id);
                    CurrentInvoice.IsPosted = true;
                    CurrentInvoice.Status = InvoiceStatus.Posted;
                    System.Windows.MessageBox.Show("Invoice posted successfully.");
                    await NewInvoiceAsync();
                }
                catch (Exception ex)
                {
                    System.Windows.MessageBox.Show($"Error posting invoice: {ex.Message}");
                }
            }
        }
    }
}
