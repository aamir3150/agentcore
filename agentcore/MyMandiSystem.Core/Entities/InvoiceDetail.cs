namespace MyMandiSystem.Core.Entities;

public class InvoiceDetail : AuditBase
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public virtual Invoice Invoice { get; set; } = null!;
    public int LineNo { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public decimal DiscountAmount { get; set; }
}
