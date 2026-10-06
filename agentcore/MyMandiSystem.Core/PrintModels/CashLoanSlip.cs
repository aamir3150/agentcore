using MyMandiSystem.Core.Printing;

namespace MyMandiSystem.Core.PrintModels;

/// <summary>
/// Print model for "Cash Udhaar Slip" – when a kisaan takes cash loan.
/// Requires Kisaan and Munshi signatures.
/// </summary>
public sealed class CashLoanSlip : IPrintable
{
    public string VoucherNo { get; init; } = "";
    public DateTime Date { get; init; }
    public string PartyName { get; init; } = "";
    public string PartyUrduName { get; init; } = "";
    public string PartyPhone { get; init; } = "";
    public string PartyAddress { get; init; } = "";
    public decimal Amount { get; init; }
    public string Narration { get; init; } = "";
    public string NarrationUrdu { get; init; } = "";

    public PrintOptions DefaultOptions =>
        new(PaperKind.A5, Title: "Cash Udhaar Slip - نقد ادھار پرچی", Copies: 2);
}
