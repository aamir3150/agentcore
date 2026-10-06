namespace MyMandiSystem.Core.PrintModels;

/// <summary>
/// A single line item on a print slip (product, qty, rate, amount).
/// Reusable across Maal Udhaar Slip, Cash Loan Slip, Invoice, etc.
/// </summary>
public sealed record SlipLine(
    string Product,
    decimal Qty,
    string Unit,
    decimal Rate,
    decimal Amount);
