using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class PestroService : IPestroService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    public PestroService(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
    }

    public async Task<PestroInvoice?> GetPestroInvoiceByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var inv = await context.PestroInvoices
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Include(i => i.Details).ThenInclude(d => d.Batch)
            .Include(i => i.Party)
            .Include(i => i.Salesman)
            .Include(i => i.CropSeason)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (inv != null)
        {
            await LoadLegacyMetadataAsync(context, inv);
        }
        return inv;
    }

    public async Task<PestroInvoice?> GetPestroInvoiceByNoAsync(string invoiceNo, InvoiceType type)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var inv = await context.PestroInvoices
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Include(i => i.Details).ThenInclude(d => d.Batch)
            .Include(i => i.Party)
            .Include(i => i.Salesman)
            .Include(i => i.CropSeason)
            .FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNo && i.InvoiceType == type);

        if (inv != null)
        {
            await LoadLegacyMetadataAsync(context, inv);
            return inv;
        }

        // Seamless fallback to legacy tables (PurchasesPest / SalesPest) if not yet in EF
        if (int.TryParse(invoiceNo, out int legacyId))
        {
            return await LoadFromLegacyAsync(context, legacyId, type);
        }

        return null;
    }

    public async Task<IEnumerable<PestroInvoice>> GetAllPestroInvoicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PestroInvoices
            .Include(i => i.Party)
            .Include(i => i.CropSeason)
            .OrderByDescending(i => i.Id)
            .ToListAsync();
    }

    public async Task<IEnumerable<PestroStockBatchDto>> GetAvailableBatchesForProductAsync(string productCode)
    {
        var result = new List<PestroStockBatchDto>();
        using var context = await _contextFactory.CreateDbContextAsync();
        var connection = context.Database.GetDbConnection();
        await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        cmd.CommandText = @"
            SELECT ProductId, BatchNo, ExpiryDate, Quantity, Cost 
            FROM CurrentStockPestro 
            WHERE ProductId = @prodCode AND Quantity > 0
            ORDER BY ExpiryDate ASC";

        var param = cmd.CreateParameter();
        param.ParameterName = "@prodCode";
        param.Value = productCode;
        cmd.Parameters.Add(param);

        using var reader = await cmd.ExecuteReaderAsync();
        while (await reader.ReadAsync())
        {
            result.Add(new PestroStockBatchDto
            {
                ProductCode = reader.GetString(0),
                BatchNo = reader.GetString(1),
                ExpiryDate = reader.IsDBNull(2) ? DateTime.MinValue : reader.GetDateTime(2),
                AvailableQty = reader.IsDBNull(3) ? 0 : reader.GetInt32(3),
                CostPrice = reader.IsDBNull(4) ? 0 : reader.GetDecimal(4)
            });
        }

        return result;
    }

    public async Task<List<PestroStockBatchDto>> AutoAllocateBatchesAsync(string productCode, decimal requiredQty)
    {
        var allocated = new List<PestroStockBatchDto>();
        var availableBatches = (await GetAvailableBatchesForProductAsync(productCode))
            .Where(b => b.ExpiryDate > DateTime.Today)
            .OrderBy(b => b.ExpiryDate)
            .ToList();

        decimal remainingToAllocate = requiredQty;

        foreach (var batch in availableBatches)
        {
            if (remainingToAllocate <= 0) break;

            decimal qtyFromThisBatch = Math.Min(batch.AvailableQty, remainingToAllocate);
            allocated.Add(new PestroStockBatchDto
            {
                ProductCode = batch.ProductCode,
                BatchNo = batch.BatchNo,
                ExpiryDate = batch.ExpiryDate,
                AvailableQty = qtyFromThisBatch,
                CostPrice = batch.CostPrice
            });

            remainingToAllocate -= qtyFromThisBatch;
        }

        return allocated;
    }

    public async Task SavePestroInvoiceAsync(PestroInvoice invoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        if (invoice.Id == 0)
        {
            if (string.IsNullOrWhiteSpace(invoice.InvoiceNo))
            {
                invoice.InvoiceNo = await GetNextPestroInvoiceNoAsync(invoice.InvoiceType, invoice.FinancialYearId);
            }
            context.PestroInvoices.Add(invoice);
        }
        else
        {
            context.PestroInvoices.Update(invoice);
        }
        await context.SaveChangesAsync();

        // Synchronize with AgenSoft Native Database Tables
        if (invoice.InvoiceType == InvoiceType.PestroPurchase)
        {
            await SyncLegacyPestroPurchaseAsync(context, invoice);
        }
        else if (invoice.InvoiceType == InvoiceType.PestroSale)
        {
            await SyncLegacyPestroSaleAsync(context, invoice);
        }
    }

    public async Task DeletePestroInvoiceAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.PestroInvoices
            .Include(i => i.Details)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return;

        int invNoInt = 0;
        int.TryParse(invoice.InvoiceNo, out invNoInt);
        if (invNoInt == 0) invNoInt = invoice.Id;

        // 1. Delete GL Transactions
        var glEntries = await context.GeneralLedger
            .Where(gl => gl.SourceType == SourceType.PestroInvoice && gl.SourceId == invoice.Id)
            .ToListAsync();
        context.GeneralLedger.RemoveRange(glEntries);

        // 2. Delete Legacy Records (Row-by-row triggers restore CurrentStockPestro)
        if (invoice.InvoiceType == InvoiceType.PestroPurchase)
        {
            await DeleteLegacyPestroPurchaseAsync(context, invNoInt);
        }
        else if (invoice.InvoiceType == InvoiceType.PestroSale)
        {
            await DeleteLegacyPestroSaleAsync(context, invNoInt);
        }

        // 3. Delete EF Records
        context.PestroInvoiceDetails.RemoveRange(invoice.Details);
        context.PestroInvoices.Remove(invoice);
        await context.SaveChangesAsync();
    }

    public async Task PostPestroInvoiceToGLAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.PestroInvoices
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Include(i => i.Party)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null || invoice.IsPosted) return;

        // Load legacy amounts if applicable
        await LoadLegacyMetadataAsync(context, invoice);

        var partyAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Id == invoice.Party.AccountId);
        var cashAccount = await GetOrCreateSystemAccount(context, "Cash in Hand", AccountType.Asset, "11101");
        var salesAccount = await GetOrCreateSystemAccount(context, "Pestro Sales", AccountType.Revenue, "419");
        var purchaseAccount = await GetOrCreateSystemAccount(context, "Pestro Purchases", AccountType.Expense, "11328");
        var staxInputAccount = await GetOrCreateSystemAccount(context, "Sales Tax Input", AccountType.Asset, "11340");
        var staxOutputAccount = await GetOrCreateSystemAccount(context, "Sales Tax Output", AccountType.Liability, "21400");
        var discountAccount = await GetOrCreateSystemAccount(context, "Special Discount Allowed", AccountType.Expense, "5128");
        var expenseAccount = await GetOrCreateSystemAccount(context, "Purchase Expenses / Freight", AccountType.Expense, "5609");
        var cogsAccount = await GetOrCreateSystemAccount(context, "Cost of Goods Sold (Pestro)", AccountType.Expense, "51120");
        var stockAccount = await GetOrCreateSystemAccount(context, "Stock Inventory (Pestro)", AccountType.Asset, "11335");

        if (invoice.InvoiceType == InvoiceType.PestroPurchase)
        {
            decimal ttlGross = invoice.Details.Sum(d => d.TTLValue > 0 ? d.TTLValue : d.Amount);
            decimal ttlSTax = invoice.Details.Sum(d => d.TTLSalestax);
            decimal expenses = invoice.Expenses;
            decimal paidAmount = invoice.PaidAmount;
            decimal netPayable = (ttlGross + ttlSTax + expenses) - paidAmount;

            // 1. Debit Purchase Account (TTLValue)
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = purchaseAccount.Id,
                Narration = $"Pestro Purchase Inv #{invoice.InvoiceNo}",
                Debit = ttlGross,
                Credit = 0,
                SourceType = SourceType.PestroInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId
            });

            // 2. Debit Sales Tax Input (if any)
            if (ttlSTax > 0)
            {
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = staxInputAccount.Id,
                    Narration = $"Pestro Purchase STax Inv #{invoice.InvoiceNo}",
                    Debit = ttlSTax,
                    Credit = 0,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });
            }

            // 3. Debit Expenses (if any)
            if (expenses > 0)
            {
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = expenseAccount.Id,
                    Narration = $"Purchase Expenses Inv #{invoice.InvoiceNo}",
                    Debit = expenses,
                    Credit = 0,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });
            }

            // 4. Credit Cash Account (PaidAmount if paid immediately)
            if (paidAmount > 0)
            {
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = cashAccount.Id,
                    Narration = $"Cash Paid on Pur Inv #{invoice.InvoiceNo}",
                    Debit = 0,
                    Credit = paidAmount,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });
            }

            // 5. Credit Vendor Account (Net balance payable)
            if (partyAccount != null && netPayable != 0)
            {
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = partyAccount.Id,
                    Narration = $"Pur Inv #{invoice.InvoiceNo} - {invoice.NarrationUrdu}",
                    Debit = netPayable < 0 ? Math.Abs(netPayable) : 0,
                    Credit = netPayable > 0 ? netPayable : 0,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });
            }
        }
        else if (invoice.InvoiceType == InvoiceType.PestroSale)
        {
            decimal ttlGross = invoice.Details.Sum(d => d.TTLValue > 0 ? d.TTLValue : d.Amount);
            decimal ttlSTax = invoice.Details.Sum(d => d.TTLSalestax);
            decimal specialDiscount = invoice.SpecialDiscount;
            decimal receivedAmount = invoice.ReceivedAmount;
            decimal netReceivable = (ttlGross + ttlSTax) - specialDiscount - receivedAmount;

            // 1. Credit Sales Account (TTLValue)
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = salesAccount.Id,
                Narration = $"Pestro Sale Inv #{invoice.InvoiceNo}",
                Debit = 0,
                Credit = ttlGross,
                SourceType = SourceType.PestroInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId
            });

            // 2. Credit Sales Tax Output (if any)
            if (ttlSTax > 0)
            {
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = staxOutputAccount.Id,
                    Narration = $"Pestro Sale STax Inv #{invoice.InvoiceNo}",
                    Debit = 0,
                    Credit = ttlSTax,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });
            }

            // 3. Debit Special Discount (if any)
            if (specialDiscount > 0)
            {
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = discountAccount.Id,
                    Narration = $"Special Discount on Inv #{invoice.InvoiceNo}",
                    Debit = specialDiscount,
                    Credit = 0,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });
            }

            // 4. Debit Cash Account (ReceivedAmount if cash collected)
            if (receivedAmount > 0)
            {
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = cashAccount.Id,
                    Narration = $"Cash Received on Sale Inv #{invoice.InvoiceNo}",
                    Debit = receivedAmount,
                    Credit = 0,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });
            }

            // 5. Debit Customer Account (Net balance receivable)
            if (partyAccount != null && netReceivable != 0)
            {
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = partyAccount.Id,
                    Narration = $"Sale Inv #{invoice.InvoiceNo} - {invoice.Remarks}",
                    Debit = netReceivable > 0 ? netReceivable : 0,
                    Credit = netReceivable < 0 ? Math.Abs(netReceivable) : 0,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });
            }

            // 6. COGS & Stock Reduction GL Entry (Batch Cost calculation)
            int invNoInt = 0;
            int.TryParse(invoice.InvoiceNo, out invNoInt);
            if (invNoInt == 0) invNoInt = invoice.Id;

            decimal totalBatchCost = await GetSaleBatchTotalCostAsync(context, invNoInt);
            if (totalBatchCost > 0)
            {
                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = cogsAccount.Id,
                    Narration = $"COGS Pestro Inv #{invoice.InvoiceNo}",
                    Debit = totalBatchCost,
                    Credit = 0,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });

                context.GeneralLedger.Add(new GeneralLedger
                {
                    TransactionDate = invoice.InvoiceDate,
                    AccountId = stockAccount.Id,
                    Narration = $"Stock Deducted Inv #{invoice.InvoiceNo}",
                    Debit = 0,
                    Credit = totalBatchCost,
                    SourceType = SourceType.PestroInvoice,
                    SourceId = invoice.Id,
                    FinancialYearId = invoice.FinancialYearId,
                    CropSeasonId = invoice.CropSeasonId
                });
            }
        }

        invoice.IsPosted = true;
        invoice.Status = InvoiceStatus.Posted;
        await context.SaveChangesAsync();

        // Update IsPosted in legacy tables as well
        int idInt = 0;
        int.TryParse(invoice.InvoiceNo, out idInt);
        if (idInt == 0) idInt = invoice.Id;

        if (invoice.InvoiceType == InvoiceType.PestroPurchase)
        {
            await context.Database.ExecuteSqlRawAsync(
                "UPDATE PurchasesPest SET IsPosted = 1 WHERE PurchaseId = {0}", idInt);
        }
        else if (invoice.InvoiceType == InvoiceType.PestroSale)
        {
            await context.Database.ExecuteSqlRawAsync(
                "UPDATE SalesPest SET IsPosted = 1 WHERE SaleId = {0}", idInt);
        }
    }

    private async Task<decimal> GetSaleBatchTotalCostAsync(AppDbContext context, int saleId)
    {
        try
        {
            var connection = context.Database.GetDbConnection();
            if (connection.State != ConnectionState.Open) await connection.OpenAsync();
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT ISNULL(SUM(Quantity * Cost), 0) FROM SalesBatch WHERE SaleId = @saleId AND IsDeleted = 0";
            var p = cmd.CreateParameter();
            p.ParameterName = "@saleId";
            p.Value = saleId;
            cmd.Parameters.Add(p);

            var res = await cmd.ExecuteScalarAsync();
            return res != null && res != DBNull.Value ? Convert.ToDecimal(res) : 0;
        }
        catch
        {
            return 0;
        }
    }

    private async Task SyncLegacyPestroPurchaseAsync(AppDbContext context, PestroInvoice invoice)
    {
        int purId = 0;
        int.TryParse(invoice.InvoiceNo, out purId);
        if (purId == 0) purId = invoice.Id;

        var party = await context.Parties.FirstOrDefaultAsync(p => p.Id == invoice.PartyId);
        string vendorId = party?.PartyNo ?? invoice.PartyId.ToString();

        string cropSeason = "";
        try
        {
            var season = await context.CropSeasons.FirstOrDefaultAsync(s => s.Id == invoice.CropSeasonId);
            cropSeason = season?.Name ?? "";
        }
        catch { }

        // Delete previous legacy records row-by-row so TD triggers run
        await DeleteLegacyPestroPurchaseAsync(context, purId);

        // Insert Master PurchasesPest
        var sqlMaster = @"
            INSERT INTO PurchasesPest (
                PurchaseId, LastUpdateDate, EntryDate, BillNo, BillDate,
                OrderID, PreviousCredit, Expenses, PaidAmount, VendorId,
                IsPosted, IsLocal, NarrationUrdu, CropSeason, Narration
            ) VALUES (
                {0}, GETDATE(), {1}, {2}, {3},
                0, 0, {4}, {5}, {6},
                {7}, 0, {8}, {9}, {10}
            )";

        await context.Database.ExecuteSqlRawAsync(sqlMaster,
            purId,
            invoice.InvoiceDate,
            invoice.BillNo ?? "",
            invoice.BillDate ?? (object)DBNull.Value,
            invoice.Expenses,
            invoice.PaidAmount,
            vendorId,
            invoice.IsPosted ? 1 : 0,
            invoice.NarrationUrdu ?? "",
            cropSeason,
            $"Purchase Inv #{purId}"
        );

        // Insert Body & Batch row-by-row (Triggers require @@ROWCOUNT = 1)
        int srNo = 1;
        foreach (var detail in invoice.Details)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == detail.ProductId);
            string prodCode = product?.ProductCode ?? detail.ProductId.ToString();

            decimal gross = detail.Quantity * detail.Rate;
            decimal disc1 = gross * (detail.DiscRatio1 / 100m);
            decimal afterDisc1 = gross - disc1;
            decimal disc2 = afterDisc1 * (detail.DiscRatio2 / 100m);
            decimal ttlValue = detail.TTLValue > 0 ? detail.TTLValue : (afterDisc1 - disc2);
            decimal ttlSTax = detail.TTLSalestax > 0 ? detail.TTLSalestax : (ttlValue * (detail.STRatio / 100m));
            decimal totalUnits = detail.Quantity + detail.Bonus;
            decimal unitCost = totalUnits > 0 ? (ttlValue / totalUnits) : detail.Rate;

            var sqlBody = @"
                INSERT INTO PurchasesBodyPest (
                    PurchaseId, ProductId, Quantity, Bonus, Price,
                    DiscRatio1, DiscRatio2, STValue, STRatio, TTLSalestax,
                    TTLValue, IsDeleted
                ) VALUES (
                    {0}, {1}, {2}, {3}, {4},
                    {5}, {6}, {7}, {8}, {9},
                    {10}, 0
                )";

            await context.Database.ExecuteSqlRawAsync(sqlBody,
                purId,
                prodCode,
                detail.Quantity,
                detail.Bonus,
                detail.Rate,
                detail.DiscRatio1,
                detail.DiscRatio2,
                detail.STValue,
                detail.STRatio,
                ttlSTax,
                ttlValue
            );

            // Insert Batch row-by-row (Triggers TI_PurchaseBatch to update CurrentStockPestro)
            string batchNo = string.IsNullOrWhiteSpace(detail.BatchNo) ? $"B-{purId}-{srNo}" : detail.BatchNo;
            DateTime expiry = detail.ExpiryDate ?? invoice.InvoiceDate.AddYears(2);

            var sqlBatch = @"
                INSERT INTO PurchasesBatch (
                    PurchaseId, ProductId, BatchNo, ExpiryDate, Quantity, Cost, IsDeleted
                ) VALUES (
                    {0}, {1}, {2}, {3}, {4}, {5}, 0
                )";

            await context.Database.ExecuteSqlRawAsync(sqlBatch,
                purId,
                prodCode,
                batchNo,
                expiry,
                (int)totalUnits,
                unitCost
            );

            srNo++;
        }
    }

    private async Task DeleteLegacyPestroPurchaseAsync(AppDbContext context, int purId)
    {
        // Deleting from PurchasesBatch triggers TD_PurchaseBatch to adjust CurrentStockPestro
        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        // 1. Fetch batches to delete one by one
        var batches = new List<(string prodId, string batchNo)>();
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT ProductId, BatchNo FROM PurchasesBatch WHERE PurchaseId = @pid";
            var p = cmd.CreateParameter();
            p.ParameterName = "@pid";
            p.Value = purId;
            cmd.Parameters.Add(p);

            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                batches.Add((r.GetString(0), r.GetString(1)));
            }
        }

        foreach (var b in batches)
        {
            await context.Database.ExecuteSqlRawAsync(
                "DELETE FROM PurchasesBatch WHERE PurchaseId = {0} AND ProductId = {1} AND BatchNo = {2}",
                purId, b.prodId, b.batchNo);
        }

        await context.Database.ExecuteSqlRawAsync("DELETE FROM PurchasesBodyPest WHERE PurchaseId = {0}", purId);
        await context.Database.ExecuteSqlRawAsync("DELETE FROM PurchasesPest WHERE PurchaseId = {0}", purId);
    }

    private async Task SyncLegacyPestroSaleAsync(AppDbContext context, PestroInvoice invoice)
    {
        int saleId = 0;
        int.TryParse(invoice.InvoiceNo, out saleId);
        if (saleId == 0) saleId = invoice.Id;

        var party = await context.Parties.FirstOrDefaultAsync(p => p.Id == invoice.PartyId);
        string customerId = party?.PartyNo ?? invoice.PartyId.ToString();

        var salesman = await context.Salesmen.Include(s => s.Account).FirstOrDefaultAsync(s => s.Id == invoice.SalesmanId);
        string salesmanId = salesman?.Account?.AccountNo ?? salesman?.Id.ToString() ?? "";

        string cropSeason = "";
        try
        {
            var season = await context.CropSeasons.FirstOrDefaultAsync(s => s.Id == invoice.CropSeasonId);
            cropSeason = season?.Name ?? "";
        }
        catch { }

        // Delete previous legacy records row-by-row (TD_SALESBATCH restores CurrentStockPestro)
        await DeleteLegacyPestroSaleAsync(context, saleId);

        int isAutoBatch = invoice.BatchMode == BatchMode.Auto ? 1 : 0;

        // Insert Master SalesPest
        var sqlMaster = @"
            INSERT INTO SalesPest (
                SaleId, SaleDate, CustomerId, SalesManId, Remarks,
                IsCashInvoice, ReceivedAmount, SpecialDiscount, DueDate,
                IsAutoBatch, CropSeason, NarrationUrdu, IsPosted, IsPrinted
            ) VALUES (
                {0}, {1}, {2}, {3}, {4},
                {5}, {6}, {7}, {8},
                {9}, {10}, {11}, {12}, 0
            )";

        await context.Database.ExecuteSqlRawAsync(sqlMaster,
            saleId,
            invoice.InvoiceDate,
            customerId,
            salesmanId,
            invoice.Remarks ?? "",
            invoice.IsCashInvoice ? 1 : 0,
            invoice.ReceivedAmount,
            invoice.SpecialDiscount,
            invoice.DueDate ?? (object)DBNull.Value,
            isAutoBatch,
            cropSeason,
            invoice.NarrationUrdu ?? "",
            invoice.IsPosted ? 1 : 0
        );

        // Insert Body row-by-row
        // Note: For IsAutoBatch = 1, TI_SALES_AUTOBATCH trigger automatically generates SalesBatch & deducts CurrentStockPestro!
        int srNo = 1;
        foreach (var detail in invoice.Details)
        {
            var product = await context.Products.FirstOrDefaultAsync(p => p.Id == detail.ProductId);
            string prodCode = product?.ProductCode ?? detail.ProductId.ToString();

            decimal gross = detail.Quantity * detail.Rate;
            decimal disc1 = gross * (detail.DiscRatio1 / 100m);
            decimal afterDisc1 = gross - disc1;
            decimal disc2 = afterDisc1 * (detail.DiscRatio2 / 100m);
            decimal ttlValue = detail.TTLValue > 0 ? detail.TTLValue : (afterDisc1 - disc2);
            decimal ttlSTax = detail.TTLSalestax > 0 ? detail.TTLSalestax : (ttlValue * (detail.STRatio / 100m));

            var sqlBody = @"
                INSERT INTO SalesBodyPest (
                    SerialNo, SaleId, ProductId, IsSaleOnTp, Quantity,
                    Bonus, Price, DiscRatio1, DiscRatio2, STValue,
                    STRatio, TTLValue, TTLSalestax, IsDeleted
                ) VALUES (
                    {0}, {1}, {2}, 0, {3},
                    {4}, {5}, {6}, {7}, {8},
                    {9}, {10}, {11}, 0
                )";

            await context.Database.ExecuteSqlRawAsync(sqlBody,
                srNo++,
                saleId,
                prodCode,
                detail.Quantity,
                detail.Bonus,
                detail.Rate,
                detail.DiscRatio1,
                detail.DiscRatio2,
                detail.STValue,
                detail.STRatio,
                ttlValue,
                ttlSTax
            );

            // If Manual Batch mode (IsAutoBatch = 0), manually insert into SalesBatch row-by-row
            if (isAutoBatch == 0 && !string.IsNullOrWhiteSpace(detail.BatchNo))
            {
                decimal totalQty = detail.Quantity + detail.Bonus;
                DateTime exp = detail.ExpiryDate ?? DateTime.Today.AddYears(1);

                var sqlManualBatch = @"
                    INSERT INTO SalesBatch (
                        SaleId, ProductId, BatchNo, ExpiryDate, Quantity, Cost, IsDeleted
                    ) VALUES (
                        {0}, {1}, {2}, {3}, {4}, 0, 0
                    )";

                await context.Database.ExecuteSqlRawAsync(sqlManualBatch,
                    saleId,
                    prodCode,
                    detail.BatchNo,
                    exp,
                    (int)totalQty
                );
            }
        }
    }

    private async Task DeleteLegacyPestroSaleAsync(AppDbContext context, int saleId)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        // 1. Fetch batches to delete one by one (TD_SALESBATCH triggers restore CurrentStockPestro)
        var batches = new List<(string prodId, string batchNo)>();
        using (var cmd = connection.CreateCommand())
        {
            cmd.CommandText = "SELECT ProductId, BatchNo FROM SalesBatch WHERE SaleId = @sid";
            var p = cmd.CreateParameter();
            p.ParameterName = "@sid";
            p.Value = saleId;
            cmd.Parameters.Add(p);

            using var r = await cmd.ExecuteReaderAsync();
            while (await r.ReadAsync())
            {
                batches.Add((r.GetString(0), r.GetString(1)));
            }
        }

        foreach (var b in batches)
        {
            await context.Database.ExecuteSqlRawAsync(
                "DELETE FROM SalesBatch WHERE SaleId = {0} AND ProductId = {1} AND BatchNo = {2}",
                saleId, b.prodId, b.batchNo);
        }

        await context.Database.ExecuteSqlRawAsync("DELETE FROM SalesBodyPest WHERE SaleId = {0}", saleId);
        await context.Database.ExecuteSqlRawAsync("DELETE FROM SalesPest WHERE SaleId = {0}", saleId);
    }

    private async Task LoadLegacyMetadataAsync(AppDbContext context, PestroInvoice invoice)
    {
        int invId = 0;
        int.TryParse(invoice.InvoiceNo, out invId);
        if (invId == 0) invId = invoice.Id;

        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        if (invoice.InvoiceType == InvoiceType.PestroPurchase)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT BillNo, BillDate, Expenses, PaidAmount, NarrationUrdu FROM PurchasesPest WHERE PurchaseId = @pid";
            var p = cmd.CreateParameter();
            p.ParameterName = "@pid";
            p.Value = invId;
            cmd.Parameters.Add(p);

            using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                invoice.BillNo = r.IsDBNull(0) ? "" : r.GetString(0);
                invoice.BillDate = r.IsDBNull(1) ? null : r.GetDateTime(1);
                invoice.Expenses = r.IsDBNull(2) ? 0 : r.GetDecimal(2);
                invoice.PaidAmount = r.IsDBNull(3) ? 0 : r.GetDecimal(3);
                invoice.NarrationUrdu = r.IsDBNull(4) ? "" : r.GetString(4);
            }
        }
        else if (invoice.InvoiceType == InvoiceType.PestroSale)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = "SELECT Remarks, IsCashInvoice, ReceivedAmount, SpecialDiscount, DueDate, IsAutoBatch, NarrationUrdu FROM SalesPest WHERE SaleId = @sid";
            var p = cmd.CreateParameter();
            p.ParameterName = "@sid";
            p.Value = invId;
            cmd.Parameters.Add(p);

            using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                invoice.Remarks = r.IsDBNull(0) ? "" : r.GetString(0);
                invoice.IsCashInvoice = !r.IsDBNull(1) && r.GetBoolean(1);
                invoice.ReceivedAmount = r.IsDBNull(2) ? 0 : r.GetDecimal(2);
                invoice.SpecialDiscount = r.IsDBNull(3) ? 0 : r.GetDecimal(3);
                invoice.DueDate = r.IsDBNull(4) ? null : r.GetDateTime(4);
                bool isAuto = !r.IsDBNull(5) && r.GetBoolean(5);
                invoice.BatchMode = isAuto ? BatchMode.Auto : BatchMode.Manual;
                invoice.NarrationUrdu = r.IsDBNull(6) ? "" : r.GetString(6);
            }
        }
    }

    private async Task<PestroInvoice?> LoadFromLegacyAsync(AppDbContext context, int legacyId, InvoiceType type)
    {
        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        if (type == InvoiceType.PestroPurchase)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT p.PurchaseId, p.EntryDate, p.VendorId, p.BillNo, p.BillDate, 
                       p.Expenses, p.PaidAmount, p.NarrationUrdu, p.CropSeason, p.IsPosted,
                       pa.Id as PartyId
                FROM PurchasesPest p
                LEFT JOIN Parties pa ON p.VendorId = pa.PartyNo
                WHERE p.PurchaseId = @pid";
            var p = cmd.CreateParameter();
            p.ParameterName = "@pid";
            p.Value = legacyId;
            cmd.Parameters.Add(p);

            using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                var inv = new PestroInvoice
                {
                    InvoiceNo = legacyId.ToString(),
                    InvoiceType = InvoiceType.PestroPurchase,
                    InvoiceDate = r.GetDateTime(1),
                    PartyId = r.IsDBNull(10) ? 1 : r.GetInt32(10),
                    BillNo = r.IsDBNull(3) ? "" : r.GetString(3),
                    BillDate = r.IsDBNull(4) ? null : r.GetDateTime(4),
                    Expenses = r.IsDBNull(5) ? 0 : r.GetDecimal(5),
                    PaidAmount = r.IsDBNull(6) ? 0 : r.GetDecimal(6),
                    NarrationUrdu = r.IsDBNull(7) ? "" : r.GetString(7),
                    IsPosted = !r.IsDBNull(9) && r.GetBoolean(9)
                };
                return inv;
            }
        }
        else if (type == InvoiceType.PestroSale)
        {
            using var cmd = connection.CreateCommand();
            cmd.CommandText = @"
                SELECT s.SaleId, s.SaleDate, s.CustomerId, s.Remarks, s.IsCashInvoice, 
                       s.ReceivedAmount, s.SpecialDiscount, s.DueDate, s.IsAutoBatch, s.NarrationUrdu, s.IsPosted,
                       pa.Id as PartyId
                FROM SalesPest s
                LEFT JOIN Parties pa ON s.CustomerId = pa.PartyNo
                WHERE s.SaleId = @sid";
            var p = cmd.CreateParameter();
            p.ParameterName = "@sid";
            p.Value = legacyId;
            cmd.Parameters.Add(p);

            using var r = await cmd.ExecuteReaderAsync();
            if (await r.ReadAsync())
            {
                var inv = new PestroInvoice
                {
                    InvoiceNo = legacyId.ToString(),
                    InvoiceType = InvoiceType.PestroSale,
                    InvoiceDate = r.GetDateTime(1),
                    PartyId = r.IsDBNull(11) ? 1 : r.GetInt32(11),
                    Remarks = r.IsDBNull(3) ? "" : r.GetString(3),
                    IsCashInvoice = !r.IsDBNull(4) && r.GetBoolean(4),
                    ReceivedAmount = r.IsDBNull(5) ? 0 : r.GetDecimal(5),
                    SpecialDiscount = r.IsDBNull(6) ? 0 : r.GetDecimal(6),
                    DueDate = r.IsDBNull(7) ? null : r.GetDateTime(7),
                    BatchMode = (!r.IsDBNull(8) && r.GetBoolean(8)) ? BatchMode.Auto : BatchMode.Manual,
                    NarrationUrdu = r.IsDBNull(9) ? "" : r.GetString(9),
                    IsPosted = !r.IsDBNull(10) && r.GetBoolean(10)
                };
                return inv;
            }
        }

        return null;
    }

    public async Task<string> GetNextPestroInvoiceNoAsync(InvoiceType type, int financialYearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var connection = context.Database.GetDbConnection();
        if (connection.State != ConnectionState.Open) await connection.OpenAsync();

        using var cmd = connection.CreateCommand();
        if (type == InvoiceType.PestroPurchase)
        {
            cmd.CommandText = "SELECT ISNULL(MAX(PurchaseId), 0) + 1 FROM PurchasesPest";
        }
        else
        {
            cmd.CommandText = "SELECT ISNULL(MAX(SaleId), 0) + 1 FROM SalesPest";
        }

        var res = await cmd.ExecuteScalarAsync();
        int nextId = res != null ? Convert.ToInt32(res) : 1;
        return nextId.ToString();
    }

    private async Task<Account> GetOrCreateSystemAccount(AppDbContext context, string name, AccountType type, string fallbackAcNo)
    {
        var account = await context.Accounts.FirstOrDefaultAsync(a => a.Name == name || a.AccountNo == fallbackAcNo);
        if (account == null)
        {
            account = new Account
            {
                Name = name,
                AccountNo = fallbackAcNo,
                AccountType = type,
                AccountGroupId = 1,
                IsSystemAccount = true
            };
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }
        return account;
    }

    public async Task<IEnumerable<PestroBatch>> GetAllBatchesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PestroBatches.Include(b => b.Product).ToListAsync();
    }

    public async Task<IEnumerable<PestroBatch>> GetActiveBatchesAsync(int productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PestroBatches
            .Where(b => b.ProductId == productId && b.Status == BatchStatus.Active && b.RemainingQty > 0)
            .OrderBy(b => b.ExpiryDate)
            .ToListAsync();
    }

    public async Task SaveBatchAsync(PestroBatch batch)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (batch.Id == 0) context.PestroBatches.Add(batch);
        else context.PestroBatches.Update(batch);
        await context.SaveChangesAsync();
    }
}
