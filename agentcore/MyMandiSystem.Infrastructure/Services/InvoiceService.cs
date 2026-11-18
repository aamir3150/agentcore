using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;
    private readonly IStockService _stockService;

    public InvoiceService(
        IDbContextFactory<AppDbContext> contextFactory,
        ISystemService systemService,
        IStockService stockService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
        _stockService = stockService;
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.Invoices
            .Include(i => i.Details).ThenInclude(d => d.Product).ThenInclude(p => p.Unit)
            .Include(i => i.Party).ThenInclude(p => p.Account)
            .Include(i => i.CropSeason)
            .Include(i => i.FinancialYear)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice != null)
        {
            await LoadLegacyMetadataAsync(context, invoice);
        }

        return invoice;
    }

    public async Task<string> GetNextInvoiceNoAsync(InvoiceType type, int financialYearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = type switch
            {
                InvoiceType.GenPurchase => "SELECT ISNULL(MAX(PurchaseID), 0) + 1 FROM Purchase",
                InvoiceType.GenSale => "SELECT ISNULL(MAX(SaleID), 0) + 1 FROM Sales",
                InvoiceType.GenPurchaseReturn => "SELECT ISNULL(MAX(PurReturnID), 0) + 1 FROM PurReturns",
                InvoiceType.GenSaleReturn => "SELECT ISNULL(MAX(SaleReturnID), 0) + 1 FROM SaleReturns",
                _ => "SELECT ISNULL(MAX(CAST(InvoiceNo AS INT)), 0) + 1 FROM Invoices"
            };

            var scalar = await cmd.ExecuteScalarAsync();
            if (scalar != null && scalar != DBNull.Value)
            {
                int nextId = Convert.ToInt32(scalar);
                if (nextId > 0)
                    return nextId.ToString();
            }
        }
        catch
        {
            // Fallback to EF
        }

        DocumentType docType = type switch
        {
            InvoiceType.GenPurchase => DocumentType.GenPurchaseInvoice,
            InvoiceType.GenSale => DocumentType.GenSaleInvoice,
            _ => DocumentType.GenSaleInvoice
        };

        return await _systemService.GetNextDocumentNoAsync(docType, financialYearId);
    }

    public async Task<decimal> GetProductStockAsync(string productCode)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(SUM(Qty), 0) FROM CurrentStock WHERE ProductID = @prodCode";
            var p = cmd.CreateParameter();
            p.ParameterName = "@prodCode";
            p.Value = productCode;
            cmd.Parameters.Add(p);

            var result = await cmd.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
                return Convert.ToDecimal(result);
        }
        catch
        {
            // Fallback
        }

        return 0;
    }

    public async Task<decimal> GetProductAvgCostAsync(string productCode)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(Cost, 0) FROM CurrentStock WHERE ProductID = @prodCode";
            var p = cmd.CreateParameter();
            p.ParameterName = "@prodCode";
            p.Value = productCode;
            cmd.Parameters.Add(p);

            var result = await cmd.ExecuteScalarAsync();
            if (result != null && result != DBNull.Value)
                return Convert.ToDecimal(result);
        }
        catch
        {
            // Fallback
        }

        return 0;
    }

    public async Task SaveInvoiceAsync(Invoice invoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        if (invoice.Id == 0)
        {
            context.Invoices.Add(invoice);
        }
        else
        {
            var existing = await context.Invoices
                .Include(i => i.Details)
                .FirstOrDefaultAsync(i => i.Id == invoice.Id);

            if (existing != null)
            {
                existing.InvoiceNo = invoice.InvoiceNo;
                existing.InvoiceDate = invoice.InvoiceDate;
                existing.PartyId = invoice.PartyId;
                existing.FinancialYearId = invoice.FinancialYearId;
                existing.CropSeasonId = invoice.CropSeasonId;
                existing.SubTotal = invoice.SubTotal;
                existing.DiscountAmount = invoice.DiscountAmount;
                existing.TaxAmount = invoice.TaxAmount;
                existing.FreightCharges = invoice.FreightCharges;
                existing.LaborCharges = invoice.LaborCharges;
                existing.MarketCommitteeFee = invoice.MarketCommitteeFee;
                existing.NetAmount = invoice.NetAmount;

                context.InvoiceDetails.RemoveRange(existing.Details);
                foreach (var d in invoice.Details)
                {
                    existing.Details.Add(new InvoiceDetail
                    {
                        LineNo = d.LineNo,
                        ProductId = d.ProductId,
                        Quantity = d.Quantity,
                        Rate = d.Rate,
                        Amount = d.Amount,
                        DiscountAmount = d.DiscountAmount
                    });
                }
            }
        }

        await context.SaveChangesAsync();

        // Row-by-row sync to AgenSoft legacy tables
        await SyncToLegacyDatabaseAsync(context, invoice);
    }

    public async Task SaveMultiInvoiceAsync(Invoice purchaseInvoice, Invoice saleInvoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        purchaseInvoice.IsMulti = true;
        saleInvoice.IsMulti = true;

        if (purchaseInvoice.Id == 0)
            context.Invoices.Add(purchaseInvoice);
        else
            context.Invoices.Update(purchaseInvoice);

        if (saleInvoice.Id == 0)
            context.Invoices.Add(saleInvoice);
        else
            context.Invoices.Update(saleInvoice);

        await context.SaveChangesAsync();

        purchaseInvoice.LinkedInvoiceId = saleInvoice.Id;
        purchaseInvoice.AssociatedSaleId = int.TryParse(saleInvoice.InvoiceNo, out int sId) ? sId : null;

        saleInvoice.LinkedInvoiceId = purchaseInvoice.Id;
        saleInvoice.AssociatedPurchaseId = int.TryParse(purchaseInvoice.InvoiceNo, out int pId) ? pId : null;

        await context.SaveChangesAsync();

        // Sync both to legacy tables
        await SyncToLegacyDatabaseAsync(context, purchaseInvoice);
        await SyncToLegacyDatabaseAsync(context, saleInvoice);
    }

    public async Task DeleteInvoiceAsync(int invoiceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.Invoices
            .Include(i => i.Details)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);

        if (invoice == null) return;

        // Delete from legacy tables (row-by-row to trigger stock adjustments)
        await DeleteFromLegacyAsync(context, invoice);

        context.InvoiceDetails.RemoveRange(invoice.Details);
        context.Invoices.Remove(invoice);
        await context.SaveChangesAsync();
    }

    public async Task<IEnumerable<Invoice>> GetInvoicesByPartyAsync(int partyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Invoices
            .Include(i => i.Details)
            .Where(i => i.PartyId == partyId)
            .ToListAsync();
    }

    public async Task<IEnumerable<Invoice>> SearchInvoicesAsync(InvoiceType? type, string? searchTerm, DateTime? fromDate, DateTime? toDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Invoices
            .Include(i => i.Party)
            .Include(i => i.CropSeason)
            .AsQueryable();

        if (type.HasValue)
            query = query.Where(i => i.InvoiceType == type.Value);

        if (fromDate.HasValue)
            query = query.Where(i => i.InvoiceDate >= fromDate.Value.Date);

        if (toDate.HasValue)
            query = query.Where(i => i.InvoiceDate <= toDate.Value.Date.AddDays(1).AddTicks(-1));

        if (!string.IsNullOrWhiteSpace(searchTerm))
        {
            string term = searchTerm.Trim().ToLower();
            query = query.Where(i => i.InvoiceNo.ToLower().Contains(term) ||
                                     i.Party.Name.ToLower().Contains(term) ||
                                     i.Party.PartyNo.ToLower().Contains(term));
        }

        return await query.OrderByDescending(i => i.InvoiceDate).ThenByDescending(i => i.Id).Take(150).ToListAsync();
    }

    public async Task PostInvoiceToGLAsync(int invoiceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.Invoices
            .Include(i => i.Details)
            .Include(i => i.Party)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);

        if (invoice == null || invoice.IsPosted) return;

        var salesAccount = await GetOrCreateSystemAccount(context, "General Sales", AccountType.Revenue);
        var purchaseAccount = await GetOrCreateSystemAccount(context, "General Purchases", AccountType.Expense);
        var taxAccount = await GetOrCreateSystemAccount(context, "Tax Payable", AccountType.Liability);
        var freightAccount = await GetOrCreateSystemAccount(context, "Freight Expense", AccountType.Expense);
        var commAccount = await GetOrCreateSystemAccount(context, "General Commission", AccountType.Revenue);
        var discountAccount = await GetOrCreateSystemAccount(context, "Discount Allowed/Received", AccountType.Expense);
        var cashAccount = await GetOrCreateSystemAccount(context, "Cash in Hand", AccountType.Asset);

        bool isPurchaseOrReturn = (invoice.InvoiceType == InvoiceType.GenPurchase || invoice.InvoiceType == InvoiceType.GenSaleReturn);

        // 1. Party Ledger Entry
        var partyEntry = new GeneralLedger
        {
            TransactionDate = invoice.InvoiceDate,
            AccountId = invoice.Party.AccountId,
            Narration = $"{invoice.InvoiceType} No: {invoice.InvoiceNo}",
            SourceType = SourceType.GenInvoice,
            SourceId = invoice.Id,
            FinancialYearId = invoice.FinancialYearId,
            CropSeasonId = invoice.CropSeasonId
        };

        if (isPurchaseOrReturn)
            partyEntry.Credit = invoice.NetAmount;
        else
            partyEntry.Debit = invoice.NetAmount;

        context.GeneralLedger.Add(partyEntry);

        // 2. Sales / Purchase Base Entry (SubTotal)
        var tradeEntry = new GeneralLedger
        {
            TransactionDate = invoice.InvoiceDate,
            AccountId = isPurchaseOrReturn ? purchaseAccount.Id : salesAccount.Id,
            Narration = $"Goods {(isPurchaseOrReturn ? "Purchased" : "Sold")} on Inv #{invoice.InvoiceNo}",
            SourceType = SourceType.GenInvoice,
            SourceId = invoice.Id,
            FinancialYearId = invoice.FinancialYearId,
            CropSeasonId = invoice.CropSeasonId
        };

        if (isPurchaseOrReturn)
            tradeEntry.Debit = invoice.SubTotal;
        else
            tradeEntry.Credit = invoice.SubTotal;

        context.GeneralLedger.Add(tradeEntry);

        // 3. Tax Entry
        if (invoice.TaxAmount > 0)
        {
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = taxAccount.Id,
                Debit = isPurchaseOrReturn ? invoice.TaxAmount : 0,
                Credit = isPurchaseOrReturn ? 0 : invoice.TaxAmount,
                Narration = $"Sales Tax on Inv #{invoice.InvoiceNo}",
                SourceType = SourceType.GenInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId
            });
        }

        // 4. Freight Charges Entry
        if (invoice.FreightCharges > 0)
        {
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = freightAccount.Id,
                Debit = isPurchaseOrReturn ? invoice.FreightCharges : 0,
                Credit = isPurchaseOrReturn ? 0 : invoice.FreightCharges,
                Narration = $"Freight on Inv #{invoice.InvoiceNo}",
                SourceType = SourceType.GenInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId
            });
        }

        // 5. Discount Entry
        if (invoice.DiscountAmount > 0)
        {
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = discountAccount.Id,
                Debit = isPurchaseOrReturn ? 0 : invoice.DiscountAmount,
                Credit = isPurchaseOrReturn ? invoice.DiscountAmount : 0,
                Narration = $"Discount on Inv #{invoice.InvoiceNo}",
                SourceType = SourceType.GenInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId
            });
        }

        // 6. Cash Paid / Received Entry at time of invoice
        if (invoice.PaidAmount > 0 && isPurchaseOrReturn)
        {
            // Cash paid to vendor
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = invoice.Party.AccountId,
                Debit = invoice.PaidAmount,
                Narration = $"Cash Paid on Inv #{invoice.InvoiceNo}",
                SourceType = SourceType.GenInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId
            });

            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = cashAccount.Id,
                Credit = invoice.PaidAmount,
                Narration = $"Cash Payment for Inv #{invoice.InvoiceNo}",
                SourceType = SourceType.GenInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId
            });
        }
        else if (invoice.ReceivedAmount > 0 && !isPurchaseOrReturn)
        {
            // Cash received from customer
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = cashAccount.Id,
                Debit = invoice.ReceivedAmount,
                Narration = $"Cash Received on Inv #{invoice.InvoiceNo}",
                SourceType = SourceType.GenInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId
            });

            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = invoice.Party.AccountId,
                Credit = invoice.ReceivedAmount,
                Narration = $"Cash Received for Inv #{invoice.InvoiceNo}",
                SourceType = SourceType.GenInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId
            });
        }

        invoice.IsPosted = true;
        invoice.Status = InvoiceStatus.Posted;
        invoice.PostedAt = DateTime.Now;

        await context.SaveChangesAsync();

        // AgenSoft: Update IsPosted = 1 in Sales or Purchase table so it is recognized in VuAccVouchersOthers
        try
        {
            int legId = int.TryParse(invoice.InvoiceNo, out int pInv) ? pInv : invoice.Id;
            if (isPurchaseOrReturn)
            {
                await context.Database.ExecuteSqlRawAsync("IF OBJECT_ID('Purchase', 'U') IS NOT NULL UPDATE Purchase SET IsPosted = 1 WHERE PurchaseID = {0}", legId);
            }
            else
            {
                await context.Database.ExecuteSqlRawAsync("IF OBJECT_ID('Sales', 'U') IS NOT NULL UPDATE Sales SET IsPosted = 1 WHERE SaleID = {0}", legId);
            }
        }
        catch { }
    }

    private async Task SyncToLegacyDatabaseAsync(AppDbContext context, Invoice invoice)
    {
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            int legacyId = int.TryParse(invoice.InvoiceNo, out int parsedId) ? parsedId : invoice.Id;

            // Load Party code and Season Name
            string partyCode = (await context.Parties.FirstOrDefaultAsync(p => p.Id == invoice.PartyId))?.PartyNo ?? invoice.PartyId.ToString();
            string cropSeason = "2025";
            try
            {
                cropSeason = (await context.CropSeasons.FirstOrDefaultAsync(s => s.Id == invoice.CropSeasonId))?.Name ?? "2025";
            }
            catch { }
            if (cropSeason.Length > 5) cropSeason = cropSeason.Substring(0, 5);

            // Load Product code map (AgenSoft requires short ProductID <= 5 chars, matching SalesBody/PurchaseBody)
            var prodIds = invoice.Details.Select(d => d.ProductId).Distinct().ToList();
            var prodMap = await context.Products
                .Where(p => prodIds.Contains(p.Id))
                .ToDictionaryAsync(p => p.Id, p => p.ProductCode.Length <= 5 ? p.ProductCode : p.Id.ToString());

            switch (invoice.InvoiceType)
            {
                case InvoiceType.GenPurchase:
                    await SyncPurchaseToLegacyAsync(conn, invoice, legacyId, partyCode, cropSeason, prodMap);
                    break;
                case InvoiceType.GenSale:
                    await SyncSaleToLegacyAsync(conn, invoice, legacyId, partyCode, cropSeason, prodMap);
                    break;
                case InvoiceType.GenPurchaseReturn:
                    await SyncPurchaseReturnToLegacyAsync(conn, invoice, legacyId, partyCode, cropSeason, prodMap);
                    break;
                case InvoiceType.GenSaleReturn:
                    await SyncSaleReturnToLegacyAsync(conn, invoice, legacyId, partyCode, cropSeason, prodMap);
                    break;
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InvoiceService SyncToLegacy] {ex.Message}");
        }
    }

    private async Task SyncPurchaseToLegacyAsync(DbConnection conn, Invoice inv, int legacyId, string vendorId, string cropSeason, Dictionary<int, string> prodMap)
    {
        // 1. Check or Insert Purchase Header
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
                IF EXISTS (SELECT 1 FROM Purchase WHERE PurchaseID = @pId)
                BEGIN
                    UPDATE Purchase SET 
                        PurchaseDate = @pDate,
                        VendorID = @vId,
                        BillNo = @billNo,
                        BillDate = @billDate,
                        ExpPlus = @expPlus,
                        ExpMinus = @expMinus,
                        ComRatio = @comRatio,
                        PaidAmount = @paidAmt,
                        Narration = @narration,
                        SpecialDiscount = @specDisc,
                        SaleID = @saleId,
                        NarrationUrdu = @narrationUrdu,
                        IsMulti = @isMulti,
                        Freight = @freight,
                        VehicleNo = @vehNo,
                        CropSeason = @season
                    WHERE PurchaseID = @pId
                END
                ELSE
                BEGIN
                    INSERT INTO Purchase (PurchaseID, PurchaseDate, VendorID, BillNo, BillDate, ExpPlus, ExpMinus, ComRatio, PaidAmount, 
                                          Narration, IsPosted, SpecialDiscount, SaleID, NarrationUrdu, IsMulti, Freight, VehicleNo, CropSeason)
                    VALUES (@pId, @pDate, @vId, @billNo, @billDate, @expPlus, @expMinus, @comRatio, @paidAmt, 
                            @narration, 0, @specDisc, @saleId, @narrationUrdu, @isMulti, @freight, @vehNo, @season)
                END";

            AddParam(cmd, "@pId", legacyId);
            AddParam(cmd, "@pDate", inv.InvoiceDate);
            AddParam(cmd, "@vId", vendorId);
            AddParam(cmd, "@billNo", (object?)inv.BillNo ?? DBNull.Value);
            AddParam(cmd, "@billDate", (object?)inv.BillDate ?? DBNull.Value);
            AddParam(cmd, "@expPlus", inv.ExpPlus);
            AddParam(cmd, "@expMinus", inv.ExpMinus);
            AddParam(cmd, "@comRatio", inv.ComRatio);
            AddParam(cmd, "@paidAmt", inv.PaidAmount);
            AddParam(cmd, "@narration", (object?)inv.Narration ?? DBNull.Value);
            AddParam(cmd, "@specDisc", inv.SpecialDiscount);
            AddParam(cmd, "@saleId", (object?)inv.AssociatedSaleId ?? DBNull.Value);
            AddParam(cmd, "@narrationUrdu", (object?)inv.NarrationUrdu ?? DBNull.Value);
            AddParam(cmd, "@isMulti", inv.IsMulti);
            AddParam(cmd, "@freight", inv.FreightCharges);
            AddParam(cmd, "@vehNo", (object?)inv.VehicleNo ?? DBNull.Value);
            AddParam(cmd, "@season", cropSeason);

            await cmd.ExecuteNonQueryAsync();
        }

        // 2. Delete existing PurchaseBody row-by-row (Triggers enforce single row deletion!)
        var existingBodyIds = new List<string>();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT ProductID FROM PurchaseBody WHERE PurchaseID = @pId";
            AddParam(cmd, "@pId", legacyId);
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
                existingBodyIds.Add(rdr.GetString(0));
        }

        foreach (var prodCode in existingBodyIds)
        {
            using var delCmd = conn.CreateCommand();
            delCmd.CommandText = "DELETE FROM PurchaseBody WHERE PurchaseID = @pId AND ProductID = @prodCode";
            AddParam(delCmd, "@pId", legacyId);
            AddParam(delCmd, "@prodCode", prodCode);
            await delCmd.ExecuteNonQueryAsync();
        }

        // 3. Insert new PurchaseBody row-by-row (Triggers enforce single row insertion!)
        foreach (var d in inv.Details)
        {
            string pCode = prodMap.GetValueOrDefault(d.ProductId, d.ProductId.ToString());
            using var insCmd = conn.CreateCommand();
            insCmd.CommandText = @"
                INSERT INTO PurchaseBody (PurchaseID, ProductID, Multiplier, Qty, Bonus, Price, DiscRatio, DiscValue, 
                                          STRatio, STValue, TTLValue, TTLSalesTax, Cost, IsDeleted, PaidAmount)
                VALUES (@pId, @pCode, @mult, @qty, @bonus, @price, @discR, @discV, 
                        @stR, @stV, @ttlV, @ttlSt, @cost, 0, 0)";

            AddParam(insCmd, "@pId", legacyId);
            AddParam(insCmd, "@pCode", pCode);
            AddParam(insCmd, "@mult", d.Multiplier > 0 ? d.Multiplier : 1m);
            AddParam(insCmd, "@qty", d.Quantity);
            AddParam(insCmd, "@bonus", d.Bonus);
            AddParam(insCmd, "@price", d.Rate);
            AddParam(insCmd, "@discR", d.DiscRatio);
            AddParam(insCmd, "@discV", d.DiscValue);
            AddParam(insCmd, "@stR", d.STRatio);
            AddParam(insCmd, "@stV", d.STValue);
            AddParam(insCmd, "@ttlV", d.TTLValue > 0 ? d.TTLValue : d.Amount);
            AddParam(insCmd, "@ttlSt", d.TTLSalesTax);
            AddParam(insCmd, "@cost", d.Cost);

            await insCmd.ExecuteNonQueryAsync();
        }
    }

    private async Task SyncSaleToLegacyAsync(DbConnection conn, Invoice inv, int legacyId, string custId, string cropSeason, Dictionary<int, string> prodMap)
    {
        // 1. Check or Insert Sales Header
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
                IF EXISTS (SELECT 1 FROM Sales WHERE SaleID = @sId)
                BEGIN
                    UPDATE Sales SET 
                        SaleDate = @sDate,
                        CustomerID = @cId,
                        SectorID = @secId,
                        ExpPlus = @expPlus,
                        ExpMinus = @expMinus,
                        ComRatio = @comRatio,
                        ReceivedAmount = @recAmt,
                        SpecialDiscount = @specDisc,
                        Narration = @narration,
                        PurchaseID = @purId,
                        NarrationUrdu = @narrationUrdu,
                        IsMulti = @isMulti,
                        Freight = @freight,
                        VehicleNo = @vehNo,
                        Amount = @amount,
                        WHTRatio = @whtRatio,
                        WHTAmount = @whtAmount,
                        CropSeason = @season
                    WHERE SaleID = @sId
                END
                ELSE
                BEGIN
                    INSERT INTO Sales (SaleID, SaleDate, CustomerID, SectorID, ExpPlus, ExpMinus, ComRatio, ReceivedAmount, 
                                       SpecialDiscount, Narration, IsPosted, PurchaseID, NarrationUrdu, IsMulti, Freight, 
                                       VehicleNo, Amount, WHTRatio, WHTAmount, CropSeason)
                    VALUES (@sId, @sDate, @cId, @secId, @expPlus, @expMinus, @comRatio, @recAmt, 
                            @specDisc, @narration, 0, @purId, @narrationUrdu, @isMulti, @freight, 
                            @vehNo, @amount, @whtRatio, @whtAmount, @season)
                END";

            AddParam(cmd, "@sId", legacyId);
            AddParam(cmd, "@sDate", inv.InvoiceDate);
            AddParam(cmd, "@cId", custId);
            AddParam(cmd, "@secId", (object?)inv.SectorId ?? 1);
            AddParam(cmd, "@expPlus", inv.ExpPlus);
            AddParam(cmd, "@expMinus", inv.ExpMinus);
            AddParam(cmd, "@comRatio", inv.ComRatio);
            AddParam(cmd, "@recAmt", inv.ReceivedAmount);
            AddParam(cmd, "@specDisc", inv.SpecialDiscount);
            AddParam(cmd, "@narration", (object?)inv.Narration ?? DBNull.Value);
            AddParam(cmd, "@purId", (object?)inv.AssociatedPurchaseId ?? DBNull.Value);
            AddParam(cmd, "@narrationUrdu", (object?)inv.NarrationUrdu ?? DBNull.Value);
            AddParam(cmd, "@isMulti", inv.IsMulti);
            AddParam(cmd, "@freight", inv.FreightCharges);
            AddParam(cmd, "@vehNo", (object?)inv.VehicleNo ?? DBNull.Value);
            AddParam(cmd, "@amount", inv.NetAmount);
            AddParam(cmd, "@whtRatio", inv.WHTRatio);
            AddParam(cmd, "@whtAmount", inv.WHTAmount);
            AddParam(cmd, "@season", cropSeason);

            await cmd.ExecuteNonQueryAsync();
        }

        // 2. Delete existing SalesBody row-by-row (Triggers enforce single row deletion!)
        var existingSrNos = new List<int>();
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = "SELECT SrNo FROM SalesBody WHERE SaleID = @sId";
            AddParam(cmd, "@sId", legacyId);
            using var rdr = await cmd.ExecuteReaderAsync();
            while (await rdr.ReadAsync())
                existingSrNos.Add(rdr.GetInt32(0));
        }

        foreach (var sr in existingSrNos)
        {
            using var delCmd = conn.CreateCommand();
            delCmd.CommandText = "DELETE FROM SalesBody WHERE SaleID = @sId AND SrNo = @sr";
            AddParam(delCmd, "@sId", legacyId);
            AddParam(delCmd, "@sr", sr);
            await delCmd.ExecuteNonQueryAsync();
        }

        // 3. Insert new SalesBody row-by-row (Triggers enforce single row insertion!)
        int srCounter = 1;
        foreach (var d in inv.Details)
        {
            string pCode = prodMap.GetValueOrDefault(d.ProductId, d.ProductId.ToString());
            using var insCmd = conn.CreateCommand();
            insCmd.CommandText = @"
                INSERT INTO SalesBody (SaleID, ProductID, Multiplier, Qty, Bonus, Price, DiscRatio, DiscValue, 
                                       STRatio, STValue, TTLValue, TTLSalesTax, Cost, IsDeleted, Narration, SrNo, PurchaseID)
                VALUES (@sId, @pCode, @mult, @qty, @bonus, @price, @discR, @discV, 
                        @stR, @stV, @ttlV, @ttlSt, @cost, 0, @narr, @sr, @purId)";

            AddParam(insCmd, "@sId", legacyId);
            AddParam(insCmd, "@pCode", pCode);
            AddParam(insCmd, "@mult", d.Multiplier > 0 ? d.Multiplier : 1m);
            AddParam(insCmd, "@qty", d.Quantity);
            AddParam(insCmd, "@bonus", d.Bonus);
            AddParam(insCmd, "@price", d.Rate);
            AddParam(insCmd, "@discR", d.DiscRatio);
            AddParam(insCmd, "@discV", d.DiscValue);
            AddParam(insCmd, "@stR", d.STRatio);
            AddParam(insCmd, "@stV", d.STValue);
            AddParam(insCmd, "@ttlV", d.TTLValue > 0 ? d.TTLValue : d.Amount);
            AddParam(insCmd, "@ttlSt", d.TTLSalesTax);
            AddParam(insCmd, "@cost", d.Cost);
            AddParam(insCmd, "@narr", (object?)d.DetailNarration ?? DBNull.Value);
            AddParam(insCmd, "@sr", srCounter++);
            AddParam(insCmd, "@purId", (object?)inv.AssociatedPurchaseId ?? DBNull.Value);

            await insCmd.ExecuteNonQueryAsync();
        }
    }

    private async Task SyncPurchaseReturnToLegacyAsync(DbConnection conn, Invoice inv, int legacyId, string vendorId, string cropSeason, Dictionary<int, string> prodMap)
    {
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
                IF EXISTS (SELECT 1 FROM PurReturns WHERE PurReturnID = @pId)
                BEGIN
                    UPDATE PurReturns SET 
                        PurReturnDate = @pDate,
                        VendorID = @vId,
                        BillNo = @billNo,
                        BillDate = @billDate,
                        ExpPlus = @expPlus,
                        ExpMinus = @expMinus,
                        ComRatio = @comRatio,
                        ReceivedAmount = @recAmt,
                        Narration = @narration,
                        NarrationUrdu = @narrationUrdu,
                        CropSeason = @season
                    WHERE PurReturnID = @pId
                END
                ELSE
                BEGIN
                    INSERT INTO PurReturns (PurReturnID, PurReturnDate, VendorID, BillNo, BillDate, ExpPlus, ExpMinus, 
                                            ComRatio, ReceivedAmount, Narration, IsPosted, NarrationUrdu, CropSeason)
                    VALUES (@pId, @pDate, @vId, @billNo, @billDate, @expPlus, @expMinus, 
                            @comRatio, @recAmt, @narration, 0, @narrationUrdu, @season)
                END";

            AddParam(cmd, "@pId", legacyId);
            AddParam(cmd, "@pDate", inv.InvoiceDate);
            AddParam(cmd, "@vId", vendorId);
            AddParam(cmd, "@billNo", (object?)inv.BillNo ?? DBNull.Value);
            AddParam(cmd, "@billDate", (object?)inv.BillDate ?? DBNull.Value);
            AddParam(cmd, "@expPlus", inv.ExpPlus);
            AddParam(cmd, "@expMinus", inv.ExpMinus);
            AddParam(cmd, "@comRatio", inv.ComRatio);
            AddParam(cmd, "@recAmt", inv.ReceivedAmount);
            AddParam(cmd, "@narration", (object?)inv.Narration ?? DBNull.Value);
            AddParam(cmd, "@narrationUrdu", (object?)inv.NarrationUrdu ?? DBNull.Value);
            AddParam(cmd, "@season", cropSeason);

            await cmd.ExecuteNonQueryAsync();
        }

        // Delete & Insert PurReturnsBody row-by-row
        using (var delCmd = conn.CreateCommand())
        {
            delCmd.CommandText = "DELETE FROM PurReturnsBody WHERE PurReturnID = @pId";
            AddParam(delCmd, "@pId", legacyId);
            await delCmd.ExecuteNonQueryAsync();
        }

        foreach (var d in inv.Details)
        {
            string pCode = prodMap.GetValueOrDefault(d.ProductId, d.ProductId.ToString());
            using var insCmd = conn.CreateCommand();
            insCmd.CommandText = @"
                INSERT INTO PurReturnsBody (PurReturnID, ProductID, Multiplier, Qty, Bonus, Price, DiscRatio, DiscValue, 
                                            STRatio, STValue, TTLValue, TTLSalesTax, Cost, IsDeleted)
                VALUES (@pId, @pCode, @mult, @qty, @bonus, @price, @discR, @discV, 
                        @stR, @stV, @ttlV, @ttlSt, @cost, 0)";

            AddParam(insCmd, "@pId", legacyId);
            AddParam(insCmd, "@pCode", pCode);
            AddParam(insCmd, "@mult", d.Multiplier > 0 ? d.Multiplier : 1m);
            AddParam(insCmd, "@qty", d.Quantity);
            AddParam(insCmd, "@bonus", d.Bonus);
            AddParam(insCmd, "@price", d.Rate);
            AddParam(insCmd, "@discR", d.DiscRatio);
            AddParam(insCmd, "@discV", d.DiscValue);
            AddParam(insCmd, "@stR", d.STRatio);
            AddParam(insCmd, "@stV", d.STValue);
            AddParam(insCmd, "@ttlV", d.TTLValue > 0 ? d.TTLValue : d.Amount);
            AddParam(insCmd, "@ttlSt", d.TTLSalesTax);
            AddParam(insCmd, "@cost", d.Cost);

            await insCmd.ExecuteNonQueryAsync();
        }
    }

    private async Task SyncSaleReturnToLegacyAsync(DbConnection conn, Invoice inv, int legacyId, string custId, string cropSeason, Dictionary<int, string> prodMap)
    {
        using (var cmd = conn.CreateCommand())
        {
            cmd.CommandText = @"
                IF EXISTS (SELECT 1 FROM SaleReturns WHERE SaleReturnID = @sId)
                BEGIN
                    UPDATE SaleReturns SET 
                        SaleReturnDate = @sDate,
                        CustomerID = @cId,
                        ExpPlus = @expPlus,
                        ExpMinus = @expMinus,
                        ComRatio = @comRatio,
                        PaidAmount = @paidAmt,
                        SpecialDiscount = @specDisc,
                        Narration = @narration,
                        NarrationUrdu = @narrationUrdu,
                        CropSeason = @season
                    WHERE SaleReturnID = @sId
                END
                ELSE
                BEGIN
                    INSERT INTO SaleReturns (SaleReturnID, SaleReturnDate, CustomerID, ExpPlus, ExpMinus, ComRatio, 
                                             PaidAmount, SpecialDiscount, Narration, IsPosted, NarrationUrdu, CropSeason)
                    VALUES (@sId, @sDate, @cId, @expPlus, @expMinus, @comRatio, 
                            @paidAmt, @specDisc, @narration, 0, @narrationUrdu, @season)
                END";

            AddParam(cmd, "@sId", legacyId);
            AddParam(cmd, "@sDate", inv.InvoiceDate);
            AddParam(cmd, "@cId", custId);
            AddParam(cmd, "@expPlus", inv.ExpPlus);
            AddParam(cmd, "@expMinus", inv.ExpMinus);
            AddParam(cmd, "@comRatio", inv.ComRatio);
            AddParam(cmd, "@paidAmt", inv.PaidAmount);
            AddParam(cmd, "@specDisc", inv.SpecialDiscount);
            AddParam(cmd, "@narration", (object?)inv.Narration ?? DBNull.Value);
            AddParam(cmd, "@narrationUrdu", (object?)inv.NarrationUrdu ?? DBNull.Value);
            AddParam(cmd, "@season", cropSeason);

            await cmd.ExecuteNonQueryAsync();
        }

        // Delete & Insert SaleReturnsBody row-by-row
        using (var delCmd = conn.CreateCommand())
        {
            delCmd.CommandText = "DELETE FROM SaleReturnsBody WHERE SaleReturnID = @sId";
            AddParam(delCmd, "@sId", legacyId);
            await delCmd.ExecuteNonQueryAsync();
        }

        foreach (var d in inv.Details)
        {
            string pCode = prodMap.GetValueOrDefault(d.ProductId, d.ProductId.ToString());
            using var insCmd = conn.CreateCommand();
            insCmd.CommandText = @"
                INSERT INTO SaleReturnsBody (SaleReturnID, ProductID, Multiplier, Qty, Bonus, Price, DiscRatio, DiscValue, 
                                             STRatio, STValue, TTLValue, TTLSalesTax, Cost, IsDeleted)
                VALUES (@sId, @pCode, @mult, @qty, @bonus, @price, @discR, @discV, 
                        @stR, @stV, @ttlV, @ttlSt, @cost, 0)";

            AddParam(insCmd, "@sId", legacyId);
            AddParam(insCmd, "@pCode", pCode);
            AddParam(insCmd, "@mult", d.Multiplier > 0 ? d.Multiplier : 1m);
            AddParam(insCmd, "@qty", d.Quantity);
            AddParam(insCmd, "@bonus", d.Bonus);
            AddParam(insCmd, "@price", d.Rate);
            AddParam(insCmd, "@discR", d.DiscRatio);
            AddParam(insCmd, "@discV", d.DiscValue);
            AddParam(insCmd, "@stR", d.STRatio);
            AddParam(insCmd, "@stV", d.STValue);
            AddParam(insCmd, "@ttlV", d.TTLValue > 0 ? d.TTLValue : d.Amount);
            AddParam(insCmd, "@ttlSt", d.TTLSalesTax);
            AddParam(insCmd, "@cost", d.Cost);

            await insCmd.ExecuteNonQueryAsync();
        }
    }

    private async Task DeleteFromLegacyAsync(AppDbContext context, Invoice inv)
    {
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            int legacyId = int.TryParse(inv.InvoiceNo, out int parsedId) ? parsedId : inv.Id;

            if (inv.InvoiceType == InvoiceType.GenPurchase)
            {
                // Delete body row-by-row
                var prods = new List<string>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT ProductID FROM PurchaseBody WHERE PurchaseID = @pId";
                    AddParam(cmd, "@pId", legacyId);
                    using var rdr = await cmd.ExecuteReaderAsync();
                    while (await rdr.ReadAsync()) prods.Add(rdr.GetString(0));
                }
                foreach (var p in prods)
                {
                    using var delCmd = conn.CreateCommand();
                    delCmd.CommandText = "DELETE FROM PurchaseBody WHERE PurchaseID = @pId AND ProductID = @prod";
                    AddParam(delCmd, "@pId", legacyId);
                    AddParam(delCmd, "@prod", p);
                    await delCmd.ExecuteNonQueryAsync();
                }
                using var delHead = conn.CreateCommand();
                delHead.CommandText = "DELETE FROM Purchase WHERE PurchaseID = @pId";
                AddParam(delHead, "@pId", legacyId);
                await delHead.ExecuteNonQueryAsync();
            }
            else if (inv.InvoiceType == InvoiceType.GenSale)
            {
                var srNos = new List<int>();
                using (var cmd = conn.CreateCommand())
                {
                    cmd.CommandText = "SELECT SrNo FROM SalesBody WHERE SaleID = @sId";
                    AddParam(cmd, "@sId", legacyId);
                    using var rdr = await cmd.ExecuteReaderAsync();
                    while (await rdr.ReadAsync()) srNos.Add(rdr.GetInt32(0));
                }
                foreach (var sr in srNos)
                {
                    using var delCmd = conn.CreateCommand();
                    delCmd.CommandText = "DELETE FROM SalesBody WHERE SaleID = @sId AND SrNo = @sr";
                    AddParam(delCmd, "@sId", legacyId);
                    AddParam(delCmd, "@sr", sr);
                    await delCmd.ExecuteNonQueryAsync();
                }
                using var delHead = conn.CreateCommand();
                delHead.CommandText = "DELETE FROM Sales WHERE SaleID = @sId";
                AddParam(delHead, "@sId", legacyId);
                await delHead.ExecuteNonQueryAsync();
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InvoiceService DeleteLegacy] {ex.Message}");
        }
    }

    private async Task LoadLegacyMetadataAsync(AppDbContext context, Invoice invoice)
    {
        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != System.Data.ConnectionState.Open)
                await conn.OpenAsync();

            int legacyId = int.TryParse(invoice.InvoiceNo, out int parsedId) ? parsedId : invoice.Id;

            if (invoice.InvoiceType == InvoiceType.GenPurchase)
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT BillNo, BillDate, ExpPlus, ExpMinus, ComRatio, PaidAmount, SpecialDiscount, Freight, VehicleNo, Narration, NarrationUrdu, SaleID FROM Purchase WHERE PurchaseID = @pId";
                AddParam(cmd, "@pId", legacyId);
                using var rdr = await cmd.ExecuteReaderAsync();
                if (await rdr.ReadAsync())
                {
                    invoice.BillNo = rdr.IsDBNull(0) ? null : rdr.GetString(0);
                    invoice.BillDate = rdr.IsDBNull(1) ? null : rdr.GetDateTime(1);
                    invoice.ExpPlus = rdr.IsDBNull(2) ? 0 : rdr.GetDecimal(2);
                    invoice.ExpMinus = rdr.IsDBNull(3) ? 0 : rdr.GetDecimal(3);
                    invoice.ComRatio = rdr.IsDBNull(4) ? 0 : rdr.GetDecimal(4);
                    invoice.PaidAmount = rdr.IsDBNull(5) ? 0 : rdr.GetDecimal(5);
                    invoice.SpecialDiscount = rdr.IsDBNull(6) ? 0 : Convert.ToDecimal(rdr.GetValue(6));
                    invoice.Freight = rdr.IsDBNull(7) ? 0 : rdr.GetDecimal(7);
                    invoice.VehicleNo = rdr.IsDBNull(8) ? null : rdr.GetString(8);
                    invoice.Narration = rdr.IsDBNull(9) ? null : rdr.GetString(9);
                    invoice.NarrationUrdu = rdr.IsDBNull(10) ? null : rdr.GetString(10);
                    invoice.AssociatedSaleId = rdr.IsDBNull(11) ? null : rdr.GetInt32(11);
                }
            }
            else if (invoice.InvoiceType == InvoiceType.GenSale)
            {
                using var cmd = conn.CreateCommand();
                cmd.CommandText = "SELECT ExpPlus, ExpMinus, ComRatio, ReceivedAmount, SpecialDiscount, Freight, VehicleNo, Narration, NarrationUrdu, PurchaseID, WHTRatio, WHTAmount FROM Sales WHERE SaleID = @sId";
                AddParam(cmd, "@sId", legacyId);
                using var rdr = await cmd.ExecuteReaderAsync();
                if (await rdr.ReadAsync())
                {
                    invoice.ExpPlus = rdr.IsDBNull(0) ? 0 : rdr.GetDecimal(0);
                    invoice.ExpMinus = rdr.IsDBNull(1) ? 0 : rdr.GetDecimal(1);
                    invoice.ComRatio = rdr.IsDBNull(2) ? 0 : rdr.GetDecimal(2);
                    invoice.ReceivedAmount = rdr.IsDBNull(3) ? 0 : rdr.GetDecimal(3);
                    invoice.SpecialDiscount = rdr.IsDBNull(4) ? 0 : rdr.GetDecimal(4);
                    invoice.Freight = rdr.IsDBNull(5) ? 0 : rdr.GetDecimal(5);
                    invoice.VehicleNo = rdr.IsDBNull(6) ? null : rdr.GetString(6);
                    invoice.Narration = rdr.IsDBNull(7) ? null : rdr.GetString(7);
                    invoice.NarrationUrdu = rdr.IsDBNull(8) ? null : rdr.GetString(8);
                    invoice.AssociatedPurchaseId = rdr.IsDBNull(9) ? null : rdr.GetInt32(9);
                    invoice.WHTRatio = rdr.IsDBNull(10) ? 0 : rdr.GetDecimal(10);
                    invoice.WHTAmount = rdr.IsDBNull(11) ? 0 : rdr.GetDecimal(11);
                }
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"[InvoiceService LoadLegacyMeta] {ex.Message}");
        }
    }

    private void AddParam(DbCommand cmd, string name, object value)
    {
        var p = cmd.CreateParameter();
        p.ParameterName = name;
        p.Value = value;
        cmd.Parameters.Add(p);
    }

    private async Task<Account> GetOrCreateSystemAccount(AppDbContext context, string name, AccountType type)
    {
        var account = await context.Accounts.FirstOrDefaultAsync(a => a.Name == name);
        if (account == null)
        {
            account = new Account
            {
                Name = name,
                AccountNo = "SYS-" + name.Replace(" ", ""),
                AccountType = type,
                AccountGroupId = 1,
                IsSystemAccount = true
            };
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }
        return account;
    }
}
