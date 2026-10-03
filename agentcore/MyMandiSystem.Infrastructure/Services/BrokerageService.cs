using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class BrokerageService : IBrokerageService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    public BrokerageService(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
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

    public async Task<IEnumerable<BrokerageInvoice>> GetAllBrokerageInvoicesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.BrokerageInvoices.Include(i => i.Party).ToListAsync();
    }

    public async Task SaveBrokerageInvoiceAsync(BrokerageInvoice invoice)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (invoice.Id == 0)
        {
            invoice.InvoiceNo = await _systemService.GetNextDocumentNoAsync(DocumentType.BrokerageInvoice, invoice.FinancialYearId);
            context.BrokerageInvoices.Add(invoice);
        }
        else
        {
            context.BrokerageInvoices.Update(invoice);
        }
        await context.SaveChangesAsync();
    }

    public async Task PostBrokerageInvoiceToGLAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var invoice = await context.BrokerageInvoices.Include(i => i.Party).FirstOrDefaultAsync(i => i.Id == id);
        if (invoice == null || invoice.IsPosted) return;

        // 1. Post to GL (SimpilFIED version for now, similar to InvoiceService)
        // In real Mandi: Commission (Income), WHT (Liability), Party (A/R or A/P)
        
        var partyEntry = new GeneralLedger
        {
            TransactionDate = invoice.InvoiceDate,
            AccountId = invoice.Party.AccountId,
            Narration = $"Brokerage Inv: {invoice.InvoiceNo}",
            SourceType = SourceType.BrkInvoice,
            SourceId = invoice.Id,
            FinancialYearId = invoice.FinancialYearId,
            CropSeasonId = invoice.CropSeasonId,
            Debit = invoice.NetAmount // Assuming Receivable
        };
        context.GeneralLedger.Add(partyEntry);

        invoice.IsPosted = true;
        invoice.Status = InvoiceStatus.Posted;
        await context.SaveChangesAsync();
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
