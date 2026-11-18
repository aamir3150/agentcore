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

public partial class PestroPurchaseInvoiceViewModel : ObservableObject
{
    private readonly IPestroService _pestroService;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;
    private readonly ISystemService _systemService;

    [ObservableProperty] private int _loadedInvoiceId = 0;
    [ObservableProperty] private string _purchaseId = "";
    [ObservableProperty] private DateTime _invoiceDate = DateTime.Today;
    [ObservableProperty] private string _vendorId = "";
    [ObservableProperty] private string _vendorName = "";
    [ObservableProperty] private string _address = "";
    [ObservableProperty] private decimal _previousBalance = 0;
    [ObservableProperty] private string _billNo = "";
    [ObservableProperty] private DateTime? _billDate = DateTime.Today;

    // Crop Season
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = string.Empty;

    // Financial
    [ObservableProperty] private decimal _expenses = 0;
    [ObservableProperty] private decimal _paidAmount = 0;
    [ObservableProperty] private string _narrationUrdu = "";
    [ObservableProperty] private string _narration = "";

    // Product entry inputs
    [ObservableProperty] private ObservableCollection<Product> _availableProducts = new();
    [ObservableProperty] private Product? _selectedProduct;
    [ObservableProperty] private string _currentProductCode = "";
    [ObservableProperty] private string _currentProductName = "";
    [ObservableProperty] private string _currentPacking = "PIECE";
    [ObservableProperty] private string _currentBatchNo = "";
    [ObservableProperty] private DateTime? _currentExpiryDate = DateTime.Today.AddYears(2);
    [ObservableProperty] private decimal _currentQty = 1;
    [ObservableProperty] private decimal _currentBonus = 0;
    [ObservableProperty] private decimal _currentPrice = 0;
    [ObservableProperty] private decimal _currentDiscRatio1 = 0;
    [ObservableProperty] private decimal _currentDiscRatio2 = 0;
    [ObservableProperty] private decimal _currentSTRatio = 0;
    [ObservableProperty] private decimal _currentSTValue = 0;
    [ObservableProperty] private decimal _currentTTLValue = 0;
    [ObservableProperty] private decimal _currentTotalValue = 0;
    [ObservableProperty] private decimal _currentCost = 0;

    // Grid Collection
    [ObservableProperty] private ObservableCollection<PestroGridItemDto> _items = new();
    [ObservableProperty] private PestroGridItemDto? _selectedItem;

    // Totals
    [ObservableProperty] private decimal _totalQty = 0;
    [ObservableProperty] private decimal _totalBonus = 0;
    [ObservableProperty] private decimal _grossTotal = 0;
    [ObservableProperty] private decimal _totalDiscount = 0;
    [ObservableProperty] private decimal _totalSalesTax = 0;
    [ObservableProperty] private decimal _netTotal = 0;
    [ObservableProperty] private decimal _grandTotal = 0;
    [ObservableProperty] private decimal _balancePayable = 0;

    // Party directory
    [ObservableProperty] private ObservableCollection<Party> _availableVendors = new();
    [ObservableProperty] private Party? _selectedVendor;

    public Action? RequestClose { get; set; }

    public PestroPurchaseInvoiceViewModel(
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

        // 2. Vendors (Parties)
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
        PurchaseId = await _pestroService.GetNextPestroInvoiceNoAsync(InvoiceType.PestroPurchase, yearId);
    }

    partial void OnSelectedVendorChanged(Party? value)
    {
        if (value != null)
        {
            VendorId = value.PartyNo;
            VendorName = value.Name;
            Address = value.Address ?? "";
            PreviousBalance = value.Account?.CurrentBalance ?? 0;
        }
    }

    partial void OnSelectedProductChanged(Product? value)
    {
        if (value != null)
        {
            CurrentProductCode = value.ProductCode;
            CurrentProductName = value.Name;
            CurrentPacking = value.Unit?.Name ?? "PIECE";
            CurrentPrice = value.PurchasePrice > 0 ? value.PurchasePrice : value.SalePrice;
            CurrentBatchNo = $"B-{DateTime.Today:yyMMdd}-{value.Id}";
            CurrentExpiryDate = DateTime.Today.AddYears(2);
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
        decimal totalUnits = CurrentQty + CurrentBonus;
        CurrentCost = totalUnits > 0 ? Math.Round(CurrentTTLValue / totalUnits, 4) : CurrentPrice;
    }

    [RelayCommand]
    private void AddLineItem()
    {
        if (SelectedProduct == null && string.IsNullOrWhiteSpace(CurrentProductName))
        {
            MessageBox.Show("برائے مہربانی پروڈکٹ منتخب کریں۔", "آئٹم درکار ہے", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CurrentQty <= 0)
        {
            MessageBox.Show("تعداد (Quantity) درج کریں۔", "تعداد درکار ہے", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var item = new PestroGridItemDto
        {
            SerialNo = Items.Count + 1,
            ProductId = SelectedProduct?.Id ?? 0,
            ProductCode = CurrentProductCode,
            ProductName = CurrentProductName,
            Packing = CurrentPacking,
            BatchNo = string.IsNullOrWhiteSpace(CurrentBatchNo) ? $"B-{PurchaseId}-{Items.Count + 1}" : CurrentBatchNo,
            ExpiryDate = CurrentExpiryDate,
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

        // Reset inputs
        CurrentQty = 1;
        CurrentBonus = 0;
        CurrentBatchNo = "";
    }

    [RelayCommand]
    private void RemoveLineItem()
    {
        if (SelectedItem != null)
        {
            Items.Remove(SelectedItem);
            // Re-sequence serial numbers
            int sr = 1;
            foreach (var itm in Items) itm.SerialNo = sr++;
            RecalculateTotals();
        }
    }

    partial void OnExpensesChanged(decimal value) => RecalculateTotals();
    partial void OnPaidAmountChanged(decimal value) => RecalculateTotals();

    public void RecalculateTotals()
    {
        TotalQty = Items.Sum(i => i.Quantity);
        TotalBonus = Items.Sum(i => i.Bonus);
        GrossTotal = Items.Sum(i => i.Quantity * i.Price);
        TotalDiscount = GrossTotal - Items.Sum(i => i.TTLValue);
        TotalSalesTax = Items.Sum(i => i.TTLSalestax);
        NetTotal = Items.Sum(i => i.TotalValue);
        GrandTotal = NetTotal + Expenses;
        BalancePayable = GrandTotal - PaidAmount;
    }

    [RelayCommand]
    private async Task Save()
    {
        if (SelectedVendor == null && string.IsNullOrWhiteSpace(VendorId))
        {
            MessageBox.Show("وینڈر (Vendor/Party) منتخب کریں۔", "وینڈر درکار ہے", MessageBoxButton.OK, MessageBoxImage.Warning);
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
                InvoiceNo = PurchaseId,
                InvoiceType = InvoiceType.PestroPurchase,
                InvoiceDate = InvoiceDate,
                FinancialYearId = year?.Id ?? 1,
                CropSeasonId = season?.Id ?? 1,
                PartyId = SelectedVendor?.Id ?? 1,
                SubTotal = GrossTotal,
                DiscountAmount = TotalDiscount,
                NetAmount = GrandTotal,
                BatchMode = BatchMode.Auto,
                BillNo = BillNo,
                BillDate = BillDate,
                Expenses = Expenses,
                PaidAmount = PaidAmount,
                NarrationUrdu = NarrationUrdu,
                Remarks = Narration,
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

            MessageBox.Show($"پیسٹرو پرچیز انوائس (#{invoice.InvoiceNo}) کامیابی سے محفوظ اور پوسٹ ہو گئی ہے۔\nاسٹاک اور کھاتہ جات اپ ڈیٹ ہو چکے ہیں۔", 
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
        lookupVm.SetTypeFilter(InvoiceType.PestroPurchase);

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
                PurchaseId = invoice.InvoiceNo;
                InvoiceDate = invoice.InvoiceDate;
                SelectedVendor = AvailableVendors.FirstOrDefault(v => v.Id == invoice.PartyId);
                VendorId = SelectedVendor?.PartyNo ?? invoice.PartyId.ToString();
                VendorName = SelectedVendor?.Name ?? "";
                Address = SelectedVendor?.Address ?? "";
                BillNo = invoice.BillNo;
                BillDate = invoice.BillDate;
                Expenses = invoice.Expenses;
                PaidAmount = invoice.PaidAmount;
                NarrationUrdu = invoice.NarrationUrdu;
                Narration = invoice.Remarks;
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
                        BatchNo = d.BatchNo ?? d.Batch?.BatchNo ?? "",
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
        SelectedVendor = null;
        VendorId = "";
        VendorName = "";
        Address = "";
        PreviousBalance = 0;
        BillNo = "";
        BillDate = DateTime.Today;
        Expenses = 0;
        PaidAmount = 0;
        NarrationUrdu = "";
        Narration = "";
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
                $"کیا آپ واقعی یہ پیسٹرو پرچیز انوائس (#{PurchaseId}) ڈیلیٹ کرنا چاہتے ہیں؟\nاس سے اسٹاک ریورس ہو جائے گا اور کھاتہ جات صاف ہو جائیں گے۔",
                "تصدیق برائے ڈیلیٹ",
                MessageBoxButton.YesNo,
                MessageBoxImage.Question);

            if (res == MessageBoxResult.Yes)
            {
                await _pestroService.DeletePestroInvoiceAsync(LoadedInvoiceId);
                MessageBox.Show($"انوائس #{PurchaseId} کامیابی سے ڈیلیٹ ہو گئی ہے۔", "ڈیلیٹ مکمل", MessageBoxButton.OK, MessageBoxImage.Information);
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
        MessageBox.Show($"Printing Pestro Purchase Invoice #{PurchaseId}...", "پرنٹ پریویو", MessageBoxButton.OK, MessageBoxImage.Information);
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
