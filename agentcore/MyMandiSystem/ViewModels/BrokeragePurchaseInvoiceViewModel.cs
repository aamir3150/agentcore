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

public class BrokeragePurchaseItemDto : ObservableObject
{
    public string ProductId { get; set; } = string.Empty;
    public string ProductName { get; set; } = string.Empty;
    public int TQty { get; set; } = 0;
    public int BQty { get; set; } = 0;
    public int PQty { get; set; } = 0;
    public decimal WtPerBag { get; set; } = 0;
    public decimal ExtWt { get; set; } = 0;
    public decimal Shortage { get; set; } = 0;
    public decimal Deduction { get; set; } = 0;
    public decimal TotalKgs { get; set; } = 0;
    public decimal MoundStandard { get; set; } = 40;
    public string RateType { get; set; } = "Rate/Mound";
    public decimal Rate { get; set; } = 0;
    public decimal RateBardana { get; set; } = 0;
    public decimal TotalValue { get; set; } = 0;
}

public partial class BrokeragePurchaseInvoiceViewModel : ObservableObject
{
    private readonly IBrokerageService _brokerageService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    // Header Fields
    [ObservableProperty] private string _purchaseId1 = "";
    [ObservableProperty] private string _purchaseId2 = "";
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
    [ObservableProperty] private string _selectedCropSeason = string.Empty;

    // Delivery & Shortage Flags
    [ObservableProperty] private bool _withShortage = false;
    [ObservableProperty] private bool _contractWithDelivery = false;

    // Line Item Entry Inputs
    [ObservableProperty] private string _currentProductId = string.Empty;
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
            }
        }
        catch
        {
            // Graceful fallback if database is not reachable
        }

        Items.Clear();
        RecalculateTotals();
    }

    public void RecalculateCurrentLine()
    {
        // 1. Calculate Total Weight (Kgs)
        decimal totalKgs = (CurrentTQty * CurrentBhartiKgs) + CurrentLooseWeight;
        
        // Shortage deduction
        if (CurrentShortageDed > 0)
        {
            if (SelectedShortType == "%")
            {
                totalKgs -= Math.Round((totalKgs * CurrentShortageDed) / 100m, 2);
            }
            else if (SelectedShortType == "Mnd-")
            {
                decimal std = SelectedMoundStandard > 0 ? SelectedMoundStandard : 40m;
                totalKgs -= (CurrentShortageDed * std);
            }
            else // "Mnd+"
            {
                decimal std = SelectedMoundStandard > 0 ? SelectedMoundStandard : 40m;
                totalKgs += (CurrentShortageDed * std);
            }
        }
        
        if (totalKgs < 0) totalKgs = 0;
        CurrentTotalWeight = Math.Round(totalKgs, 2);

        // 2. Calculate Total Value
        decimal stdMound = SelectedMoundStandard > 0 ? SelectedMoundStandard : 40m;
        decimal baseValue = 0;

        if (SelectedRateType == "Rate/Mound")
        {
            decimal mds = CurrentTotalWeight / stdMound;
            baseValue = mds * CurrentRate;
        }
        else if (SelectedRateType == "Rate/Kg")
        {
            baseValue = CurrentTotalWeight * CurrentRate;
        }
        else if (SelectedRateType == "Rate/Bag")
        {
            baseValue = CurrentTQty * CurrentRate;
        }
        else // "Fixed"
        {
            baseValue = CurrentRate;
        }

        // Rate Bardana (Bardana charged per bag if CurrentRateBardana > 0)
        decimal bardanaValue = CurrentTQty * CurrentRateBardana;

        CurrentTotalValue = Math.Round(baseValue + bardanaValue, 2);
    }

    public void RecalculateTotals()
    {
        GrossTotal = Math.Round(Items.Sum(i => i.TotalValue), 2);
        decimal totalKgs = Items.Sum(i => i.TotalKgs);
        int totalBags = Items.Sum(i => i.TQty);
        
        // Display mounds as "Mounds- Kgs" (e.g. 100- 25)
        decimal mds = Math.Floor(totalKgs / 40m);
        decimal kgs = Math.Round(totalKgs % 40m, 2);
        Mounds = $"{mds}- {kgs:00}";

        // 1. Dami / Commission (Arhat) Calculation
        if (DamiPercentage > 0)
        {
            if (SelectedDamiType == "TotalValueWise")
            {
                DamiValue = Math.Round((GrossTotal * DamiPercentage) / 100m, 2);
            }
            else if (SelectedDamiType == "PerBag")
            {
                DamiValue = Math.Round(totalBags * DamiPercentage, 2);
            }
            else if (SelectedDamiType == "PerMound")
            {
                DamiValue = Math.Round((totalKgs / 40m) * DamiPercentage, 2);
            }
        }
        else
        {
            DamiValue = 0;
        }

        // 2. Ref Commission
        if (RefCommissionPercentage > 0)
        {
            if (SelectedRefCommissionType == "TotalValueWise")
            {
                RefCommissionValue = Math.Round((GrossTotal * RefCommissionPercentage) / 100m, 2);
            }
            else if (SelectedRefCommissionType == "PerBag")
            {
                RefCommissionValue = Math.Round(totalBags * RefCommissionPercentage, 2);
            }
            else if (SelectedRefCommissionType == "PerMound")
            {
                RefCommissionValue = Math.Round((totalKgs / 40m) * RefCommissionPercentage, 2);
            }
        }
        else
        {
            RefCommissionValue = 0;
        }

        // 3. Withholding Tax (WHT)
        if (WhtPercentage > 0)
        {
            if (CommissionMinusFromBalance)
            {
                // In AgenSOFT: WHT on commission
                WhtValue = Math.Round((DamiValue * WhtPercentage) / 100m, 2);
            }
            else
            {
                // WHT on gross total
                WhtValue = Math.Round((GrossTotal * WhtPercentage) / 100m, 2);
            }
        }
        else
        {
            WhtValue = 0;
        }

        // 4. Vehicle Charges (Freight / Kiraya)
        if (VehicleCharges1 > 0)
        {
            if (IsVehiclePerBag)
            {
                VehicleCharges2 = Math.Round(totalBags * VehicleCharges1, 2);
            }
            else if (IsVehiclePerMound)
            {
                VehicleCharges2 = Math.Round((totalKgs / 40m) * VehicleCharges1, 2);
            }
            else
            {
                VehicleCharges2 = VehicleCharges1;
            }
        }

        // 5. Invoice Value & Net Value
        // InvoiceValue = GrossTotal - Discount + Bardana
        InvoiceValue = Math.Round(GrossTotal - Discount + Bardana, 2);

        decimal commissionDeduction = CommissionMinusFromBalance ? DamiValue : 0;
        decimal vehicleAdjustment = IsVehiclePlus ? VehicleCharges2 : (IsVehicleMinus ? -VehicleCharges2 : 0);

        NetValue = Math.Round(InvoiceValue - Advance - OtherExpMinus + OtherExpPlus - commissionDeduction + vehicleAdjustment, 2);
        FinalBalance = Math.Round(PreviousBalance + NetValue - Paid, 2);
    }

    [RelayCommand]
    private void AddItem()
    {
        if (CurrentTotalValue <= 0 && CurrentRate <= 0) return;

        RecalculateCurrentLine();

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
            TotalKgs = CurrentTotalWeight,
            MoundStandard = SelectedMoundStandard,
            RateType = SelectedRateType,
            Rate = CurrentRate,
            RateBardana = CurrentRateBardana,
            TotalValue = CurrentTotalValue
        });

        // Clear line entry inputs for next item
        CurrentProductId = string.Empty;
        CurrentProductName = string.Empty;
        CurrentTQty = 0;
        CurrentBQty = 0;
        CurrentPQty = 0;
        CurrentLooseWeight = 0;
        CurrentShortageDed = 0;
        CurrentTotalWeight = 0;
        CurrentRate = 0;
        CurrentRateBardana = 0;
        CurrentTotalValue = 0;

        RecalculateTotals();
    }

    // Line Input Change Listeners for Instant Calculation
    partial void OnCurrentTQtyChanged(int value) => RecalculateCurrentLine();
    partial void OnCurrentBhartiKgsChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentLooseWeightChanged(decimal value) => RecalculateCurrentLine();
    partial void OnSelectedShortTypeChanged(string value) => RecalculateCurrentLine();
    partial void OnCurrentShortageDedChanged(decimal value) => RecalculateCurrentLine();
    partial void OnSelectedMoundStandardChanged(decimal value) => RecalculateCurrentLine();
    partial void OnSelectedRateTypeChanged(string value) => RecalculateCurrentLine();
    partial void OnCurrentRateChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentRateBardanaChanged(decimal value) => RecalculateCurrentLine();

    // Expenses and Totals Change Listeners
    partial void OnSelectedDamiTypeChanged(string value) => RecalculateTotals();
    partial void OnDamiPercentageChanged(decimal value) => RecalculateTotals();
    partial void OnCommissionMinusFromBalanceChanged(bool value) => RecalculateTotals();
    partial void OnWhtPercentageChanged(decimal value) => RecalculateTotals();
    partial void OnDiscountChanged(decimal value) => RecalculateTotals();
    partial void OnBardanaChanged(decimal value) => RecalculateTotals();
    partial void OnAdvanceChanged(decimal value) => RecalculateTotals();
    partial void OnSelectedRefCommissionTypeChanged(string value) => RecalculateTotals();
    partial void OnRefCommissionPercentageChanged(decimal value) => RecalculateTotals();
    partial void OnOtherExpMinusChanged(decimal value) => RecalculateTotals();
    partial void OnOtherExpPlusChanged(decimal value) => RecalculateTotals();
    partial void OnVehicleCharges1Changed(decimal value) => RecalculateTotals();
    partial void OnIsVehiclePerBagChanged(bool value) => RecalculateTotals();
    partial void OnIsVehiclePerMoundChanged(bool value) => RecalculateTotals();
    partial void OnIsVehiclePlusChanged(bool value) => RecalculateTotals();
    partial void OnIsVehicleMinusChanged(bool value) => RecalculateTotals();
    partial void OnSeparateVehicleChargesChanged(bool value) => RecalculateTotals();
    partial void OnPaidChanged(decimal value) => RecalculateTotals();
    partial void OnPreviousBalanceChanged(decimal value) => RecalculateTotals();

    partial void OnPartyIdChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            PartyName = "";
            Address = "";
            Phone = "";
            PreviousBalance = 0;
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
            Address = party.Address ?? "";
            Phone = party.Phone ?? "";
            PreviousBalance = party.Account?.CurrentBalance ?? 0;
            RecalculateTotals();
        }
    }

    partial void OnCurrentProductIdChanged(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            CurrentProductName = "";
            return;
        }

        _ = LookupProductAsync(value);
    }

    private async Task LookupProductAsync(string code)
    {
        var prod = await _productService.GetProductByCodeAsync(code);
        if (prod != null)
        {
            CurrentProductName = prod.Name;
            CurrentBhartiKgs = 50;
        }
    }

    [RelayCommand]
    private async Task Save()
    {
        if (!Items.Any())
        {
            System.Windows.MessageBox.Show("Please add at least one line item.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var year = await _systemService.GetCurrentYearAsync();
        if (year == null)
        {
            System.Windows.MessageBox.Show("Active Financial Year not found.", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            return;
        }

        var party = await _partyService.GetPartyByNoAsync(PartyId);
        if (party == null)
        {
            System.Windows.MessageBox.Show("Please enter a valid Party No.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var activeSeasons = await _systemService.GetActiveSeasonsAsync();
        var activeSeason = activeSeasons.FirstOrDefault(s => s.Name == SelectedCropSeason) ?? activeSeasons.FirstOrDefault();

        try
        {
            BrokerageInvoice invoice;
            if (LoadedInvoiceId > 0)
            {
                invoice = await _brokerageService.GetBrokerageInvoiceByIdAsync(LoadedInvoiceId) ?? new BrokerageInvoice();
                invoice.Details.Clear();
            }
            else
            {
                invoice = new BrokerageInvoice();
            }

            invoice.InvoiceNo = PurchaseId1;
            invoice.InvoiceType = InvoiceType.BrkPurchase;
            invoice.InvoiceDate = InvoiceDate;
            invoice.FinancialYearId = year.Id;
            invoice.CropSeasonId = activeSeason?.Id ?? 1;
            invoice.PartyId = party.Id;
            invoice.GrossAmount = GrossTotal;
            invoice.CommissionAmount = DamiValue;
            invoice.WHT_Amount = WhtValue;
            invoice.LaborCharges = Bardana;
            invoice.NetAmount = NetValue;
            invoice.VehicleNo = VehicleNo;
            invoice.Status = InvoiceStatus.Draft;

            int lineNo = 1;
            foreach (var item in Items)
            {
                var prod = await _productService.GetProductByCodeAsync(item.ProductId);
                if (prod != null)
                {
                    invoice.Details.Add(new BrokerageInvoiceDetail
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

            await _brokerageService.SaveBrokerageInvoiceAsync(invoice);
            await _brokerageService.PostBrokerageInvoiceToGLAsync(invoice.Id);

            System.Windows.MessageBox.Show($"Brokerage Purchase Invoice {invoice.InvoiceNo} saved to database and posted successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            Clear();
            PurchaseId1 = await _systemService.GetNextDocumentNoAsync(DocumentType.BrokerageInvoice, year.Id);
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Brokerage Purchase Invoice Save");
        }
    }

    [ObservableProperty] private int _loadedInvoiceId = 0;

    [RelayCommand]
    private void Clear()
    {
        LoadedInvoiceId = 0;
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
        System.Windows.MessageBox.Show($"Printing Brokerage Purchase Invoice #{PurchaseId1}...", "Print Preview", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private async Task Open()
    {
        var lookupVm = new BrokerageInvoiceLookupViewModel(_brokerageService);
        lookupVm.SetTypeFilter(InvoiceType.BrkPurchase);

        var lookupWin = new Views.BrokerageInvoiceLookupWindow
        {
            DataContext = lookupVm,
            Owner = Application.Current.MainWindow
        };

        lookupWin.ShowDialog();

        if (lookupVm.IsConfirmed && lookupVm.SelectedInvoice != null)
        {
            var invoice = await _brokerageService.GetBrokerageInvoiceByIdAsync(lookupVm.SelectedInvoice.Id);
            if (invoice != null)
            {
                LoadedInvoiceId = invoice.Id;
                PurchaseId1 = invoice.InvoiceNo;
                InvoiceDate = invoice.InvoiceDate;
                PartyId = invoice.Party?.PartyNo ?? "";
                PartyName = invoice.Party?.Name ?? "";
                VehicleNo = invoice.VehicleNo ?? "";
                GrossTotal = invoice.GrossAmount;
                DamiValue = invoice.CommissionAmount;
                WhtValue = invoice.WHT_Amount;
                Bardana = invoice.LaborCharges;
                NetValue = invoice.NetAmount;

                Items.Clear();
                foreach (var detail in invoice.Details)
                {
                    decimal wtPerBag = detail.Bags > 0 ? (detail.GrossWeight / detail.Bags) : 50;
                    Items.Add(new BrokeragePurchaseItemDto
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
                RecalculateTotals();
            }
        }
    }

    [RelayCommand]
    private async Task Remove()
    {
        if (LoadedInvoiceId > 0)
        {
            var result = MessageBox.Show(
                $"Are you sure you want to delete this invoice (#{PurchaseId1}) from database?\n\n(Yes = Delete invoice and all accounting entries\nNo = Remove only selected item from list)",
                "Confirm Delete Invoice",
                MessageBoxButton.YesNoCancel,
                MessageBoxImage.Question);

            if (result == MessageBoxResult.Yes)
            {
                await _brokerageService.DeleteBrokerageInvoiceAsync(LoadedInvoiceId);
                MessageBox.Show($"Invoice #{PurchaseId1} deleted successfully.", "Delete Complete", MessageBoxButton.OK, MessageBoxImage.Information);
                Clear();
                var year = await _systemService.GetCurrentYearAsync();
                if (year != null)
                {
                    PurchaseId1 = await _systemService.GetNextDocumentNoAsync(DocumentType.BrokerageInvoice, year.Id);
                }
                return;
            }
            else if (result == MessageBoxResult.No)
            {
                if (SelectedItem != null)
                {
                    Items.Remove(SelectedItem);
                    RecalculateTotals();
                }
                return;
            }
            return;
        }

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
