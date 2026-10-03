using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public class BrokerageMultiPurchaseItemDto : ObservableObject
{
    public string ProductId { get; set; } = "01002";
    public string ProductName { get; set; } = "Cotton Premium";
    public int TQty { get; set; } = 100;
    public int BQty { get; set; } = 100;
    public int PQty { get; set; } = 0;
    public decimal WtPerBag { get; set; } = 50;
    public decimal ExtWt { get; set; } = 0;
    public decimal Shortage { get; set; } = 0;
    public decimal Deduction { get; set; } = 0;
    public decimal TotalKgs { get; set; } = 5000;
    public decimal MoundStandard { get; set; } = 37.324m;
    public string RateType { get; set; } = "Rate/Mound";
    public decimal Rate { get; set; } = 8500;
    public decimal RateBardana { get; set; } = 0;
    public decimal TotalValue { get; set; } = 1138664;
    public string ShortPer { get; set; } = "";
    public string ShortType { get; set; } = "";
}

public class BrokerageMultiSaleItemDto : ObservableObject
{
    public string ProductId { get; set; } = "01002";
    public string ProductName { get; set; } = "Cotton Premium";
    public int TQty { get; set; } = 100;
    public string VehicleNo { get; set; } = "LES-8841";
    public int BQty { get; set; } = 100;
    public int PQty { get; set; } = 0;
    public decimal WtPerBag { get; set; } = 50;
    public decimal ExtWt { get; set; } = 0;
    public decimal Shortage { get; set; } = 0;
    public decimal Deduction { get; set; } = 0;
    public decimal TotalKgs { get; set; } = 5000;
    public decimal MoundStandard { get; set; } = 37.324m;
    public string RateType { get; set; } = "Rate/Mound";
    public decimal Rate { get; set; } = 8900;
    public decimal RateBardana { get; set; } = 0;
    public decimal TotalValue { get; set; } = 1192246;
    public string ShortPer { get; set; } = "";
    public string ShortType { get; set; } = "";
    public string ContractId { get; set; } = "";
}

public partial class BrokerageMultiInvoiceViewModel : ObservableObject
{
    private readonly IBrokerageService _brokerageService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    // Header Identifiers (Row 1)
    [ObservableProperty] private string _multiId = "1";
    [ObservableProperty] private string _purchaseId1 = "1357";
    [ObservableProperty] private string _purchaseId2 = "1783";
    [ObservableProperty] private string _contractId = "";
    [ObservableProperty] private DateTime _invoiceDate = DateTime.Today;
    [ObservableProperty] private string _billNo = "";
    [ObservableProperty] private string _gatePassNo = "";
    [ObservableProperty] private string _partyId = "";
    [ObservableProperty] private string _partyName = "";
    [ObservableProperty] private string _partyAddress = "";
    [ObservableProperty] private string _partyPhone = "";

    // Header Identifiers (Row 2 - Sale / Customer)
    [ObservableProperty] private string _saleId1 = "426";
    [ObservableProperty] private string _saleId2 = "1784";
    [ObservableProperty] private string _vehicleNo = "";
    [ObservableProperty] private string _customerId = "";
    [ObservableProperty] private string _customerName = "";
    [ObservableProperty] private string _customerAddress = "";
    [ObservableProperty] private string _customerPhone = "";
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = "Cotton";

    // Purchase Section Controls
    [ObservableProperty] private string _purchaseVehicleNo = "";
    [ObservableProperty] private bool _contractWithDelivery = false;
    [ObservableProperty] private string _purchaseMounds = "0- 0";

    // Purchase Entry Row Inputs
    [ObservableProperty] private string _purchaseProductId = "01002";
    [ObservableProperty] private string _purchaseProductName = "";
    [ObservableProperty] private int _purchaseTQty = 0;
    [ObservableProperty] private int _purchaseBQty = 0;
    [ObservableProperty] private int _purchasePQty = 0;
    [ObservableProperty] private decimal _purchaseBhartiKgs = 50;
    [ObservableProperty] private decimal _purchaseLooseWeight = 0;
    [ObservableProperty] private decimal _purchaseShort = 0;
    [ObservableProperty] private ObservableCollection<string> _shortTypes = new() { "Mnd+", "Mnd-", "%" };
    [ObservableProperty] private string _purchaseSelectedShortType = "Mnd+";
    [ObservableProperty] private string _purchasePer = "";
    [ObservableProperty] private decimal _purchaseDed = 0;
    [ObservableProperty] private decimal _purchaseTotalWeight = 0;
    [ObservableProperty] private ObservableCollection<decimal> _moundStandards = new() { 37.324m, 40, 50, 100 };
    [ObservableProperty] private decimal _purchaseSelectedMoundStandard = 37.324m;
    [ObservableProperty] private ObservableCollection<string> _rateTypes = new() { "Rate/Mound", "Rate/Kg", "Rate/Bag", "Fixed" };
    [ObservableProperty] private string _purchaseSelectedRateType = "Rate/Mound";
    [ObservableProperty] private decimal _purchaseRate = 0;
    [ObservableProperty] private decimal _purchaseRateBardana = 0;
    [ObservableProperty] private decimal _purchaseTotalValue = 0;

    // Purchase Grid
    [ObservableProperty] private ObservableCollection<BrokerageMultiPurchaseItemDto> _purchaseItems = new();
    [ObservableProperty] private BrokerageMultiPurchaseItemDto? _selectedPurchaseItem;

    // Sale Section Header
    [ObservableProperty] private string _saleContractId = "";
    [ObservableProperty] private string _saleContractDate = "";
    [ObservableProperty] private string _saleContractQty = "";
    [ObservableProperty] private string _saleContractRemaining = "";
    [ObservableProperty] private string _saleMounds = "";

    // Sale Entry Row Inputs
    [ObservableProperty] private string _saleProductId = "01002";
    [ObservableProperty] private string _saleProductName = "";
    [ObservableProperty] private int _saleTQty = 0;
    [ObservableProperty] private string _saleRowVehicleNo = "";
    [ObservableProperty] private int _saleBQty = 0;
    [ObservableProperty] private int _salePQty = 0;
    [ObservableProperty] private decimal _saleBhartiKgs = 50;
    [ObservableProperty] private decimal _saleLooseWeight = 0;
    [ObservableProperty] private decimal _saleShort = 0;
    [ObservableProperty] private string _saleSelectedShortType = "Mnd-";
    [ObservableProperty] private string _salePer = "";
    [ObservableProperty] private decimal _saleDed = 0;
    [ObservableProperty] private decimal _saleTotalWeight = 0;
    [ObservableProperty] private decimal _saleSelectedMoundStandard = 37.324m;
    [ObservableProperty] private string _saleSelectedRateType = "Rate/Mound";
    [ObservableProperty] private decimal _saleRate = 0;
    [ObservableProperty] private decimal _saleRateBardana = 0;
    [ObservableProperty] private decimal _saleTotalValue = 0;

    // Sale Grid
    [ObservableProperty] private ObservableCollection<BrokerageMultiSaleItemDto> _saleItems = new();
    [ObservableProperty] private BrokerageMultiSaleItemDto? _selectedSaleItem;

    // Bottom Purchase Calculations
    [ObservableProperty] private ObservableCollection<string> _damiTypes = new() { "TotalValueWise", "PerBag", "PerMound" };
    [ObservableProperty] private string _purchaseDamiType = "TotalValueWise";
    [ObservableProperty] private decimal _purchaseDamiPercentage = 0;
    [ObservableProperty] private decimal _purchaseDamiValue = 0;
    [ObservableProperty] private bool _purchaseCommissionMinus = false;
    [ObservableProperty] private decimal _purchaseWhtPercentage = 0;
    [ObservableProperty] private decimal _purchaseWhtValue = 0;
    [ObservableProperty] private decimal _purchaseGrossTotal = 0;
    [ObservableProperty] private decimal _purchaseBardana = 0;
    [ObservableProperty] private bool _purchaseAddBardana = true;
    [ObservableProperty] private decimal _purchaseCharity = 0;
    [ObservableProperty] private bool _purchaseCharityPercentage = true;
    [ObservableProperty] private bool _purchaseCharityMound = false;
    [ObservableProperty] private decimal _purchaseOtherExpMinus = 0;
    [ObservableProperty] private decimal _purchaseOtherExpPlus = 0;
    [ObservableProperty] private decimal _purchaseVehicleCharges = 0;
    [ObservableProperty] private bool _purchaseVehiclePlus = false;
    [ObservableProperty] private bool _purchaseVehicleMinus = false;
    [ObservableProperty] private bool _purchaseVehicleSeparate = false;
    [ObservableProperty] private bool _purchaseVehicleRadioPercent = false;
    [ObservableProperty] private bool _purchaseVehicleRadioBag = true;
    [ObservableProperty] private bool _purchaseVehicleRadioMound = false;
    [ObservableProperty] private decimal _purchaseMazdoori = 0;
    [ObservableProperty] private bool _purchaseMazdooriPlus = false;
    [ObservableProperty] private bool _purchaseMazdooriMinus = false;
    [ObservableProperty] private bool _purchaseMazdooriWoShort = false;
    [ObservableProperty] private decimal _purchasePaid = 0;
    [ObservableProperty] private decimal _purchaseNetValue = 0;
    [ObservableProperty] private decimal _purchasePreviousBalance = 0;
    [ObservableProperty] private decimal _purchaseFinalBalance = 0;
    [ObservableProperty] private string _purchaseNarration = "";

    // Bottom Sale Calculations
    [ObservableProperty] private string _saleDamiType = "TotalValueWise";
    [ObservableProperty] private decimal _saleDamiPercentage = 0;
    [ObservableProperty] private decimal _saleDamiValue = 0;
    [ObservableProperty] private bool _saleCommissionMinus = false;
    [ObservableProperty] private decimal _saleGrossTotal = 0;
    [ObservableProperty] private decimal _saleBardana = 0;
    [ObservableProperty] private bool _saleAddBardana = true;
    [ObservableProperty] private decimal _saleOtherExpMinus = 0;
    [ObservableProperty] private decimal _saleOtherExpPlus = 0;
    [ObservableProperty] private decimal _saleVehicleCharges = 0;
    [ObservableProperty] private bool _saleVehiclePlus = false;
    [ObservableProperty] private bool _saleVehicleMinus = false;
    [ObservableProperty] private bool _saleVehicleSeparate = false;
    [ObservableProperty] private decimal _saleMazdoori = 0;
    [ObservableProperty] private bool _saleMazdooriPlus = false;
    [ObservableProperty] private bool _saleMazdooriMinus = false;
    [ObservableProperty] private bool _saleMazdooriWoShort = false;
    [ObservableProperty] private bool _saleMazdooriRadioPercent = false;
    [ObservableProperty] private bool _saleMazdooriRadioBag = true;
    [ObservableProperty] private bool _saleMazdooriRadioMound = false;
    [ObservableProperty] private decimal _saleWhTax1 = 0;
    [ObservableProperty] private decimal _saleWhTax2 = 0;
    [ObservableProperty] private bool _saleWhTaxRadioPercent = true;
    [ObservableProperty] private bool _saleWhTaxRadio100Kg = false;
    [ObservableProperty] private bool _saleWhTaxPlus = false;
    [ObservableProperty] private bool _saleWhTaxMinus = false;
    [ObservableProperty] private decimal _saleReceived = 0;
    [ObservableProperty] private decimal _saleNetValue = 0;
    [ObservableProperty] private decimal _salePreviousBalance = 0;
    [ObservableProperty] private decimal _saleFinalBalance = 0;
    [ObservableProperty] private string _saleNarration = "";

    // Bottom Global Summary
    [ObservableProperty] private decimal _profitLoss = 0;
    [ObservableProperty] private string _accountNo = "";
    [ObservableProperty] private bool _sendSMS = false;

    public Action? RequestClose { get; set; }

    public BrokerageMultiInvoiceViewModel(
        IBrokerageService brokerageService, 
        IPartyService partyService, 
        IProductService productService, 
        ISystemService systemService)
    {
        _brokerageService = brokerageService;
        _partyService = partyService;
        _productService = productService;
        _systemService = systemService;

        _ = InitializeDataAsync();
    }

    private async Task InitializeDataAsync()
    {
        CropSeasons = new ObservableCollection<string> { "Cotton", "Wheat", "Rice", "Maize", "Mustard" };
        SelectedCropSeason = "Cotton";

        // Initial sample row for Purchase
        PurchaseItems.Add(new BrokerageMultiPurchaseItemDto
        {
            ProductId = "01002",
            ProductName = "Cotton Phutti",
            TQty = 100,
            BQty = 100,
            PQty = 0,
            WtPerBag = 50,
            ExtWt = 0,
            Shortage = 0,
            Deduction = 0,
            TotalKgs = 5000,
            MoundStandard = 37.324m,
            RateType = "Rate/Mound",
            Rate = 8500,
            RateBardana = 0,
            TotalValue = 1138664
        });

        // Initial sample row for Sale
        SaleItems.Add(new BrokerageMultiSaleItemDto
        {
            ProductId = "01002",
            ProductName = "Cotton Phutti",
            TQty = 100,
            VehicleNo = "LES-8841",
            BQty = 100,
            PQty = 0,
            WtPerBag = 50,
            ExtWt = 0,
            Shortage = 0,
            Deduction = 0,
            TotalKgs = 5000,
            MoundStandard = 37.324m,
            RateType = "Rate/Mound",
            Rate = 8900,
            RateBardana = 0,
            TotalValue = 1192246,
            ContractId = ""
        });

        RecalculateTotals();
    }

    public void RecalculateTotals()
    {
        // Purchase calculations
        PurchaseGrossTotal = PurchaseItems.Sum(i => i.TotalValue);
        decimal pKgs = PurchaseItems.Sum(i => i.TotalKgs);
        decimal pMds = Math.Floor(pKgs / 37.324m);
        decimal pRem = pKgs % 37.324m;
        PurchaseMounds = $"{pMds}- {pRem:00}";

        PurchaseNetValue = PurchaseGrossTotal + (PurchaseAddBardana ? PurchaseBardana : 0) - PurchaseOtherExpMinus + PurchaseOtherExpPlus;
        PurchaseFinalBalance = PurchasePreviousBalance + PurchaseNetValue - PurchasePaid;

        // Sale calculations
        SaleGrossTotal = SaleItems.Sum(i => i.TotalValue);
        SaleNetValue = SaleGrossTotal + (SaleAddBardana ? SaleBardana : 0) - SaleOtherExpMinus + SaleOtherExpPlus;
        SaleFinalBalance = SalePreviousBalance + SaleNetValue - SaleReceived;

        // Profit / Loss Calculation
        ProfitLoss = SaleNetValue - PurchaseNetValue;
    }

    [RelayCommand]
    private void AddPurchaseItem()
    {
        if (PurchaseTotalValue <= 0 && PurchaseRate <= 0) return;

        decimal totalKgs = (PurchaseTQty * PurchaseBhartiKgs) + PurchaseLooseWeight - PurchaseShort - PurchaseDed;
        decimal mds = PurchaseSelectedMoundStandard > 0 ? totalKgs / PurchaseSelectedMoundStandard : totalKgs / 37.324m;
        decimal value = PurchaseSelectedRateType == "Rate/Mound" ? mds * PurchaseRate : totalKgs * PurchaseRate;

        PurchaseItems.Add(new BrokerageMultiPurchaseItemDto
        {
            ProductId = PurchaseProductId,
            ProductName = PurchaseProductName,
            TQty = PurchaseTQty,
            BQty = PurchaseBQty,
            PQty = PurchasePQty,
            WtPerBag = PurchaseBhartiKgs,
            ExtWt = PurchaseLooseWeight,
            Shortage = PurchaseShort,
            Deduction = PurchaseDed,
            TotalKgs = totalKgs,
            MoundStandard = PurchaseSelectedMoundStandard,
            RateType = PurchaseSelectedRateType,
            Rate = PurchaseRate,
            RateBardana = PurchaseRateBardana,
            TotalValue = value
        });

        RecalculateTotals();
    }

    [RelayCommand]
    private void AddSaleItem()
    {
        if (SaleTotalValue <= 0 && SaleRate <= 0) return;

        decimal totalKgs = (SaleTQty * SaleBhartiKgs) + SaleLooseWeight - SaleShort - SaleDed;
        decimal mds = SaleSelectedMoundStandard > 0 ? totalKgs / SaleSelectedMoundStandard : totalKgs / 37.324m;
        decimal value = SaleSelectedRateType == "Rate/Mound" ? mds * SaleRate : totalKgs * SaleRate;

        SaleItems.Add(new BrokerageMultiSaleItemDto
        {
            ProductId = SaleProductId,
            ProductName = SaleProductName,
            TQty = SaleTQty,
            VehicleNo = SaleRowVehicleNo,
            BQty = SaleBQty,
            PQty = SalePQty,
            WtPerBag = SaleBhartiKgs,
            ExtWt = SaleLooseWeight,
            Shortage = SaleShort,
            Deduction = SaleDed,
            TotalKgs = totalKgs,
            MoundStandard = SaleSelectedMoundStandard,
            RateType = SaleSelectedRateType,
            Rate = SaleRate,
            RateBardana = SaleRateBardana,
            TotalValue = value
        });

        RecalculateTotals();
    }

    [RelayCommand]
    private void Save()
    {
        System.Windows.MessageBox.Show("Brokerage Multi Invoice saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Clear()
    {
        PurchaseItems.Clear();
        SaleItems.Clear();
        PurchaseGrossTotal = 0;
        PurchaseNetValue = 0;
        PurchaseFinalBalance = 0;
        SaleGrossTotal = 0;
        SaleNetValue = 0;
        SaleFinalBalance = 0;
        ProfitLoss = 0;
        PurchaseMounds = "0- 0";
    }

    [RelayCommand]
    private void Print()
    {
        System.Windows.MessageBox.Show("Printing Brokerage Multi Invoice...", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Open()
    {
        System.Windows.MessageBox.Show("Open Multi Invoice Lookup dialog.", "Open", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Remove()
    {
        if (SelectedPurchaseItem != null)
        {
            PurchaseItems.Remove(SelectedPurchaseItem);
        }
        if (SelectedSaleItem != null)
        {
            SaleItems.Remove(SelectedSaleItem);
        }
        RecalculateTotals();
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
