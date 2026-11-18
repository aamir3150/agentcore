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

public class SalesmanDto
{
    public string Id { get; set; } = "";
    public string Name { get; set; } = "";
}

public class PestroSaleReturnGridItemDto : ObservableObject
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

    private decimal _originalSaleQty = 0;
    public decimal OriginalSaleQty { get => _originalSaleQty; set => SetProperty(ref _originalSaleQty, value); }

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

public partial class PestroSaleReturnViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    [ObservableProperty] private bool _isWithInvoice = true;
    [ObservableProperty] private string _windowTitle = "Sale Return (With Invoice) - فروخت واپسی بل کے ساتھ";

    // Header fields
    [ObservableProperty] private int _returnId = 0;
    [ObservableProperty] private DateTime _returnDate = DateTime.Today;
    [ObservableProperty] private int _saleId = 0;
    [ObservableProperty] private DateTime? _saleDate;

    [ObservableProperty] private string _customerId = "";
    [ObservableProperty] private string _customerName = "";
    [ObservableProperty] private string _customerAddress = "";

    [ObservableProperty] private string _salesmanId = "";
    [ObservableProperty] private string _salesmanName = "";

    [ObservableProperty] private decimal _previousDebit = 0;
    [ObservableProperty] private decimal _specialDiscount = 0;

    // Crop Seasons
    [ObservableProperty] private ObservableCollection<string> _cropSeasons = new();
    [ObservableProperty] private string _selectedCropSeason = "";

    // Customers for WoI mode
    [ObservableProperty] private ObservableCollection<Party> _availableCustomers = new();
    [ObservableProperty] private Party? _selectedCustomer;

    // Salesmen
    [ObservableProperty] private ObservableCollection<SalesmanDto> _availableSalesmen = new();
    [ObservableProperty] private SalesmanDto? _selectedSalesman;

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
    [ObservableProperty] private ObservableCollection<PestroSaleReturnGridItemDto> _items = new();
    [ObservableProperty] private PestroSaleReturnGridItemDto? _selectedItem;

    // Financial Totals
    [ObservableProperty] private decimal _totalReturnQty = 0;
    [ObservableProperty] private decimal _totalGrossValue = 0;
    [ObservableProperty] private decimal _totalDiscount = 0;
    [ObservableProperty] private decimal _totalSalesTax = 0;
    [ObservableProperty] private decimal _totalNetValue = 0;
    [ObservableProperty] private decimal _paidAmount = 0;
    [ObservableProperty] private decimal _netBalance = 0;

    public Action? RequestClose { get; set; }

    public PestroSaleReturnViewModel(
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
            ? "Sale Return (With Invoice) - فروخت واپسی بل کے ساتھ" 
            : "Sale Return (Without Invoice) - فروخت واپسی بغیر بل کے";

        await LoadSeasonsAsync();
        await LoadCustomersSalesmenAndProductsAsync();
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

    private async Task LoadCustomersSalesmenAndProductsAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var parties = await context.Parties
                .Where(p => p.PartyType == PartyType.Customer || p.PartyType == PartyType.Both)
                .OrderBy(p => p.Name)
                .ToListAsync();
            AvailableCustomers = new ObservableCollection<Party>(parties);

            var prods = await context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();
            AvailableProducts = new ObservableCollection<Product>(prods);

            // Load Salesmen from Parties or Users
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT SalesManId, SalesManName FROM SalesMen ORDER BY SalesManName";
            using var rdr = await cmd.ExecuteReaderAsync();
            var smList = new ObservableCollection<SalesmanDto>();
            while (await rdr.ReadAsync())
            {
                smList.Add(new SalesmanDto
                {
                    Id = rdr.IsDBNull(0) ? "" : rdr.GetValue(0).ToString() ?? "",
                    Name = rdr.IsDBNull(1) ? "" : rdr.GetString(1)
                });
            }
            AvailableSalesmen = smList;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadCustomersSalesmen error: {ex.Message}");
        }
    }

    [RelayCommand]
    public async Task NewInvoiceAsync()
    {
        ReturnId = await GetNextReturnIdAsync();
        ReturnDate = DateTime.Today;
        SaleId = 0;
        SaleDate = null;
        CustomerId = "";
        CustomerName = "";
        CustomerAddress = "";
        SalesmanId = "";
        SalesmanName = "";
        PreviousDebit = 0;
        PaidAmount = 0;
        SpecialDiscount = 0;

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
            cmd.CommandText = "SELECT ISNULL(MAX(SReturnId), 0) + 1 FROM SaleReturnsPest";
            var val = await cmd.ExecuteScalarAsync();
            return Convert.ToInt32(val);
        }
        catch
        {
            return 1;
        }
    }

    [RelayCommand]
    public async Task LoadOriginalSaleAsync()
    {
        if (SaleId <= 0)
        {
            MessageBox.Show("براہ کرم فروخت بل نمبر درج کریں۔ (Please enter a valid Sale ID)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();

            // 1. Load SalesPest header
            using var cmdH = conn.CreateCommand();
            cmdH.CommandText = @"
                SELECT s.SaleId, s.SaleDate, s.CustomerId, pa.PartyName, pa.Address, pa.City, 
                       s.SalesManId, sm.SalesManName, s.CropSeason 
                FROM SalesPest s 
                LEFT JOIN Parties pa ON s.CustomerId = pa.PartyID 
                LEFT JOIN SalesMen sm ON s.SalesManId = sm.SalesManId 
                WHERE s.SaleId = @sid";
            var prm = cmdH.CreateParameter(); prm.ParameterName = "@sid"; prm.Value = SaleId; cmdH.Parameters.Add(prm);

            using var rdrH = await cmdH.ExecuteReaderAsync();
            if (!await rdrH.ReadAsync())
            {
                MessageBox.Show($"فروخت بل نمبر {SaleId} نہیں ملا۔ (Sale Invoice #{SaleId} not found)", "Not Found", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            SaleDate = rdrH.IsDBNull(1) ? null : rdrH.GetDateTime(1);
            CustomerId = rdrH.IsDBNull(2) ? "" : rdrH.GetString(2);
            CustomerName = rdrH.IsDBNull(3) ? "" : rdrH.GetString(3);
            string addr = rdrH.IsDBNull(4) ? "" : rdrH.GetString(4);
            string city = rdrH.IsDBNull(5) ? "" : rdrH.GetString(5);
            CustomerAddress = $"{addr} {city}".Trim();
            SalesmanId = rdrH.IsDBNull(6) ? "" : rdrH.GetString(6);
            SalesmanName = rdrH.IsDBNull(7) ? "" : rdrH.GetString(7);
            if (!rdrH.IsDBNull(8)) SelectedCropSeason = rdrH.GetString(8);
            rdrH.Close();

            // 2. Load line items and batches from SalesBodyPest & SalesBatch
            using var cmdB = conn.CreateCommand();
            cmdB.CommandText = @"
                SELECT sb.SerialNo, sb.ProductId, pr.ProductName, sb.Quantity, sb.Price, 
                       sb.DiscRatio1, sb.STRatio, b.BatchNo, b.ExpiryDate, ISNULL(b.Quantity, sb.Quantity) as BatchQty 
                FROM SalesBodyPest sb 
                INNER JOIN Products pr ON sb.ProductId = pr.ProductID 
                LEFT JOIN SalesBatch b ON sb.SaleId = b.SaleId AND sb.ProductId = b.ProductId 
                WHERE sb.SaleId = @sid AND ISNULL(sb.IsDeleted, 0) = 0";
            var prmB = cmdB.CreateParameter(); prmB.ParameterName = "@sid"; prmB.Value = SaleId; cmdB.Parameters.Add(prmB);

            using var rdrB = await cmdB.ExecuteReaderAsync();
            var list = new ObservableCollection<PestroSaleReturnGridItemDto>();
            int sNo = 1;
            while (await rdrB.ReadAsync())
            {
                var item = new PestroSaleReturnGridItemDto
                {
                    SerialNo = sNo++,
                    ProductId = rdrB.GetString(1),
                    ProductName = rdrB.GetString(2),
                    OriginalSaleQty = Convert.ToDecimal(rdrB.GetValue(9)),
                    ReturnQty = Convert.ToDecimal(rdrB.GetValue(9)),
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

        var item = new PestroSaleReturnGridItemDto
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

        // Reset
        InputBatchNo = "";
        InputReturnQty = 1;
    }

    [RelayCommand]
    private void RemoveItem(PestroSaleReturnGridItemDto? item)
    {
        if (item == null) return;
        Items.Remove(item);
        int idx = 1;
        foreach (var i in Items) i.SerialNo = idx++;
        RecalculateTotals();
    }

    partial void OnSelectedCustomerChanged(Party? value)
    {
        if (value != null)
        {
            CustomerId = !string.IsNullOrWhiteSpace(value.PartyNo) ? value.PartyNo : value.Id.ToString();
            CustomerName = value.Name;
            CustomerAddress = value.Address ?? "";
        }
    }

    partial void OnSelectedSalesmanChanged(SalesmanDto? value)
    {
        if (value != null)
        {
            SalesmanId = value.Id;
            SalesmanName = value.Name;
        }
    }

    partial void OnPaidAmountChanged(decimal value)
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
        NetBalance = TotalNetValue - PaidAmount;
    }

    [RelayCommand]
    public async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(CustomerId))
        {
            MessageBox.Show("براہ کرم کسٹمر منتخب کریں۔ (Please specify a customer)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
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

            // 1. Insert into SaleReturnsPest
            using (var cmdH = conn.CreateCommand())
            {
                cmdH.Transaction = trans;
                cmdH.CommandText = @"
                    INSERT INTO SaleReturnsPest (
                        SReturnId, SaleId, LastUpdateDate, ReturnDate, 
                        PreviousDebit, PaidAmount, SpecialDiscount, CustomerId, 
                        IsUndelivered, IsPosted, SalesManId, NarrationUrdu, CropSeason
                    ) VALUES (
                        @rid, @sid, GETDATE(), @rdate, 
                        @prevDebit, @paidAmount, @spDisc, @cid, 
                        0, 1, @smid, @urdu, @cropSeason
                    )";

                var p1 = cmdH.CreateParameter(); p1.ParameterName = "@rid"; p1.Value = ReturnId; cmdH.Parameters.Add(p1);
                var p2 = cmdH.CreateParameter(); p2.ParameterName = "@sid"; p2.Value = IsWithInvoice ? SaleId : 0; cmdH.Parameters.Add(p2);
                var p3 = cmdH.CreateParameter(); p3.ParameterName = "@rdate"; p3.Value = ReturnDate; cmdH.Parameters.Add(p3);
                var p4 = cmdH.CreateParameter(); p4.ParameterName = "@prevDebit"; p4.Value = PreviousDebit; cmdH.Parameters.Add(p4);
                var p5 = cmdH.CreateParameter(); p5.ParameterName = "@paidAmount"; p5.Value = PaidAmount; cmdH.Parameters.Add(p5);
                var p6 = cmdH.CreateParameter(); p6.ParameterName = "@spDisc"; p6.Value = SpecialDiscount; cmdH.Parameters.Add(p6);
                var p7 = cmdH.CreateParameter(); p7.ParameterName = "@cid"; p7.Value = CustomerId; cmdH.Parameters.Add(p7);
                var p8 = cmdH.CreateParameter(); p8.ParameterName = "@smid"; p8.Value = SalesmanId; cmdH.Parameters.Add(p8);
                var p9 = cmdH.CreateParameter(); p9.ParameterName = "@urdu"; p9.Value = $"فروخت واپسی واؤچر #{ReturnId}"; cmdH.Parameters.Add(p9);
                var p10 = cmdH.CreateParameter(); p10.ParameterName = "@cropSeason"; p10.Value = SelectedCropSeason; cmdH.Parameters.Add(p10);

                await cmdH.ExecuteNonQueryAsync();
            }

            // 2. Insert into SaleReturnsBodyPest, SaleReturnsBatch, and restore stock in CurrentStockPestro
            int sNo = 1;
            foreach (var item in Items)
            {
                // SaleReturnsBodyPest
                using (var cmdB = conn.CreateCommand())
                {
                    cmdB.Transaction = trans;
                    cmdB.CommandText = @"
                        INSERT INTO SaleReturnsBodyPest (
                            SerialNo, SReturnId, ProductId, Quantity, Bonus, 
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

                // SaleReturnsBatch
                using (var cmdBt = conn.CreateCommand())
                {
                    cmdBt.Transaction = trans;
                    cmdBt.CommandText = @"
                        INSERT INTO SaleReturnsBatch (
                            SReturnId, ProductId, BatchNo, ExpiryDate, Quantity, Cost, IsDeleted
                        ) VALUES (
                            @rid, @pid, @bno, @exp, @qty, @cost, 0
                        )";

                    var bt1 = cmdBt.CreateParameter(); bt1.ParameterName = "@rid"; bt1.Value = ReturnId; cmdBt.Parameters.Add(bt1);
                    var bt2 = cmdBt.CreateParameter(); bt2.ParameterName = "@pid"; bt2.Value = item.ProductId; cmdBt.Parameters.Add(bt2);
                    var bt3 = cmdBt.CreateParameter(); bt3.ParameterName = "@bno"; bt3.Value = item.BatchNo; cmdBt.Parameters.Add(bt3);
                    var bt4 = cmdBt.CreateParameter(); bt4.ParameterName = "@exp"; bt4.Value = item.ExpiryDate; cmdBt.Parameters.Add(bt4);
                    var bt5 = cmdBt.CreateParameter(); bt5.ParameterName = "@qty"; bt5.Value = (int)item.ReturnQty; cmdBt.Parameters.Add(bt5);
                    var bt6 = cmdBt.CreateParameter(); bt6.ParameterName = "@cost"; bt6.Value = item.Price; cmdBt.Parameters.Add(bt6);

                    await cmdBt.ExecuteNonQueryAsync();
                }

                // Add stock back into CurrentStockPestro
                using (var cmdStk = conn.CreateCommand())
                {
                    cmdStk.Transaction = trans;
                    cmdStk.CommandText = @"
                        UPDATE CurrentStockPestro 
                        SET Quantity = Quantity + @qty 
                        WHERE ProductId = @pid AND BatchNo = @bno;
                        IF @@ROWCOUNT = 0
                            INSERT INTO CurrentStockPestro (ProductId, BatchNo, Quantity, Cost, ExpiryDate) 
                            VALUES (@pid, @bno, @qty, @cost, @exp);";

                    var s1 = cmdStk.CreateParameter(); s1.ParameterName = "@qty"; s1.Value = (int)item.ReturnQty; cmdStk.Parameters.Add(s1);
                    var s2 = cmdStk.CreateParameter(); s2.ParameterName = "@pid"; s2.Value = item.ProductId; cmdStk.Parameters.Add(s2);
                    var s3 = cmdStk.CreateParameter(); s3.ParameterName = "@bno"; s3.Value = item.BatchNo; cmdStk.Parameters.Add(s3);
                    var s4 = cmdStk.CreateParameter(); s4.ParameterName = "@cost"; s4.Value = item.Price; cmdStk.Parameters.Add(s4);
                    var s5 = cmdStk.CreateParameter(); s5.ParameterName = "@exp"; s5.Value = item.ExpiryDate; cmdStk.Parameters.Add(s5);

                    await cmdStk.ExecuteNonQueryAsync();
                }

                sNo++;
            }

            trans.Commit();

            MessageBox.Show($"Sale Return #{ReturnId} successfully saved.\n(فروخت واپسی بل کامیابی سے محفوظ ہو گیا اور اسٹاک شامل ہو گیا)", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
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
