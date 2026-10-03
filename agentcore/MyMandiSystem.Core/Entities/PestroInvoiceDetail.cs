using System;

namespace MyMandiSystem.Core.Entities;

public class PestroInvoiceDetail : AuditBase
{
    public int Id { get; set; }
    public int InvoiceId { get; set; }
    public virtual PestroInvoice Invoice { get; set; } = null!;
    public int LineNo { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public int? BatchId { get; set; }
    public virtual PestroBatch? Batch { get; set; }
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal Amount { get; set; }
    public DateTime? ExpiryDate { get; set; }
}
