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
        return await context.Vouchers
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .Include(v => v.CropSeason)
            .FirstOrDefaultAsync(v => v.Id == id);
    }

    public async Task<Voucher?> GetVoucherByNoAsync(VoucherType voucherType, string voucherNo, int financialYearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var cleanNo = voucherNo.Trim();
        return await context.Vouchers
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .Include(v => v.CropSeason)
            .FirstOrDefaultAsync(v => v.VoucherType == voucherType 
                                   && (v.VoucherNo == cleanNo || v.VoucherNo.EndsWith(cleanNo))
                                   && (financialYearId <= 0 || v.FinancialYearId == financialYearId));
    }

    public async Task<Voucher?> GetPreviousVoucherAsync(VoucherType voucherType, int currentVoucherId, int financialYearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var query = context.Vouchers
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .Include(v => v.CropSeason)
            .Where(v => v.VoucherType == voucherType && (financialYearId <= 0 || v.FinancialYearId == financialYearId));

        if (currentVoucherId > 0)
        {
            query = query.Where(v => v.Id < currentVoucherId);
        }

        return await query.OrderByDescending(v => v.Id).FirstOrDefaultAsync();
    }

    public async Task<Voucher?> GetNextVoucherAsync(VoucherType voucherType, int currentVoucherId, int financialYearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Vouchers
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .Include(v => v.CropSeason)
            .Where(v => v.VoucherType == voucherType 
                     && (financialYearId <= 0 || v.FinancialYearId == financialYearId)
                     && v.Id > currentVoucherId)
            .OrderBy(v => v.Id)
            .FirstOrDefaultAsync();
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

        // Ensure FinancialYearId is valid
        if (voucher.FinancialYearId <= 0)
        {
            var year = await context.FinancialYears.FirstOrDefaultAsync(y => y.IsCurrent)
                    ?? await context.FinancialYears.OrderByDescending(y => y.Id).FirstOrDefaultAsync();
            if (year != null)
            {
                voucher.FinancialYearId = year.Id;
            }
        }

        // Ensure CropSeasonId is valid or null
        if (voucher.CropSeasonId.HasValue && voucher.CropSeasonId.Value <= 0)
        {
            voucher.CropSeasonId = null;
        }
        else if (voucher.CropSeasonId.HasValue)
        {
            var seasonExists = await context.CropSeasons.AnyAsync(s => s.Id == voucher.CropSeasonId.Value);
            if (!seasonExists)
            {
                voucher.CropSeasonId = null;
            }
        }

        // Validate Cash Vouchers do not contain internal cash accounts
        if (voucher.VoucherType == VoucherType.CashReceiving || voucher.VoucherType == VoucherType.CashPayment)
        {
            var cashAccountIds = await context.Accounts
                .Where(a => !a.IsDeleted && (a.AccountNo == "1001" || a.Name.Contains("Cash") || a.Name.Contains("روکڑ") || a.Name == "General Cash Account"))
                .Select(a => a.Id)
                .ToListAsync();

            if (voucher.Details.Any(d => cashAccountIds.Contains(d.AccountId)))
            {
                throw new InvalidOperationException("نقد کھاتہ (Cash in Hand) واؤچر کی تفصیلات (Details) میں شامل نہیں کیا جا سکتا۔");
            }
        }

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
        var voucher = await context.Vouchers
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .FirstOrDefaultAsync(v => v.Id == voucherId);
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

        // Add offset entry for Cash Vouchers to maintain Double-Entry balance directly with Cash in Hand
        if (voucher.VoucherType == VoucherType.CashReceiving || voucher.VoucherType == VoucherType.CashPayment)
        {
            var cashAccount = await context.Accounts.FirstOrDefaultAsync(a => !a.IsDeleted && (a.AccountNo == "1001" || a.Name.Contains("Cash") || a.Name.Contains("روکڑ") || a.Name == "General Cash Account" || a.AccountNo == "101"));
            if (cashAccount != null)
            {
                if (voucher.VoucherType == VoucherType.CashReceiving)
                {
                    foreach (var detail in voucher.Details)
                    {
                        var partyName = detail.Account?.Name ?? $"A/C {detail.AccountId}";
                        var narration = string.IsNullOrWhiteSpace(detail.Narration)
                            ? $"Cash received from {partyName} (Voucher #{voucher.VoucherNo})"
                            : $"Cash received from {partyName} - {detail.Narration} (Voucher #{voucher.VoucherNo})";

                        var cashEntry = new GeneralLedger
                        {
                            TransactionDate = voucher.VoucherDate,
                            AccountId = cashAccount.Id,
                            Debit = detail.Credit,
                            Credit = 0m,
                            Narration = narration,
                            SourceType = SourceType.Voucher,
                            SourceId = voucher.Id,
                            FinancialYearId = voucher.FinancialYearId,
                            CropSeasonId = voucher.CropSeasonId
                        };
                        context.GeneralLedger.Add(cashEntry);
                        cashAccount.CurrentBalance += detail.Credit;
                    }
                }
                else // CashPayment
                {
                    foreach (var detail in voucher.Details)
                    {
                        var partyName = detail.Account?.Name ?? $"A/C {detail.AccountId}";
                        var narration = string.IsNullOrWhiteSpace(detail.Narration)
                            ? $"Cash paid to {partyName} (Voucher #{voucher.VoucherNo})"
                            : $"Cash paid to {partyName} - {detail.Narration} (Voucher #{voucher.VoucherNo})";

                        var cashEntry = new GeneralLedger
                        {
                            TransactionDate = voucher.VoucherDate,
                            AccountId = cashAccount.Id,
                            Debit = 0m,
                            Credit = detail.Debit,
                            Narration = narration,
                            SourceType = SourceType.Voucher,
                            SourceId = voucher.Id,
                            FinancialYearId = voucher.FinancialYearId,
                            CropSeasonId = voucher.CropSeasonId
                        };
                        context.GeneralLedger.Add(cashEntry);
                        cashAccount.CurrentBalance -= detail.Debit;
                    }
                }
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

    public async Task DeleteVoucherAsync(int voucherId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var voucher = await context.Vouchers.Include(v => v.Details).FirstOrDefaultAsync(v => v.Id == voucherId);
        if (voucher != null)
        {
            var glEntries = await context.GeneralLedger
                .Where(gl => gl.SourceType == SourceType.Voucher && gl.SourceId == voucherId)
                .ToListAsync();
            context.GeneralLedger.RemoveRange(glEntries);

            context.VoucherDetails.RemoveRange(voucher.Details);
            context.Vouchers.Remove(voucher);
            await context.SaveChangesAsync();
        }
    }
}
