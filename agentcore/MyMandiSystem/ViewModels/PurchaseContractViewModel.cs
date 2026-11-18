using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Infrastructure;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public class ContractItemDto : ObservableObject
{
    public int SerialNo { get; set; } = 1;
    public string ProductId { get; set; } = "";
    public string ProductName { get; set; } = "";
    public int TruckQty { get; set; } = 1;
    public int BagsQty { get; set; } = 0;
    public decimal WeightPerBag { get; set; } = 50;
    public decimal RatePerMound { get; set; } = 0;
    public string RatePerBag { get; set; } = "Rate/Mound";
    public int Bardana { get; set; } = 0;
    public decimal TotalKgs { get; set; } = 0;
    public decimal TotalValue { get; set; } = 0;
    public short MoundStd { get; set; } = 40;
    public decimal ExtraWeight { get; set; } = 0;

    public string InMounds => MoundStd > 0 ? $"{(int)(TotalKgs / MoundStd)}-{(TotalKgs % MoundStd):F1}" : "0-0";
}

public partial class PurchaseContractViewModel : ObservableObject
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly IPartyService _partyService;
    private readonly IProductService _productService;

    // Header Identifiers
    [ObservableProperty] private int _contractId = 0;
    [ObservableProperty] private int _purContractId = 0;
    [ObservableProperty] private DateTime _cDate = DateTime.Today;

    // Vendor Information
    [ObservableProperty] private string _partyId = "";
    [ObservableProperty] private string _partyName = "";
    [ObservableProperty] private string _town = "";
    [ObservableProperty] private string _sector = "";
    [ObservableProperty] private string _address = "";
    [ObservableProperty] private string _phone = "";

    // Broker Information
    [ObservableProperty] private string _brokerId = "";
    [ObservableProperty] private string _brokerName = "";

    // Header Flags & Expenses
    [ObservableProperty] private decimal _advance = 0;
    [ObservableProperty] private decimal _vehicalCharges = 0;
    [ObservableProperty] private decimal _otherExpence = 0;
    [ObservableProperty] private decimal _brokerCommission = 0;
    [ObservableProperty] private decimal _totalGross = 0;
    [ObservableProperty] private bool _isReached = false;
    [ObservableProperty] private bool _isCompleted = false;
    [ObservableProperty] private string _narration = "";

    // Line Item Inputs
    [ObservableProperty] private string _currentProductId = "";
    [ObservableProperty] private string _currentProductName = "";
    [ObservableProperty] private int _currentTruckQty = 1;
    [ObservableProperty] private int _currentBagsQty = 0;
    [ObservableProperty] private decimal _currentWeightPerBag = 50;
    [ObservableProperty] private decimal _currentRatePerMound = 0;
    [ObservableProperty] private string _currentRateType = "Rate/Mound";
    [ObservableProperty] private int _currentBardana = 0;
    [ObservableProperty] private decimal _currentTotalKgs = 0;
    [ObservableProperty] private decimal _currentTotalValue = 0;
    [ObservableProperty] private short _currentMoundStd = 40;
    [ObservableProperty] private decimal _currentExtraWeight = 0;

    // Totals Display
    [ObservableProperty] private int _totalTrucks = 0;
    [ObservableProperty] private int _totalBags = 0;
    [ObservableProperty] private decimal _totalKgs = 0;
    [ObservableProperty] private string _totalMoundsDisplay = "0-0";
    [ObservableProperty] private decimal _totalContractAmount = 0;

    public ObservableCollection<ContractItemDto> Items { get; } = new();
    [ObservableProperty] private ContractItemDto? _selectedItem;

    public PurchaseContractViewModel(
        IDbContextFactory<AppDbContext> contextFactory,
        IPartyService partyService,
        IProductService productService)
    {
        _contextFactory = contextFactory;
        _partyService = partyService;
        _productService = productService;

        _ = InitializeAsync();
    }

    private async Task InitializeAsync()
    {
        await GenerateNextIdAsync();
    }

    public async Task GenerateNextIdAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(MAX(ContractID),0)+1 FROM Contract";
            var nextCId = await cmd.ExecuteScalarAsync();
            ContractId = Convert.ToInt32(nextCId);

            cmd.CommandText = "SELECT ISNULL(MAX(PurContractID),0)+1 FROM Contract WHERE IsSaleContract = 0";
            var nextPurId = await cmd.ExecuteScalarAsync();
            PurContractId = Convert.ToInt32(nextPurId);
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to generate next Purchase Contract ID: {ex.Message}", "ID Generation Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    public async Task OnPartyIdChangedAsync(string partyId)
    {
        if (string.IsNullOrWhiteSpace(partyId)) return;
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Parties.PartyID, Parties.PartyName, Parties.Address, Parties.Phone1,
                       ISNULL(Sectors.SectorName,'') AS SectorName, ISNULL(Towns.TownName,'') AS TownName
                FROM Parties 
                LEFT OUTER JOIN Sectors ON Parties.SectorID = Sectors.SectorID
                LEFT OUTER JOIN Towns ON Sectors.TownID = Towns.TownID
                WHERE Parties.PartyID = @pId AND Parties.IsVendor = 1";
            var pParam = cmd.CreateParameter();
            pParam.ParameterName = "@pId";
            pParam.Value = partyId;
            cmd.Parameters.Add(pParam);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                PartyName = reader["PartyName"]?.ToString() ?? "";
                Address = reader["Address"]?.ToString() ?? "";
                Phone = reader["Phone1"]?.ToString() ?? "";
                Sector = reader["SectorName"]?.ToString() ?? "";
                Town = reader["TownName"]?.ToString() ?? "";
            }
        }
        catch { }
    }

    public async Task OnBrokerIdChangedAsync(string brokerId)
    {
        if (string.IsNullOrWhiteSpace(brokerId)) return;
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT PartyName FROM Parties WHERE PartyID = @pId AND IsBroker = 1";
            var pParam = cmd.CreateParameter();
            pParam.ParameterName = "@pId";
            pParam.Value = brokerId;
            cmd.Parameters.Add(pParam);

            var res = await cmd.ExecuteScalarAsync();
            if (res != null)
            {
                BrokerName = res.ToString() ?? "";
            }
        }
        catch { }
    }

    public async Task OnProductIdChangedAsync(string productId)
    {
        if (string.IsNullOrWhiteSpace(productId)) return;
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ProductName, ISNULL(Weight, 50) AS Weight, ISNULL(Packing, 40) AS Packing FROM Products WHERE ProductID = @prodId";
            var pParam = cmd.CreateParameter();
            pParam.ParameterName = "@prodId";
            pParam.Value = productId;
            cmd.Parameters.Add(pParam);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                CurrentProductName = reader["ProductName"]?.ToString() ?? "";
                if (decimal.TryParse(reader["Weight"]?.ToString(), out var wt) && wt > 0)
                {
                    CurrentWeightPerBag = wt;
                }
                if (short.TryParse(reader["Packing"]?.ToString(), out var mStd) && mStd > 0)
                {
                    CurrentMoundStd = mStd;
                }
            }
            CalculateCurrentLine();
        }
        catch { }
    }

    public void CalculateCurrentLine()
    {
        CurrentTotalKgs = (CurrentBagsQty * CurrentWeightPerBag) + CurrentExtraWeight;
        if (CurrentMoundStd > 0)
        {
            decimal mounds = CurrentTotalKgs / CurrentMoundStd;
            CurrentTotalValue = Math.Round(mounds * CurrentRatePerMound, 2);
        }
        else
        {
            CurrentTotalValue = 0;
        }
    }

    [RelayCommand]
    private void AddItem()
    {
        if (string.IsNullOrWhiteSpace(CurrentProductId))
        {
            MessageBox.Show("Please select a Product!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (CurrentBagsQty <= 0 && CurrentTotalKgs <= 0)
        {
            MessageBox.Show("Please enter Bags Quantity or Weight!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        CalculateCurrentLine();

        var item = new ContractItemDto
        {
            SerialNo = Items.Count + 1,
            ProductId = CurrentProductId,
            ProductName = CurrentProductName,
            TruckQty = CurrentTruckQty,
            BagsQty = CurrentBagsQty,
            WeightPerBag = CurrentWeightPerBag,
            RatePerMound = CurrentRatePerMound,
            RatePerBag = CurrentRateType,
            Bardana = CurrentBardana,
            TotalKgs = CurrentTotalKgs,
            TotalValue = CurrentTotalValue,
            MoundStd = CurrentMoundStd,
            ExtraWeight = CurrentExtraWeight
        };

        Items.Add(item);
        RecalculateTotals();

        // Reset line input
        CurrentProductId = "";
        CurrentProductName = "";
        CurrentTruckQty = 1;
        CurrentBagsQty = 0;
        CurrentTotalKgs = 0;
        CurrentRatePerMound = 0;
        CurrentTotalValue = 0;
        CurrentExtraWeight = 0;
    }

    [RelayCommand]
    private void RemoveItem()
    {
        if (SelectedItem != null)
        {
            Items.Remove(SelectedItem);
            RecalculateTotals();
        }
    }

    public void RecalculateTotals()
    {
        TotalTrucks = Items.Sum(i => i.TruckQty);
        TotalBags = Items.Sum(i => i.BagsQty);
        TotalKgs = Items.Sum(i => i.TotalKgs);
        TotalContractAmount = Items.Sum(i => i.TotalValue);

        if (CurrentMoundStd > 0)
        {
            int mnds = (int)(TotalKgs / CurrentMoundStd);
            decimal rem = TotalKgs % CurrentMoundStd;
            TotalMoundsDisplay = $"{mnds}-{rem:F1}";
        }

        TotalGross = TotalContractAmount + VehicalCharges + OtherExpence - Advance;
    }

    [RelayCommand]
    private async Task SaveAsync()
    {
        if (string.IsNullOrWhiteSpace(PartyName))
        {
            MessageBox.Show("Vendor (Party) is required!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        if (Items.Count == 0)
        {
            MessageBox.Show("Please add at least one line item to the Contract!", "Validation", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();

            // AgenSoft Re-save pattern: clean existing records
            await context.Database.ExecuteSqlRawAsync("DELETE FROM ContractBody WHERE ContractID = {0}", ContractId);
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Contract WHERE ContractID = {0}", ContractId);

            string insertHdr = @"
                INSERT INTO Contract 
                (ContractID, CDate, PartyID, BrokerID, Refree, VehicalCharges, OtherExpence, Advance, 
                 IsReached, BrokerCommission, TotalGross, IsSaleContract, Narration, SaleContractId, 
                 PurContractId, IsCompleted, BranchID, IDBranch, IDBranchPur, IDBranchSale) 
                VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11}, {12}, {13}, {14}, {15}, {16}, {17}, {18}, {19})";

            await context.Database.ExecuteSqlRawAsync(insertHdr,
                ContractId, CDate, PartyId, BrokerId, BrokerName, VehicalCharges, OtherExpence, Advance,
                IsReached ? 1 : 0, BrokerCommission, TotalGross, 0, Narration, 0, PurContractId,
                IsCompleted ? 1 : 0, 1, ContractId, PurContractId, 0);

            foreach (var item in Items)
            {
                string insertBody = @"
                    INSERT INTO ContractBody 
                    (ContractID, ProductID, TruckQty, BagsQty, WeightPerBag, RatePerBag, RatePerMound, 
                     Bardana, TotalValue, TotalKgs, MoundStd, ExtraWeight) 
                    VALUES ({0}, {1}, {2}, {3}, {4}, {5}, {6}, {7}, {8}, {9}, {10}, {11})";

                await context.Database.ExecuteSqlRawAsync(insertBody,
                    ContractId, item.ProductId, item.TruckQty, item.BagsQty, item.WeightPerBag,
                    item.RatePerBag, item.RatePerMound, item.Bardana, item.TotalValue, item.TotalKgs,
                    item.MoundStd, item.ExtraWeight);
            }

            MessageBox.Show($"Purchase Contract #{ContractId} (Pur#: {PurContractId}) saved successfully!", "Saved", MessageBoxButton.OK, MessageBoxImage.Information);
            await ClearAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error saving Purchase Contract: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task ClearAsync()
    {
        PartyId = "";
        PartyName = "";
        Town = "";
        Sector = "";
        Address = "";
        Phone = "";
        BrokerId = "";
        BrokerName = "";
        Advance = 0;
        VehicalCharges = 0;
        OtherExpence = 0;
        BrokerCommission = 0;
        TotalGross = 0;
        IsReached = false;
        IsCompleted = false;
        Narration = "";
        Items.Clear();
        RecalculateTotals();
        await GenerateNextIdAsync();
    }

    [RelayCommand]
    private async Task DeleteAsync()
    {
        if (ContractId <= 0) return;

        var res = MessageBox.Show($"Are you sure you want to remove Purchase Contract #{ContractId}?", "Confirmation", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (res != MessageBoxResult.Yes) return;

        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            await context.Database.ExecuteSqlRawAsync("DELETE FROM ContractBody WHERE ContractID = {0}", ContractId);
            await context.Database.ExecuteSqlRawAsync("DELETE FROM Contract WHERE ContractID = {0}", ContractId);

            MessageBox.Show($"Purchase Contract #{ContractId} deleted successfully!", "Deleted", MessageBoxButton.OK, MessageBoxImage.Information);
            await ClearAsync();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Error deleting Contract: {ex.Message}", "Delete Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    [RelayCommand]
    private async Task LoadContractAsync(int targetContractId)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            using var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                SELECT Contract.*, Parties.PartyName, Parties.Address, Parties.Phone1,
                       (SELECT PartyName FROM Parties P WHERE P.PartyID = Contract.BrokerID) AS BrokerName,
                       ISNULL(Sectors.SectorName,'') AS SectorName, ISNULL(Towns.TownName,'') AS TownName
                FROM Contract 
                INNER JOIN Parties ON Contract.PartyID = Parties.PartyID
                LEFT OUTER JOIN Sectors ON Parties.SectorID = Sectors.SectorID
                LEFT OUTER JOIN Towns ON Sectors.TownID = Towns.TownID
                WHERE Contract.ContractID = @cId";
            var p = cmd.CreateParameter();
            p.ParameterName = "@cId";
            p.Value = targetContractId;
            cmd.Parameters.Add(p);

            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                ContractId = Convert.ToInt32(reader["ContractID"]);
                PurContractId = Convert.ToInt32(reader["PurContractId"]);
                CDate = Convert.ToDateTime(reader["CDate"]);
                PartyId = reader["PartyID"]?.ToString() ?? "";
                PartyName = reader["PartyName"]?.ToString() ?? "";
                Address = reader["Address"]?.ToString() ?? "";
                Phone = reader["Phone1"]?.ToString() ?? "";
                Sector = reader["SectorName"]?.ToString() ?? "";
                Town = reader["TownName"]?.ToString() ?? "";
                BrokerId = reader["BrokerID"]?.ToString() ?? "";
                BrokerName = reader["BrokerName"]?.ToString() ?? "";
                Advance = Convert.ToDecimal(reader["Advance"]);
                VehicalCharges = Convert.ToDecimal(reader["VehicalCharges"]);
                OtherExpence = Convert.ToDecimal(reader["OtherExpence"]);
                BrokerCommission = Convert.ToDecimal(reader["BrokerCommission"]);
                TotalGross = Convert.ToDecimal(reader["TotalGross"]);
                IsReached = Convert.ToBoolean(reader["IsReached"]);
                IsCompleted = Convert.ToBoolean(reader["IsCompleted"]);
                Narration = reader["Narration"]?.ToString() ?? "";
            }
            reader.Close();

            // Load line items
            Items.Clear();
            cmd.CommandText = @"
                SELECT ContractBody.*, Products.ProductName 
                FROM ContractBody 
                INNER JOIN Products ON ContractBody.ProductID = Products.ProductID 
                WHERE ContractBody.ContractID = @cId";

            using var itemReader = await cmd.ExecuteReaderAsync();
            int sNo = 1;
            while (await itemReader.ReadAsync())
            {
                Items.Add(new ContractItemDto
                {
                    SerialNo = sNo++,
                    ProductId = itemReader["ProductID"]?.ToString() ?? "",
                    ProductName = itemReader["ProductName"]?.ToString() ?? "",
                    TruckQty = Convert.ToInt32(itemReader["TruckQty"]),
                    BagsQty = Convert.ToInt32(itemReader["BagsQty"]),
                    WeightPerBag = Convert.ToDecimal(itemReader["WeightPerBag"]),
                    RatePerMound = Convert.ToDecimal(itemReader["RatePerMound"]),
                    RatePerBag = itemReader["RatePerBag"]?.ToString() ?? "Rate/Mound",
                    Bardana = Convert.ToInt32(itemReader["Bardana"]),
                    TotalValue = Convert.ToDecimal(itemReader["TotalValue"]),
                    TotalKgs = Convert.ToDecimal(itemReader["TotalKgs"]),
                    MoundStd = Convert.ToInt16(itemReader["MoundStd"]),
                    ExtraWeight = Convert.ToDecimal(itemReader["ExtraWeight"])
                });
            }

            RecalculateTotals();
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Failed to load Purchase Contract #{targetContractId}: {ex.Message}", "Load Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }
}
