using System;
using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public enum BatchStatus
{
    Active = 1,
    Depleted = 2,
    Expired = 3
}

public class PestroBatch : AuditBase
{
    public int Id { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public string BatchNo { get; set; } = string.Empty;
    public DateTime? ExpiryDate { get; set; }
    public DateTime? ManufactureDate { get; set; }
    public int? PurchaseInvoiceId { get; set; }
    public decimal OriginalQty { get; set; }
    public decimal RemainingQty { get; set; }
    public decimal CostPrice { get; set; }
    public BatchStatus Status { get; set; } = BatchStatus.Active;
    
    public virtual ICollection<PestroInvoiceDetail> SaleAllocations { get; set; } = new List<PestroInvoiceDetail>();
}
