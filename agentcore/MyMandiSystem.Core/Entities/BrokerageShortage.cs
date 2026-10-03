namespace MyMandiSystem.Core.Entities;

public class BrokerageShortage : AuditBase
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public virtual BrokerageInvoice Invoice { get; set; } = null!;
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public decimal ShortQty { get; set; }
    public decimal ShortAmount { get; set; }
    public int? AdjustmentVoucherId { get; set; }
    public virtual Voucher? AdjustmentVoucher { get; set; }
}
