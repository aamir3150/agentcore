using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public class PestroReturnGridItemDto : ObservableObject
{
    private int _serialNo = 1;
    public int SerialNo { get => _serialNo; set => SetProperty(ref _serialNo, value); }

    private string _productId = "";
    public string ProductId { get => _productId; set => SetProperty(ref _productId, value); }

    private string _productName = "";
    public string ProductName { get => _productName; set => SetProperty(ref _productName, value); }

    private string _batchNo = "";
    public string BatchNo { get => _batchNo; set => SetProperty(ref _batchNo, value); }

    private DateTime _expiryDate = DateTime.Today.AddYears(2);
    public DateTime ExpiryDate { get => _expiryDate; set => SetProperty(ref _expiryDate, value); }

    private decimal _originalQty = 0;
    public decimal OriginalQty { get => _originalQty; set => SetProperty(ref _originalQty, value); }

    private decimal _prevReturnQty = 0;
    public decimal PrevReturnQty { get => _prevReturnQty; set => SetProperty(ref _prevReturnQty, value); }

    private decimal _returnQty = 1;
    public decimal ReturnQty
    {
        get => _returnQty;
        set
        {
            if (SetProperty(ref _returnQty, value))
                Recalculate();
        }
    }

    private decimal _price = 0;
    public decimal Price
    {
        get => _price;
        set
        {
            if (SetProperty(ref _price, value))
                Recalculate();
        }
    }

    private decimal _discRatio1 = 0;
    public decimal DiscRatio1
    {
        get => _discRatio1;
        set
        {
            if (SetProperty(ref _discRatio1, value))
                Recalculate();
        }
    }

    private decimal _stRatio = 0;
    public decimal STRatio
    {
        get => _stRatio;
        set
        {
            if (SetProperty(ref _stRatio, value))
                Recalculate();
        }
    }

    private decimal _stValue = 0;
    public decimal STValue { get => _stValue; set => SetProperty(ref _stValue, value); }

    private decimal _ttlValue = 0;
    public decimal TTLValue { get => _ttlValue; set => SetProperty(ref _ttlValue, value); }

    private decimal _totalValue = 0;
    public decimal TotalValue { get => _totalValue; set => SetProperty(ref _totalValue, value); }

    public void Recalculate()
    {
        decimal gross = ReturnQty * Price;
        decimal d1 = gross * (DiscRatio1 / 100m);
        TTLValue = Math.Round(gross - d1, 2);
        STValue = Math.Round(TTLValue * (STRatio / 100m), 2);
        TotalValue = TTLValue + STValue;
    }
}

public partial class PestroPurchaseReturnViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    [ObservableProperty] private bool _isWithInvoice = true;
    [ObservableProperty] private string _windowTitle = "Purchase Return (With Invoice) - خریداری واپسی بل کے ساتھ";

    // Header fields
    [ObservableProperty] private int _returnId = 0;
    [ObservableProperty] private DateTime _returnDate = DateTime.Today;
    [ObservableProperty] private int _purchaseId = 0;
    [ObservableProperty] private DateTime? _purchaseDate;
    [ObservableProperty] private string _billNo = "";

    [ObservableProperty] private string _vendorId = "";
    [ObservableProperty] private string _vendorName = "";
    [ObservableProperty] private string _vendorAddress = "";
    [ObservableProperty] private decimal _previousCredit = 0;

    // Crop Seasons
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = "";

    // Vendors for WoI mode
    [ObservableProperty] private ObservableCollection<Party> _availableVendors = new();
    [ObservableProperty] private Party? _selectedVendor;

    // Products for WoI mode
    [ObservableProperty] private ObservableCollection<Product> _availableProducts = new();
    [ObservableProperty] private Product? _selectedProduct;

    // Line entry inputs (WoI mode)
    [ObservableProperty] private string _inputBatchNo = "";
    [ObservableProperty] private DateTime? _inputExpiryDate = DateTime.Today.AddYears(2);
    [ObservableProperty] private decimal _inputReturnQty = 1;
    [ObservableProperty] private decimal _inputPrice = 0;
    [ObservableProperty] private decimal _inputDiscRatio = 0;
    [ObservableProperty] private decimal _inputSTRatio = 0;

    // Grid Items
    [ObservableProperty] private ObservableCollection<PestroReturnGridItemDto> _items = new();
    [ObservableProperty] private PestroReturnGridItemDto? _selectedItem;

    // Financial Totals
    [ObservableProperty] private decimal _totalReturnQty = 0;
    [ObservableProperty] private decimal _totalGrossValue = 0;
    [ObservableProperty] private decimal _totalDiscount = 0;
    [ObservableProperty] private decimal _totalSalesTax = 0;
    [ObservableProperty] private decimal _totalNetValue = 0;
    [ObservableProperty] private decimal _receivedAmount = 0;
    [ObservableProperty] private decimal _netBalance = 0;

    public Action? RequestClose { get; set; }

    public PestroPurchaseReturnViewModel(
        IDbContextFactory<AppDbContext> contextFactory,
        ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
    }

    public async Task InitializeAsync(bool isWithInvoice)
    {
        IsWithInvoice = isWithInvoice;
        WindowTitle = isWithInvoice 
            ? "Purchase Return (With Invoice) - خریداری واپسی بل کے ساتھ" 
            : "Purchase Return (Without Invoice) - خریداری واپسی بغیر بل کے";

        await LoadSeasonsAsync();
        await LoadVendorsAndProductsAsync();
        await NewInvoiceAsync();
    }

    private async Task LoadSeasonsAsync()
    {
        try
        {
            var seasons = await _systemService.GetAllSeasonsAsync();
            CropSeasons = new ObservableCollection<string>(seasons.Select(s => s.Name));
            var active = seasons.FirstOrDefault(s => s.IsActive);
            SelectedCropSeason = active?.Name ?? CropSeasons.FirstOrDefault() ?? "";
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadSeasons error: {ex.Message}");
        }
    }

    private async Task LoadVendorsAndProductsAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var parties = await context.Parties
                .Where(p => p.PartyType == PartyType.Vendor || p.PartyType == PartyType.Both)
                .OrderBy(p => p.Name)
                .ToListAsync();
            AvailableVendors = new ObservableCollection<Party>(parties);

            var prods = await context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
            AvailableProducts = new ObservableCollection<Product>(prods);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadVendorsAndProducts error: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task NewInvoiceAsync()
    {
        ReturnId = await GetNextReturnIdAsync();
        ReturnDate = DateTime.Today;
        PurchaseId = 0;
        PurchaseDate = null;
        BillNo = "";
        VendorId = "";
        VendorName = "";
        VendorAddress = "";
        PreviousCredit = 0;
        ReceivedAmount = 0;

        Items.Clear();
        RecalculateTotals();
    }

    private async Task<int> GetNextReturnIdAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(MAX(PReturnId), 0) + 1 FROM PurReturnsPest";
            var val = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(val);
        }
        catch
        {
            return 1;
        }
    }

    // In With Invoice Mode: Load Original Purchase
    [RelayCommand]
    public async Task LoadOriginalPurchaseAsync()
    {
        if (PurchaseId <= 0)
        {
            MessageBox.Show("براہ کرم خریداری بل نمبر درج کریں۔ (Please enter a valid Purchase ID)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();

            // 1. Load PurchasesPest header
            using var cmdH = conn.CreateCommand();
            cmdH.CommandText = @"
                SELECT p.PurchaseId, p.EntryDate, p.BillNo, p.BillDate, p.VendorId, 
                       pa.PartyName, pa.Address, pa.City, p.CropSeason 
                FROM PurchasesPest p 
                LEFT JOIN Parties pa ON p.VendorId = pa.PartyID 
                WHERE p.PurchaseId = @pid";
            var prm = cmdH.CreateParameter(); prm.ParameterName = "@pid"; prm.Value = PurchaseId; cmdH.Parameters.Add(prm);

            using var rdrH = await cmdH.ExecuteReaderAsync();
            if (!await rdrH.ReadAsync())
            {
                MessageBox.Show($"خریداری بل نمبر {PurchaseId} نہیں ملا۔ (Purchase Invoice #{PurchaseId} not found)", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            PurchaseDate = rdrH.IsDBNull(1) ? null : rdrH.GetDateTime(1);
            BillNo = rdrH.IsDBNull(2) ? "" : rdrH.GetString(2);
            VendorId = rdrH.IsDBNull(4) ? "" : rdrH.GetString(4);
            VendorName = rdrH.IsDBNull(5) ? "" : rdrH.GetString(5);
            string addr = rdrH.IsDBNull(6) ? "" : rdrH.GetString(6);
            string city = rdrH.IsDBNull(7) ? "" : rdrH.GetString(7);
            VendorAddress = $"{addr} {city}".Trim();
            if (!rdrH.IsDBNull(8)) SelectedCropSeason = rdrH.GetString(8);
            rdrH.Close();

            // 2. Load Line Items and Batches from PurchasesBodyPest & PurchasesBatch
            using var cmdB = conn.CreateCommand();
            cmdB.CommandText = @"
                SELECT pb.SerialNo, pb.ProductId, pr.ProductName, pb.Quantity, pb.Price, 
                       pb.DiscRatio1, pb.STRatio, b.BatchNo, b.ExpiryDate, ISNULL(b.Quantity, pb.Quantity) as BatchQty 
                FROM PurchasesBodyPest pb 
                INNER JOIN Products pr ON pb.ProductId = pr.ProductID 
                LEFT JOIN PurchasesBatch b ON pb.PurchaseId = b.PurchaseId AND pb.ProductId = b.ProductId 
                WHERE pb.PurchaseId = @pid AND ISNULL(pb.IsDeleted, 0) = 0";
            var prmB = cmdB.CreateParameter(); prmB.ParameterName = "@pid"; prmB.Value = PurchaseId; cmdB.Parameters.Add(prmB);

            using var rdrB = await cmdB.ExecuteReaderAsync();
            var list = new ObservableCollection<PestroReturnGridItemDto>();
            int sNo = 1;
            while (await rdrB.ReadAsync())
            {
                var item = new PestroReturnGridItemDto
                {
                    SerialNo = sNo++,
                    ProductId = rdrB.GetString(1),
                    ProductName = rdrB.GetString(2),
                    OriginalQty = Convert.ToDecimal(rdrB.GetValue(9)),
                    ReturnQty = Convert.ToDecimal(rdrB.GetValue(9)), // Default to full qty, user can edit
                    Price = Convert.ToDecimal(rdrB.GetValue(4)),
                    DiscRatio1 = Convert.ToDecimal(rdrB.GetValue(5)),
                    STRatio = Convert.ToDecimal(rdrB.GetValue(6)),
                    BatchNo = rdrB.IsDBNull(7) ? "" : rdrB.GetString(7),
                    ExpiryDate = rdrB.IsDBNull(8) ? DateTime.Today.AddYears(2) : rdrB.GetDateTime(8)
                };
                item.Recalculate();
                list.Add(item);
            }

            Items = list;
            RecalculateTotals();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خرابی: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    // In Without Invoice Mode: Add Line
    [RelayCommand]
    private void AddItemWithoutInvoice()
    {
        if (SelectedProduct == null)
        {
            MessageBox.Show("پروڈکٹ منتخب کریں۔ (Please select a product)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(InputBatchNo))
        {
            MessageBox.Show("بیچ نمبر درج کریں۔ (Specify Batch No)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (InputReturnQty <= 0)
        {
            MessageBox.Show("واپسی تعداد صفر سے زیادہ ہونی چاہیے۔ (Quantity must be > 0)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var item = new PestroReturnGridItemDto
        {
            SerialNo = Items.Count + 1,
            ProductId = !string.IsNullOrWhiteSpace(SelectedProduct.ProductCode) ? SelectedProduct.ProductCode : SelectedProduct.Id.ToString(),
            ProductName = SelectedProduct.Name,
            BatchNo = InputBatchNo.Trim(),
            ExpiryDate = InputExpiryDate ?? DateTime.Today.AddYears(2),
            ReturnQty = InputReturnQty,
            Price = InputPrice,
            DiscRatio1 = InputDiscRatio,
            STRatio = InputSTRatio
        };
        item.Recalculate();
        Items.Add(item);
        RecalculateTotals();

        // Reset inputs
        InputBatchNo = "";
        InputReturnQty = 1;
    }

    [RelayCommand]
    private void RemoveItem(PestroReturnGridItemDto? item)
    {
        if (item == null) return;
        Items.Remove(item);
        int idx = 1;
        foreach (var i in Items) i.SerialNo = idx++;
        RecalculateTotals();
    }

    partial void OnSelectedVendorChanged(Party? value)
    {
        if (value != null)
        {
            VendorId = !string.IsNullOrWhiteSpace(value.PartyNo) ? value.PartyNo : value.Id.ToString();
            VendorName = value.Name;
            VendorAddress = value.Address ?? "";
        }
    }

    partial void OnReceivedAmountChanged(decimal value)
    {
        RecalculateTotals();
    }

    public void RecalculateTotals()
    {
        TotalReturnQty = Items.Sum(i => i.ReturnQty);
        TotalGrossValue = Items.Sum(i => i.ReturnQty * i.Price);
        TotalDiscount = Items.Sum(i => (i.ReturnQty * i.Price) * (i.DiscRatio1 / 100m));
        TotalSalesTax = Items.Sum(i => i.STValue);
        TotalNetValue = Items.Sum(i => i.TotalValue);
        NetBalance = TotalNetValue - ReceivedAmount;
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(VendorId))
        {
            MessageBox.Show("براہ کرم وینڈر / سپلائر منتخب کریں۔ (Please specify a vendor)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Items.Count == 0)
        {
            MessageBox.Show("واپسی کے لیے کم از کم ایک پروڈکٹ شامل کریں۔ (No items in return grid)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();
            using var trans = conn.BeginTransaction();

            // 1. Insert into PurReturnsPest
            using (var cmdH = conn.CreateCommand())
            {
                cmdH.Transaction = trans;
                cmdH.CommandText = @"
                    INSERT INTO PurReturnsPest (
                        PReturnId, PurchaseId, LastUpdateDate, ReturnDate, 
                        PreviousCredit, ReceivedAmount, VendorId, IsPosted, 
                        NarrationUrdu, CropSeason
                    ) VALUES (
                        @rid, @pid, GETDATE(), @rdate, 
                        @prevCredit, @rcvAmount, @vid, 1, 
                        @urdu, @cropSeason
                    )";

                var p1 = cmdH.CreateParameter(); p1.ParameterName = "@rid"; p1.Value = ReturnId; cmdH.Parameters.Add(p1);
                var p2 = cmdH.CreateParameter(); p2.ParameterName = "@pid"; p2.Value = IsWithInvoice ? PurchaseId : 0; cmdH.Parameters.Add(p2);
                var p3 = cmdH.CreateParameter(); p3.ParameterName = "@rdate"; p3.Value = ReturnDate; cmdH.Parameters.Add(p3);
                var p4 = cmdH.CreateParameter(); p4.ParameterName = "@prevCredit"; p4.Value = PreviousCredit; cmdH.Parameters.Add(p4);
                var p5 = cmdH.CreateParameter(); p5.ParameterName = "@rcvAmount"; p5.Value = ReceivedAmount; cmdH.Parameters.Add(p5);
                var p6 = cmdH.CreateParameter(); p6.ParameterName = "@vid"; p6.Value = VendorId; cmdH.Parameters.Add(p6);
                var p7 = cmdH.CreateParameter(); p7.ParameterName = "@urdu"; p7.Value = $"خریداری واپسی واؤچر #{ReturnId}"; cmdH.Parameters.Add(p7);
                var p8 = cmdH.CreateParameter(); p8.ParameterName = "@cropSeason"; p8.Value = SelectedCropSeason; cmdH.Parameters.Add(p8);

                await cmdH.ExecuteNonQueryAsync();
            }

            // 2. Insert into PurReturnsBodyPest & PurReturnsBatch, & Update CurrentStockPestro
            int sNo = 1;
            foreach (var item in Items)
            {
                // PurReturnsBodyPest
                using (var cmdB = conn.CreateCommand())
                {
                    cmdB.Transaction = trans;
                    cmdB.CommandText = @"
                        INSERT INTO PurReturnsBodyPest (
                            SerialNo, PReturnId, ProductId, Quantity, Bonus, 
                            Price, DiscRatio1, DiscRatio2, STValue, STRatio, 
                            TTLValue, TTLSalestax, IsDeleted
                        ) VALUES (
                            @sno, @rid, @pid, @qty, 0, 
                            @price, @disc1, 0, @stVal, @stRatio, 
                            @ttlVal, @ttlStax, 0
                        )";

                    var b1 = cmdB.CreateParameter(); b1.ParameterName = "@sno"; b1.Value = sNo; cmdB.Parameters.Add(b1);
                    var b2 = cmdB.CreateParameter(); b2.ParameterName = "@rid"; b2.Value = ReturnId; cmdB.Parameters.Add(b2);
                    var b3 = cmdB.CreateParameter(); b3.ParameterName = "@pid"; b3.Value = item.ProductId; cmdB.Parameters.Add(b3);
                    var b4 = cmdB.CreateParameter(); b4.ParameterName = "@qty"; b4.Value = (int)item.ReturnQty; cmdB.Parameters.Add(b4);
                    var b5 = cmdB.CreateParameter(); b5.ParameterName = "@price"; b5.Value = item.Price; cmdB.Parameters.Add(b5);
                    var b6 = cmdB.CreateParameter(); b6.ParameterName = "@disc1"; b6.Value = item.DiscRatio1; cmdB.Parameters.Add(b6);
                    var b7 = cmdB.CreateParameter(); b7.ParameterName = "@stVal"; b7.Value = item.STValue; cmdB.Parameters.Add(b7);
                    var b8 = cmdB.CreateParameter(); b8.ParameterName = "@stRatio"; b8.Value = item.STRatio; cmdB.Parameters.Add(b8);
                    var b9 = cmdB.CreateParameter(); b9.ParameterName = "@ttlVal"; b9.Value = item.TTLValue; cmdB.Parameters.Add(b9);
                    var b10 = cmdB.CreateParameter(); b10.ParameterName = "@ttlStax"; b10.Value = item.STValue; cmdB.Parameters.Add(b10);

                    await cmdB.ExecuteNonQueryAsync();
                }

                // PurReturnsBatch
                using (var cmdBt = conn.CreateCommand())
                {
                    cmdBt.Transaction = trans;
                    cmdBt.CommandText = @"
                        INSERT INTO PurReturnsBatch (
                            PReturnId, ProductId, BatchNo, Quantity, ExpiryDate, Cost, IsDeleted
                        ) VALUES (
                            @rid, @pid, @bno, @qty, @exp, @cost, 0
                        )";

                    var bt1 = cmdBt.CreateParameter(); bt1.ParameterName = "@rid"; bt1.Value = ReturnId; cmdBt.Parameters.Add(bt1);
                    var bt2 = cmdBt.CreateParameter(); bt2.ParameterName = "@pid"; bt2.Value = item.ProductId; cmdBt.Parameters.Add(bt2);
                    var bt3 = cmdBt.CreateParameter(); bt3.ParameterName = "@bno"; bt3.Value = item.BatchNo; cmdBt.Parameters.Add(bt3);
                    var bt4 = cmdBt.CreateParameter(); bt4.ParameterName = "@qty"; bt4.Value = (int)item.ReturnQty; cmdBt.Parameters.Add(bt4);
                    var bt5 = cmdBt.CreateParameter(); bt5.ParameterName = "@exp"; bt5.Value = item.ExpiryDate; cmdBt.Parameters.Add(bt5);
                    var bt6 = cmdBt.CreateParameter(); bt6.ParameterName = "@cost"; bt6.Value = item.Price; cmdBt.Parameters.Add(bt6);

                    await cmdBt.ExecuteNonQueryAsync();
                }

                // Deduct stock from CurrentStockPestro
                using (var cmdStk = conn.CreateCommand())
                {
                    cmdStk.Transaction = trans;
                    cmdStk.CommandText = @"
                        UPDATE CurrentStockPestro 
                        SET Quantity = CASE WHEN Quantity >= @qty THEN Quantity - @qty ELSE 0 END 
                        WHERE ProductId = @pid AND BatchNo = @bno";

                    var s1 = cmdStk.CreateParameter(); s1.ParameterName = "@qty"; s1.Value = (int)item.ReturnQty; cmdStk.Parameters.Add(s1);
                    var s2 = cmdStk.CreateParameter(); s2.ParameterName = "@pid"; s2.Value = item.ProductId; cmdStk.Parameters.Add(s2);
                    var s3 = cmdStk.CreateParameter(); s3.ParameterName = "@bno"; s3.Value = item.BatchNo; cmdStk.Parameters.Add(s3);

                    await cmdStk.ExecuteNonQueryAsync();
                }

                sNo++;
            }

            trans.Commit();

            MessageBox.Show($"Purchase Return #{ReturnId} successfully saved.\n(خریداری واپسی بل کامیابی سے محفوظ ہو گیا)", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            await NewInvoiceAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"محفوظ کرنے میں نقص: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
