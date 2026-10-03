namespace MyMandiSystem.Core.Entities;

public class BrokerageInvoiceDetail : AuditBase
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public virtual BrokerageInvoice Invoice { get; set; } = null!;
    public int LineNo { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public int Bags { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal TareWeight { get; set; }
    public decimal NetWeight { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
}
