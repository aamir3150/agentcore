using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Printing;
using MyMandiSystem.Core.PrintModels;

namespace MyMandiSystem.Infrastructure.Printing;

/// <summary>
/// Implements IPrintModelFactory to convert database entities into disconnected
/// snapshot print models. Keeps the print engine completely free of EF Core references.
/// </summary>
public sealed class PrintModelFactory : IPrintModelFactory
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public PrintModelFactory(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public CreditVoucherSlip BuildCreditSlip(int voucherId)
    {
        using var db = _contextFactory.CreateDbContext();
        var voucher = db.Vouchers
            .AsNoTracking()
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .FirstOrDefault(v => v.Id == voucherId)
            ?? throw new InvalidOperationException($"Voucher with ID {voucherId} not found.");

        // Find associated party through account
        var accountIds = voucher.Details.Select(d => d.AccountId).Distinct().ToList();
        var party = db.Parties
            .AsNoTracking()
            .FirstOrDefault(p => accountIds.Contains(p.AccountId));

        return BuildCreditSlip(voucher, party);
    }

    public CreditVoucherSlip BuildCreditSlipFromInvoice(int invoiceId)
    {
        using var db = _contextFactory.CreateDbContext();
        var invoice = db.Invoices
            .AsNoTracking()
            .Include(i => i.Party)
            .Include(i => i.Details)
                .ThenInclude(d => d.Product)
                    .ThenInclude(p => p.Unit)
            .FirstOrDefault(i => i.Id == invoiceId)
            ?? throw new InvalidOperationException($"Invoice with ID {invoiceId} not found.");

        var lines = invoice.Details.Select(d => new SlipLine(
            Product: d.Product?.Name ?? "Item",
            Qty: d.Quantity,
            Unit: d.Product?.Unit?.Name ?? "Bag",
            Rate: d.Rate,
            Amount: d.Amount
        )).ToList();

        return new CreditVoucherSlip
        {
            VoucherNo = invoice.InvoiceNo,
            Date = invoice.InvoiceDate,
            PartyName = invoice.Party?.Name ?? "Cash Customer",
            PartyUrduName = invoice.Party?.UrduName ?? string.Empty,
            PartyPhone = invoice.Party?.Phone ?? string.Empty,
            PartyAddress = invoice.Party?.Address ?? string.Empty,
            Items = lines,
            TotalAmount = invoice.NetAmount > 0 ? invoice.NetAmount : invoice.SubTotal,
            Remarks = $"Invoice #{invoice.InvoiceNo}",
            RemarksUrdu = string.Empty
        };
    }

    public CreditVoucherSlip BuildCreditSlip(Voucher voucher, Party? party = null, IEnumerable<SlipLine>? items = null)
    {
        var linesList = items?.ToList() ?? voucher.Details.Select(d => new SlipLine(
            Product: d.Narration ?? d.Account?.Name ?? "Item",
            Qty: 1,
            Unit: "Qty",
            Rate: d.Debit > 0 ? d.Debit : d.Credit,
            Amount: d.Debit > 0 ? d.Debit : d.Credit
        )).ToList();

        decimal total = voucher.TotalDebit > 0 ? voucher.TotalDebit : linesList.Sum(l => l.Amount);

        return new CreditVoucherSlip
        {
            VoucherNo = voucher.VoucherNo,
            Date = voucher.VoucherDate,
            PartyName = party?.Name ?? voucher.Details.FirstOrDefault()?.Account?.Name ?? "Party",
            PartyUrduName = party?.UrduName ?? voucher.Details.FirstOrDefault()?.Account?.UrduName ?? string.Empty,
            PartyPhone = party?.Phone ?? string.Empty,
            PartyAddress = party?.Address ?? string.Empty,
            Items = linesList,
            TotalAmount = total,
            Remarks = voucher.NarrationEnglish ?? string.Empty,
            RemarksUrdu = voucher.NarrationUrdu ?? string.Empty
        };
    }

    public CashLoanSlip BuildCashLoanSlip(int voucherId)
    {
        using var db = _contextFactory.CreateDbContext();
        var voucher = db.Vouchers
            .AsNoTracking()
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .FirstOrDefault(v => v.Id == voucherId)
            ?? throw new InvalidOperationException($"Voucher with ID {voucherId} not found.");

        var accountIds = voucher.Details.Select(d => d.AccountId).Distinct().ToList();
        var party = db.Parties
            .AsNoTracking()
            .FirstOrDefault(p => accountIds.Contains(p.AccountId));

        return BuildCashLoanSlip(voucher, party);
    }

    public CashLoanSlip BuildCashLoanSlip(Voucher voucher, Party? party = null)
    {
        decimal amount = voucher.TotalDebit > 0 ? voucher.TotalDebit : voucher.TotalCredit;

        return new CashLoanSlip
        {
            VoucherNo = voucher.VoucherNo,
            Date = voucher.VoucherDate,
            PartyName = party?.Name ?? voucher.Details.FirstOrDefault()?.Account?.Name ?? "Party",
            PartyUrduName = party?.UrduName ?? voucher.Details.FirstOrDefault()?.Account?.UrduName ?? string.Empty,
            PartyPhone = party?.Phone ?? string.Empty,
            PartyAddress = party?.Address ?? string.Empty,
            Amount = amount,
            Narration = voucher.NarrationEnglish ?? "Cash Udhaar (Advance Loan)",
            NarrationUrdu = voucher.NarrationUrdu ?? "نقد ادھار"
        };
    }

    public VoucherSlip BuildVoucherSlip(int voucherId)
    {
        using var db = _contextFactory.CreateDbContext();
        var voucher = db.Vouchers
            .AsNoTracking()
            .Include(v => v.Details)
                .ThenInclude(d => d.Account)
            .FirstOrDefault(v => v.Id == voucherId)
            ?? throw new InvalidOperationException($"Voucher with ID {voucherId} not found.");

        return BuildVoucherSlip(voucher);
    }

    public VoucherSlip BuildVoucherSlip(Voucher voucher)
    {
        string typeName = voucher.VoucherType.ToString();
        string typeUrdu = voucher.VoucherType switch
        {
            Core.Enums.VoucherType.CashPayment => "نقد ادائیگی",
            Core.Enums.VoucherType.CashReceiving => "نقد وصولی",
            Core.Enums.VoucherType.BankPayment => "بینک ادائیگی",
            Core.Enums.VoucherType.BankReceipt => "بینک وصولی",
            Core.Enums.VoucherType.JournalVoucher => "جنرل واؤچر",
            _ => "واؤچر"
        };

        var firstDetail = voucher.Details.FirstOrDefault();
        decimal totalAmount = voucher.TotalDebit > 0 ? voucher.TotalDebit : voucher.TotalCredit;

        var detailLines = voucher.Details.Select(d => new VoucherSlipLine(
            AccountNo: d.Account?.AccountNo ?? string.Empty,
            AccountName: d.Account?.Name ?? string.Empty,
            Narration: d.Narration,
            Debit: d.Debit,
            Credit: d.Credit
        )).ToList();

        return new VoucherSlip
        {
            VoucherNo = voucher.VoucherNo,
            VoucherType = typeName,
            VoucherTypeUrdu = typeUrdu,
            Date = voucher.VoucherDate,
            AccountNo = firstDetail?.Account?.AccountNo ?? string.Empty,
            AccountName = firstDetail?.Account?.Name ?? string.Empty,
            AccountUrduName = firstDetail?.Account?.UrduName ?? string.Empty,
            Narration = voucher.NarrationEnglish ?? firstDetail?.Narration ?? string.Empty,
            NarrationUrdu = voucher.NarrationUrdu ?? string.Empty,
            TotalDebit = voucher.TotalDebit,
            TotalCredit = voucher.TotalCredit,
            Amount = totalAmount,
            Details = detailLines
        };
    }
}
