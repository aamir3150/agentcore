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

    // Extended fields for AgenSoft compatibility & accurate batch/tax calculations
    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public string BatchNo { get; set; } = string.Empty;

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public decimal Bonus { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public decimal DiscRatio1 { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public decimal DiscRatio2 { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public decimal STRatio { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public decimal STValue { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public decimal TTLValue { get; set; }

    [System.ComponentModel.DataAnnotations.Schema.NotMapped]
    public decimal TTLSalestax { get; set; }
}
