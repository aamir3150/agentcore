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

public class BrokerageMultiPurchaseItemDto : ObservableObject
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int TQty { get; set; } = 0;
    public int BQty { get; set; } = 0;
    public int PQty { get; set; } = 0;
    public decimal WtPerBag { get; set; } = 50;
    public decimal ExtWt { get; set; } = 0;
    public decimal Shortage { get; set; } = 0;
    public decimal Deduction { get; set; } = 0;
    public decimal TotalKgs { get; set; } = 0;
    public decimal MoundStandard { get; set; } = 37.324m;
    public string RateType { get; set; } = "Rate/Mound";
    public decimal Rate { get; set; } = 0;
    public decimal RateBardana { get; set; } = 0;
    public decimal TotalValue { get; set; } = 0;
    public string ShortPer { get; set; } = "";
    public string ShortType { get; set; } = "";
}

public class BrokerageMultiSaleItemDto : ObservableObject
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int TQty { get; set; } = 0;
    public string VehicleNo { get; set; } = string.Empty;
    public int BQty { get; set; } = 0;
    public int PQty { get; set; } = 0;
    public decimal WtPerBag { get; set; } = 50;
    public decimal ExtWt { get; set; } = 0;
    public decimal Shortage { get; set; } = 0;
    public decimal Deduction { get; set; } = 0;
    public decimal TotalKgs { get; set; } = 0;
    public decimal MoundStandard { get; set; } = 37.324m;
    public string RateType { get; set; } = "Rate/Mound";
    public decimal Rate { get; set; } = 0;
    public decimal RateBardana { get; set; } = 0;
    public decimal TotalValue { get; set; } = 0;
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
    [ObservableProperty] private string _multiId = "";
    [ObservableProperty] private string _purchaseId1 = "";
    [ObservableProperty] private string _purchaseId2 = "";
    [ObservableProperty] private string _contractId = "";
    [ObservableProperty] private DateTime _invoiceDate = DateTime.Today;
    [ObservableProperty] private string _billNo = "";
    [ObservableProperty] private string _gatePassNo = "";
    [ObservableProperty] private string _partyId = "";
    [ObservableProperty] private string _partyName = "";
    [ObservableProperty] private string _partyAddress = "";
    [ObservableProperty] private string _partyPhone = "";

    // Header Identifiers (Row 2 - Sale / Customer)
    [ObservableProperty] private string _saleId1 = "";
    [ObservableProperty] private string _saleId2 = "";
    [ObservableProperty] private string _vehicleNo = "";
    [ObservableProperty] private string _customerId = "";
    [ObservableProperty] private string _customerName = "";
    [ObservableProperty] private string _customerAddress = "";
    [ObservableProperty] private string _customerPhone = "";
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = string.Empty;

    // Purchase Section Controls
    [ObservableProperty] private string _purchaseVehicleNo = "";
    [ObservableProperty] private bool _contractWithDelivery = false;
    [ObservableProperty] private string _purchaseMounds = "0- 0";

    // Purchase Entry Row Inputs
    [ObservableProperty] private string _purchaseProductId = string.Empty;
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
    [ObservableProperty] private string _saleProductId = string.Empty;
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
        try
        {
            var seasons = await _systemService.GetActiveSeasonsAsync();
            if (seasons.Any())
            {
                CropSeasons = new ObservableCollection<string>(seasons.Select(s => s.Name));
                SelectedCropSeason = seasons.FirstOrDefault(s => s.IsActive)?.Name ?? seasons.First().Name;
            }

            var year = await _systemService.GetCurrentYearAsync();
            if (year != null)
            {
                PurchaseId1 = await _systemService.GetNextDocumentNoAsync(Core.Entities.DocumentType.BrokerageInvoice, year.Id);
                SaleId1 = PurchaseId1;
                MultiId = PurchaseId1;
            }
        }
        catch
        {
            // Graceful fallback if database is not reachable
        }

        PurchaseItems.Clear();
        SaleItems.Clear();
        RecalculateTotals();
    }

    private void RecalculatePurchaseCurrentLine()
    {
        decimal grossKgs = (PurchaseTQty * PurchaseBhartiKgs) + PurchaseLooseWeight;
        decimal totalKgs = grossKgs;

        if (PurchaseShort > 0)
        {
            if (PurchaseSelectedShortType == "%")
            {
                totalKgs -= Math.Round((totalKgs * PurchaseShort) / 100m, 2);
            }
            else if (PurchaseSelectedShortType == "Mnd-")
            {
                decimal std = PurchaseSelectedMoundStandard > 0 ? PurchaseSelectedMoundStandard : 40m;
                totalKgs -= (PurchaseShort * std);
            }
            else // "Mnd+"
            {
                decimal std = PurchaseSelectedMoundStandard > 0 ? PurchaseSelectedMoundStandard : 40m;
                totalKgs += (PurchaseShort * std);
            }
        }

        if (PurchaseDed > 0)
        {
            totalKgs -= PurchaseDed;
        }

        if (totalKgs < 0) totalKgs = 0;
        PurchaseTotalWeight = Math.Round(totalKgs, 2);

        decimal stdMound = PurchaseSelectedMoundStandard > 0 ? PurchaseSelectedMoundStandard : 40m;
        decimal baseValue = 0;

        if (PurchaseSelectedRateType == "Rate/Mound")
        {
            decimal mds = PurchaseTotalWeight / stdMound;
            baseValue = mds * PurchaseRate;
        }
        else if (PurchaseSelectedRateType == "Rate/Kg")
        {
            baseValue = PurchaseTotalWeight * PurchaseRate;
        }
        else if (PurchaseSelectedRateType == "Rate/Bag")
        {
            baseValue = PurchaseTQty * PurchaseRate;
        }
        else // "Fixed"
        {
            baseValue = PurchaseRate;
        }

        decimal bardanaValue = PurchaseTQty * PurchaseRateBardana;
        PurchaseTotalValue = Math.Round(baseValue + bardanaValue, 2);
    }

    private void RecalculateSaleCurrentLine()
    {
        decimal grossKgs = (SaleTQty * SaleBhartiKgs) + SaleLooseWeight;
        decimal totalKgs = grossKgs;

        if (SaleShort > 0)
        {
            if (SaleSelectedShortType == "%")
            {
                totalKgs -= Math.Round((totalKgs * SaleShort) / 100m, 2);
            }
            else if (SaleSelectedShortType == "Mnd-")
            {
                decimal std = SaleSelectedMoundStandard > 0 ? SaleSelectedMoundStandard : 40m;
                totalKgs -= (SaleShort * std);
            }
            else // "Mnd+"
            {
                decimal std = SaleSelectedMoundStandard > 0 ? SaleSelectedMoundStandard : 40m;
                totalKgs += (SaleShort * std);
            }
        }

        if (SaleDed > 0)
        {
            totalKgs -= SaleDed;
        }

        if (totalKgs < 0) totalKgs = 0;
        SaleTotalWeight = Math.Round(totalKgs, 2);

        decimal stdMound = SaleSelectedMoundStandard > 0 ? SaleSelectedMoundStandard : 40m;
        decimal baseValue = 0;

        if (SaleSelectedRateType == "Rate/Mound")
        {
            decimal mds = SaleTotalWeight / stdMound;
            baseValue = mds * SaleRate;
        }
        else if (SaleSelectedRateType == "Rate/Kg")
        {
            baseValue = SaleTotalWeight * SaleRate;
        }
        else if (SaleSelectedRateType == "Rate/Bag")
        {
            baseValue = SaleTQty * SaleRate;
        }
        else // "Fixed"
        {
            baseValue = SaleRate;
        }

        decimal bardanaValue = SaleTQty * SaleRateBardana;
        SaleTotalValue = Math.Round(baseValue + bardanaValue, 2);
    }

    public void RecalculateTotals()
    {
        // 1. Purchase Calculations
        PurchaseGrossTotal = Math.Round(PurchaseItems.Sum(i => i.TotalValue), 2);
        decimal pKgs = PurchaseItems.Sum(i => i.TotalKgs);
        int pBags = PurchaseItems.Sum(i => i.TQty);
        decimal pMds = Math.Floor(pKgs / 40m);
        decimal pRem = Math.Round(pKgs % 40m, 2);
        PurchaseMounds = $"{pMds}- {pRem:00}";

        if (PurchaseDamiPercentage > 0)
        {
            if (PurchaseDamiType == "TotalValueWise")
                PurchaseDamiValue = Math.Round((PurchaseGrossTotal * PurchaseDamiPercentage) / 100m, 2);
            else if (PurchaseDamiType == "PerBag")
                PurchaseDamiValue = Math.Round(pBags * PurchaseDamiPercentage, 2);
            else
                PurchaseDamiValue = Math.Round((pKgs / 40m) * PurchaseDamiPercentage, 2);
        }
        else PurchaseDamiValue = 0;

        if (PurchaseWhtPercentage > 0)
        {
            if (PurchaseCommissionMinus)
                PurchaseWhtValue = Math.Round((PurchaseDamiValue * PurchaseWhtPercentage) / 100m, 2);
            else
                PurchaseWhtValue = Math.Round((PurchaseGrossTotal * PurchaseWhtPercentage) / 100m, 2);
        }
        else PurchaseWhtValue = 0;

        decimal pVeh = 0;
        if (PurchaseVehicleCharges > 0)
        {
            if (PurchaseVehicleRadioBag) pVeh = Math.Round(pBags * PurchaseVehicleCharges, 2);
            else if (PurchaseVehicleRadioMound) pVeh = Math.Round((pKgs / 40m) * PurchaseVehicleCharges, 2);
            else if (PurchaseVehicleRadioPercent) pVeh = Math.Round((PurchaseGrossTotal * PurchaseVehicleCharges) / 100m, 2);
            else pVeh = PurchaseVehicleCharges;
        }

        decimal pMaz = 0;
        if (PurchaseMazdoori > 0) pMaz = Math.Round(pBags * PurchaseMazdoori, 2);

        decimal pBardana = PurchaseAddBardana ? PurchaseBardana : 0;
        decimal pCommDeduction = PurchaseCommissionMinus ? PurchaseDamiValue : 0;
        decimal pVehicleAdj = PurchaseVehiclePlus ? pVeh : (PurchaseVehicleMinus ? -pVeh : 0);
        decimal pMazdooriAdj = PurchaseMazdooriPlus ? pMaz : (PurchaseMazdooriMinus ? -pMaz : 0);

        PurchaseNetValue = Math.Round(PurchaseGrossTotal + pBardana - PurchaseOtherExpMinus + PurchaseOtherExpPlus - pCommDeduction + pVehicleAdj + pMazdooriAdj - PurchaseWhtValue, 2);
        PurchaseFinalBalance = Math.Round(PurchasePreviousBalance + PurchaseNetValue - PurchasePaid, 2);

        // 2. Sale Calculations
        SaleGrossTotal = Math.Round(SaleItems.Sum(i => i.TotalValue), 2);
        decimal sKgs = SaleItems.Sum(i => i.TotalKgs);
        int sBags = SaleItems.Sum(i => i.TQty);
        decimal sMds = Math.Floor(sKgs / 40m);
        decimal sRem = Math.Round(sKgs % 40m, 2);
        SaleMounds = $"{sMds}- {sRem:00}";

        if (SaleDamiPercentage > 0)
        {
            if (SaleDamiType == "TotalValueWise")
                SaleDamiValue = Math.Round((SaleGrossTotal * SaleDamiPercentage) / 100m, 2);
            else if (SaleDamiType == "PerBag")
                SaleDamiValue = Math.Round(sBags * SaleDamiPercentage, 2);
            else
                SaleDamiValue = Math.Round((sKgs / 40m) * SaleDamiPercentage, 2);
        }
        else SaleDamiValue = 0;

        decimal sMaz = 0;
        if (SaleMazdoori > 0)
        {
            if (SaleMazdooriRadioBag) sMaz = Math.Round(sBags * SaleMazdoori, 2);
            else if (SaleMazdooriRadioMound) sMaz = Math.Round((sKgs / 40m) * SaleMazdoori, 2);
            else if (SaleMazdooriRadioPercent) sMaz = Math.Round((SaleGrossTotal * SaleMazdoori) / 100m, 2);
            else sMaz = SaleMazdoori;
        }

        if (SaleWhTax1 > 0)
        {
            if (SaleWhTaxRadioPercent) SaleWhTax2 = Math.Round((SaleGrossTotal * SaleWhTax1) / 100m, 2);
            else if (SaleWhTaxRadio100Kg) SaleWhTax2 = Math.Round((sKgs / 100m) * SaleWhTax1, 2);
            else SaleWhTax2 = SaleWhTax1;
        }
        else SaleWhTax2 = 0;

        decimal sBardana = SaleAddBardana ? SaleBardana : 0;
        decimal sCommDeduction = SaleCommissionMinus ? SaleDamiValue : 0;
        decimal sVehicleAdj = SaleVehiclePlus ? SaleVehicleCharges : (SaleVehicleMinus ? -SaleVehicleCharges : 0);
        decimal sMazdooriAdj = SaleMazdooriPlus ? sMaz : (SaleMazdooriMinus ? -sMaz : 0);
        decimal sWhtAdj = SaleWhTaxPlus ? SaleWhTax2 : (SaleWhTaxMinus ? -SaleWhTax2 : 0);

        SaleNetValue = Math.Round(SaleGrossTotal + sBardana - SaleOtherExpMinus + SaleOtherExpPlus - sCommDeduction + sVehicleAdj + sMazdooriAdj + sWhtAdj, 2);
        SaleFinalBalance = Math.Round(SalePreviousBalance + SaleNetValue - SaleReceived, 2);

        // 3. Profit / Loss Calculation
        ProfitLoss = Math.Round(SaleNetValue - PurchaseNetValue, 2);
    }

    [RelayCommand]
    private void AddPurchaseItem()
    {
        if (PurchaseTotalValue <= 0 && PurchaseRate <= 0) return;

        RecalculatePurchaseCurrentLine();

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
            TotalKgs = PurchaseTotalWeight,
            MoundStandard = PurchaseSelectedMoundStandard,
            RateType = PurchaseSelectedRateType,
            Rate = PurchaseRate,
            RateBardana = PurchaseRateBardana,
            TotalValue = PurchaseTotalValue,
            ShortType = PurchaseSelectedShortType,
            ShortPer = PurchasePer
        });

        RecalculateTotals();
    }

    [RelayCommand]
    private void AddSaleItem()
    {
        if (SaleTotalValue <= 0 && SaleRate <= 0) return;

        RecalculateSaleCurrentLine();

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
            TotalKgs = SaleTotalWeight,
            MoundStandard = SaleSelectedMoundStandard,
            RateType = SaleSelectedRateType,
            Rate = SaleRate,
            RateBardana = SaleRateBardana,
            TotalValue = SaleTotalValue,
            ShortType = SaleSelectedShortType,
            ShortPer = SalePer,
            ContractId = SaleContractId
        });

        RecalculateTotals();
    }

    partial void OnPartyIdChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            PartyName = "";
            PartyAddress = "";
            PartyPhone = "";
            PurchasePreviousBalance = 0;
            return;
        }

        _ = LookupPartyAsync(value);
    }

    private async Task LookupPartyAsync(string partyNo)
    {
        var party = await _partyService.GetPartyByNoAsync(partyNo);
        if (party != null)
        {
            PartyName = party.Name;
            PartyAddress = party.Address ?? "";
            PartyPhone = party.Phone ?? "";
            PurchasePreviousBalance = party.Account?.CurrentBalance ?? 0;
            RecalculateTotals();
        }
    }

    partial void OnCustomerIdChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            CustomerName = "";
            CustomerAddress = "";
            CustomerPhone = "";
            SalePreviousBalance = 0;
            return;
        }

        _ = LookupCustomerAsync(value);
    }

    private async Task LookupCustomerAsync(string partyNo)
    {
        var customer = await _partyService.GetPartyByNoAsync(partyNo);
        if (customer != null)
        {
            CustomerName = customer.Name;
            CustomerAddress = customer.Address ?? "";
            CustomerPhone = customer.Phone ?? "";
            SalePreviousBalance = customer.Account?.CurrentBalance ?? 0;
            RecalculateTotals();
        }
    }

    partial void OnPurchaseProductIdChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            PurchaseProductName = "";
            return;
        }

        _ = LookupPurchaseProductAsync(value);
    }

    private async Task LookupPurchaseProductAsync(string code)
    {
        var prod = await _productService.GetProductByCodeAsync(code);
        if (prod != null)
        {
            PurchaseProductName = prod.Name;
            PurchaseBhartiKgs = 50;
        }
    }

    partial void OnSaleProductIdChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            SaleProductName = "";
            return;
        }

        _ = LookupSaleProductAsync(value);
    }

    private async Task LookupSaleProductAsync(string code)
    {
        var prod = await _productService.GetProductByCodeAsync(code);
        if (prod != null)
        {
            SaleProductName = prod.Name;
            SaleBhartiKgs = 50;
        }
    }

    // Purchase line inputs
    partial void OnPurchaseTQtyChanged(int value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseBQtyChanged(int value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchasePQtyChanged(int value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseBhartiKgsChanged(decimal value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseLooseWeightChanged(decimal value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseShortChanged(decimal value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseSelectedShortTypeChanged(string value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseDedChanged(decimal value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseSelectedMoundStandardChanged(decimal value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseSelectedRateTypeChanged(string value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseRateChanged(decimal value) => RecalculatePurchaseCurrentLine();
    partial void OnPurchaseRateBardanaChanged(decimal value) => RecalculatePurchaseCurrentLine();

    // Sale line inputs
    partial void OnSaleTQtyChanged(int value) => RecalculateSaleCurrentLine();
    partial void OnSaleBQtyChanged(int value) => RecalculateSaleCurrentLine();
    partial void OnSalePQtyChanged(int value) => RecalculateSaleCurrentLine();
    partial void OnSaleBhartiKgsChanged(decimal value) => RecalculateSaleCurrentLine();
    partial void OnSaleLooseWeightChanged(decimal value) => RecalculateSaleCurrentLine();
    partial void OnSaleShortChanged(decimal value) => RecalculateSaleCurrentLine();
    partial void OnSaleSelectedShortTypeChanged(string value) => RecalculateSaleCurrentLine();
    partial void OnSaleDedChanged(decimal value) => RecalculateSaleCurrentLine();
    partial void OnSaleSelectedMoundStandardChanged(decimal value) => RecalculateSaleCurrentLine();
    partial void OnSaleSelectedRateTypeChanged(string value) => RecalculateSaleCurrentLine();
    partial void OnSaleRateChanged(decimal value) => RecalculateSaleCurrentLine();
    partial void OnSaleRateBardanaChanged(decimal value) => RecalculateSaleCurrentLine();

    // Purchase bottom calculations
    partial void OnPurchaseDamiTypeChanged(string value) => RecalculateTotals();
    partial void OnPurchaseDamiPercentageChanged(decimal value) => RecalculateTotals();
    partial void OnPurchaseCommissionMinusChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseWhtPercentageChanged(decimal value) => RecalculateTotals();
    partial void OnPurchaseBardanaChanged(decimal value) => RecalculateTotals();
    partial void OnPurchaseAddBardanaChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseCharityChanged(decimal value) => RecalculateTotals();
    partial void OnPurchaseCharityPercentageChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseCharityMoundChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseOtherExpMinusChanged(decimal value) => RecalculateTotals();
    partial void OnPurchaseOtherExpPlusChanged(decimal value) => RecalculateTotals();
    partial void OnPurchaseVehicleChargesChanged(decimal value) => RecalculateTotals();
    partial void OnPurchaseVehiclePlusChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseVehicleMinusChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseVehicleRadioPercentChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseVehicleRadioBagChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseVehicleRadioMoundChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseMazdooriChanged(decimal value) => RecalculateTotals();
    partial void OnPurchaseMazdooriPlusChanged(bool value) => RecalculateTotals();
    partial void OnPurchaseMazdooriMinusChanged(bool value) => RecalculateTotals();
    partial void OnPurchasePaidChanged(decimal value) => RecalculateTotals();

    // Sale bottom calculations
    partial void OnSaleDamiTypeChanged(string value) => RecalculateTotals();
    partial void OnSaleDamiPercentageChanged(decimal value) => RecalculateTotals();
    partial void OnSaleCommissionMinusChanged(bool value) => RecalculateTotals();
    partial void OnSaleBardanaChanged(decimal value) => RecalculateTotals();
    partial void OnSaleAddBardanaChanged(bool value) => RecalculateTotals();
    partial void OnSaleOtherExpMinusChanged(decimal value) => RecalculateTotals();
    partial void OnSaleOtherExpPlusChanged(decimal value) => RecalculateTotals();
    partial void OnSaleVehicleChargesChanged(decimal value) => RecalculateTotals();
    partial void OnSaleVehiclePlusChanged(bool value) => RecalculateTotals();
    partial void OnSaleVehicleMinusChanged(bool value) => RecalculateTotals();
    partial void OnSaleMazdooriChanged(decimal value) => RecalculateTotals();
    partial void OnSaleMazdooriPlusChanged(bool value) => RecalculateTotals();
    partial void OnSaleMazdooriMinusChanged(bool value) => RecalculateTotals();
    partial void OnSaleMazdooriRadioPercentChanged(bool value) => RecalculateTotals();
    partial void OnSaleMazdooriRadioBagChanged(bool value) => RecalculateTotals();
    partial void OnSaleMazdooriRadioMoundChanged(bool value) => RecalculateTotals();
    partial void OnSaleWhTax1Changed(decimal value) => RecalculateTotals();
    partial void OnSaleWhTaxRadioPercentChanged(bool value) => RecalculateTotals();
    partial void OnSaleWhTaxRadio100KgChanged(bool value) => RecalculateTotals();
    partial void OnSaleWhTaxPlusChanged(bool value) => RecalculateTotals();
    partial void OnSaleWhTaxMinusChanged(bool value) => RecalculateTotals();
    partial void OnSaleReceivedChanged(decimal value) => RecalculateTotals();

    [RelayCommand]
    private async Task Save()
    {
        if (!PurchaseItems.Any() && !SaleItems.Any())
        {
            System.Windows.MessageBox.Show("Please add at least one Purchase or Sale item.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var year = await _systemService.GetCurrentYearAsync();
        if (year == null)
        {
            System.Windows.MessageBox.Show("Active Financial Year not found.", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var activeSeasons = await _systemService.GetActiveSeasonsAsync();
        var activeSeason = activeSeasons.FirstOrDefault(s => s.Name == SelectedCropSeason) ?? activeSeasons.FirstOrDefault();

        try
        {
            BrokerageInvoice? pInvoice = null;
            BrokerageInvoice? sInvoice = null;

            // 1. Save Purchase side if exists
            if (PurchaseItems.Any() && !string.IsNullOrWhiteSpace(PartyId))
            {
                var party = await _partyService.GetPartyByNoAsync(PartyId);
                if (party != null)
                {
                    if (LoadedPurchaseInvoiceId > 0)
                    {
                        pInvoice = await _brokerageService.GetBrokerageInvoiceByIdAsync(LoadedPurchaseInvoiceId) ?? new BrokerageInvoice();
                        pInvoice.Details.Clear();
                    }
                    else
                    {
                        pInvoice = new BrokerageInvoice();
                    }

                    pInvoice.InvoiceNo = PurchaseId1;
                    pInvoice.InvoiceType = InvoiceType.BrkPurchase;
                    pInvoice.InvoiceDate = InvoiceDate;
                    pInvoice.FinancialYearId = year.Id;
                    pInvoice.CropSeasonId = activeSeason?.Id ?? 1;
                    pInvoice.PartyId = party.Id;
                    pInvoice.GrossAmount = PurchaseGrossTotal;
                    pInvoice.CommissionAmount = PurchaseDamiValue;
                    pInvoice.WHT_Amount = PurchaseWhtValue;
                    pInvoice.LaborCharges = PurchaseMazdoori;
                    pInvoice.NetAmount = PurchaseNetValue;
                    pInvoice.VehicleNo = PurchaseVehicleNo;
                    pInvoice.Status = InvoiceStatus.Draft;

                    int lineNo = 1;
                    foreach (var item in PurchaseItems)
                    {
                        var prod = await _productService.GetProductByCodeAsync(item.ProductId);
                        if (prod != null)
                        {
                            pInvoice.Details.Add(new BrokerageInvoiceDetail
                            {
                                LineNo = lineNo++,
                                ProductId = prod.Id,
                                Bags = item.TQty,
                                GrossWeight = item.TotalKgs,
                                TareWeight = item.Deduction + item.Shortage,
                                NetWeight = item.TotalKgs,
                                Rate = item.Rate,
                                Amount = item.TotalValue
                            });
                        }
                    }

                    await _brokerageService.SaveBrokerageInvoiceAsync(pInvoice);
                    await _brokerageService.PostBrokerageInvoiceToGLAsync(pInvoice.Id);
                }
            }

            // 2. Save Sale side if exists
            if (SaleItems.Any() && !string.IsNullOrWhiteSpace(CustomerId))
            {
                var customer = await _partyService.GetPartyByNoAsync(CustomerId);
                if (customer != null)
                {
                    if (LoadedSaleInvoiceId > 0)
                    {
                        sInvoice = await _brokerageService.GetBrokerageInvoiceByIdAsync(LoadedSaleInvoiceId) ?? new BrokerageInvoice();
                        sInvoice.Details.Clear();
                    }
                    else
                    {
                        sInvoice = new BrokerageInvoice();
                    }

                    sInvoice.InvoiceNo = !string.IsNullOrWhiteSpace(SaleId1) ? SaleId1 : PurchaseId1;
                    sInvoice.InvoiceType = InvoiceType.BrkSale;
                    sInvoice.InvoiceDate = InvoiceDate;
                    sInvoice.FinancialYearId = year.Id;
                    sInvoice.CropSeasonId = activeSeason?.Id ?? 1;
                    sInvoice.PartyId = customer.Id;
                    sInvoice.GrossAmount = SaleGrossTotal;
                    sInvoice.CommissionAmount = SaleDamiValue;
                    sInvoice.WHT_Amount = SaleWhTax2;
                    sInvoice.LaborCharges = SaleMazdoori;
                    sInvoice.NetAmount = SaleNetValue;
                    sInvoice.VehicleNo = VehicleNo;
                    sInvoice.Status = InvoiceStatus.Draft;

                    int lineNo = 1;
                    foreach (var item in SaleItems)
                    {
                        var prod = await _productService.GetProductByCodeAsync(item.ProductId);
                        if (prod != null)
                        {
                            sInvoice.Details.Add(new BrokerageInvoiceDetail
                            {
                                LineNo = lineNo++,
                                ProductId = prod.Id,
                                Bags = item.TQty,
                                GrossWeight = item.TotalKgs,
                                TareWeight = item.Deduction + item.Shortage,
                                NetWeight = item.TotalKgs,
                                Rate = item.Rate,
                                Amount = item.TotalValue
                            });
                        }
                    }

                    await _brokerageService.SaveBrokerageInvoiceAsync(sInvoice);
                    await _brokerageService.PostBrokerageInvoiceToGLAsync(sInvoice.Id);
                }
            }

            // 3. Sync Legacy Multi Invoice tables (MultiInvoice, MultiBody, ContractInvoice)
            await _brokerageService.SyncLegacyMultiInvoiceAsync(MultiId, pInvoice, sInvoice, ProfitLoss, AccountNo);

            System.Windows.MessageBox.Show($"Brokerage Multi Invoice {MultiId} saved to database and posted successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            Clear();
            PurchaseId1 = await _systemService.GetNextDocumentNoAsync(DocumentType.BrokerageInvoice, year.Id);
            SaleId1 = PurchaseId1;
            MultiId = PurchaseId1;
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Brokerage Multi Invoice Save");
        }
    }

    [ObservableProperty] private int _loadedPurchaseInvoiceId = 0;
    [ObservableProperty] private int _loadedSaleInvoiceId = 0;

    [RelayCommand]
    private void Clear()
    {
        LoadedPurchaseInvoiceId = 0;
        LoadedSaleInvoiceId = 0;
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
        System.Windows.MessageBox.Show($"Printing Brokerage Multi Invoice #{MultiId}...", "Print Preview", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private async Task Open()
    {
        var lookupVm = new BrokerageMultiInvoiceLookupViewModel(_brokerageService);

        var lookupWin = new Views.BrokerageMultiInvoiceLookupWindow
        {
            DataContext = lookupVm,
            Owner = Application.Current.MainWindow
        };

        lookupWin.ShowDialog();

        if (lookupVm.IsConfirmed && lookupVm.SelectedInvoice != null)
        {
            var multiData = await _brokerageService.GetMultiInvoiceDataAsync(lookupVm.SelectedInvoice.MultiId);
            MultiId = lookupVm.SelectedInvoice.MultiId;
            AccountNo = string.IsNullOrWhiteSpace(multiData.PlsAccount) ? "40001" : multiData.PlsAccount;

            // Load Purchase Side
            if (multiData.Purchase != null)
            {
                LoadedPurchaseInvoiceId = multiData.Purchase.Id;
                PurchaseId1 = multiData.Purchase.InvoiceNo;
                InvoiceDate = multiData.Purchase.InvoiceDate;
                PartyId = multiData.Purchase.Party?.PartyNo ?? "";
                PartyName = multiData.Purchase.Party?.Name ?? "";
                PartyPhone = multiData.Purchase.Party?.Phone ?? "";
                PartyAddress = multiData.Purchase.Party?.Address ?? "";
                PurchaseVehicleNo = multiData.Purchase.VehicleNo ?? "";
                PurchaseGrossTotal = multiData.Purchase.GrossAmount;
                PurchaseDamiValue = multiData.Purchase.CommissionAmount;
                PurchaseWhtValue = multiData.Purchase.WHT_Amount;
                PurchaseMazdoori = multiData.Purchase.LaborCharges;
                PurchaseNetValue = multiData.Purchase.NetAmount;

                PurchaseItems.Clear();
                foreach (var detail in multiData.Purchase.Details)
                {
                    decimal wtPerBag = detail.Bags > 0 ? (detail.GrossWeight / detail.Bags) : 50;
                    PurchaseItems.Add(new BrokerageMultiPurchaseItemDto
                    {
                        ProductId = detail.Product?.ProductCode ?? detail.ProductId.ToString(),
                        ProductName = detail.Product?.Name ?? "",
                        TQty = detail.Bags,
                        BQty = detail.Bags,
                        PQty = 0,
                        WtPerBag = wtPerBag,
                        TotalKgs = detail.NetWeight,
                        Rate = detail.Rate,
                        TotalValue = detail.Amount,
                        Shortage = detail.TareWeight,
                        RateType = "Rate/Mound"
                    });
                }
            }

            // Load Sale Side
            if (multiData.Sale != null)
            {
                LoadedSaleInvoiceId = multiData.Sale.Id;
                SaleId1 = multiData.Sale.InvoiceNo;
                CustomerId = multiData.Sale.Party?.PartyNo ?? "";
                CustomerName = multiData.Sale.Party?.Name ?? "";
                CustomerPhone = multiData.Sale.Party?.Phone ?? "";
                CustomerAddress = multiData.Sale.Party?.Address ?? "";
                VehicleNo = multiData.Sale.VehicleNo ?? "";
                SaleGrossTotal = multiData.Sale.GrossAmount;
                SaleDamiValue = multiData.Sale.CommissionAmount;
                SaleWhTax2 = multiData.Sale.WHT_Amount;
                SaleMazdoori = multiData.Sale.LaborCharges;
                SaleNetValue = multiData.Sale.NetAmount;

                SaleItems.Clear();
                foreach (var detail in multiData.Sale.Details)
                {
                    decimal wtPerBag = detail.Bags > 0 ? (detail.GrossWeight / detail.Bags) : 50;
                    SaleItems.Add(new BrokerageMultiSaleItemDto
                    {
                        ProductId = detail.Product?.ProductCode ?? detail.ProductId.ToString(),
                        ProductName = detail.Product?.Name ?? "",
                        TQty = detail.Bags,
                        BQty = detail.Bags,
                        PQty = 0,
                        WtPerBag = wtPerBag,
                        TotalKgs = detail.NetWeight,
                        Rate = detail.Rate,
                        TotalValue = detail.Amount,
                        Shortage = detail.TareWeight,
                        RateType = "Rate/Mound",
                        VehicleNo = multiData.Sale.VehicleNo ?? ""
                    });
                }
            }

            ProfitLoss = multiData.ProfitLoss;
            RecalculateTotals();
        }
    }

    [RelayCommand]
    private async Task Remove()
    {
        if (LoadedPurchaseInvoiceId > 0 || LoadedSaleInvoiceId > 0 || !string.IsNullOrWhiteSpace(MultiId))
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete this multi invoice (#{MultiId}) from database?\n\n(Yes = Delete multi invoice and all accounting entries\nNo = Remove only selected item from list)",
                "Confirm Delete Multi Invoice",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await _brokerageService.DeleteMultiInvoiceAsync(MultiId);
                MessageBox.Show($"Multi Invoice #{MultiId} deleted successfully.", "Delete Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                Clear();
                var year = await _systemService.GetCurrentYearAsync();
                if (year != null)
                {
                    PurchaseId1 = await _systemService.GetNextDocumentNoAsync(DocumentType.BrokerageInvoice, year.Id);
                    SaleId1 = PurchaseId1;
                    MultiId = PurchaseId1;
                }
                return;
            }
            else if (result == MessageBoxResult.No)
            {
                if (SelectedPurchaseItem != null) PurchaseItems.Remove(SelectedPurchaseItem);
                if (SelectedSaleItem != null) SaleItems.Remove(SelectedSaleItem);
                RecalculateTotals();
                return;
            }
            return;
        }

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
