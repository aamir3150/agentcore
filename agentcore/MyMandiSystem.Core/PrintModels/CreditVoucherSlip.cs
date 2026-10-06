using MyMandiSystem.Core.Printing;

namespace MyMandiSystem.Core.PrintModels;

/// <summary>
/// Print model for "Maal Udhaar Slip" – when a kisaan takes commodity on credit.
/// This is a snapshot; it does NOT hold EF entities or database references.
/// </summary>
public sealed class CreditVoucherSlip : IPrintable
{
    public string VoucherNo { get; init; } = "";
    public DateTime Date { get; init; }
    public string PartyName { get; init; } = "";
    public string PartyUrduName { get; init; } = "";
    public string PartyPhone { get; init; } = "";
    public string PartyAddress { get; init; } = "";
    public IReadOnlyList<SlipLine> Items { get; init; } = Array.Empty<SlipLine>();
    public decimal TotalAmount { get; init; }
    public string Remarks { get; init; } = "";
    public string RemarksUrdu { get; init; } = "";

    public PrintOptions DefaultOptions =>
        new(PaperKind.A5, Title: "Maal Udhaar Slip - مال ادھار پرچی", Copies: 2);
}
