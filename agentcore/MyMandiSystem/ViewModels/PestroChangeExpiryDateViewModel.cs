using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public class PestroStockBatchRowDto : ObservableObject
{
    public string ProductId { get; set; } = "";
    public string ProductName { get; set; } = "";
    public string BatchNo { get; set; } = "";
    
    private DateTime _expiryDate = DateTime.Today;
    public DateTime ExpiryDate { get => _expiryDate; set => SetProperty(ref _expiryDate, value); }

    public int Quantity { get; set; } = 0;
    public decimal Cost { get; set; } = 0;
}

public partial class PestroChangeExpiryDateViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    [ObservableProperty] private ObservableCollection<Product> _availableProducts = new();
    [ObservableProperty] private Product? _selectedProduct;

    [ObservableProperty] private ObservableCollection<PestroStockBatchRowDto> _batches = new();
    [ObservableProperty] private PestroStockBatchRowDto? _selectedBatch;

    // Selected Batch Editing fields
    [ObservableProperty] private string _selectedBatchNo = "";
    [ObservableProperty] private DateTime? _currentExpiryDate;
    [ObservableProperty] private DateTime? _newExpiryDate = DateTime.Today.AddYears(1);

    public Action? RequestClose { get; set; }

    public PestroChangeExpiryDateViewModel(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
        _ = LoadProductsAsync();
    }

    public async Task LoadProductsAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var prods = await context.Products
                .Where(p => p.IsActive)
                .OrderBy(p => p.Name)
                .ToListAsync();

            AvailableProducts = new ObservableCollection<Product>(prods);
            if (AvailableProducts.Count > 0)
            {
                SelectedProduct = AvailableProducts[0];
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadProducts error: {ex.Message}");
        }
    }

    partial void OnSelectedProductChanged(Product? value)
    {
        if (value != null)
        {
            string pCode = !string.IsNullOrWhiteSpace(value.ProductCode) ? value.ProductCode : value.Id.ToString();
            _ = LoadBatchesForProductAsync(pCode, value.Name);
        }
        else
        {
            Batches.Clear();
            ClearSelection();
        }
    }

    public async Task LoadBatchesForProductAsync(string productId, string productName)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT BatchNo, ExpiryDate, Quantity, Cost 
                FROM CurrentStockPestro 
                WHERE ProductId = @pid 
                ORDER BY ExpiryDate ASC";

            var param = cmd.CreateParameter();
            param.ParameterName = "@pid";
            param.Value = productId;
            cmd.Parameters.Add(param);

            using var reader = await cmd.ExecuteReaderAsync();
            var list = new ObservableCollection<PestroStockBatchRowDto>();
            while (await reader.ReadAsync())
            {
                list.Add(new PestroStockBatchRowDto
                {
                    ProductId = productId,
                    ProductName = productName,
                    BatchNo = reader.IsDBNull(0) ? "" : reader.GetString(0),
                    ExpiryDate = reader.IsDBNull(1) ? DateTime.MinValue : reader.GetDateTime(1),
                    Quantity = reader.IsDBNull(2) ? 0 : reader.GetInt32(2),
                    Cost = reader.IsDBNull(3) ? 0 : reader.GetDecimal(3)
                });
            }

            Batches = list;
            ClearSelection();
            if (Batches.Count > 0)
            {
                SelectedBatch = Batches[0];
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"LoadBatches error: {ex.Message}");
        }
    }

    partial void OnSelectedBatchChanged(PestroStockBatchRowDto? value)
    {
        if (value != null)
        {
            SelectedBatchNo = value.BatchNo;
            CurrentExpiryDate = value.ExpiryDate;
            NewExpiryDate = value.ExpiryDate;
        }
        else
        {
            ClearSelection();
        }
    }

    private void ClearSelection()
    {
        SelectedBatchNo = "";
        CurrentExpiryDate = null;
        NewExpiryDate = DateTime.Today.AddYears(1);
    }

    [RelayCommand]
    public async Task UpdateExpiryDateAsync()
    {
        if (SelectedProduct == null || SelectedBatch == null)
        {
            MessageBox.Show("براہ کرم تبدیل کرنے کے لیے بیچ منتخب کریں۔ (Please select a batch)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (NewExpiryDate == null)
        {
            MessageBox.Show("براہ کرم نئی ایکسپائری تاریخ منتخب کریں۔ (Please choose new expiry date)", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var conn = context.Database.GetDbConnection();
            await conn.OpenAsync();
            using var trans = conn.BeginTransaction();

            string pId = SelectedBatch.ProductId;
            string bNo = SelectedBatch.BatchNo;
            DateTime newExp = NewExpiryDate.Value;

            // 1. Update CurrentStockPestro
            using (var cmd1 = conn.CreateCommand())
            {
                cmd1.Transaction = trans;
                cmd1.CommandText = "UPDATE CurrentStockPestro SET ExpiryDate = @exp WHERE ProductId = @pid AND BatchNo = @bno";
                var p1 = cmd1.CreateParameter(); p1.ParameterName = "@exp"; p1.Value = newExp; cmd1.Parameters.Add(p1);
                var p2 = cmd1.CreateParameter(); p2.ParameterName = "@pid"; p2.Value = pId; cmd1.Parameters.Add(p2);
                var p3 = cmd1.CreateParameter(); p3.ParameterName = "@bno"; p3.Value = bNo; cmd1.Parameters.Add(p3);
                await cmd1.ExecuteNonQueryAsync();
            }

            // 2. Update OpeningBatchPestro
            using (var cmd2 = conn.CreateCommand())
            {
                cmd2.Transaction = trans;
                cmd2.CommandText = "UPDATE OpeningBatchPestro SET ExpiryDate = @exp WHERE ProductId = @pid AND BatchNo = @bno";
                var p1 = cmd2.CreateParameter(); p1.ParameterName = "@exp"; p1.Value = newExp; cmd2.Parameters.Add(p1);
                var p2 = cmd2.CreateParameter(); p2.ParameterName = "@pid"; p2.Value = pId; cmd2.Parameters.Add(p2);
                var p3 = cmd2.CreateParameter(); p3.ParameterName = "@bno"; p3.Value = bNo; cmd2.Parameters.Add(p3);
                await cmd2.ExecuteNonQueryAsync();
            }

            // 3. Update PurchasesBatch
            using (var cmd3 = conn.CreateCommand())
            {
                cmd3.Transaction = trans;
                cmd3.CommandText = "UPDATE PurchasesBatch SET ExpiryDate = @exp WHERE ProductId = @pid AND BatchNo = @bno";
                var p1 = cmd3.CreateParameter(); p1.ParameterName = "@exp"; p1.Value = newExp; cmd3.Parameters.Add(p1);
                var p2 = cmd3.CreateParameter(); p2.ParameterName = "@pid"; p2.Value = pId; cmd3.Parameters.Add(p2);
                var p3 = cmd3.CreateParameter(); p3.ParameterName = "@bno"; p3.Value = bNo; cmd3.Parameters.Add(p3);
                await cmd3.ExecuteNonQueryAsync();
            }

            // 4. Update SaleReturnsBatch
            using (var cmd4 = conn.CreateCommand())
            {
                cmd4.Transaction = trans;
                cmd4.CommandText = "UPDATE SaleReturnsBatch SET ExpiryDate = @exp WHERE ProductId = @pid AND BatchNo = @bno";
                var p1 = cmd4.CreateParameter(); p1.ParameterName = "@exp"; p1.Value = newExp; cmd4.Parameters.Add(p1);
                var p2 = cmd4.CreateParameter(); p2.ParameterName = "@pid"; p2.Value = pId; cmd4.Parameters.Add(p2);
                var p3 = cmd4.CreateParameter(); p3.ParameterName = "@bno"; p3.Value = bNo; cmd4.Parameters.Add(p3);
                await cmd4.ExecuteNonQueryAsync();
            }

            trans.Commit();

            SelectedBatch.ExpiryDate = newExp;
            CurrentExpiryDate = newExp;

            MessageBox.Show($"بیچ '{bNo}' کی ایکسپائری تاریخ کامیابی سے تبدیل کر دی گئی۔\n(Expiry Date updated successfully to {newExp:dd/MM/yyyy})", "Updated", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"اپڈیٹ کرنے میں نقص: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private void Close()
    {
        RequestClose?.Invoke();
    }
}
