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

public class BrokeragePurchaseItemDto : ObservableObject
{
    public string ProductId { get; set; } = "01002";
    public string ProductName { get; set; } = "Wheat Special";
    public int TQty { get; set; } = 100;
    public int BQty { get; set; } = 100;
    public int PQty { get; set; } = 0;
    public decimal WtPerBag { get; set; } = 50;
    public decimal ExtWt { get; set; } = 0;
    public decimal Shortage { get; set; } = 0;
    public decimal Deduction { get; set; } = 0;
    public decimal TotalKgs { get; set; } = 5000;
    public decimal MoundStandard { get; set; } = 40;
    public string RateType { get; set; } = "Rate/Mound";
    public decimal Rate { get; set; } = 3800;
    public decimal RateBardana { get; set; } = 0;
    public decimal TotalValue { get; set; } = 475000;
}

public partial class BrokeragePurchaseInvoiceViewModel : ObservableObject
{
    private readonly IBrokerageService _brokerageService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    // Header Fields
    [ObservableProperty] private string _purchaseId1 = "1357";
    [ObservableProperty] private string _purchaseId2 = "1783";
    [ObservableProperty] private string _contractId = "";
    [ObservableProperty] private string _partyId = "";
    [ObservableProperty] private string _partyName = "";
    [ObservableProperty] private DateTime _invoiceDate = DateTime.Today;
    [ObservableProperty] private string _customerId = "";
    [ObservableProperty] private string _customerName = "";

    [ObservableProperty] private string _sector = "";
    [ObservableProperty] private string _refreeId = "";
    [ObservableProperty] private string _refreeName = "";
    [ObservableProperty] private string _bagVendorId = "";
    [ObservableProperty] private string _bagVendorName = "";

    [ObservableProperty] private string _vehicleNo = "";
    [ObservableProperty] private string _town = "";
    [ObservableProperty] private string _address = "";
    [ObservableProperty] private string _phone = "";
    [ObservableProperty] private string _mounds = "0- 0";
    [ObservableProperty] private string _gatePassNo = "";
    [ObservableProperty] private string _billNo = "";

    // Crop Season
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = "Cotton";

    // Delivery & Shortage Flags
    [ObservableProperty] private bool _withShortage = false;
    [ObservableProperty] private bool _contractWithDelivery = false;

    // Line Item Entry Inputs
    [ObservableProperty] private string _currentProductId = "01002";
    [ObservableProperty] private string _currentProductName = "";
    [ObservableProperty] private int _currentTQty = 0;
    [ObservableProperty] private int _currentBQty = 0;
    [ObservableProperty] private int _currentPQty = 0;
    [ObservableProperty] private decimal _currentBhartiKgs = 50;
    [ObservableProperty] private decimal _currentLooseWeight = 0;
    [ObservableProperty] private ObservableCollection<string> _shortTypes = new() { "Mnd+", "Mnd-", "%" };
    [ObservableProperty] private string _selectedShortType = "Mnd+";
    [ObservableProperty] private decimal _currentShortageDed = 0;
    [ObservableProperty] private decimal _currentTotalWeight = 0;
    [ObservableProperty] private ObservableCollection<decimal> _moundStandards = new() { 40, 50, 100, 37.324m };
    [ObservableProperty] private decimal _selectedMoundStandard = 40;
    [ObservableProperty] private ObservableCollection<string> _rateTypes = new() { "Rate/Mound", "Rate/Kg", "Rate/Bag", "Fixed" };
    [ObservableProperty] private string _selectedRateType = "Rate/Mound";
    [ObservableProperty] private decimal _currentRate = 0;
    [ObservableProperty] private decimal _currentRateBardana = 0;
    [ObservableProperty] private decimal _currentTotalValue = 0;

    // Grid Collection
    [ObservableProperty] private ObservableCollection<BrokeragePurchaseItemDto> _items = new();
    [ObservableProperty] private BrokeragePurchaseItemDto? _selectedItem;

    // General Expenses Tab Controls
    [ObservableProperty] private ObservableCollection<string> _damiTypes = new() { "TotalValueWise", "PerBag", "PerMound" };
    [ObservableProperty] private string _selectedDamiType = "TotalValueWise";
    [ObservableProperty] private decimal _damiPercentage = 0;
    [ObservableProperty] private decimal _damiValue = 0;
    [ObservableProperty] private bool _commissionMinusFromBalance = false;
    [ObservableProperty] private decimal _whtPercentage = 0;
    [ObservableProperty] private decimal _whtValue = 0;

    [ObservableProperty] private decimal _grossTotal = 0;
    [ObservableProperty] private decimal _discount = 0;
    [ObservableProperty] private decimal _bardana = 0;
    [ObservableProperty] private decimal _advance = 0;
    [ObservableProperty] private decimal _invoiceValue = 0;

    [ObservableProperty] private ObservableCollection<string> _refCommissionTypes = new() { "TotalValueWise", "PerBag", "PerMound" };
    [ObservableProperty] private string _selectedRefCommissionType = "TotalValueWise";
    [ObservableProperty] private decimal _refCommissionPercentage = 0;
    [ObservableProperty] private decimal _refCommissionValue = 0;

    [ObservableProperty] private decimal _otherExpMinus = 0;
    [ObservableProperty] private decimal _otherExpPlus = 0;
    [ObservableProperty] private decimal _vehicleCharges1 = 0;
    [ObservableProperty] private decimal _vehicleCharges2 = 0;
    [ObservableProperty] private bool _isVehiclePerBag = true;
    [ObservableProperty] private bool _isVehiclePerMound = false;
    [ObservableProperty] private bool _isVehiclePlus = false;
    [ObservableProperty] private bool _isVehicleMinus = false;
    [ObservableProperty] private bool _separateVehicleCharges = false;
    [ObservableProperty] private string _vehicleAccountNo = "";

    // Financial Totals
    [ObservableProperty] private decimal _paid = 0;
    [ObservableProperty] private decimal _netValue = 0;
    [ObservableProperty] private decimal _previousBalance = 0;
    [ObservableProperty] private decimal _finalBalance = 0;

    // Bottom Narrations
    [ObservableProperty] private string _narration = "";
    [ObservableProperty] private ObservableCollection<string> _expenseTypes = new() { "Standard", "Mandi Exp", "Direct Delivery" };
    [ObservableProperty] private string _selectedExpenseType = "Standard";
    [ObservableProperty] private bool _markupInvoice = false;
    [ObservableProperty] private string _narrationInUrdu = "";
    [ObservableProperty] private bool _sendSMS = false;

    public Action? RequestClose { get; set; }

    public BrokeragePurchaseInvoiceViewModel(
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

        // Sample initial row for authentic preview feel
        Items.Add(new BrokeragePurchaseItemDto
        {
            ProductId = "01002",
            ProductName = "Phutti Special",
            TQty = 100,
            BQty = 100,
            PQty = 0,
            WtPerBag = 50,
            ExtWt = 0,
            Shortage = 0,
            Deduction = 0,
            TotalKgs = 5000,
            MoundStandard = 40,
            RateType = "Rate/Mound",
            Rate = 8500,
            RateBardana = 0,
            TotalValue = 1062500
        });

        RecalculateTotals();
    }

    public void RecalculateTotals()
    {
        GrossTotal = Items.Sum(i => i.TotalValue);
        decimal totalKgs = Items.Sum(i => i.TotalKgs);
        decimal mds = Math.Floor(totalKgs / 40);
        decimal kgs = totalKgs % 40;
        Mounds = $"{mds}- {kgs:00}";

        if (DamiPercentage > 0)
        {
            DamiValue = Math.Round((GrossTotal * DamiPercentage) / 100m, 2);
        }

        if (WhtPercentage > 0)
        {
            WhtValue = Math.Round((GrossTotal * WhtPercentage) / 100m, 2);
        }

        InvoiceValue = GrossTotal - Discount + Bardana;
        NetValue = InvoiceValue - Advance - OtherExpMinus + OtherExpPlus;
        FinalBalance = PreviousBalance + NetValue - Paid;
    }

    [RelayCommand]
    private void AddItem()
    {
        if (CurrentTotalValue <= 0 && CurrentRate <= 0) return;

        decimal totalKgs = (CurrentTQty * CurrentBhartiKgs) + CurrentLooseWeight - CurrentShortageDed;
        decimal mds = SelectedMoundStandard > 0 ? totalKgs / SelectedMoundStandard : totalKgs / 40;
        decimal value = SelectedRateType == "Rate/Mound" ? mds * CurrentRate : (SelectedRateType == "Rate/Kg" ? totalKgs * CurrentRate : CurrentTQty * CurrentRate);

        Items.Add(new BrokeragePurchaseItemDto
        {
            ProductId = CurrentProductId,
            ProductName = CurrentProductName,
            TQty = CurrentTQty,
            BQty = CurrentBQty,
            PQty = CurrentPQty,
            WtPerBag = CurrentBhartiKgs,
            ExtWt = CurrentLooseWeight,
            Shortage = CurrentShortageDed,
            Deduction = 0,
            TotalKgs = totalKgs,
            MoundStandard = SelectedMoundStandard,
            RateType = SelectedRateType,
            Rate = CurrentRate,
            RateBardana = CurrentRateBardana,
            TotalValue = value
        });

        RecalculateTotals();
    }

    [RelayCommand]
    private void Save()
    {
        System.Windows.MessageBox.Show("Brokerage Purchase Invoice saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Clear()
    {
        Items.Clear();
        GrossTotal = 0;
        Discount = 0;
        Bardana = 0;
        Advance = 0;
        InvoiceValue = 0;
        Paid = 0;
        NetValue = 0;
        FinalBalance = 0;
        Mounds = "0- 0";
    }

    [RelayCommand]
    private void Print()
    {
        System.Windows.MessageBox.Show("Printing Brokerage Purchase Invoice...", "Print", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Open()
    {
        System.Windows.MessageBox.Show("Open Invoice Lookup dialog.", "Open", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Remove()
    {
        if (SelectedItem != null)
        {
            Items.Remove(SelectedItem);
            RecalculateTotals();
        }
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
