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

public class GeneralMultiItemDto : ObservableObject
{
    private int _serialNo = 1;
    public int SerialNo { get => _serialNo; set => SetProperty(ref _serialNo, value); }

    private int _productId;
    public int ProductId { get => _productId; set => SetProperty(ref _productId, value); }

    private string _productCode = string.Empty;
    public string ProductCode { get => _productCode; set => SetProperty(ref _productCode, value); }

    private string _productName = string.Empty;
    public string ProductName { get => _productName; set => SetProperty(ref _productName, value); }

    private string _unitName = "PIECE";
    public string UnitName { get => _unitName; set => SetProperty(ref _unitName, value); }

    private decimal _multiplier = 1;
    public decimal Multiplier { get => _multiplier; set { if (SetProperty(ref _multiplier, value)) Recalculate(); } }

    private decimal _quantity = 1;
    public decimal Quantity { get => _quantity; set { if (SetProperty(ref _quantity, value)) Recalculate(); } }

    private decimal _bonus = 0;
    public decimal Bonus { get => _bonus; set { if (SetProperty(ref _bonus, value)) Recalculate(); } }

    // Purchase Side
    private decimal _purPrice = 0;
    public decimal PurPrice { get => _purPrice; set { if (SetProperty(ref _purPrice, value)) Recalculate(); } }

    private decimal _purDiscRatio = 0;
    public decimal PurDiscRatio { get => _purDiscRatio; set { if (SetProperty(ref _purDiscRatio, value)) Recalculate(); } }

    private decimal _purSTRatio = 0;
    public decimal PurSTRatio { get => _purSTRatio; set { if (SetProperty(ref _purSTRatio, value)) Recalculate(); } }

    private decimal _purGross = 0;
    public decimal PurGross { get => _purGross; set => SetProperty(ref _purGross, value); }

    private decimal _purNetValue = 0;
    public decimal PurNetValue { get => _purNetValue; set => SetProperty(ref _purNetValue, value); }

    // Sale Side
    private decimal _salePrice = 0;
    public decimal SalePrice { get => _salePrice; set { if (SetProperty(ref _salePrice, value)) Recalculate(); } }

    private decimal _saleDiscRatio = 0;
    public decimal SaleDiscRatio { get => _saleDiscRatio; set { if (SetProperty(ref _saleDiscRatio, value)) Recalculate(); } }

    private decimal _saleSTRatio = 0;
    public decimal SaleSTRatio { get => _saleSTRatio; set { if (SetProperty(ref _saleSTRatio, value)) Recalculate(); } }

    private decimal _saleGross = 0;
    public decimal SaleGross { get => _saleGross; set => SetProperty(ref _saleGross, value); }

    private decimal _saleNetValue = 0;
    public decimal SaleNetValue { get => _saleNetValue; set => SetProperty(ref _saleNetValue, value); }

    private decimal _margin = 0;
    public decimal Margin { get => _margin; set => SetProperty(ref _margin, value); }

    public void Recalculate()
    {
        decimal mult = Multiplier > 0 ? Multiplier : 1m;
        // Purchase calculation
        PurGross = Math.Round((Quantity * mult) * PurPrice, 2);
        decimal pDisc = Math.Round(PurGross * (PurDiscRatio / 100m), 2);
        decimal pTtl = PurGross - pDisc;
        decimal pStax = Math.Round(pTtl * (PurSTRatio / 100m), 2);
        PurNetValue = pTtl + pStax;

        // Sale calculation
        SaleGross = Math.Round((Quantity * mult) * SalePrice, 2);
        decimal sDisc = Math.Round(SaleGross * (SaleDiscRatio / 100m), 2);
        decimal sTtl = SaleGross - sDisc;
        decimal sStax = Math.Round(sTtl * (SaleSTRatio / 100m), 2);
        SaleNetValue = sTtl + sStax;

        Margin = SaleNetValue - PurNetValue;
    }
}

public partial class GeneralMultiInvoiceViewModel : ObservableObject
{
    private readonly IInvoiceService _invoiceService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    // Header Common
    [ObservableProperty] private int _loadedPurchaseInvoiceId = 0;
    [ObservableProperty] private int _loadedSaleInvoiceId = 0;
    [ObservableProperty] private DateTime _invoiceDate = DateTime.Today;
    [ObservableProperty] private string _vehicleNo = string.Empty;
    [ObservableProperty] private string _selectedCropSeason = string.Empty;
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();

    // Vendor (Purchase) Side
    [ObservableProperty] private string _purchaseId = string.Empty;
    [ObservableProperty] private ObservableCollection<Party> _availableVendors = new();
    [ObservableProperty] private Party? _selectedVendor;
    [ObservableProperty] private string _vendorId = string.Empty;
    [ObservableProperty] private string _vendorName = string.Empty;
    [ObservableProperty] private string _vendorAddress = string.Empty;
    [ObservableProperty] private decimal _vendorPrevBalance = 0;

    [ObservableProperty] private decimal _purExpPlus = 0;
    [ObservableProperty] private decimal _purExpMinus = 0;
    [ObservableProperty] private decimal _purFreight = 0;
    [ObservableProperty] private decimal _purComRatio = 0;
    [ObservableProperty] private decimal _purCommission = 0;
    [ObservableProperty] private decimal _purSpecialDiscount = 0;
    [ObservableProperty] private decimal _purPaidAmount = 0;
    [ObservableProperty] private decimal _purGrossTotal = 0;
    [ObservableProperty] private decimal _purNetPayable = 0;
    [ObservableProperty] private decimal _purRemBalance = 0;

    // Customer (Sale) Side
    [ObservableProperty] private string _saleId = string.Empty;
    [ObservableProperty] private ObservableCollection<Party> _availableCustomers = new();
    [ObservableProperty] private Party? _selectedCustomer;
    [ObservableProperty] private string _customerId = string.Empty;
    [ObservableProperty] private string _customerName = string.Empty;
    [ObservableProperty] private string _customerAddress = string.Empty;
    [ObservableProperty] private decimal _customerPrevBalance = 0;

    [ObservableProperty] private decimal _saleExpPlus = 0;
    [ObservableProperty] private decimal _saleExpMinus = 0;
    [ObservableProperty] private decimal _saleFreight = 0;
    [ObservableProperty] private decimal _saleComRatio = 0;
    [ObservableProperty] private decimal _saleCommission = 0;
    [ObservableProperty] private decimal _saleSpecialDiscount = 0;
    [ObservableProperty] private decimal _saleReceivedAmount = 0;
    [ObservableProperty] private decimal _saleGrossTotal = 0;
    [ObservableProperty] private decimal _saleNetReceivable = 0;
    [ObservableProperty] private decimal _saleRemBalance = 0;

    // Item Entry
    [ObservableProperty] private ObservableCollection<Product> _availableProducts = new();
    [ObservableProperty] private Product? _selectedProduct;
    [ObservableProperty] private string _currentUnitName = "PIECE";
    [ObservableProperty] private decimal _currentMultiplier = 1;
    [ObservableProperty] private decimal _currentQty = 1;
    [ObservableProperty] private decimal _currentBonus = 0;
    [ObservableProperty] private decimal _currentPurPrice = 0;
    [ObservableProperty] private decimal _currentSalePrice = 0;
    [ObservableProperty] private decimal _currentPurDisc = 0;
    [ObservableProperty] private decimal _currentSaleDisc = 0;

    // Shared Items Grid
    [ObservableProperty] private ObservableCollection<GeneralMultiItemDto> _items = new();
    [ObservableProperty] private GeneralMultiItemDto? _selectedGridItem;

    // Total Margin
    [ObservableProperty] private decimal _totalProfitMargin = 0;
    [ObservableProperty] private string _narration = string.Empty;
    [ObservableProperty] private string _narrationUrdu = string.Empty;

    public GeneralMultiInvoiceViewModel(
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
        await GenerateNewInvoiceNumbersAsync();
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

        // 2. Vendors & Customers
        var allParties = await _partyService.GetAllPartiesAsync();
        AvailableVendors = new ObservableCollection<Party>(allParties.Where(p => p.PartyType == PartyType.Vendor || p.PartyType == PartyType.Both));
        AvailableCustomers = new ObservableCollection<Party>(allParties.Where(p => p.PartyType == PartyType.Customer || p.PartyType == PartyType.Both));

        // 3. Products
        var prods = await _productService.GetAllProductsAsync();
        AvailableProducts = new ObservableCollection<Product>(prods);
    }

    private async Task GenerateNewInvoiceNumbersAsync()
    {
        var year = await _systemService.GetCurrentYearAsync();
        int yearId = year?.Id ?? 1;
        PurchaseId = await _invoiceService.GetNextInvoiceNoAsync(InvoiceType.GenPurchase, yearId);
        SaleId = await _invoiceService.GetNextInvoiceNoAsync(InvoiceType.GenSale, yearId);
    }

    partial void OnSelectedVendorChanged(Party? value)
    {
        if (value != null)
        {
            VendorId = value.PartyNo;
            VendorName = value.Name;
            VendorAddress = value.Address ?? "";
            VendorPrevBalance = value.Account?.CurrentBalance ?? 0;
            RecalculateTotals();
        }
    }

    partial void OnSelectedCustomerChanged(Party? value)
    {
        if (value != null)
        {
            CustomerId = value.PartyNo;
            CustomerName = value.Name;
            CustomerAddress = value.Address ?? "";
            CustomerPrevBalance = value.Account?.CurrentBalance ?? 0;
            RecalculateTotals();
        }
    }

    partial void OnSelectedProductChanged(Product? value)
    {
        if (value != null)
        {
            CurrentUnitName = value.Unit?.Name ?? "PIECE";
            CurrentMultiplier = 1;
            CurrentPurPrice = value.PurchasePrice;
            CurrentSalePrice = value.SalePrice;
        }
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

        var item = new GeneralMultiItemDto
        {
            SerialNo = Items.Count + 1,
            ProductId = SelectedProduct.Id,
            ProductCode = SelectedProduct.ProductCode,
            ProductName = SelectedProduct.Name,
            UnitName = CurrentUnitName,
            Multiplier = CurrentMultiplier > 0 ? CurrentMultiplier : 1m,
            Quantity = CurrentQty,
            Bonus = CurrentBonus,
            PurPrice = CurrentPurPrice,
            PurDiscRatio = CurrentPurDisc,
            SalePrice = CurrentSalePrice,
            SaleDiscRatio = CurrentSaleDisc
        };

        item.Recalculate();
        Items.Add(item);

        CurrentQty = 1;
        CurrentBonus = 0;
        SelectedProduct = null;

        RecalculateTotals();
    }

    [RelayCommand]
    private void RemoveItem(GeneralMultiItemDto? item)
    {
        if (item != null && Items.Contains(item))
        {
            Items.Remove(item);
            int sr = 1;
            foreach (var itm in Items) itm.SerialNo = sr++;
            RecalculateTotals();
        }
    }

    partial void OnPurExpPlusChanged(decimal value) => RecalculateTotals();
    partial void OnPurExpMinusChanged(decimal value) => RecalculateTotals();
    partial void OnPurFreightChanged(decimal value) => RecalculateTotals();
    partial void OnPurComRatioChanged(decimal value) => RecalculateTotals();
    partial void OnPurSpecialDiscountChanged(decimal value) => RecalculateTotals();
    partial void OnPurPaidAmountChanged(decimal value) => RecalculateTotals();

    partial void OnSaleExpPlusChanged(decimal value) => RecalculateTotals();
    partial void OnSaleExpMinusChanged(decimal value) => RecalculateTotals();
    partial void OnSaleFreightChanged(decimal value) => RecalculateTotals();
    partial void OnSaleComRatioChanged(decimal value) => RecalculateTotals();
    partial void OnSaleSpecialDiscountChanged(decimal value) => RecalculateTotals();
    partial void OnSaleReceivedAmountChanged(decimal value) => RecalculateTotals();

    private void RecalculateTotals()
    {
        // Purchase Totals
        PurGrossTotal = Items.Sum(i => i.PurGross);
        decimal pBase = Items.Sum(i => i.PurNetValue);
        PurCommission = Math.Round(pBase * (PurComRatio / 100m), 2);
        PurNetPayable = pBase + PurExpPlus - PurExpMinus + PurFreight - PurSpecialDiscount + PurCommission;
        PurRemBalance = VendorPrevBalance + PurNetPayable - PurPaidAmount;

        // Sale Totals
        SaleGrossTotal = Items.Sum(i => i.SaleGross);
        decimal sBase = Items.Sum(i => i.SaleNetValue);
        SaleCommission = Math.Round(sBase * (SaleComRatio / 100m), 2);
        SaleNetReceivable = sBase + SaleExpPlus - SaleExpMinus + SaleFreight - SaleSpecialDiscount + SaleCommission;
        SaleRemBalance = CustomerPrevBalance + SaleNetReceivable - SaleReceivedAmount;

        // Net Margin
        TotalProfitMargin = Items.Sum(i => i.Margin) + (SaleCommission - PurCommission);
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedVendor == null || SelectedCustomer == null)
        {
            MessageBox.Show("Please select both Vendor and Customer.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            // Purchase Invoice
            var purInvoice = new Invoice
            {
                Id = LoadedPurchaseInvoiceId,
                InvoiceNo = PurchaseId,
                InvoiceType = InvoiceType.GenPurchase,
                InvoiceDate = InvoiceDate,
                FinancialYearId = year?.Id ?? 1,
                CropSeasonId = season?.Id ?? 1,
                PartyId = SelectedVendor.Id,
                SubTotal = PurGrossTotal,
                DiscountAmount = Items.Sum(i => i.PurGross - i.PurNetValue) + PurSpecialDiscount,
                FreightCharges = PurFreight,
                NetAmount = PurNetPayable,
                Status = InvoiceStatus.Draft,
                IsPosted = false,
                IsMulti = true,

                ExpPlus = PurExpPlus,
                ExpMinus = PurExpMinus,
                ComRatio = PurComRatio,
                ComAmount = PurCommission,
                PaidAmount = PurPaidAmount,
                SpecialDiscount = PurSpecialDiscount,
                VehicleNo = VehicleNo,
                Narration = Narration,
                NarrationUrdu = NarrationUrdu
            };

            int pLine = 1;
            foreach (var item in Items)
            {
                purInvoice.Details.Add(new InvoiceDetail
                {
                    LineNo = pLine++,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Rate = item.PurPrice,
                    Amount = item.PurNetValue,
                    Multiplier = item.Multiplier,
                    Bonus = item.Bonus,
                    DiscRatio = item.PurDiscRatio,
                    TTLValue = item.PurNetValue,
                    Cost = item.PurPrice
                });
            }

            // Sale Invoice
            var saleInvoice = new Invoice
            {
                Id = LoadedSaleInvoiceId,
                InvoiceNo = SaleId,
                InvoiceType = InvoiceType.GenSale,
                InvoiceDate = InvoiceDate,
                FinancialYearId = year?.Id ?? 1,
                CropSeasonId = season?.Id ?? 1,
                PartyId = SelectedCustomer.Id,
                SubTotal = SaleGrossTotal,
                DiscountAmount = Items.Sum(i => i.SaleGross - i.SaleNetValue) + SaleSpecialDiscount,
                FreightCharges = SaleFreight,
                NetAmount = SaleNetReceivable,
                Status = InvoiceStatus.Draft,
                IsPosted = false,
                IsMulti = true,

                ExpPlus = SaleExpPlus,
                ExpMinus = SaleExpMinus,
                ComRatio = SaleComRatio,
                ComAmount = SaleCommission,
                ReceivedAmount = SaleReceivedAmount,
                SpecialDiscount = SaleSpecialDiscount,
                VehicleNo = VehicleNo,
                Narration = Narration,
                NarrationUrdu = NarrationUrdu,
                SectorId = SelectedCustomer.SectorId
            };

            int sLine = 1;
            foreach (var item in Items)
            {
                saleInvoice.Details.Add(new InvoiceDetail
                {
                    LineNo = sLine++,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Rate = item.SalePrice,
                    Amount = item.SaleNetValue,
                    Multiplier = item.Multiplier,
                    Bonus = item.Bonus,
                    DiscRatio = item.SaleDiscRatio,
                    TTLValue = item.SaleNetValue,
                    Cost = item.PurPrice
                });
            }

            await _invoiceService.SaveMultiInvoiceAsync(purInvoice, saleInvoice);
            await _invoiceService.PostInvoiceToGLAsync(purInvoice.Id);
            await _invoiceService.PostInvoiceToGLAsync(saleInvoice.Id);

            MessageBox.Show($"General Multi Invoice saved successfully!\nPurchase Invoice #{purInvoice.InvoiceNo} and Sale Invoice #{saleInvoice.InvoiceNo} linked and posted.", "Success", MessageBoxButton.OK, MessageBoxImage.Information);

            await New();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to save multi invoice:\n{ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task New()
    {
        LoadedPurchaseInvoiceId = 0;
        LoadedSaleInvoiceId = 0;
        SelectedVendor = null;
        SelectedCustomer = null;
        VendorPrevBalance = 0;
        CustomerPrevBalance = 0;
        VehicleNo = "";
        Narration = "";
        NarrationUrdu = "";
        Items.Clear();
        PurPaidAmount = 0;
        SaleReceivedAmount = 0;
        RecalculateTotals();
        await GenerateNewInvoiceNumbersAsync();
    }

    [RelayCommand]
    private void Print()
    {
        if (Items.Count == 0)
        {
            MessageBox.Show("No line items found to print.", "Print Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }
        MessageBox.Show($"Printing General Multi Invoice (Purchase #{PurchaseId} / Sale #{SaleId})...", "Print Preview", MessageBoxButton.OK, MessageBoxImage.Information);
    }
}
