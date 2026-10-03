using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class InvoiceService : IInvoiceService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;
    private readonly IStockService _stockService;

    public InvoiceService(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService, IStockService stockService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
        _stockService = stockService;
    }

    public async Task<Invoice?> GetInvoiceByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Invoices.Include(i => i.Details).FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<string> GetNextInvoiceNoAsync(InvoiceType type, int financialYearId)
    {
        DocumentType docType = type switch
        {
            InvoiceType.GenPurchase => DocumentType.GenPurchaseInvoice,
            InvoiceType.GenSale => DocumentType.GenSaleInvoice,
            _ => DocumentType.GenSaleInvoice
        };

        return await _systemService.GetNextDocumentNoAsync(docType, financialYearId);
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
            context.Invoices.Update(invoice);
        }
        await context.SaveChangesAsync();
    }

    public async Task PostInvoiceToGLAsync(int invoiceId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.Invoices
            .Include(i => i.Details)
            .Include(i => i.Party)
            .FirstOrDefaultAsync(i => i.Id == invoiceId);
            
        if (invoice == null || invoice.IsPosted) return;

        // 1. Post to General Ledger (Balanced Double Entry)
        
        // Define accounts (In production, these would be in SystemConfig/Settings)
        var salesAccount = await GetOrCreateSystemAccount(context, "General Sales", AccountType.Revenue);
        var purchaseAccount = await GetOrCreateSystemAccount(context, "General Purchases", AccountType.Expense);
        var taxAccount = await GetOrCreateSystemAccount(context, "Tax Payable", AccountType.Liability);
        var laborAccount = await GetOrCreateSystemAccount(context, "Labor Expense", AccountType.Expense);
        var freightAccount = await GetOrCreateSystemAccount(context, "Freight Expense", AccountType.Expense);
        var mcAccount = await GetOrCreateSystemAccount(context, "Market Committee Fee", AccountType.Expense);

        // A. Entry for Party (Net Amount)
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

        if (invoice.InvoiceType == InvoiceType.GenPurchase || invoice.InvoiceType == InvoiceType.GenSaleReturn)
            partyEntry.Credit = invoice.NetAmount;
        else
            partyEntry.Debit = invoice.NetAmount;

        context.GeneralLedger.Add(partyEntry);

        // B. Entry for Trade (Subtotal)
        var tradeEntry = new GeneralLedger
        {
            TransactionDate = invoice.InvoiceDate,
            AccountId = (invoice.InvoiceType == InvoiceType.GenPurchase || invoice.InvoiceType == InvoiceType.GenPurchaseReturn) ? purchaseAccount.Id : salesAccount.Id,
            Narration = $"Trade Value - {invoice.InvoiceNo}",
            SourceType = SourceType.GenInvoice,
            SourceId = invoice.Id,
            FinancialYearId = invoice.FinancialYearId,
            CropSeasonId = invoice.CropSeasonId
        };

        if (invoice.InvoiceType == InvoiceType.GenPurchase || invoice.InvoiceType == InvoiceType.GenSaleReturn)
            tradeEntry.Debit = invoice.SubTotal;
        else
            tradeEntry.Credit = invoice.SubTotal;

        context.GeneralLedger.Add(tradeEntry);

        // C. Optional Entries (Tax, Labor, etc.)
        if (invoice.TaxAmount != 0)
        {
             context.GeneralLedger.Add(new GeneralLedger {
                 TransactionDate = invoice.InvoiceDate,
                 AccountId = taxAccount.Id,
                 Debit = (invoice.InvoiceType == InvoiceType.GenPurchase || invoice.InvoiceType == InvoiceType.GenSaleReturn) ? 0 : invoice.TaxAmount,
                 Credit = (invoice.InvoiceType == InvoiceType.GenPurchase || invoice.InvoiceType == InvoiceType.GenSaleReturn) ? invoice.TaxAmount : 0,
                 Narration = $"Tax on {invoice.InvoiceNo}", SourceType = SourceType.GenInvoice, SourceId = invoice.Id, FinancialYearId = invoice.Id, CropSeasonId = invoice.CropSeasonId
             });
        }
        
        // (Similar logic for Labor, Freight, MC Fee - adding if they are non-zero)
        if (invoice.LaborCharges != 0)
        {
             context.GeneralLedger.Add(new GeneralLedger {
                 TransactionDate = invoice.InvoiceDate, AccountId = laborAccount.Id,
                 Debit = (invoice.InvoiceType == InvoiceType.GenPurchase || invoice.InvoiceType == InvoiceType.GenSaleReturn) ? 0 : invoice.LaborCharges,
                 Credit = (invoice.InvoiceType == InvoiceType.GenPurchase || invoice.InvoiceType == InvoiceType.GenSaleReturn) ? invoice.LaborCharges : 0,
                 Narration = $"Labor on {invoice.InvoiceNo}", SourceType = SourceType.GenInvoice, SourceId = invoice.Id, FinancialYearId = invoice.FinancialYearId, CropSeasonId = invoice.CropSeasonId
             });
        }

        // 2. Add Stock Transactions
        foreach (var detail in invoice.Details)
        {
            var stockTx = new StockTransaction
            {
                TransactionDate = invoice.InvoiceDate,
                ProductId = detail.ProductId,
                FinancialYearId = invoice.FinancialYearId,
                SourceModule = SourceModule.General,
                SourceInvoiceId = invoice.Id,
                Rate = detail.Rate,
                Narration = $"Inv: {invoice.InvoiceNo}"
            };

            if (invoice.InvoiceType == InvoiceType.GenPurchase || invoice.InvoiceType == InvoiceType.GenSaleReturn)
            {
                stockTx.TransactionType = invoice.InvoiceType == InvoiceType.GenPurchase ? StockTransactionType.Purchase : StockTransactionType.SaleReturn;
                stockTx.QtyIn = detail.Quantity;
            }
            else
            {
                stockTx.TransactionType = invoice.InvoiceType == InvoiceType.GenSale ? StockTransactionType.Sale : StockTransactionType.PurchaseReturn;
                stockTx.QtyOut = detail.Quantity;
            }

            await _stockService.AddStockTransactionAsync(stockTx);
        }

        invoice.IsPosted = true;
        invoice.Status = InvoiceStatus.Posted;
        invoice.PostedAt = DateTime.Now;

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

    public async Task<IEnumerable<Invoice>> GetInvoicesByPartyAsync(int partyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Invoices.Where(i => i.PartyId == partyId).ToListAsync();
    }
}
