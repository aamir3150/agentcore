using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class ReportService : IReportService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public ReportService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<dynamic>> GetLedgerReportAsync(int accountId, DateTime? from = null, DateTime? to = null, bool withoutOpening = false, bool isUrdu = false)
    {
        using var context = await _contextFactory.CreateDbContextAsync();

        var account = await context.Accounts.FindAsync(accountId);
        decimal baseOpening = account?.OpeningBalance ?? 0m;

        bool isFromBounded = from.HasValue && from.Value > new DateTime(1900, 1, 1);
        bool isToBounded = to.HasValue && to.Value < new DateTime(2900, 1, 1);

        DateTime? startDate = isFromBounded ? from!.Value.Date : null;
        DateTime? endDate = isToBounded ? to!.Value : null;

        decimal openingBalance = baseOpening;

        if (isFromBounded)
        {
            var priorDebit = await context.GeneralLedger
                .Where(g => g.AccountId == accountId && g.TransactionDate < startDate!.Value)
                .SumAsync(g => (decimal?)g.Debit) ?? 0m;

            var priorCredit = await context.GeneralLedger
                .Where(g => g.AccountId == accountId && g.TransactionDate < startDate!.Value)
                .SumAsync(g => (decimal?)g.Credit) ?? 0m;

            openingBalance += (priorDebit - priorCredit);
        }

        var query = context.GeneralLedger
            .Include(g => g.Account)
            .Where(g => g.AccountId == accountId);

        if (isFromBounded)
        {
            query = query.Where(g => g.TransactionDate >= startDate!.Value);
        }

        if (isToBounded)
        {
            query = query.Where(g => g.TransactionDate <= endDate!.Value);
        }

        var rawTransactions = await query
            .OrderBy(g => g.TransactionDate)
            .ThenBy(g => g.Id)
            .Select(g => new
            {
                g.TransactionDate,
                Narration = g.Narration ?? "",
                g.Debit,
                g.Credit,
                g.SourceType,
                g.SourceId
            })
            .ToListAsync();

        // 1. Resolve Voucher Numbers
        var voucherIds = rawTransactions
            .Where(t => t.SourceType == SourceType.Voucher && t.SourceId > 0)
            .Select(t => t.SourceId)
            .Distinct()
            .ToList();

        var vouchers = voucherIds.Count > 0
            ? await context.Vouchers.Where(v => voucherIds.Contains(v.Id)).ToDictionaryAsync(v => v.Id, v => v.VoucherNo)
            : new Dictionary<int, string>();

        // 2. Resolve Invoice Numbers (for General / Brokerage / Pestro Invoices)
        var invoiceIds = rawTransactions
            .Where(t => t.SourceType != SourceType.Voucher && t.SourceId > 0)
            .Select(t => t.SourceId)
            .Distinct()
            .ToList();

        var invoices = invoiceIds.Count > 0
            ? await context.Invoices.Where(i => invoiceIds.Contains(i.Id)).ToDictionaryAsync(i => i.Id, i => i.InvoiceNo)
            : new Dictionary<int, string>();

        static string FormatBalance(decimal bal)
        {
            if (bal == 0m) return "0";
            return bal > 0m ? $"{bal:N0} Dr" : $"{Math.Abs(bal):N0} Cr";
        }

        var reportData = new List<LedgerReportItem>();
        decimal runningBalance = 0m;

        // 3. Opening Balance Row
        if (!withoutOpening)
        {
            DateTime opDate;
            if (isFromBounded)
            {
                opDate = startDate!.Value > DateTime.MinValue.AddDays(1) ? startDate!.Value.AddDays(-1) : startDate!.Value;
            }
            else
            {
                opDate = rawTransactions.Count > 0 ? rawTransactions[0].TransactionDate.Date : DateTime.Today;
            }

            string narration = isUrdu ? "سابقہ بقایا / اوپننگ بیلنس" : "Opening Balance";
            string vNo = isUrdu ? "سابقہ" : "OPEN";

            decimal opDebit = openingBalance > 0 ? openingBalance : 0m;
            decimal opCredit = openingBalance < 0 ? Math.Abs(openingBalance) : 0m;
            runningBalance = openingBalance;

            reportData.Add(new LedgerReportItem
            {
                VoucherNo = vNo,
                TransactionDate = opDate,
                Narration = narration,
                Debit = opDebit,
                Credit = opCredit,
                Balance = FormatBalance(runningBalance)
            });
        }

        // 4. Transaction Rows with Running Balance
        foreach (var t in rawTransactions)
        {
            string vNo;
            if (t.SourceType == SourceType.Voucher && vouchers.TryGetValue(t.SourceId, out var vn) && !string.IsNullOrWhiteSpace(vn))
            {
                vNo = vn;
            }
            else if (invoices.TryGetValue(t.SourceId, out var invNo) && !string.IsNullOrWhiteSpace(invNo))
            {
                vNo = invNo;
            }
            else
            {
                vNo = $"{t.SourceType}-{t.SourceId}";
            }

            runningBalance += (t.Debit - t.Credit);

            reportData.Add(new LedgerReportItem
            {
                VoucherNo = vNo,
                TransactionDate = t.TransactionDate,
                Narration = t.Narration,
                Debit = t.Debit,
                Credit = t.Credit,
                Balance = FormatBalance(runningBalance)
            });
        }

        return reportData;
    }

    public async Task<IEnumerable<dynamic>> GetDayBookReportAsync(DateTime date)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        bool isAllDates = date >= new DateTime(2900, 1, 1);
        var query = context.GeneralLedger.Include(g => g.Account).AsQueryable();

        if (!isAllDates)
        {
            var targetDate = date.Date;
            var nextDate = targetDate.AddDays(1);
            query = query.Where(g => g.TransactionDate >= targetDate && g.TransactionDate < nextDate);
        }

        return await query
            .OrderBy(g => g.Id)
            .Select(g => new
            {
                g.TransactionDate,
                AccountName = g.Account.Name,
                g.Narration,
                g.Debit,
                g.Credit,
                Reference = g.SourceType.ToString() + "-" + g.SourceId
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetStockReportAsync(int? productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.StockTransactions.Include(s => s.Product).AsQueryable();
        
        if (productId.HasValue)
            query = query.Where(s => s.ProductId == productId.Value);

        return await query
            .GroupBy(s => new { s.ProductId, s.Product.Name, s.Product.ProductCode })
            .Select(g => new
            {
                g.Key.ProductCode,
                g.Key.Name,
                TotalIn = g.Sum(x => x.QtyIn),
                TotalOut = g.Sum(x => x.QtyOut),
                CurrentStock = g.Sum(x => x.QtyIn) - g.Sum(x => x.QtyOut)
            })
            .ToListAsync();
    }
    public async Task<IEnumerable<dynamic>> GetPurchaseRegisterAsync(DateTime from, DateTime to)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Invoices
            .Include(i => i.Party)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && (i.InvoiceType == InvoiceType.GenPurchase || i.InvoiceType == InvoiceType.GenPurchaseReturn))
            .OrderBy(i => i.InvoiceDate)
            .Select(i => new
            {
                i.InvoiceNo,
                i.InvoiceDate,
                PartyName = i.Party.Name,
                i.SubTotal,
                i.TaxAmount,
                i.NetAmount,
                i.Status
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetSaleRegisterAsync(DateTime from, DateTime to)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Invoices
            .Include(i => i.Party)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && (i.InvoiceType == InvoiceType.GenSale || i.InvoiceType == InvoiceType.GenSaleReturn))
            .OrderBy(i => i.InvoiceDate)
            .Select(i => new
            {
                i.InvoiceNo,
                i.InvoiceDate,
                PartyName = i.Party.Name,
                i.SubTotal,
                i.TaxAmount,
                i.NetAmount,
                i.Status
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetBrokerageCommissionAsync(DateTime from, DateTime to)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.BrokerageInvoices
            .Include(i => i.Party)
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.IsPosted)
            .OrderBy(i => i.InvoiceDate)
            .Select(i => new
            {
                i.InvoiceNo,
                i.InvoiceDate,
                PartyName = i.Party.Name,
                i.GrossAmount,
                i.CommissionAmount,
                i.Soodh_Amount,
                i.LaborCharges,
                i.NetAmount
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetGatePassRegisterAsync(DateTime from, DateTime to)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GatePasses
            .Include(g => g.Party)
            .Include(g => g.Product)
            .Where(g => g.GatePassDate >= from && g.GatePassDate <= to)
            .OrderBy(g => g.GatePassDate)
            .Select(g => new
            {
                g.GatePassNo,
                g.GatePassDate,
                PartyName = g.Party.Name,
                ProductName = g.Product.Name,
                g.GrossWeight,
                g.NetWeight,
                g.VehicleNo
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetPestroExpiryReportAsync(int daysToExpiry)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var thresholdDate = DateTime.Today.AddDays(daysToExpiry);
        return await context.PestroBatches
            .Include(b => b.Product)
            .Where(b => b.ExpiryDate != null && b.ExpiryDate <= thresholdDate)
            .OrderBy(b => b.ExpiryDate)
            .Select(b => new
            {
                b.BatchNo,
                ProductName = b.Product.Name,
                b.ManufactureDate,
                b.ExpiryDate,
                DaysRemaining = b.ExpiryDate.HasValue ? (b.ExpiryDate.Value - DateTime.Today).TotalDays : 0
            })
            .ToListAsync();
    }

    public async Task<IEnumerable<dynamic>> GetProfitabilityReportAsync(DateTime from, DateTime to, string type)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        var safeFrom = from < new DateTime(1900, 1, 1) ? new DateTime(1900, 1, 1) : from;
        var safeTo = to > new DateTime(2900, 1, 1) ? new DateTime(2900, 1, 1) : to;

        // This is a simplified calculation of Gross Profit
        var salesTotal = await context.Invoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo && i.InvoiceType == InvoiceType.GenSale)
            .SumAsync(i => (decimal?)i.NetAmount) ?? 0;
            
        var purchasesTotal = await context.Invoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo && i.InvoiceType == InvoiceType.GenPurchase)
            .SumAsync(i => (decimal?)i.NetAmount) ?? 0;
            
        var brkCommission = await context.BrokerageInvoices
            .Where(i => i.InvoiceDate >= safeFrom && i.InvoiceDate <= safeTo && i.IsPosted)
            .SumAsync(i => (decimal?)(i.CommissionAmount + i.Soodh_Amount)) ?? 0;

        // Return as a single row for report summary or grouped by type
        return new List<dynamic> { new {
            Sales = salesTotal,
            Purchases = purchasesTotal,
            Commissions = brkCommission,
            GrossProfit = (salesTotal + brkCommission) - purchasesTotal
        }};
    }

    public async Task<IEnumerable<dynamic>> GetMasterListAsync(string entityName)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (entityName == "Parties") return await context.Parties.Select(p => new { Name = p.Name, Phone = p.Phone, Type = p.PartyType.ToString() }).ToListAsync();
        if (entityName == "Products") return await context.Products.Select(p => new { Name = p.Name, Price = p.SalePrice, Stock = p.CurrentStock }).ToListAsync();
        return new List<dynamic>();
    }
}
