using MyMandiSystem.Core.Printing;

namespace MyMandiSystem.Core.PrintModels;

/// <summary>
/// Print model for Cash Payment / Receiving Voucher slip.
/// Snapshot of voucher data for printing – no EF entities.
/// </summary>
public sealed class VoucherSlip : IPrintable
{
    public string VoucherNo { get; init; } = "";
    public string VoucherType { get; init; } = "";    // "Cash Payment" / "Cash Receiving" / "Journal"
    public string VoucherTypeUrdu { get; init; } = ""; // "نقد ادائیگی" / "نقد وصولی" / "جنرل واؤچر"
    public DateTime Date { get; init; }
    public string AccountNo { get; init; } = "";
    public string AccountName { get; init; } = "";
    public string AccountUrduName { get; init; } = "";
    public string Narration { get; init; } = "";
    public string NarrationUrdu { get; init; } = "";
    public decimal TotalDebit { get; init; }
    public decimal TotalCredit { get; init; }
    public decimal Amount { get; init; }

    /// <summary>Line items (for multi-line vouchers like Journal Voucher).</summary>
    public IReadOnlyList<VoucherSlipLine> Details { get; init; } = Array.Empty<VoucherSlipLine>();

    public PrintOptions DefaultOptions =>
        new(PaperKind.A5, Title: $"{VoucherType} Voucher - {VoucherTypeUrdu}", Copies: 1);
}

/// <summary>Single line in a voucher slip (account, narration, debit, credit).</summary>
public sealed record VoucherSlipLine(
    string AccountNo,
    string AccountName,
    string? Narration,
    decimal Debit,
    decimal Credit);
