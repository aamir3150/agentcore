using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Collections.Generic;
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
        return await context.PestroInvoices
            .Include(i => i.Details).ThenInclude(d => d.Product)
            .Include(i => i.Details).ThenInclude(d => d.Batch)
            .Include(i => i.Party)
            .Include(i => i.Salesman)
            .FirstOrDefaultAsync(i => i.Id == id);
    }

    public async Task<IEnumerable<PestroInvoice>> GetAllPestroInvoicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PestroInvoices.Include(i => i.Party).ToListAsync();
    }

    public async Task SavePestroInvoiceAsync(PestroInvoice invoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (invoice.Id == 0)
        {
            invoice.InvoiceNo = await GetNextPestroInvoiceNoAsync(invoice.InvoiceType, invoice.FinancialYearId);
            context.PestroInvoices.Add(invoice);
        }
        else
        {
            context.PestroInvoices.Update(invoice);
        }
        await context.SaveChangesAsync();
    }

    public async Task PostPestroInvoiceToGLAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.PestroInvoices.Include(i => i.Details).Include(i => i.Party).FirstOrDefaultAsync(i => i.Id == id);
        if (invoice == null || invoice.IsPosted) return;

        // Post to GL logic (Net amount to Party account)
        var glEntry = new GeneralLedger
        {
            TransactionDate = invoice.InvoiceDate,
            AccountId = invoice.Party.AccountId,
            Narration = $"Pestro Inv: {invoice.InvoiceNo}",
            SourceType = SourceType.PestroInvoice,
            SourceId = invoice.Id,
            FinancialYearId = invoice.FinancialYearId
        };
        
        if (invoice.InvoiceType == InvoiceType.PestroPurchase) glEntry.Credit = invoice.NetAmount;
        else glEntry.Debit = invoice.NetAmount;
        
        context.GeneralLedger.Add(glEntry);

        // Update Batch Stock
        foreach (var detail in invoice.Details)
        {
            if (detail.BatchId != null)
            {
                var batch = await context.PestroBatches.FindAsync(detail.BatchId);
                if (batch != null)
                {
                    if (invoice.InvoiceType == InvoiceType.PestroPurchase)
                        batch.RemainingQty += detail.Quantity;
                    else
                        batch.RemainingQty -= detail.Quantity;
                        
                    if (batch.RemainingQty <= 0) batch.Status = BatchStatus.Depleted;
                }
            }
        }

        invoice.IsPosted = true;
        invoice.Status = InvoiceStatus.Posted;
        await context.SaveChangesAsync();
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

    public async Task<string> GetNextPestroInvoiceNoAsync(InvoiceType type, int financialYearId)
    {
        return await _systemService.GetNextDocumentNoAsync(DocumentType.PestroInvoice, financialYearId);
    }
}
