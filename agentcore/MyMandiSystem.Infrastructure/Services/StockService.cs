using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class StockService : IStockService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public StockService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<decimal> GetCurrentStockAsync(int productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StockTransactions
            .Where(s => s.ProductId == productId)
            .SumAsync(s => s.QtyIn - s.QtyOut);
    }

    public async Task<IEnumerable<StockTransaction>> GetStockLedgerAsync(int productId, DateTime fromDate, DateTime toDate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.StockTransactions
            .Where(s => s.ProductId == productId && s.TransactionDate >= fromDate && s.TransactionDate <= toDate)
            .OrderBy(s => s.TransactionDate)
            .ToListAsync();
    }

    public async Task AddStockTransactionAsync(StockTransaction transaction)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        // Calculate Moving Average Cost before adding new transaction if it's an Inward
        if (transaction.QtyIn > 0)
        {
            transaction.MovingAvgCost = await CalculateMovingAverageCostAsync(transaction.ProductId);
        }

        context.StockTransactions.Add(transaction);
        await context.SaveChangesAsync();
    }

    public async Task<decimal> CalculateMovingAverageCostAsync(int productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var transactions = await context.StockTransactions
            .Where(s => s.ProductId == productId)
            .OrderBy(s => s.TransactionDate)
            .ToListAsync();

        if (!transactions.Any()) return 0;

        decimal totalQty = 0;
        decimal totalValue = 0;

        foreach (var t in transactions)
        {
            if (t.QtyIn > 0)
            {
                totalValue += (t.QtyIn * t.Rate);
                totalQty += t.QtyIn;
            }
            else if (t.QtyOut > 0)
            {
                // On Outward, we deduct using previous average
                decimal avg = totalQty > 0 ? totalValue / totalQty : 0;
                totalValue -= (t.QtyOut * avg);
                totalQty -= t.QtyOut;
            }
        }

        return totalQty > 0 ? totalValue / totalQty : 0;
    }

    public async Task SetOpeningStockAsync(int productId, decimal qty, decimal rate)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        // Remove existing opening transaction for this product if any
        var existingOpening = await context.StockTransactions
            .FirstOrDefaultAsync(s => s.ProductId == productId && s.TransactionType == StockTransactionType.Opening);
            
        if (existingOpening != null)
        {
            context.StockTransactions.Remove(existingOpening);
        }

        if (qty != 0)
        {
            var financialYear = await context.FinancialYears.FirstOrDefaultAsync(f => f.IsCurrent);
            const int SystemUserId = 1;

            var transaction = new StockTransaction
            {
                ProductId = productId,
                TransactionDate = financialYear?.StartDate ?? DateTime.Today,
                TransactionType = StockTransactionType.Opening,
                SourceModule = SourceModule.Manual,
                QtyIn = qty > 0 ? qty : 0,
                QtyOut = qty < 0 ? Math.Abs(qty) : 0,
                Rate = rate,
                FinancialYearId = financialYear?.Id ?? 0,
                Narration = "Opening Balance",
                CreatedAt = DateTime.Now,
                CreatedBy = SystemUserId
            };

            context.StockTransactions.Add(transaction);
        }

        await context.SaveChangesAsync();
    }
    public async Task RecalculateStockAsync(int? productId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        var products = productId.HasValue 
            ? await context.Products.Where(p => p.Id == productId.Value).ToListAsync()
            : await context.Products.ToListAsync();

        foreach (var product in products)
        {
            var currentTransactions = await context.StockTransactions
                .Where(s => s.ProductId == product.Id && s.TransactionType != StockTransactionType.Opening)
                .ToListAsync();
            context.StockTransactions.RemoveRange(currentTransactions);
            await context.SaveChangesAsync();

            var invoiceItems = await context.InvoiceDetails
                .Include(d => d.Invoice)
                .Where(d => d.ProductId == product.Id && d.Invoice.Status == InvoiceStatus.Posted)
                .ToListAsync();

            foreach (var item in invoiceItems)
            {
                var type = item.Invoice.InvoiceType switch
                {
                    InvoiceType.GenPurchase => StockTransactionType.Purchase,
                    InvoiceType.GenSale => StockTransactionType.Sale,
                    InvoiceType.GenPurchaseReturn => StockTransactionType.PurchaseReturn,
                    InvoiceType.GenSaleReturn => StockTransactionType.SaleReturn,
                    _ => StockTransactionType.Adjustment
                };

                context.StockTransactions.Add(new StockTransaction
                {
                    ProductId = product.Id,
                    TransactionDate = item.Invoice.InvoiceDate,
                    QtyIn = (type == StockTransactionType.Purchase || type == StockTransactionType.SaleReturn) ? item.Quantity : 0,
                    QtyOut = (type == StockTransactionType.Sale || type == StockTransactionType.PurchaseReturn) ? item.Quantity : 0,
                    Rate = item.Rate,
                    TransactionType = type,
                    SourceModule = SourceModule.General,
                    SourceInvoiceId = item.InvoiceId,
                    FinancialYearId = item.Invoice.FinancialYearId,
                    Narration = $"Recalc: {item.Invoice.InvoiceNo}"
                });
            }

            await context.SaveChangesAsync();
            var finalQty = await context.StockTransactions
                .Where(s => s.ProductId == product.Id)
                .SumAsync(s => s.QtyIn - s.QtyOut);
            
            product.CurrentStock = finalQty;
            context.Products.Update(product);
            await context.SaveChangesAsync();
        }
    }
}
