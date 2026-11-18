using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class PestroSaleAutoBatchViewModel : ObservableObject
{
    private readonly IPestroService _pestroService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private int _loadedInvoiceId = 0;
    [ObservableProperty] private string _saleId = "";
    [ObservableProperty] private DateTime _saleDate = DateTime.Today;
    [ObservableProperty] private string _customerId = "";
    [ObservableProperty] private string _customerName = "";
    [ObservableProperty] private string _address = "";
    [ObservableProperty] private decimal _previousBalance = 0;
    [ObservableProperty] private DateTime? _dueDate = DateTime.Today.AddDays(15);
    [ObservableProperty] private bool _isCashInvoice = false;

    // Salesman
    [ObservableProperty] private ObservableCollection<Salesman> _availableSalesmen = new();
    [ObservableProperty] private Salesman? _selectedSalesman;
    [ObservableProperty] private string _salesmanName = "";

    // Crop Season
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = string.Empty;

    // Financial
    [ObservableProperty] private decimal _specialDiscount = 0;
    [ObservableProperty] private decimal _receivedAmount = 0;
    [ObservableProperty] private string _remarks = "";
    [ObservableProperty] private string _narrationUrdu = "";

    // Product entry inputs
    [ObservableProperty] private ObservableCollection<Product> _availableProducts = new();
    [ObservableProperty] private Product? _selectedProduct;
    [ObservableProperty] private string _currentProductCode = "";
    [ObservableProperty] private string _currentProductName = "";
    [ObservableProperty] private string _currentPacking = "PIECE";
    [ObservableProperty] private decimal _availableStock = 0;
    [ObservableProperty] private string _batchAllocationInfo = "FIFO: Earliest Expiry Batch";
    [ObservableProperty] private decimal _currentQty = 1;
    [ObservableProperty] private decimal _currentBonus = 0;
    [ObservableProperty] private decimal _currentPrice = 0;
    [ObservableProperty] private decimal _currentDiscRatio1 = 0;
    [ObservableProperty] private decimal _currentDiscRatio2 = 0;
    [ObservableProperty] private decimal _currentSTRatio = 0;
    [ObservableProperty] private decimal _currentSTValue = 0;
    [ObservableProperty] private decimal _currentTTLValue = 0;
    [ObservableProperty] private decimal _currentTotalValue = 0;

    // Grid Collection
    [ObservableProperty] private ObservableCollection<PestroGridItemDto> _items = new();
    [ObservableProperty] private PestroGridItemDto? _selectedItem;

    // Totals
    [ObservableProperty] private decimal _totalQty = 0;
    [ObservableProperty] private decimal _totalBonus = 0;
    [ObservableProperty] private decimal _grossTotal = 0;
    [ObservableProperty] private decimal _totalDiscount = 0;
    [ObservableProperty] private decimal _totalSalesTax = 0;
    [ObservableProperty] private decimal _netValue = 0;
    [ObservableProperty] private decimal _netPayable = 0;
    [ObservableProperty] private decimal _balanceReceivable = 0;

    // Customer directory
    [ObservableProperty] private ObservableCollection<Party> _availableCustomers = new();
    [ObservableProperty] private Party? _selectedCustomer;

    public Action? RequestClose { get; set; }

    public PestroSaleAutoBatchViewModel(
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

    private async Task InitializeAsync()
    {
        await LoadMasterDataAsync();
        await GenerateNewInvoiceNumberAsync();
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

        // 2. Customers
        var allParties = await _partyService.GetAllPartiesAsync();
        var customers = allParties.Where(p => p.PartyType == PartyType.Customer || p.PartyType == PartyType.Both).ToList();
        AvailableCustomers = new ObservableCollection<Party>(customers);

        // 3. Salesmen
        var salesmen = await _partyService.GetAllSalesmenAsync();
        AvailableSalesmen = new ObservableCollection<Salesman>(salesmen);
        SelectedSalesman = AvailableSalesmen.FirstOrDefault();

        // 4. Products
        var prods = await _productService.GetAllProductsAsync();
        AvailableProducts = new ObservableCollection<Product>(prods);
    }

    private async Task GenerateNewInvoiceNumberAsync()
    {
        var year = await _systemService.GetCurrentYearAsync();
        int yearId = year?.Id ?? 1;
        SaleId = await _pestroService.GetNextPestroInvoiceNoAsync(InvoiceType.PestroSale, yearId);
    }

    partial void OnSelectedCustomerChanged(Party? value)
    {
        if (value != null)
        {
            CustomerId = value.PartyNo;
            CustomerName = value.Name;
            Address = value.Address ?? "";
            PreviousBalance = value.Account?.CurrentBalance ?? 0;
        }
    }

    partial void OnSelectedSalesmanChanged(Salesman? value)
    {
        if (value != null)
        {
            SalesmanName = value.Name;
        }
    }

    async partial void OnSelectedProductChanged(Product? value)
    {
        if (value != null)
        {
            CurrentProductCode = value.ProductCode;
            CurrentProductName = value.Name;
            CurrentPacking = value.Unit?.Name ?? "PIECE";
            CurrentPrice = value.SalePrice;

            // Query live batch stock
            var batches = await _pestroService.GetAvailableBatchesForProductAsync(value.ProductCode);
            AvailableStock = batches.Sum(b => b.AvailableQty);

            if (batches.Any())
            {
                var earliest = batches.First();
                BatchAllocationInfo = $"Earliest Batch: {earliest.BatchNo} (Exp: {earliest.ExpiryDate:dd/MM/yy}, Avail: {earliest.AvailableQty})";
            }
            else
            {
                BatchAllocationInfo = "No active batch stock available";
            }

            RecalculateCurrentLine();
        }
    }

    partial void OnCurrentQtyChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentBonusChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentPriceChanged(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentDiscRatio1Changed(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentDiscRatio2Changed(decimal value) => RecalculateCurrentLine();
    partial void OnCurrentSTRatioChanged(decimal value) => RecalculateCurrentLine();

    private void RecalculateCurrentLine()
    {
        decimal gross = CurrentQty * CurrentPrice;
        decimal d1 = gross * (CurrentDiscRatio1 / 100m);
        decimal afterD1 = gross - d1;
        decimal d2 = afterD1 * (CurrentDiscRatio2 / 100m);
        CurrentTTLValue = Math.Round(afterD1 - d2, 2);
        CurrentSTValue = Math.Round(CurrentTTLValue * (CurrentSTRatio / 100m), 2);
        CurrentTotalValue = CurrentTTLValue + CurrentSTValue;
    }

    [RelayCommand]
    private async Task AddLineItem()
    {
        if (SelectedProduct == null && string.IsNullOrWhiteSpace(CurrentProductName))
        {
            MessageBox.Show("برائے مہربانی پروڈکٹ منتخب کریں۔", "پروڈکٹ درکار ہے", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CurrentQty <= 0)
        {
            MessageBox.Show("تعداد (Quantity) درج کریں۔", "تعداد درکار ہے", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        decimal totalReq = CurrentQty + CurrentBonus;
        // Check available stock
        var availableBatches = await _pestroService.GetAvailableBatchesForProductAsync(CurrentProductCode);
        decimal totalStock = availableBatches.Sum(b => b.AvailableQty);

        if (totalStock < totalReq)
        {
            var res = MessageBox.Show(
                $"دستیاب اسٹاک ({totalStock}) درکار تعداد ({totalReq}) سے کم ہے۔ کیا آپ پھر بھی انوائس شامل کرنا چاہتے ہیں؟", 
                "اسٹاک وارننگ", MessageBoxButton.YesNo, MessageBoxImage.Warning);
            if (res != MessageBoxResult.Yes) return;
        }

        // Preview FIFO batch allocation
        var allocations = await _pestroService.AutoAllocateBatchesAsync(CurrentProductCode, totalReq);
        string batchSummary = allocations.Any() 
            ? string.Join(", ", allocations.Select(a => $"{a.BatchNo}({a.AvailableQty})"))
            : "Auto-Allocated";

        var item = new PestroGridItemDto
        {
            SerialNo = Items.Count + 1,
            ProductId = SelectedProduct?.Id ?? 0,
            ProductCode = CurrentProductCode,
            ProductName = CurrentProductName,
            Packing = CurrentPacking,
            BatchNo = batchSummary,
            ExpiryDate = allocations.FirstOrDefault()?.ExpiryDate ?? DateTime.Today.AddYears(1),
            Quantity = CurrentQty,
            Bonus = CurrentBonus,
            Price = CurrentPrice,
            DiscRatio1 = CurrentDiscRatio1,
            DiscRatio2 = CurrentDiscRatio2,
            STRatio = CurrentSTRatio
        };
        item.Recalculate();

        Items.Add(item);
        RecalculateTotals();

        // Reset line input
        CurrentQty = 1;
        CurrentBonus = 0;
    }

    [RelayCommand]
    private void RemoveLineItem()
    {
        if (SelectedItem != null)
        {
            Items.Remove(SelectedItem);
            int sr = 1;
            foreach (var itm in Items) itm.SerialNo = sr++;
            RecalculateTotals();
        }
    }

    partial void OnSpecialDiscountChanged(decimal value) => RecalculateTotals();
    partial void OnReceivedAmountChanged(decimal value) => RecalculateTotals();

    public void RecalculateTotals()
    {
        TotalQty = Items.Sum(i => i.Quantity);
        TotalBonus = Items.Sum(i => i.Bonus);
        GrossTotal = Items.Sum(i => i.Quantity * i.Price);
        TotalDiscount = GrossTotal - Items.Sum(i => i.TTLValue);
        TotalSalesTax = Items.Sum(i => i.TTLSalestax);
        NetValue = Items.Sum(i => i.TotalValue);
        NetPayable = NetValue - SpecialDiscount;
        BalanceReceivable = NetPayable - ReceivedAmount;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedCustomer == null && string.IsNullOrWhiteSpace(CustomerId))
        {
            MessageBox.Show("کسٹمر (Customer/Party) منتخب کریں۔", "کسٹمر درکار ہے", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Items.Count == 0)
        {
            MessageBox.Show("انوائس میں کم از کم ایک پروڈکٹ کا ہونا لازمی ہے۔", "کوئی ریکارڈ نہیں", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            var year = await _systemService.GetCurrentYearAsync();
            var season = (await _systemService.GetAllSeasonsAsync()).FirstOrDefault(s => s.Name == SelectedCropSeason);

            var invoice = new PestroInvoice
            {
                Id = LoadedInvoiceId,
                InvoiceNo = SaleId,
                InvoiceType = InvoiceType.PestroSale,
                InvoiceDate = SaleDate,
                FinancialYearId = year?.Id ?? 1,
                CropSeasonId = season?.Id ?? 1,
                PartyId = SelectedCustomer?.Id ?? 1,
                SalesmanId = SelectedSalesman?.Id,
                SubTotal = GrossTotal,
                DiscountAmount = TotalDiscount + SpecialDiscount,
                NetAmount = NetPayable,
                BatchMode = BatchMode.Auto,
                Remarks = Remarks,
                IsCashInvoice = IsCashInvoice,
                ReceivedAmount = ReceivedAmount,
                SpecialDiscount = SpecialDiscount,
                DueDate = DueDate,
                NarrationUrdu = NarrationUrdu,
                IsPosted = false,
                Status = InvoiceStatus.Draft
            };

            int lineNo = 1;
            foreach (var item in Items)
            {
                invoice.Details.Add(new PestroInvoiceDetail
                {
                    LineNo = lineNo++,
                    ProductId = item.ProductId,
                    Quantity = item.Quantity,
                    Rate = item.Price,
                    Amount = item.TotalValue,
                    ExpiryDate = item.ExpiryDate,
                    BatchNo = item.BatchNo,
                    Bonus = item.Bonus,
                    DiscRatio1 = item.DiscRatio1,
                    DiscRatio2 = item.DiscRatio2,
                    STRatio = item.STRatio,
                    STValue = item.STValue,
                    TTLValue = item.TTLValue,
                    TTLSalestax = item.TTLSalestax
                });
            }

            await _pestroService.SavePestroInvoiceAsync(invoice);
            await _pestroService.PostPestroInvoiceToGLAsync(invoice.Id);

            MessageBox.Show($"پیسٹرو سیل آٹو بیچ انوائس (#{invoice.InvoiceNo}) کامیابی سے محفوظ ہو گئی۔\nبیچز خودکار (FIFO) طور پر تفویض اور اسٹاک و کھاتہ جات اپ ڈیٹ ہو چکے ہیں۔", 
                "کامیابی", MessageBoxButton.OK, MessageBoxImage.Information);

            await New();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"انوائس محفوظ کرنے میں مسئلہ آیا:\n{ex.Message}", "خرابی", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task Open()
    {
        var lookupVm = new PestroInvoiceLookupViewModel(_pestroService);
        lookupVm.SetTypeFilter(InvoiceType.PestroSale, BatchMode.Auto);

        var lookupWin = new Views.PestroInvoiceLookupWindow
        {
            DataContext = lookupVm,
            Owner = Application.Current.MainWindow
        };

        lookupWin.ShowDialog();

        if (lookupVm.IsConfirmed && lookupVm.SelectedInvoice != null)
        {
            var invoice = await _pestroService.GetPestroInvoiceByIdAsync(lookupVm.SelectedInvoice.Id);
            if (invoice != null)
            {
                LoadedInvoiceId = invoice.Id;
                SaleId = invoice.InvoiceNo;
                SaleDate = invoice.InvoiceDate;
                SelectedCustomer = AvailableCustomers.FirstOrDefault(c => c.Id == invoice.PartyId);
                CustomerId = SelectedCustomer?.PartyNo ?? invoice.PartyId.ToString();
                CustomerName = SelectedCustomer?.Name ?? "";
                Address = SelectedCustomer?.Address ?? "";
                DueDate = invoice.DueDate;
                IsCashInvoice = invoice.IsCashInvoice;
                Remarks = invoice.Remarks;
                ReceivedAmount = invoice.ReceivedAmount;
                SpecialDiscount = invoice.SpecialDiscount;
                NarrationUrdu = invoice.NarrationUrdu;
                if (invoice.SalesmanId != null)
                    SelectedSalesman = AvailableSalesmen.FirstOrDefault(s => s.Id == invoice.SalesmanId);
                if (invoice.CropSeason != null)
                    SelectedCropSeason = invoice.CropSeason.Name;

                Items.Clear();
                int sr = 1;
                foreach (var d in invoice.Details)
                {
                    var itm = new PestroGridItemDto
                    {
                        SerialNo = sr++,
                        ProductId = d.ProductId,
                        ProductCode = d.Product?.ProductCode ?? d.ProductId.ToString(),
                        ProductName = d.Product?.Name ?? "",
                        Packing = d.Product?.Unit?.Name ?? "PIECE",
                        BatchNo = d.BatchNo ?? d.Batch?.BatchNo ?? "AUTO",
                        ExpiryDate = d.ExpiryDate ?? d.Batch?.ExpiryDate,
                        Quantity = d.Quantity,
                        Bonus = d.Bonus,
                        Price = d.Rate,
                        DiscRatio1 = d.DiscRatio1,
                        DiscRatio2 = d.DiscRatio2,
                        STRatio = d.STRatio,
                        STValue = d.STValue,
                        TTLValue = d.TTLValue,
                        TTLSalestax = d.TTLSalestax
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
        SelectedCustomer = null;
        CustomerId = "";
        CustomerName = "";
        Address = "";
        PreviousBalance = 0;
        DueDate = DateTime.Today.AddDays(15);
        IsCashInvoice = false;
        SpecialDiscount = 0;
        ReceivedAmount = 0;
        Remarks = "";
        NarrationUrdu = "";
        Items.Clear();
        RecalculateTotals();
        await GenerateNewInvoiceNumberAsync();
    }

    [RelayCommand]
    private async Task Delete()
    {
        if (LoadedInvoiceId > 0)
        {
            var res = MessageBox.Show(
                $"کیا آپ واقعی یہ سیل آٹو بیچ انوائس (#{SaleId}) ڈیلیٹ کرنا چاہتے ہیں؟\nاس سے فروخت شدہ بیچز کا اسٹاک دوبارہ بحال ہو جائے گا۔",
                "تصدیق برائے ڈیلیٹ",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                await _pestroService.DeletePestroInvoiceAsync(LoadedInvoiceId);
                MessageBox.Show($"انوائس #{SaleId} کامیابی سے ڈیلیٹ ہو گئی ہے۔", "ڈیلیٹ مکمل", MessageBoxButton.OK, MessageBoxImage.Information);
                await New();
            }
        }
        else if (SelectedItem != null)
        {
            Items.Remove(SelectedItem);
            RecalculateTotals();
        }
    }

    [RelayCommand]
    private void Print()
    {
        MessageBox.Show($"Printing Pestro Sale Auto Batch Invoice #{SaleId}...", "پرنٹ پریویو", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
