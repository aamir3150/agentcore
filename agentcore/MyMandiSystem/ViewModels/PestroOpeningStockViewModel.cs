using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public class PestroOpeningProductItem : ObservableObject
{
    private string _productId = "";
    public string ProductId { get => _productId; set => SetProperty(ref _productId, value); }

    private string _productName = "";
    public string ProductName { get => _productName; set => SetProperty(ref _productName, value); }

    private decimal _cost;
    public decimal Cost { get => _cost; set => SetProperty(ref _cost, value); }

    private decimal _quantity;
    public decimal Quantity { get => _quantity; set => SetProperty(ref _quantity, value); }
}

public class PestroOpeningBatchItem : ObservableObject
{
    public int SerialId { get; set; } = 0;
    public string ProductId { get; set; } = "";

    private string _batchNo = "";
    public string BatchNo { get => _batchNo; set => SetProperty(ref _batchNo, value); }

    private DateTime _expiryDate = DateTime.Today.AddYears(2);
    public DateTime ExpiryDate { get => _expiryDate; set => SetProperty(ref _expiryDate, value); }

    private decimal _quantity;
    public decimal Quantity
    {
        get => _quantity;
        set
        {
            if (SetProperty(ref _quantity, value))
                OnPropertyChanged(nameof(Value));
        }
    }

    private decimal _cost;
    public decimal Cost
    {
        get => _cost;
        set
        {
            if (SetProperty(ref _cost, value))
                OnPropertyChanged(nameof(Value));
        }
    }

    public decimal Value => Math.Round(Quantity * Cost, 2);
}

public class CompanyLookupDto
{
    public string CompanyId { get; set; } = "";
    public string CompanyName { get; set; } = "";
}

public partial class PestroOpeningStockViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    [ObservableProperty] private ObservableCollection<CompanyLookupDto> _companies = new();
    [ObservableProperty] private CompanyLookupDto? _selectedCompany;
    [ObservableProperty] private string _filterText = "";

    [ObservableProperty] private ObservableCollection<PestroOpeningProductItem> _products = new();
    [ObservableProperty] private PestroOpeningProductItem? _selectedProduct;

    [ObservableProperty] private ObservableCollection<PestroOpeningBatchItem> _batches = new();
    [ObservableProperty] private PestroOpeningBatchItem? _selectedBatch;

    // Batch Entry Fields
    [ObservableProperty] private string _inputBatchNo = "";
    [ObservableProperty] private DateTime? _inputExpiryDate = DateTime.Today.AddYears(2);
    [ObservableProperty] private decimal _inputQuantity = 1;
    [ObservableProperty] private decimal _inputCost = 0;

    // Totals
    [ObservableProperty] private decimal _totalStockQuantity = 0;
    [ObservableProperty] private decimal _totalStockValue = 0;

    public Action? RequestClose { get; set; }

    public PestroOpeningStockViewModel(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await LoadCompaniesAsync();
        await LoadProductsAsync();
    }

    public async Task LoadCompaniesAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT CompanyID, CompanyName FROM Companies ORDER BY CompanyName";
            using var reader = await cmd.ExecuteReaderAsync();

            var list = new ObservableCollection<CompanyLookupDto>
            {
                new CompanyLookupDto { CompanyId = "", CompanyName = "-- ALL COMPANIES --" }
            };

            while (await reader.ReadAsync())
            {
                list.Add(new CompanyLookupDto
                {
                    CompanyId = reader.IsDBNull(0) ? "" : reader.GetValue(0).ToString() ?? "",
                    CompanyName = reader.IsDBNull(1) ? "" : reader.GetString(1)
                });
            }

            Companies = list;
            SelectedCompany = list[0];
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadCompanies error: {ex.Message}");
        }
    }

    public async Task LoadProductsAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            string sql = @"
                SELECT p.ProductID, p.ProductName, 
                       ISNULL(os.Price, ISNULL(p.PurchasePrice, 0)) as Cost, 
                       ISNULL(os.Quantity, 0) as Qty 
                FROM Products p 
                LEFT JOIN OpeningStockPestro os ON p.ProductID = os.ProductID 
                WHERE 1=1 AND ISNULL(p.IsPest, 0) = 1";

            if (SelectedCompany != null && !string.IsNullOrWhiteSpace(SelectedCompany.CompanyId))
            {
                sql += " AND p.CompanyID = @cid";
                var p = cmd.CreateParameter();
                p.ParameterName = "@cid";
                p.Value = SelectedCompany.CompanyId;
                cmd.Parameters.Add(p);
            }

            if (!string.IsNullOrWhiteSpace(FilterText))
            {
                sql += " AND p.ProductName LIKE @filt";
                var pFilt = cmd.CreateParameter();
                pFilt.ParameterName = "@filt";
                pFilt.Value = $"%{FilterText.Trim()}%";
                cmd.Parameters.Add(pFilt);
            }

            sql += " ORDER BY p.ProductName";
            cmd.CommandText = sql;

            using var reader = await cmd.ExecuteReaderAsync();
            var list = new ObservableCollection<PestroOpeningProductItem>();
            while (await reader.ReadAsync())
            {
                list.Add(new PestroOpeningProductItem
                {
                    ProductId = reader.IsDBNull(0) ? "" : reader.GetValue(0).ToString() ?? "",
                    ProductName = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    Cost = reader.IsDBNull(2) ? 0 : Convert.ToDecimal(reader.GetValue(2)),
                    Quantity = reader.IsDBNull(3) ? 0 : Convert.ToDecimal(reader.GetValue(3))
                });
            }

            Products = list;
            if (Products.Count > 0 && SelectedProduct == null)
            {
                SelectedProduct = Products[0];
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadProducts error: {ex.Message}");
        }
    }

    partial void OnSelectedCompanyChanged(CompanyLookupDto? value)
    {
        _ = LoadProductsAsync();
    }

    partial void OnFilterTextChanged(string value)
    {
        _ = LoadProductsAsync();
    }

    partial void OnSelectedProductChanged(PestroOpeningProductItem? value)
    {
        if (value != null)
        {
            InputCost = value.Cost;
            _ = LoadBatchesForProductAsync(value.ProductId);
        }
        else
        {
            Batches.Clear();
            RecalculateTotals();
        }
    }

    public async Task LoadBatchesForProductAsync(string productId)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT SerialID, ProductId, BatchNo, Quantity, ExpiryDate, Cost 
                FROM OpeningBatchPestro 
                WHERE ProductId = @pid AND ISNULL(IsDeleted, 0) = 0
                ORDER BY ExpiryDate ASC";

            var param = cmd.CreateParameter();
            param.ParameterName = "@pid";
            param.Value = productId;
            cmd.Parameters.Add(param);

            using var reader = await cmd.ExecuteReaderAsync();
            var list = new ObservableCollection<PestroOpeningBatchItem>();
            while (await reader.ReadAsync())
            {
                list.Add(new PestroOpeningBatchItem
                {
                    SerialId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    ProductId = reader.IsDBNull(1) ? "" : reader.GetString(1),
                    BatchNo = reader.IsDBNull(2) ? "" : reader.GetString(2),
                    Quantity = reader.IsDBNull(3) ? 0 : Convert.ToDecimal(reader.GetValue(3)),
                    ExpiryDate = reader.IsDBNull(4) ? DateTime.Today.AddYears(2) : reader.GetDateTime(4),
                    Cost = reader.IsDBNull(5) ? 0 : Convert.ToDecimal(reader.GetValue(5))
                });
            }

            Batches = list;
            RecalculateTotals();
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadBatches error: {ex.Message}");
        }
    }

    [RelayCommand]
    private void AddBatch()
    {
        if (SelectedProduct == null)
        {
            MessageBox.Show("براہ کرم پروڈکٹ منتخب کریں۔ (Please select a product first)", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (string.IsNullOrWhiteSpace(InputBatchNo))
        {
            MessageBox.Show("Please, specify the Batch no for this Product.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (InputQuantity <= 0)
        {
            MessageBox.Show("Quantity must be greater than ZERO.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Batches.Any(b => b.BatchNo.Equals(InputBatchNo.Trim(), StringComparison.OrdinalIgnoreCase)))
        {
            MessageBox.Show($"Batchno already exists in the Grid for Product '{SelectedProduct.ProductName}'.", "Duplicate Batch", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var newBatch = new PestroOpeningBatchItem
        {
            SerialId = 0,
            ProductId = SelectedProduct.ProductId,
            BatchNo = InputBatchNo.Trim(),
            ExpiryDate = InputExpiryDate ?? DateTime.Today.AddYears(2),
            Quantity = InputQuantity,
            Cost = InputCost
        };

        Batches.Add(newBatch);
        RecalculateTotals();

        // Clear input fields
        InputBatchNo = "";
        InputQuantity = 1;
    }

    [RelayCommand]
    private void DeleteBatch(PestroOpeningBatchItem? batch)
    {
        if (batch == null) return;
        Batches.Remove(batch);
        RecalculateTotals();
    }

    private void RecalculateTotals()
    {
        TotalStockQuantity = Batches.Sum(b => b.Quantity);
        TotalStockValue = Batches.Sum(b => b.Value);
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (SelectedProduct == null)
        {
            MessageBox.Show("براہ کرم محفوظ کرنے کے لیے پروڈکٹ منتخب کریں۔", "Warning", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();
            using var trans = conn.BeginTransaction();

            // 1. Delete existing opening batches for this product
            using (var cmdDel = conn.CreateCommand())
            {
                cmdDel.Transaction = trans;
                cmdDel.CommandText = "DELETE FROM OpeningBatchPestro WHERE ProductId = @pid";
                var p = cmdDel.CreateParameter();
                p.ParameterName = "@pid";
                p.Value = SelectedProduct.ProductId;
                cmdDel.Parameters.Add(p);
                await cmdDel.ExecuteNonQueryAsync();
            }

            // 2. Insert new batches
            decimal totalQty = 0;
            decimal avgCost = InputCost;

            foreach (var b in Batches)
            {
                using var cmdIns = conn.CreateCommand();
                cmdIns.Transaction = trans;
                cmdIns.CommandText = @"
                    INSERT INTO OpeningBatchPestro (ProductId, BatchNo, ExpiryDate, Quantity, Cost, IsDeleted) 
                    VALUES (@pid, @bno, @exp, @qty, @cost, 0)";

                var p1 = cmdIns.CreateParameter(); p1.ParameterName = "@pid"; p1.Value = SelectedProduct.ProductId; cmdIns.Parameters.Add(p1);
                var p2 = cmdIns.CreateParameter(); p2.ParameterName = "@bno"; p2.Value = b.BatchNo; cmdIns.Parameters.Add(p2);
                var p3 = cmdIns.CreateParameter(); p3.ParameterName = "@exp"; p3.Value = b.ExpiryDate; cmdIns.Parameters.Add(p3);
                var p4 = cmdIns.CreateParameter(); p4.ParameterName = "@qty"; p4.Value = b.Quantity; cmdIns.Parameters.Add(p4);
                var p5 = cmdIns.CreateParameter(); p5.ParameterName = "@cost"; p5.Value = b.Cost; cmdIns.Parameters.Add(p5);

                await cmdIns.ExecuteNonQueryAsync();
                totalQty += b.Quantity;
                avgCost = b.Cost;

                // Sync each batch into CurrentStockPestro
                using var cmdStock = conn.CreateCommand();
                cmdStock.Transaction = trans;
                cmdStock.CommandText = @"
                    UPDATE CurrentStockPestro 
                    SET Quantity = @qty, Cost = @cost, ExpiryDate = @exp 
                    WHERE ProductId = @pid AND BatchNo = @bno;
                    IF @@ROWCOUNT = 0
                        INSERT INTO CurrentStockPestro (ProductId, BatchNo, Quantity, Cost, ExpiryDate) 
                        VALUES (@pid, @bno, @qty, @cost, @exp);";

                var s1 = cmdStock.CreateParameter(); s1.ParameterName = "@pid"; s1.Value = SelectedProduct.ProductId; cmdStock.Parameters.Add(s1);
                var s2 = cmdStock.CreateParameter(); s2.ParameterName = "@bno"; s2.Value = b.BatchNo; cmdStock.Parameters.Add(s2);
                var s3 = cmdStock.CreateParameter(); s3.ParameterName = "@qty"; s3.Value = (int)b.Quantity; cmdStock.Parameters.Add(s3);
                var s4 = cmdStock.CreateParameter(); s4.ParameterName = "@cost"; s4.Value = b.Cost; cmdStock.Parameters.Add(s4);
                var s5 = cmdStock.CreateParameter(); s5.ParameterName = "@exp"; s5.Value = b.ExpiryDate; cmdStock.Parameters.Add(s5);
                await cmdStock.ExecuteNonQueryAsync();
            }

            // 3. Upsert OpeningStockPestro
            using (var cmdUpsert = conn.CreateCommand())
            {
                cmdUpsert.Transaction = trans;
                cmdUpsert.CommandText = @"
                    IF EXISTS (SELECT 1 FROM OpeningStockPestro WHERE ProductId = @pid)
                        UPDATE OpeningStockPestro SET Quantity = @qty, Price = @price WHERE ProductId = @pid;
                    ELSE
                        INSERT INTO OpeningStockPestro (ProductId, Quantity, Bonus, Price, DiscRatio1, DiscRatio2, STValue, STRatio, TTLValue, TTLSalestax, IsDeleted) 
                        VALUES (@pid, @qty, 0, @price, 0, 0, 0, 0, @qty * @price, 0, 0);";

                var u1 = cmdUpsert.CreateParameter(); u1.ParameterName = "@pid"; u1.Value = SelectedProduct.ProductId; cmdUpsert.Parameters.Add(u1);
                var u2 = cmdUpsert.CreateParameter(); u2.ParameterName = "@qty"; u2.Value = totalQty; cmdUpsert.Parameters.Add(u2);
                var u3 = cmdUpsert.CreateParameter(); u3.ParameterName = "@price"; u3.Value = avgCost; cmdUpsert.Parameters.Add(u3);
                await cmdUpsert.ExecuteNonQueryAsync();
            }

            trans.Commit();

            SelectedProduct.Quantity = totalQty;
            SelectedProduct.Cost = avgCost;

            MessageBox.Show("Changes saved succesfully.\n(ابتدائی اسٹاک کامیابی سے محفوظ کر دیا گیا)", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"محفوظ کرنے میں نقص: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task DeleteAllStockForProductAsync()
    {
        if (SelectedProduct == null) return;

        var res = MessageBox.Show($"کیا آپ واقعی '{SelectedProduct.ProductName}' کا ابتدائی اسٹاک حذف کرنا چاہتے ہیں؟", "Confirm Delete", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (res != MessageBoxResult.Yes) return;

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();
            using var trans = conn.BeginTransaction();

            using (var cmd = conn.CreateCommand())
            {
                cmd.Transaction = trans;
                cmd.CommandText = "DELETE FROM OpeningBatchPestro WHERE ProductId = @pid; DELETE FROM OpeningStockPestro WHERE ProductId = @pid;";
                var p = cmd.CreateParameter(); p.ParameterName = "@pid"; p.Value = SelectedProduct.ProductId; cmd.Parameters.Add(p);
                await cmd.ExecuteNonQueryAsync();
            }

            trans.Commit();
            Batches.Clear();
            SelectedProduct.Quantity = 0;
            RecalculateTotals();

            MessageBox.Show("اسٹاک کامیابی سے حذف کر دیا گیا۔", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"حذف کرنے میں نقص: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Clear()
    {
        InputBatchNo = "";
        InputQuantity = 1;
        Batches.Clear();
        RecalculateTotals();
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
