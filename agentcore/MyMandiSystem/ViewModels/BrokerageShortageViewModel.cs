using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Infrastructure;
using System;
using System.Data;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class BrokerageShortageViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    // Header Identifiers
    [ObservableProperty] private long _invoiceId = 0;
    [ObservableProperty] private DateTime _invoiceDate = DateTime.Today;

    // Purchase Information
    [ObservableProperty] private int _brokragePurId = 0;
    [ObservableProperty] private string _vendorId = "";
    [ObservableProperty] private string _vendorName = "";
    [ObservableProperty] private decimal _purchaseRate = 0;

    // Sale Information
    [ObservableProperty] private int _brokrageSaleId = 0;
    [ObservableProperty] private string _customerId = "";
    [ObservableProperty] private string _customerName = "";
    [ObservableProperty] private decimal _saleRate = 0;

    // Shortage & Commission
    [ObservableProperty] private decimal _shortageInKgs = 0;
    [ObservableProperty] private decimal _commissionWeight = 0;
    [ObservableProperty] private decimal _commissionAmount = 0;
    [ObservableProperty] private string _narration = "";

    // Display formatted
    [ObservableProperty] private string _shortageMoundsDisplay = "0-0";

    public BrokerageShortageViewModel(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
        _ = GenerateNextIdAsync();
    }

    public async Task GenerateNextIdAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(MAX(InvoiceID), 0) + 1 FROM BrokrageShortage";
            var nextId = await cmd.ExecuteScalarAsync();
            InvoiceId = Convert.ToInt64(nextId);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to generate next Shortage Invoice ID: {ex.Message}", "ID Generation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task OnPurchaseIdChangedAsync(int purId)
    {
        if (purId <= 0) return;
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 ContractInvoice.ContractPurId, ContractInvoice.PartyID, Parties.PartyName,
                             ISNULL(ContractInvoiceBody.RatePerMound, 0) AS Rate
                FROM ContractInvoice 
                INNER JOIN Parties ON ContractInvoice.PartyID = Parties.PartyID 
                LEFT OUTER JOIN ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId 
                WHERE ContractInvoice.ContractPurID = @purId";
            var p = cmd.CreateParameter();
            p.ParameterName = "@purId";
            p.Value = purId;
            cmd.Parameters.Add(p);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                VendorId = reader["PartyID"]?.ToString() ?? "";
                VendorName = reader["PartyName"]?.ToString() ?? "";
                PurchaseRate = Convert.ToDecimal(reader["Rate"]);
            }
            CalculateShortage();
        }
        catch { }
    }

    public async Task OnSaleIdChangedAsync(int saleId)
    {
        if (saleId <= 0) return;
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT TOP 1 ContractInvoice.ContractPurId, ContractInvoice.PartyID, Parties.PartyName,
                             ISNULL(ContractInvoiceBody.RatePerMound, 0) AS Rate
                FROM ContractInvoice 
                INNER JOIN Parties ON ContractInvoice.PartyID = Parties.PartyID 
                LEFT OUTER JOIN ContractInvoiceBody ON ContractInvoice.ContractPurId = ContractInvoiceBody.ContractPurId 
                WHERE ContractInvoice.ContractPurID = @saleId";
            var p = cmd.CreateParameter();
            p.ParameterName = "@saleId";
            p.Value = saleId;
            cmd.Parameters.Add(p);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                CustomerId = reader["PartyID"]?.ToString() ?? "";
                CustomerName = reader["PartyName"]?.ToString() ?? "";
                SaleRate = Convert.ToDecimal(reader["Rate"]);
            }
            CalculateShortage();
        }
        catch { }
    }

    public void CalculateShortage()
    {
        if (ShortageInKgs > 0)
        {
            CommissionWeight = Math.Round(ShortageInKgs / 40m, 2);
            int mnds = (int)(ShortageInKgs / 40m);
            decimal rem = ShortageInKgs % 40m;
            ShortageMoundsDisplay = $"{mnds}-{rem:F1}";

            decimal rateDiff = SaleRate - PurchaseRate;
            if (rateDiff > 0)
            {
                CommissionAmount = Math.Round(CommissionWeight * rateDiff, 2);
            }
            else
            {
                CommissionAmount = Math.Round(CommissionWeight * SaleRate, 2);
            }
        }
        else
        {
            CommissionWeight = 0;
            CommissionAmount = 0;
            ShortageMoundsDisplay = "0-0";
        }
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (BrokragePurId <= 0)
        {
            MessageBox.Show("Invalid Purchase ID. Please select a valid Purchase ID and try again.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (BrokrageSaleId <= 0)
        {
            MessageBox.Show("Invalid Sale ID. Please select a valid Sale and try again.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (ShortageInKgs <= 0)
        {
            MessageBox.Show("Shortage can not be empty or zero.", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        CalculateShortage();

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // Check if invoice already exists
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var chkCmd = conn.CreateCommand();
            chkCmd.CommandText = "SELECT COUNT(*) FROM BrokrageShortage WHERE InvoiceID = @id";
            var pId = chkCmd.CreateParameter();
            pId.ParameterName = "@id";
            pId.Value = InvoiceId;
            chkCmd.Parameters.Add(pId);

            var count = Convert.ToInt32(await chkCmd.ExecuteScalarAsync());
            if (count > 0)
            {
                await context.Database.ExecuteSqlRawAsync("DELETE FROM BrokrageShortage WHERE InvoiceID = {0}", InvoiceId);
            }

            string insertSql = @"
                INSERT INTO BrokrageShortage 
                (InvoiceID, InvoiceDate, BrokragePurID, BrokrageSaleID, VendorID, CustomerID, 
                 PurchaseRate, SaleRate, ShortageInKgs, CommissionWeight, CommissionAmount, Narration) 
                VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";

            await context.Database.ExecuteSqlRawAsync(insertSql,
                InvoiceId, InvoiceDate, BrokragePurId, BrokrageSaleId, VendorId, CustomerId,
                PurchaseRate, SaleRate, ShortageInKgs, CommissionWeight, CommissionAmount, Narration);

            MessageBox.Show($"Brokerage Shortage Invoice #{InvoiceId} saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            await ClearAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving Brokerage Shortage: {ex.Message}", "Save Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task ClearAsync()
    {
        BrokragePurId = 0;
        VendorId = "";
        VendorName = "";
        PurchaseRate = 0;
        BrokrageSaleId = 0;
        CustomerId = "";
        CustomerName = "";
        SaleRate = 0;
        ShortageInKgs = 0;
        CommissionWeight = 0;
        CommissionAmount = 0;
        ShortageMoundsDisplay = "0-0";
        Narration = "";
        await GenerateNextIdAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (InvoiceId <= 0) return;

        var res = MessageBox.Show($"Are you sure you want to remove Brokerage Shortage #{InvoiceId}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (res != MessageBoxResult.Yes) return;

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Database.ExecuteSqlRawAsync("DELETE FROM BrokrageShortage WHERE InvoiceID = {0}", InvoiceId);

            MessageBox.Show($"Brokerage Shortage #{InvoiceId} deleted successfully!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            await ClearAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting Shortage: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task LoadShortageAsync(long targetInvoiceId)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT BrokrageShortage.*, 
                       ISNULL(P1.PartyName,'') AS VendorName, 
                       ISNULL(P2.PartyName,'') AS CustomerName 
                FROM BrokrageShortage 
                LEFT JOIN Parties P1 ON BrokrageShortage.VendorID = P1.PartyID 
                LEFT JOIN Parties P2 ON BrokrageShortage.CustomerID = P2.PartyID 
                WHERE BrokrageShortage.InvoiceID = @invId";
            var p = cmd.CreateParameter();
            p.ParameterName = "@invId";
            p.Value = targetInvoiceId;
            cmd.Parameters.Add(p);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                InvoiceId = Convert.ToInt64(reader["InvoiceID"]);
                InvoiceDate = Convert.ToDateTime(reader["InvoiceDate"]);
                BrokragePurId = Convert.ToInt32(reader["BrokragePurID"]);
                BrokrageSaleId = Convert.ToInt32(reader["BrokrageSaleID"]);
                VendorId = reader["VendorID"]?.ToString() ?? "";
                VendorName = reader["VendorName"]?.ToString() ?? "";
                CustomerId = reader["CustomerID"]?.ToString() ?? "";
                CustomerName = reader["CustomerName"]?.ToString() ?? "";
                PurchaseRate = Convert.ToDecimal(reader["PurchaseRate"]);
                SaleRate = Convert.ToDecimal(reader["SaleRate"]);
                ShortageInKgs = Convert.ToDecimal(reader["ShortageInKgs"]);
                CommissionWeight = Convert.ToDecimal(reader["CommissionWeight"]);
                CommissionAmount = Convert.ToDecimal(reader["CommissionAmount"]);
                Narration = reader["Narration"]?.ToString() ?? "";

                CalculateShortage();
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load Brokerage Shortage #{targetInvoiceId}: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
