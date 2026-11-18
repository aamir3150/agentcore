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

public partial class GeneralPurchaseInvoiceViewModel : ObservableObject
{
    private readonly IInvoiceService _invoiceService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    // Is Purchase Return Mode
    [ObservableProperty] private bool _isReturnMode = false;
    [ObservableProperty] private string _windowTitle = "GENERAL PURCHASE INVOICE";

    // Header Properties
    [ObservableProperty] private int _loadedInvoiceId = 0;
    [ObservableProperty] private string _purchaseId = string.Empty;
    [ObservableProperty] private DateTime _purchaseDate = DateTime.Today;
    [ObservableProperty] private string _billNo = string.Empty;
    [ObservableProperty] private DateTime? _billDate = DateTime.Today;
    [ObservableProperty] private string _vehicleNo = string.Empty;
    [ObservableProperty] private string _selectedCropSeason = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();

    // Vendor Information
    [ObservableProperty] private ObservableCollection<Party> _availableVendors = new();
    [ObservableProperty] private Party? _selectedVendor;
    [ObservableProperty] private string _vendorId = string.Empty;
    [ObservableProperty] private string _vendorName = string.Empty;
    [ObservableProperty] private string _address = string.Empty;
    [ObservableProperty] private decimal _previousBalance = 0;

    // Item Entry Controls
    [ObservableProperty] private ObservableCollection<Product> _availableProducts = new();
    [ObservableProperty] private Product? _selectedProduct;
    [ObservableProperty] private string _currentProductCode = string.Empty;
    [ObservableProperty] private string _currentProductName = string.Empty;
    [ObservableProperty] private string _currentUnitName = "PIECE";
    [ObservableProperty] private decimal _currentMultiplier = 1;
    [ObservableProperty] private decimal _currentQty = 1;
    [ObservableProperty] private decimal _currentBonus = 0;
    [ObservableProperty] private decimal _currentPrice = 0;
    [ObservableProperty] private decimal _currentDiscRatio = 0;
    [ObservableProperty] private decimal _currentSTRatio = 0;
    [ObservableProperty] private decimal _currentLineNet = 0;
    [ObservableProperty] private decimal _currentUnitCost = 0;

    // Items Grid
    [ObservableProperty] private ObservableCollection<GeneralGridItemDto> _items = new();
    [ObservableProperty] private GeneralGridItemDto? _selectedGridItem;

    // Financial Totals & Expense Controls
    [ObservableProperty] private decimal _grossTotal = 0;
    [ObservableProperty] private decimal _totalDiscount = 0;
    [ObservableProperty] private decimal _specialDiscount = 0;
    [ObservableProperty] private decimal _totalSalesTax = 0;
    [ObservableProperty] private decimal _expPlus = 0;
    [ObservableProperty] private decimal _expMinus = 0;
    [ObservableProperty] private decimal _freight = 0;
    [ObservableProperty] private decimal _comRatio = 0;
    [ObservableProperty] private decimal _commissionAmount = 0;
    [ObservableProperty] private decimal _netPayable = 0;
    [ObservableProperty] private decimal _paidAmount = 0;
    [ObservableProperty] private decimal _remainingBalance = 0;

    // Additional Information
    [ObservableProperty] private string _narration = string.Empty;
    [ObservableProperty] private string _narrationUrdu = string.Empty;
    [ObservableProperty] private bool _isCashInvoice = false;

    public GeneralPurchaseInvoiceViewModel(
        IInvoiceService invoiceService,
        IPartyService partyService,
        IProductService productService,
        ISystemService systemService)
    {
        _invoiceService = invoiceService;
        _partyService = partyService;
        _productService = productService;
        _systemService = systemService;

        _ = InitializeAsync();
    }

    public async Task InitializeAsync()
    {
        await LoadMasterDataAsync();
        await GenerateNewInvoiceNumberAsync();
    }

    public void SetReturnMode(bool isReturn)
    {
        IsReturnMode = isReturn;
        WindowTitle = isReturn 
            ? "PURCHASE RETURN INVOICE (DEBIT NOTE)" 
            : "GENERAL PURCHASE INVOICE";
        _ = GenerateNewInvoiceNumberAsync();
    }

    private async Task LoadMasterDataAsync()
    {
        // 1. Crop Seasons
        var seasons = await _systemService.GetAllSeasonsAsync();
        CropSeasons = new ObservableCollection<string>(seasons.Select(s => s.Name));
        var activeSeason = seasons.FirstOrDefault(s => s.IsActive);
        if (activeSeason != null)
            SelectedCropSeason = activeSeason.Name;
        else if (CropSeasons.Count > 0)
            SelectedCropSeason = CropSeasons[0];

        // 2. Vendors
        var allParties = await _partyService.GetAllPartiesAsync();
        var vendors = allParties.Where(p => p.PartyType == PartyType.Vendor || p.PartyType == PartyType.Both).ToList();
        AvailableVendors = new ObservableCollection<Party>(vendors);

        // 3. Products
        var prods = await _productService.GetAllProductsAsync();
        AvailableProducts = new ObservableCollection<Product>(prods);
    }

    private async Task GenerateNewInvoiceNumberAsync()
    {
        var year = await _systemService.GetCurrentYearAsync();
        int yearId = year?.Id ?? 1;
        var type = IsReturnMode ? InvoiceType.GenPurchaseReturn : InvoiceType.GenPurchase;
        PurchaseId = await _invoiceService.GetNextInvoiceNoAsync(type, yearId);
    }

    partial void OnSelectedVendorChanged(Party? value)
    {
        if (value != null)
        {
            VendorId = value.PartyNo;
            VendorName = value.Name;
            Address = value.Address ?? "";
            PreviousBalance = value.Account?.CurrentBalance ?? 0;
            RecalculateTotals();
        }
    }

    partial void OnSelectedProductChanged(Product? value)
    {
        if (value != null)
        {
            CurrentProductCode = value.ProductCode;
            CurrentProductName = value.Name;
            CurrentUnitName = value.Unit?.Name ?? "PIECE";
            CurrentMultiplier = 1;
            CurrentPrice = value.PurchasePrice;
            RecalculateCurrentLine();
        }
    }

    partial void OnCurrentQtyChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentBonusChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentPriceChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentMultiplierChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentDiscRatioChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentSTRatioChanged(decimal value) => RecalculateCurrentLine();

    private void RecalculateCurrentLine()
    {
        decimal mult = CurrentMultiplier > 0 ? CurrentMultiplier : 1m;
        decimal gross = Math.Round((CurrentQty * mult) * CurrentPrice, 2);
        decimal disc = Math.Round(gross * (CurrentDiscRatio / 100m), 2);
        decimal ttl = gross - disc;
        decimal stax = Math.Round(ttl * (CurrentSTRatio / 100m), 2);
        CurrentLineNet = ttl + stax;

        decimal totalEffQty = (CurrentQty + CurrentBonus) * mult;
        CurrentUnitCost = totalEffQty > 0 ? Math.Round(CurrentLineNet / totalEffQty, 4) : 0;
    }

    [RelayCommand]
    private void AddItem()
    {
        if (SelectedProduct == null)
        {
            MessageBox.Show("Please select a product first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CurrentQty <= 0)
        {
            MessageBox.Show("Quantity must be greater than zero.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var item = new GeneralGridItemDto
        {
            SerialNo = Items.Count + 1,
            ProductId = SelectedProduct.Id,
            ProductCode = SelectedProduct.ProductCode,
            ProductName = SelectedProduct.Name,
            UnitName = CurrentUnitName,
            Multiplier = CurrentMultiplier > 0 ? CurrentMultiplier : 1m,
            Quantity = CurrentQty,
            Bonus = CurrentBonus,
            Price = CurrentPrice,
            DiscRatio = CurrentDiscRatio,
            STRatio = CurrentSTRatio
        };

        item.Recalculate();
        Items.Add(item);

        // Reset line inputs
        CurrentQty = 1;
        CurrentBonus = 0;
        CurrentDiscRatio = 0;
        CurrentSTRatio = 0;
        SelectedProduct = null;

        RecalculateTotals();
    }

    [RelayCommand]
    private void RemoveItem(GeneralGridItemDto? item)
    {
        if (item != null && Items.Contains(item))
        {
            Items.Remove(item);
            int sr = 1;
            foreach (var itm in Items) itm.SerialNo = sr++;
            RecalculateTotals();
        }
    }

    partial void OnExpPlusChanged(decimal value) => RecalculateTotals();
    partial void OnExpMinusChanged(decimal value) => RecalculateTotals();
    partial void OnFreightChanged(decimal value) => RecalculateTotals();
    partial void OnComRatioChanged(decimal value) => RecalculateTotals();
    partial void OnSpecialDiscountChanged(decimal value) => RecalculateTotals();
    partial void OnPaidAmountChanged(decimal value) => RecalculateTotals();

    private void RecalculateTotals()
    {
        GrossTotal = Items.Sum(i => i.GrossAmount);
        decimal itemsDiscount = Items.Sum(i => i.DiscValue);
        TotalDiscount = itemsDiscount;
        TotalSalesTax = Items.Sum(i => i.STValue);

        decimal baseNetItems = Items.Sum(i => i.TTLValue);
        CommissionAmount = Math.Round(baseNetItems * (ComRatio / 100m), 2);

        decimal subNet = Items.Sum(i => i.TotalValue);
        NetPayable = subNet + ExpPlus - ExpMinus + Freight - SpecialDiscount + CommissionAmount;

        if (IsCashInvoice)
            PaidAmount = NetPayable;

        RemainingBalance = PreviousBalance + NetPayable - PaidAmount;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedVendor == null)
        {
            MessageBox.Show("Please select a vendor / supplier first.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Items.Count == 0)
        {
            MessageBox.Show("Please add at least one line item to the invoice.", "No Records", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var year = await _systemService.GetCurrentYearAsync();
            var season = (await _systemService.GetAllSeasonsAsync()).FirstOrDefault(s => s.Name == SelectedCropSeason);

            var type = IsReturnMode ? InvoiceType.GenPurchaseReturn : InvoiceType.GenPurchase;

            var invoice = new Invoice
            {
                Id = LoadedInvoiceId,
                InvoiceNo = PurchaseId,
                InvoiceType = type,
                InvoiceDate = PurchaseDate,
                FinancialYearId = year?.Id ?? 1,
                CropSeasonId = season?.Id ?? 1,
                PartyId = SelectedVendor.Id,
                SubTotal = GrossTotal,
                DiscountAmount = TotalDiscount + SpecialDiscount,
                TaxAmount = TotalSalesTax,
                FreightCharges = Freight,
                NetAmount = NetPayable,
                Status = InvoiceStatus.Draft,
                IsPosted = false,

                // Legacy metadata
                BillNo = BillNo,
                BillDate = BillDate,
                ExpPlus = ExpPlus,
                ExpMinus = ExpMinus,
                ComRatio = ComRatio,
                ComAmount = CommissionAmount,
                PaidAmount = PaidAmount,
                SpecialDiscount = SpecialDiscount,
                VehicleNo = VehicleNo,
                Narration = Narration,
                NarrationUrdu = NarrationUrdu
            };

            int lineNo = 1;
            foreach (var item in Items)
            {
                invoice.Details.Add(new InvoiceDetail
                {
                    LineNo = lineNo++,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Rate = item.Price,
                    Amount = item.TotalValue,
                    DiscountAmount = item.DiscValue,

                    // Legacy fields
                    Multiplier = item.Multiplier,
                    Bonus = item.Bonus,
                    DiscRatio = item.DiscRatio,
                    DiscValue = item.DiscValue,
                    STRatio = item.STRatio,
                    STValue = item.STValue,
                    TTLValue = item.TTLValue,
                    TTLSalesTax = item.STValue,
                    Cost = item.Cost,
                    DetailNarration = item.Narration
                });
            }

            await _invoiceService.SaveInvoiceAsync(invoice);
            await _invoiceService.PostInvoiceToGLAsync(invoice.Id);

            string mode = IsReturnMode ? "Purchase Return" : "Purchase";
            MessageBox.Show($"General {mode} Invoice (#{invoice.InvoiceNo}) saved successfully!\nInventory and General Ledger updated.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            await New();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save invoice:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task Open()
    {
        var lookupVm = new GeneralInvoiceLookupViewModel(_invoiceService);
        lookupVm.SetTypeFilter(IsReturnMode ? InvoiceType.GenPurchaseReturn : InvoiceType.GenPurchase);

        var lookupWin = new Views.GeneralInvoiceLookupWindow
        {
            DataContext = lookupVm,
            Owner = Application.Current.MainWindow
        };

        lookupWin.ShowDialog();

        if (lookupVm.IsConfirmed && lookupVm.SelectedInvoice != null)
        {
            var invoice = await _invoiceService.GetInvoiceByIdAsync(lookupVm.SelectedInvoice.Id);
            if (invoice != null)
            {
                LoadedInvoiceId = invoice.Id;
                PurchaseId = invoice.InvoiceNo;
                PurchaseDate = invoice.InvoiceDate;
                SelectedVendor = AvailableVendors.FirstOrDefault(v => v.Id == invoice.PartyId);
                VendorId = SelectedVendor?.PartyNo ?? invoice.PartyId.ToString();
                VendorName = SelectedVendor?.Name ?? "";
                Address = SelectedVendor?.Address ?? "";
                BillNo = invoice.BillNo ?? "";
                BillDate = invoice.BillDate ?? invoice.InvoiceDate;
                VehicleNo = invoice.VehicleNo ?? "";
                ExpPlus = invoice.ExpPlus;
                ExpMinus = invoice.ExpMinus;
                ComRatio = invoice.ComRatio;
                PaidAmount = invoice.PaidAmount;
                SpecialDiscount = invoice.SpecialDiscount;
                Freight = invoice.FreightCharges;
                Narration = invoice.Narration ?? "";
                NarrationUrdu = invoice.NarrationUrdu ?? "";

                if (invoice.CropSeason != null)
                    SelectedCropSeason = invoice.CropSeason.Name;

                Items.Clear();
                int sr = 1;
                foreach (var d in invoice.Details)
                {
                    var itm = new GeneralGridItemDto
                    {
                        SerialNo = sr++,
                        ProductId = d.ProductId,
                        ProductCode = d.Product?.ProductCode ?? d.ProductId.ToString(),
                        ProductName = d.Product?.Name ?? "",
                        UnitName = d.Product?.Unit?.Name ?? "PIECE",
                        Multiplier = d.Multiplier > 0 ? d.Multiplier : 1m,
                        Quantity = d.Quantity,
                        Bonus = d.Bonus,
                        Price = d.Rate,
                        DiscRatio = d.DiscRatio,
                        DiscValue = d.DiscValue,
                        STRatio = d.STRatio,
                        STValue = d.STValue,
                        TTLValue = d.TTLValue > 0 ? d.TTLValue : d.Amount,
                        TotalValue = d.Amount,
                        Cost = d.Cost,
                        Narration = d.DetailNarration
                    };
                    itm.Recalculate();
                    Items.Add(itm);
                }
                RecalculateTotals();
            }
        }
    }

    [RelayCommand]
    private async Task New()
    {
        LoadedInvoiceId = 0;
        SelectedVendor = null;
        VendorId = "";
        VendorName = "";
        Address = "";
        PreviousBalance = 0;
        BillNo = "";
        BillDate = DateTime.Today;
        VehicleNo = "";
        ExpPlus = 0;
        ExpMinus = 0;
        ComRatio = 0;
        CommissionAmount = 0;
        SpecialDiscount = 0;
        Freight = 0;
        PaidAmount = 0;
        Narration = "";
        NarrationUrdu = "";
        IsCashInvoice = false;
        Items.Clear();
        RecalculateTotals();
        await GenerateNewInvoiceNumberAsync();
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (LoadedInvoiceId == 0)
        {
            MessageBox.Show("Please open an existing saved invoice first.", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (MessageBox.Show($"Are you sure you want to delete invoice #{PurchaseId}?\nThis will automatically reverse inventory and ledger postings.", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question) == MessageBoxResult.Yes)
        {
            try
            {
                await _invoiceService.DeleteInvoiceAsync(LoadedInvoiceId);
                MessageBox.Show("Invoice deleted successfully.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
                await New();
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Failed to delete invoice:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    private void Print()
    {
        if (Items.Count == 0)
        {
            MessageBox.Show("No line items found to print.", "Print Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        MessageBox.Show($"Printing invoice #{PurchaseId}...", "Print Preview", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
