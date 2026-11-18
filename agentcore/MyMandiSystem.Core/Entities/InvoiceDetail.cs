using System.ComponentModel.DataAnnotations.Schema;

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

    // AgenSoft Legacy Compatibility Fields
    [NotMapped] public decimal Multiplier { get; set; } = 1;
    [NotMapped] public decimal Bonus { get; set; }
    [NotMapped] public decimal DiscRatio { get; set; }
    [NotMapped] public decimal DiscValue { get; set; }
    [NotMapped] public decimal STRatio { get; set; }
    [NotMapped] public decimal STValue { get; set; }
    [NotMapped] public decimal TTLValue { get; set; }
    [NotMapped] public decimal TTLSalesTax { get; set; }
    [NotMapped] public decimal Cost { get; set; }
    [NotMapped] public string? DetailNarration { get; set; }
    [NotMapped] public int? AssociatedPurchaseId { get; set; }
}
