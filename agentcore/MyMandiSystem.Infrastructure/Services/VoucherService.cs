using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Enums;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class VoucherService : IVoucherService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemService _systemService;

    public VoucherService(IDbContextFactory<AppDbContext> contextFactory, ISystemService systemService)
    {
        _contextFactory = contextFactory;
        _systemService = systemService;
    }

    public async Task<Voucher?> GetVoucherByIdAsync(int id)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Vouchers.Include(v => v.Details).FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<string> GetNextVoucherNoAsync(VoucherType voucherType, int financialYearId)
    {
        DocumentType docType = voucherType switch
        {
            VoucherType.CashReceiving => DocumentType.CashReceivingVoucher,
            VoucherType.CashPayment => DocumentType.CashPaymentVoucher,
            VoucherType.JournalVoucher => DocumentType.JournalVoucher,
            _ => DocumentType.JournalVoucher
        };

        return await _systemService.GetNextDocumentNoAsync(docType, financialYearId);
    }

    public async Task SaveVoucherAsync(Voucher voucher)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (voucher.Id == 0)
        {
            context.Vouchers.Add(voucher);
        }
        else
        {
            context.Vouchers.Update(voucher);
        }
        await context.SaveChangesAsync();
    }

    public async Task PostVoucherAsync(int voucherId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var voucher = await context.Vouchers.Include(v => v.Details).FirstOrDefaultAsync(v => v.Id == voucherId);
        if (voucher == null || voucher.IsPosted) return;

        // Create GL Entries
        foreach (var detail in voucher.Details)
        {
            var glEntry = new GeneralLedger
            {
                TransactionDate = voucher.VoucherDate,
                AccountId = detail.AccountId,
                Debit = detail.Debit,
                Credit = detail.Credit,
                Narration = detail.Narration ?? voucher.NarrationEnglish,
                SourceType = SourceType.Voucher,
                SourceId = voucher.Id,
                FinancialYearId = voucher.FinancialYearId,
                CropSeasonId = voucher.CropSeasonId
            };
            context.GeneralLedger.Add(glEntry);
            
            // Update account balance (simplified running balance)
            var account = await context.Accounts.FindAsync(detail.AccountId);
            if (account != null)
            {
                account.CurrentBalance += (detail.Debit - detail.Credit);
            }
        }

        // Add offset entry for Cash Vouchers to maintain Double-Entry balance
        if (voucher.VoucherType == VoucherType.CashReceiving || voucher.VoucherType == VoucherType.CashPayment)
        {
            var cashAccount = await context.Accounts.FirstOrDefaultAsync(a => a.Name == "General Cash Account" || a.AccountNo == "101");
            if (cashAccount != null)
            {
                decimal cashDebit = voucher.VoucherType == VoucherType.CashReceiving ? voucher.TotalCredit : 0m;
                decimal cashCredit = voucher.VoucherType == VoucherType.CashPayment ? voucher.TotalDebit : 0m;

                var cashEntry = new GeneralLedger
                {
                    TransactionDate = voucher.VoucherDate,
                    AccountId = cashAccount.Id,
                    Debit = cashDebit,
                    Credit = cashCredit,
                    Narration = voucher.NarrationEnglish ?? $"Auto Cash Offset for {voucher.VoucherNo}",
                    SourceType = SourceType.Voucher,
                    SourceId = voucher.Id,
                    FinancialYearId = voucher.FinancialYearId,
                    CropSeasonId = voucher.CropSeasonId
                };
                context.GeneralLedger.Add(cashEntry);
                cashAccount.CurrentBalance += (cashEntry.Debit - cashEntry.Credit);
            }
        }

        voucher.IsPosted = true;
        voucher.Status = VoucherStatus.Posted;
        voucher.PostedAt = DateTime.Now;
        
        await context.SaveChangesAsync();
    }

    public async Task ReverseVoucherAsync(int voucherId, string reason)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var voucher = await context.Vouchers.Include(v => v.Details).FirstOrDefaultAsync(v => v.Id == voucherId);
        if (voucher == null || !voucher.IsPosted) return;

        // Find GL entries and reverse them
        var glEntries = await context.GeneralLedger.Where(gl => gl.SourceType == SourceType.Voucher && gl.SourceId == voucherId).ToListAsync();
        foreach (var entry in glEntries)
        {
            var reversal = new GeneralLedger
            {
                TransactionDate = DateTime.Now,
                AccountId = entry.AccountId,
                Debit = entry.Credit, // Flip DR/CR
                Credit = entry.Debit,
                Narration = $"Reversal of {voucher.VoucherNo}: {reason}",
                SourceType = SourceType.Voucher,
                SourceId = voucherId,
                FinancialYearId = voucher.FinancialYearId,
                CropSeasonId = voucher.CropSeasonId,
                IsReversed = true
            };
            context.GeneralLedger.Add(reversal);
            
            var account = await context.Accounts.FindAsync(entry.AccountId);
            if (account != null)
            {
                account.CurrentBalance += (reversal.Debit - reversal.Credit);
            }
        }

        voucher.Status = VoucherStatus.Reversed;
        await context.SaveChangesAsync();
    }
}
