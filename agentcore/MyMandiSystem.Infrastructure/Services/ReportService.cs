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

    public async Task<IEnumerable<dynamic>> GetLedgerReportAsync(int accountId, DateTime from, DateTime to)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        // Fetch opening balance (entries before 'from' date)
        var openingDebit = await context.GeneralLedger
            .Where(g => g.AccountId == accountId && g.TransactionDate < from)
            .SumAsync(g => (decimal?)g.Debit) ?? 0;
            
        var openingCredit = await context.GeneralLedger
            .Where(g => g.AccountId == accountId && g.TransactionDate < from)
            .SumAsync(g => (decimal?)g.Credit) ?? 0;

        var openingBalance = openingDebit - openingCredit;

        // Fetch current transactions
        var transactions = await context.GeneralLedger
            .Include(g => g.Account)
            .Where(g => g.AccountId == accountId && g.TransactionDate >= from && g.TransactionDate <= to)
            .OrderBy(g => g.TransactionDate)
            .ThenBy(g => g.Id)
            .Select(g => new
            {
                g.TransactionDate,
                g.Narration,
                g.Debit,
                g.Credit,
                Reference = g.SourceType.ToString() + "-" + g.SourceId
            })
            .ToListAsync();

        // Integrate opening balance into the list for RDLC
        var reportData = new List<dynamic>();
        reportData.Add(new
        {
            TransactionDate = from.AddDays(-1),
            Narration = "Opening Balance",
            Debit = openingBalance > 0 ? openingBalance : 0,
            Credit = openingBalance < 0 ? Math.Abs(openingBalance) : 0,
            Reference = "OPEN"
        });
        reportData.AddRange(transactions);

        return reportData;
    }

    public async Task<IEnumerable<dynamic>> GetDayBookReportAsync(DateTime date)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.GeneralLedger
            .Include(g => g.Account)
            .Where(g => g.TransactionDate.Date == date.Date)
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
        
        // This is a simplified calculation of Gross Profit
        var salesTotal = await context.Invoices
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.InvoiceType == InvoiceType.GenSale)
            .SumAsync(i => (decimal?)i.NetAmount) ?? 0;
            
        var purchasesTotal = await context.Invoices
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.InvoiceType == InvoiceType.GenPurchase)
            .SumAsync(i => (decimal?)i.NetAmount) ?? 0;
            
        var brkCommission = await context.BrokerageInvoices
            .Where(i => i.InvoiceDate >= from && i.InvoiceDate <= to && i.IsPosted)
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
