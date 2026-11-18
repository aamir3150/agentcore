using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Common;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class BrokerageService : IBrokerageService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;
    private readonly IStockService _stockService;

    public BrokerageService(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService, IStockService stockService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
        _stockService = stockService;
    }

    public async Task<BrokerageInvoice?> GetBrokerageInvoiceByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.BrokerageInvoices
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Include(i => i.Contract)
            .Include(i => i.Party)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<BrokerageInvoice?> GetBrokerageInvoiceByNoAsync(string invoiceNo, InvoiceType invoiceType)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.BrokerageInvoices
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Include(i => i.Contract)
            .Include(i => i.Party)
            .FirstOrDefaultAsync(i => i.InvoiceNo == invoiceNo && i.InvoiceType == invoiceType);
    }

    public async Task<IEnumerable<BrokerageInvoice>> GetAllBrokerageInvoicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.BrokerageInvoices
            .Include(i => i.Party)
            .Include(i => i.Details)
            .ToListAsync();
    }

    public async Task SaveBrokerageInvoiceAsync(BrokerageInvoice invoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (invoice.Id == 0)
        {
            if (string.IsNullOrWhiteSpace(invoice.InvoiceNo))
            {
                invoice.InvoiceNo = await _systemService.GetNextDocumentNoAsync(DocumentType.BrokerageInvoice, invoice.FinancialYearId);
            }
            context.BrokerageInvoices.Add(invoice);
        }
        else
        {
            context.BrokerageInvoices.Update(invoice);
        }
        await context.SaveChangesAsync();

        // Sync with legacy AgenSoft tables (ContractInvoice & ContractInvoiceBody)
        await SyncLegacyBrokerageInvoiceAsync(context, invoice);
    }

    public async Task DeleteBrokerageInvoiceAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.BrokerageInvoices
            .Include(i => i.Details)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null) return;

        // 1. Revert and delete General Ledger entries
        var glEntries = await context.GeneralLedger
            .Where(g => g.SourceType == SourceType.BrkInvoice && g.SourceId == id)
            .ToListAsync();
        if (glEntries.Any())
        {
            context.GeneralLedger.RemoveRange(glEntries);
        }

        // 2. Revert and delete Stock Transactions
        var stockEntries = await context.StockTransactions
            .Where(s => s.SourceModule == SourceModule.Brokerage && s.SourceInvoiceId == id)
            .ToListAsync();
        if (stockEntries.Any())
        {
            context.StockTransactions.RemoveRange(stockEntries);
        }

        // 3. Delete from Agensoft legacy tables
        int purId = 0;
        if (!int.TryParse(invoice.InvoiceNo, out purId))
        {
            purId = invoice.Id;
        }

        var sqlLegacy = @"
            IF OBJECT_ID('ContractInvoiceBody', 'U') IS NOT NULL
                DELETE FROM ContractInvoiceBody WHERE ContractPurId = {0};
            IF OBJECT_ID('ContractInvoice', 'U') IS NOT NULL
                DELETE FROM ContractInvoice WHERE ContractPurId = {0};
            IF OBJECT_ID('MultiBody', 'U') IS NOT NULL
                DELETE FROM MultiBody WHERE PurchaseID = {0} OR SaleID = {0};
            IF OBJECT_ID('MultiInvoice', 'U') IS NOT NULL
                DELETE FROM MultiInvoice WHERE PurchaseID = {0} OR SaleID = {0};
        ";
        await context.Database.ExecuteSqlRawAsync(sqlLegacy, purId);

        // 4. Delete EF Entities
        context.BrokerageInvoiceDetails.RemoveRange(invoice.Details);
        context.BrokerageInvoices.Remove(invoice);

        await context.SaveChangesAsync();
    }

    private async Task SyncLegacyBrokerageInvoiceAsync(AppDbContext context, BrokerageInvoice invoice)
    {
        try
        {
            var party = await context.Parties.Include(p => p.Account).FirstOrDefaultAsync(p => p.Id == invoice.PartyId);
            var partyIdStr = party?.PartyNo ?? party?.Account?.AccountNo ?? invoice.PartyId.ToString();

            var season = await context.CropSeasons.FirstOrDefaultAsync(s => s.Id == invoice.CropSeasonId);
            var seasonStr = season?.Name ?? "";

            int purId = 0;
            if (!int.TryParse(invoice.InvoiceNo, out purId))
            {
                purId = invoice.Id;
            }

            var isSale = invoice.InvoiceType == InvoiceType.BrkSale ? 1 : 0;

            // Delete previous legacy records if any
            var sqlDelete = @"
                IF OBJECT_ID('ContractInvoiceBody', 'U') IS NOT NULL
                    DELETE FROM ContractInvoiceBody WHERE ContractPurId = {0};
                IF OBJECT_ID('ContractInvoice', 'U') IS NOT NULL
                    DELETE FROM ContractInvoice WHERE ContractPurId = {0};
            ";
            await context.Database.ExecuteSqlRawAsync(sqlDelete, purId);

            // Insert master ContractInvoice
            var sqlMaster = @"
                IF OBJECT_ID('ContractInvoice', 'U') IS NOT NULL
                BEGIN
                    INSERT INTO ContractInvoice (
                        ContractPurId, CDate, PartyID, IsSaleInvoice, IsReached, IsPay,
                        CommissionValue, WHTax, Bardana, NetValue,
                        CurDate, Narration, CropSeason, BranchID, IDBranch, IsShortInvoice
                    )
                    VALUES (
                        {0}, {1}, {2}, {3}, 0, 1,
                        {4}, {5}, {6}, {7},
                        GETDATE(), {8}, {9}, 1, {0}, 0
                    );
                END";

            await context.Database.ExecuteSqlRawAsync(sqlMaster,
                purId,
                invoice.InvoiceDate,
                partyIdStr,
                isSale,
                invoice.CommissionAmount,
                invoice.WHT_Amount,
                invoice.LaborCharges,
                invoice.NetAmount,
                $"{(invoice.InvoiceType == InvoiceType.BrkPurchase ? "Purchase" : "Sale")} Inv #{invoice.InvoiceNo}",
                seasonStr
            );

            // Insert details into ContractInvoiceBody
            int srNo = 1;
            foreach (var detail in invoice.Details)
            {
                var product = await context.Products.FirstOrDefaultAsync(p => p.Id == detail.ProductId);
                var prodCode = product?.ProductCode ?? detail.ProductId.ToString();

                var sqlBody = @"
                    IF OBJECT_ID('ContractInvoiceBody', 'U') IS NOT NULL
                    BEGIN
                        INSERT INTO ContractInvoiceBody (
                            ContractPurId, ProductID, TruckQty, BagsQty, WeightPerBag,
                            RateType, RatePerMound, Bardana, TotalValue, TotalKgs,
                            IsSaleInvoice, VehicleNo, Shortage, Deduction, MoundStd, SerialNo
                        )
                        VALUES (
                            {0}, {1}, 1, {2}, {3},
                            'Rate/Mound', {4}, 0, {5}, {6},
                            {7}, {8}, {9}, {10}, 40, {11}
                        );
                    END";

                decimal wtPerBag = detail.Bags > 0 ? (detail.GrossWeight / detail.Bags) : 50;

                await context.Database.ExecuteSqlRawAsync(sqlBody,
                    purId,
                    prodCode,
                    detail.Bags,
                    wtPerBag,
                    detail.Rate,
                    detail.Amount,
                    detail.NetWeight,
                    isSale,
                    invoice.VehicleNo ?? "",
                    detail.TareWeight,
                    0,
                    srNo++
                );
            }
        }
        catch
        {
            // Logging can be captured by higher layers
        }
    }

    public async Task SyncLegacyMultiInvoiceAsync(string multiId, BrokerageInvoice? purchaseInvoice, BrokerageInvoice? saleInvoice, decimal profitLoss, string plsAccount = "40001")
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        try
        {
            long mId = 0;
            if (!long.TryParse(multiId, out mId)) mId = 1;

            int pId = 0;
            if (purchaseInvoice != null && !int.TryParse(purchaseInvoice.InvoiceNo, out pId)) pId = purchaseInvoice.Id;

            int sId = 0;
            if (saleInvoice != null && !int.TryParse(saleInvoice.InvoiceNo, out sId)) sId = saleInvoice.Id;

            var vendor = purchaseInvoice != null ? await context.Parties.FirstOrDefaultAsync(p => p.Id == purchaseInvoice.PartyId) : null;
            var vendorNo = vendor?.PartyNo ?? "";

            var customer = saleInvoice != null ? await context.Parties.FirstOrDefaultAsync(p => p.Id == saleInvoice.PartyId) : null;
            var customerNo = customer?.PartyNo ?? "";

            int targetSeasonId = purchaseInvoice?.CropSeasonId ?? saleInvoice?.CropSeasonId ?? 1;
            var season = await context.CropSeasons.FirstOrDefaultAsync(s => s.Id == targetSeasonId);
            var seasonStr = season?.Name ?? "";

            DateTime mDate = purchaseInvoice?.InvoiceDate ?? saleInvoice?.InvoiceDate ?? DateTime.Today;
            decimal grossP = purchaseInvoice?.GrossAmount ?? 0;
            decimal grossS = saleInvoice?.GrossAmount ?? 0;
            decimal netP = purchaseInvoice?.NetAmount ?? 0;
            decimal netS = saleInvoice?.NetAmount ?? 0;

            // 1. Delete prior records
            var sqlDelete = @"
                IF OBJECT_ID('MultiBody', 'U') IS NOT NULL
                    DELETE FROM MultiBody WHERE MultiID = {0} OR (PurchaseID = {1} AND SaleID = {2});
                IF OBJECT_ID('MultiInvoice', 'U') IS NOT NULL
                    DELETE FROM MultiInvoice WHERE MultiID = {0} OR (PurchaseID = {1} AND SaleID = {2});
            ";
            await context.Database.ExecuteSqlRawAsync(sqlDelete, mId, pId, sId);

            // 2. Insert master MultiInvoice
            var sqlMulti = @"
                IF OBJECT_ID('MultiInvoice', 'U') IS NOT NULL
                BEGIN
                    INSERT INTO MultiInvoice (
                        MultiID, MultiDate, PurchaseID, SaleID,
                        VendorID, CustomerID, GrossAmountP, GrossAmountS,
                        NetAmountP, NetAmountS, IsMultiNew, PLSAccount,
                        CropSeason, BranchID, IDBranch
                    )
                    VALUES (
                        {0}, {1}, {2}, {3},
                        {4}, {5}, {6}, {7},
                        {8}, {9}, 1, {10},
                        {11}, 1, {0}
                    );
                END";

            await context.Database.ExecuteSqlRawAsync(sqlMulti,
                mId, mDate, pId, sId,
                vendorNo, customerNo, grossP, grossS,
                netP, netS, plsAccount, seasonStr
            );

            // 3. Insert line items into MultiBody
            int sr = 1;
            var allDetails = (purchaseInvoice?.Details ?? Enumerable.Empty<BrokerageInvoiceDetail>())
                .Concat(saleInvoice?.Details ?? Enumerable.Empty<BrokerageInvoiceDetail>());

            foreach (var d in allDetails)
            {
                var product = await context.Products.FirstOrDefaultAsync(p => p.Id == d.ProductId);
                var prodCode = product?.ProductCode ?? d.ProductId.ToString();

                var sqlBody = @"
                    IF OBJECT_ID('MultiBody', 'U') IS NOT NULL
                    BEGIN
                        INSERT INTO MultiBody (
                            MultiID, PurchaseID, SaleID, ProductID, Multiplier,
                            MDate, VehicleNo, GWeight, TWeight, NWeight,
                            MRate, GRate, AmountP, AmountS, NetAmountP, NetAmountS,
                            IsDeleted, SrNo
                        )
                        VALUES (
                            {0}, {1}, {2}, {3}, 40,
                            {4}, {5}, {6}, {7}, {8},
                            {9}, {9}, {10}, {11}, {10}, {11},
                            0, {12}
                        );
                    END";

                await context.Database.ExecuteSqlRawAsync(sqlBody,
                    mId, pId, sId, prodCode,
                    mDate, purchaseInvoice?.VehicleNo ?? saleInvoice?.VehicleNo ?? "",
                    d.GrossWeight, d.TareWeight, d.NetWeight,
                    d.Rate, grossP, grossS, sr++
                );
            }

            // 4. Link into ContractInvoice and ContractInvoiceBody
            var sqlLink = @"
                IF OBJECT_ID('ContractInvoice', 'U') IS NOT NULL
                BEGIN
                    UPDATE ContractInvoice 
                    SET MultiEntryID = {0}, PurchaseID = {1}, SaleID = {2}
                    WHERE ContractPurId IN ({1}, {2});
                END
                IF OBJECT_ID('ContractInvoiceBody', 'U') IS NOT NULL
                BEGIN
                    UPDATE ContractInvoiceBody 
                    SET PurchaseID = {1}, SaleID = {2}
                    WHERE ContractPurId IN ({1}, {2});
                END";
            await context.Database.ExecuteSqlRawAsync(sqlLink, mId.ToString(), pId, sId);
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BrokerageService] SyncLegacyMultiInvoice error: {ex.Message}");
        }
    }

    public async Task<(BrokerageInvoice? Purchase, BrokerageInvoice? Sale, string PlsAccount, decimal ProfitLoss)> GetMultiInvoiceDataAsync(string multiId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        long mId = 0;
        long.TryParse(multiId, out mId);

        int pId = 0;
        int sId = 0;
        string pls = "40001";

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"IF OBJECT_ID('MultiInvoice', 'U') IS NOT NULL SELECT TOP 1 PurchaseID, SaleID, ISNULL(PLSAccount, '40001') FROM MultiInvoice WHERE MultiID = {mId} ELSE SELECT 0, 0, '40001' WHERE 1=0";
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                pId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                sId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
                pls = reader.IsDBNull(2) ? "40001" : reader.GetString(2);
            }
        }
        catch
        {
            // fallback
        }

        BrokerageInvoice? pInv = null;
        BrokerageInvoice? sInv = null;

        if (pId > 0)
        {
            pInv = await context.BrokerageInvoices
                .Include(i => i.Details).ThenInclude(d => d.Product)
                .Include(i => i.Party)
                .FirstOrDefaultAsync(i => i.InvoiceNo == pId.ToString() || i.Id == pId);
        }
        if (sId > 0)
        {
            sInv = await context.BrokerageInvoices
                .Include(i => i.Details).ThenInclude(d => d.Product)
                .Include(i => i.Party)
                .FirstOrDefaultAsync(i => i.InvoiceNo == sId.ToString() || i.Id == sId);
        }

        if (pInv == null && sInv == null)
        {
            pInv = await context.BrokerageInvoices
                .Include(i => i.Details).ThenInclude(d => d.Product)
                .Include(i => i.Party)
                .FirstOrDefaultAsync(i => i.InvoiceNo == multiId && i.InvoiceType == InvoiceType.BrkPurchase);

            sInv = await context.BrokerageInvoices
                .Include(i => i.Details).ThenInclude(d => d.Product)
                .Include(i => i.Party)
                .FirstOrDefaultAsync(i => i.InvoiceNo == multiId && i.InvoiceType == InvoiceType.BrkSale);
        }

        decimal diff = (sInv?.NetAmount ?? 0) - (pInv?.NetAmount ?? 0);
        return (pInv, sInv, pls, diff);
    }

    public async Task<List<BrokerageMultiInvoiceSummaryDto>> GetAllMultiInvoicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var list = new List<BrokerageMultiInvoiceSummaryDto>();

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = @"
                IF OBJECT_ID('MultiInvoice', 'U') IS NOT NULL
                BEGIN
                    SELECT 
                        m.MultiID, m.PurchaseID, m.SaleID, m.MultiDate,
                        ISNULL(p1.PartyName, m.VendorID) AS SupplierName,
                        ISNULL(p2.PartyName, m.CustomerID) AS CustomerName,
                        ISNULL(m.NetAmountP, 0) AS NetP,
                        ISNULL(m.NetAmountS, 0) AS NetS,
                        ISNULL(m.PLSAccount, '40001') AS PLSAccount
                    FROM MultiInvoice m
                    LEFT JOIN Parties p1 ON m.VendorID = p1.PartyID OR m.VendorID = p1.PartyNo
                    LEFT JOIN Parties p2 ON m.CustomerID = p2.PartyID OR m.CustomerID = p2.PartyNo
                    ORDER BY m.MultiID DESC;
                END";

            using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dto = new BrokerageMultiInvoiceSummaryDto
                {
                    MultiId = reader[0]?.ToString() ?? "",
                    PurchaseId = reader[1]?.ToString() ?? "",
                    SaleId = reader[2]?.ToString() ?? "",
                    Date = reader.IsDBNull(3) ? DateTime.Today : Convert.ToDateTime(reader[3]),
                    SupplierName = reader[4]?.ToString() ?? "",
                    CustomerName = reader[5]?.ToString() ?? "",
                    NetPurchase = reader.IsDBNull(6) ? 0 : Convert.ToDecimal(reader[6]),
                    NetSale = reader.IsDBNull(7) ? 0 : Convert.ToDecimal(reader[7]),
                    PlsAccount = reader[8]?.ToString() ?? "40001"
                };
                dto.ProfitLoss = dto.NetSale - dto.NetPurchase;
                list.Add(dto);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[BrokerageService] GetAllMultiInvoices error: {ex.Message}");
        }

        // Fallback: If MultiInvoice table has no rows, build from paired BrokerageInvoices
        if (!list.Any())
        {
            var invoices = await context.BrokerageInvoices.Include(i => i.Party).ToListAsync();
            var grouped = invoices.GroupBy(i => i.InvoiceNo).Where(g => g.Count() >= 2 || g.Any(x => x.InvoiceType == InvoiceType.BrkPurchase));
            foreach (var g in grouped)
            {
                var p = g.FirstOrDefault(i => i.InvoiceType == InvoiceType.BrkPurchase);
                var s = g.FirstOrDefault(i => i.InvoiceType == InvoiceType.BrkSale);
                if (p != null)
                {
                    list.Add(new BrokerageMultiInvoiceSummaryDto
                    {
                        MultiId = g.Key,
                        PurchaseId = p.InvoiceNo,
                        SaleId = s?.InvoiceNo ?? p.InvoiceNo,
                        Date = p.InvoiceDate,
                        SupplierName = p.Party?.Name ?? "",
                        CustomerName = s?.Party?.Name ?? "",
                        NetPurchase = p.NetAmount,
                        NetSale = s?.NetAmount ?? 0,
                        ProfitLoss = (s?.NetAmount ?? 0) - p.NetAmount,
                        PlsAccount = "40001"
                    });
                }
            }
        }

        return list;
    }

    public async Task DeleteMultiInvoiceAsync(string multiId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        long mId = 0;
        long.TryParse(multiId, out mId);

        int pId = 0;
        int sId = 0;

        try
        {
            var conn = context.Database.GetDbConnection();
            if (conn.State != ConnectionState.Open) await conn.OpenAsync();

            using var cmd = conn.CreateCommand();
            cmd.CommandText = $"IF OBJECT_ID('MultiInvoice', 'U') IS NOT NULL SELECT TOP 1 PurchaseID, SaleID FROM MultiInvoice WHERE MultiID = {mId}";
            using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                pId = reader.IsDBNull(0) ? 0 : reader.GetInt32(0);
                sId = reader.IsDBNull(1) ? 0 : reader.GetInt32(1);
            }
        }
        catch { }

        // Clean from Multi tables
        var sqlClean = @"
            IF OBJECT_ID('MultiBody', 'U') IS NOT NULL
                DELETE FROM MultiBody WHERE MultiID = {0} OR (PurchaseID = {1} AND SaleID = {2});
            IF OBJECT_ID('MultiInvoice', 'U') IS NOT NULL
                DELETE FROM MultiInvoice WHERE MultiID = {0} OR (PurchaseID = {1} AND SaleID = {2});
        ";
        await context.Database.ExecuteSqlRawAsync(sqlClean, mId, pId, sId);

        // Delete underlying invoices
        if (pId > 0)
        {
            var pInv = await context.BrokerageInvoices.FirstOrDefaultAsync(i => i.InvoiceNo == pId.ToString() || i.Id == pId);
            if (pInv != null) await DeleteBrokerageInvoiceAsync(pInv.Id);
        }
        if (sId > 0 && sId != pId)
        {
            var sInv = await context.BrokerageInvoices.FirstOrDefaultAsync(i => i.InvoiceNo == sId.ToString() || i.Id == sId);
            if (sInv != null) await DeleteBrokerageInvoiceAsync(sInv.Id);
        }
        else if (pId == 0 && sId == 0)
        {
            var invoices = await context.BrokerageInvoices.Where(i => i.InvoiceNo == multiId).ToListAsync();
            foreach (var inv in invoices)
            {
                await DeleteBrokerageInvoiceAsync(inv.Id);
            }
        }
    }

    public async Task PostBrokerageInvoiceToGLAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.BrokerageInvoices
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Include(i => i.Party)
            .FirstOrDefaultAsync(i => i.Id == id);

        if (invoice == null || invoice.IsPosted) return;

        // Balanced Double Entry GL Posting matching AgenSOFT Mandi Accounting
        var brkTradeAccount = await GetOrCreateSystemAccount(context, 
            invoice.InvoiceType == InvoiceType.BrkPurchase ? "Brokerage Grain Purchases" : "Brokerage Grain Sales", 
            invoice.InvoiceType == InvoiceType.BrkPurchase ? AccountType.Expense : AccountType.Revenue);
        var commissionAccount = await GetOrCreateSystemAccount(context, "Commission Income", AccountType.Revenue);
        var whtAccount = await GetOrCreateSystemAccount(context, "WHT Payable", AccountType.Liability);
        var laborAccount = await GetOrCreateSystemAccount(context, "Labor Charges Payable", AccountType.Liability);
        var mcAccount = await GetOrCreateSystemAccount(context, "Market Committee Fee Payable", AccountType.Liability);

        // 1. Party Ledger Entry (Net Amount)
        var partyEntry = new GeneralLedger
        {
            TransactionDate = invoice.InvoiceDate,
            AccountId = invoice.Party.AccountId,
            Narration = $"{(invoice.InvoiceType == InvoiceType.BrkPurchase ? "Brk Purchase" : "Brk Sale")} Inv: {invoice.InvoiceNo}",
            SourceType = SourceType.BrkInvoice,
            SourceId = invoice.Id,
            FinancialYearId = invoice.FinancialYearId,
            CropSeasonId = invoice.CropSeasonId
        };

        if (invoice.InvoiceType == InvoiceType.BrkPurchase)
        {
            partyEntry.Credit = invoice.NetAmount;
        }
        else
        {
            partyEntry.Debit = invoice.NetAmount;
        }
        context.GeneralLedger.Add(partyEntry);

        // 2. Grain Trade Entry (Gross Amount)
        var tradeEntry = new GeneralLedger
        {
            TransactionDate = invoice.InvoiceDate,
            AccountId = brkTradeAccount.Id,
            Narration = $"Trade Value - {invoice.InvoiceNo}",
            SourceType = SourceType.BrkInvoice,
            SourceId = invoice.Id,
            FinancialYearId = invoice.FinancialYearId,
            CropSeasonId = invoice.CropSeasonId
        };

        if (invoice.InvoiceType == InvoiceType.BrkPurchase)
        {
            tradeEntry.Debit = invoice.GrossAmount;
        }
        else
        {
            tradeEntry.Credit = invoice.GrossAmount;
        }
        context.GeneralLedger.Add(tradeEntry);

        // 3. Commission (Arhat / Dami) Entry
        if (invoice.CommissionAmount > 0)
        {
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = commissionAccount.Id,
                Narration = $"Commission on {invoice.InvoiceNo}",
                SourceType = SourceType.BrkInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId,
                Debit = invoice.InvoiceType == InvoiceType.BrkPurchase ? 0 : invoice.CommissionAmount,
                Credit = invoice.InvoiceType == InvoiceType.BrkPurchase ? invoice.CommissionAmount : 0
            });
        }

        // 4. WHT Entry
        if (invoice.WHT_Amount > 0)
        {
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = whtAccount.Id,
                Narration = $"WHT on {invoice.InvoiceNo}",
                SourceType = SourceType.BrkInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId,
                Debit = invoice.InvoiceType == InvoiceType.BrkPurchase ? 0 : invoice.WHT_Amount,
                Credit = invoice.InvoiceType == InvoiceType.BrkPurchase ? invoice.WHT_Amount : 0
            });
        }

        // 5. Labor Charges Entry
        if (invoice.LaborCharges > 0)
        {
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = laborAccount.Id,
                Narration = $"Labor charges on {invoice.InvoiceNo}",
                SourceType = SourceType.BrkInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId,
                Debit = invoice.InvoiceType == InvoiceType.BrkPurchase ? 0 : invoice.LaborCharges,
                Credit = invoice.InvoiceType == InvoiceType.BrkPurchase ? invoice.LaborCharges : 0
            });
        }

        // 6. Market Committee Fee Entry
        if (invoice.MarketCommitteeFee > 0)
        {
            context.GeneralLedger.Add(new GeneralLedger
            {
                TransactionDate = invoice.InvoiceDate,
                AccountId = mcAccount.Id,
                Narration = $"Market committee fee on {invoice.InvoiceNo}",
                SourceType = SourceType.BrkInvoice,
                SourceId = invoice.Id,
                FinancialYearId = invoice.FinancialYearId,
                CropSeasonId = invoice.CropSeasonId,
                Debit = invoice.InvoiceType == InvoiceType.BrkPurchase ? 0 : invoice.MarketCommitteeFee,
                Credit = invoice.InvoiceType == InvoiceType.BrkPurchase ? invoice.MarketCommitteeFee : 0
            });
        }

        // 7. Stock Transactions (Per Line Item)
        foreach (var detail in invoice.Details)
        {
            var stockTx = new StockTransaction
            {
                TransactionDate = invoice.InvoiceDate,
                ProductId = detail.ProductId,
                FinancialYearId = invoice.FinancialYearId,
                SourceModule = SourceModule.Brokerage,
                SourceInvoiceId = invoice.Id,
                Rate = detail.Rate,
                Narration = $"Brk {(invoice.InvoiceType == InvoiceType.BrkPurchase ? "Pur" : "Sale")} Inv: {invoice.InvoiceNo}"
            };

            if (invoice.InvoiceType == InvoiceType.BrkPurchase)
            {
                stockTx.TransactionType = StockTransactionType.Purchase;
                stockTx.QtyIn = detail.NetWeight > 0 ? detail.NetWeight : detail.Bags;
            }
            else
            {
                stockTx.TransactionType = StockTransactionType.Sale;
                stockTx.QtyOut = detail.NetWeight > 0 ? detail.NetWeight : detail.Bags;
            }

            await _stockService.AddStockTransactionAsync(stockTx);
        }

        invoice.IsPosted = true;
        invoice.Status = InvoiceStatus.Posted;
        await context.SaveChangesAsync();
    }

    private async Task<Account> GetOrCreateSystemAccount(AppDbContext context, string name, AccountType type)
    {
        var account = await context.Accounts.FirstOrDefaultAsync(a => a.Name == name);
        if (account == null)
        {
            account = new Account { Name = name, AccountNo = "SYS-" + name.Replace(" ", ""), AccountType = type, AccountGroupId = 1, IsSystemAccount = true };
            context.Accounts.Add(account);
            await context.SaveChangesAsync();
        }
        return account;
    }

    public async Task<Contract?> GetContractByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Contracts.Include(c => c.Product).Include(c => c.Party).FirstOrDefaultAsync(c => c.Id == id);
    }

    public async Task<IEnumerable<Contract>> GetAllContractsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Contracts.Include(c => c.Party).Include(c => c.Product).ToListAsync();
    }

    public async Task SaveContractAsync(Contract contract)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (contract.Id == 0)
        {
            contract.ContractNo = await GetNextContractNoAsync(contract.FinancialYearId);
            context.Contracts.Add(contract);
        }
        else
        {
            context.Contracts.Update(contract);
        }
        await context.SaveChangesAsync();
    }

    public async Task<string> GetNextContractNoAsync(int financialYearId)
    {
        return await _systemService.GetNextDocumentNoAsync(DocumentType.Contract, financialYearId);
    }

    public async Task<IEnumerable<GatePass>> GetAllGatePassesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GatePasses.Include(g => g.Party).Include(g => g.Product).ToListAsync();
    }

    public async Task<IEnumerable<GatePass>> GetGatePassesByStatusAsync(GatePassStatus status)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GatePasses.Include(g => g.Party).Include(g => g.Product).Where(g => g.Status == status).ToListAsync();
    }

    public async Task SaveGatePassAsync(GatePass gatePass)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (gatePass.Id == 0)
        {
            gatePass.GatePassNo = await GetNextGatePassNoAsync();
            context.GatePasses.Add(gatePass);
        }
        else
        {
            context.GatePasses.Update(gatePass);
        }
        await context.SaveChangesAsync();
    }

    public async Task<string> GetNextGatePassNoAsync()
    {
        // GatePass doesn't always need financial year if it's across years, but for consistency:
        var year = await _systemService.GetCurrentYearAsync();
        return await _systemService.GetNextDocumentNoAsync(DocumentType.GatePass, year?.Id ?? 0);
    }
}
