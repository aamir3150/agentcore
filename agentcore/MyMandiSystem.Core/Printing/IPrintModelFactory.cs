using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.PrintModels;

namespace MyMandiSystem.Core.Printing;

/// <summary>
/// Factory that builds disconnected snapshot PrintModels from EF entities or IDs.
/// Keeps PrintService and templates completely decoupled from EF DbContext.
/// </summary>
public interface IPrintModelFactory
{
    /// <summary>Build a Maal Udhaar Slip model from a Voucher ID.</summary>
    CreditVoucherSlip BuildCreditSlip(int voucherId);

    /// <summary>Build a Maal Udhaar Slip model from an Invoice ID.</summary>
    CreditVoucherSlip BuildCreditSlipFromInvoice(int invoiceId);

    /// <summary>Build a Maal Udhaar Slip model from in-memory entities.</summary>
    CreditVoucherSlip BuildCreditSlip(Voucher voucher, Party? party = null, IEnumerable<SlipLine>? items = null);

    /// <summary>Build a Cash Udhaar Slip model from a Voucher ID.</summary>
    CashLoanSlip BuildCashLoanSlip(int voucherId);

    /// <summary>Build a Cash Udhaar Slip model from in-memory entities.</summary>
    CashLoanSlip BuildCashLoanSlip(Voucher voucher, Party? party = null);

    /// <summary>Build a Voucher Slip model (Cash / Bank / Journal) from a Voucher ID.</summary>
    VoucherSlip BuildVoucherSlip(int voucherId);

    /// <summary>Build a Voucher Slip model from in-memory entities.</summary>
    VoucherSlip BuildVoucherSlip(Voucher voucher);
}
