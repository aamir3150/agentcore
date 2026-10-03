using System;

namespace MyMandiSystem.Core.Entities;

public class InsuranceEntry : AuditBase
{
    public int Id { get; set; }
    public DateTime EntryDate { get; set; }
    public int PartyId { get; set; }
    public virtual Party Party { get; set; } = null!;
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public decimal InsuranceRate { get; set; }
    public decimal PremiumAmount { get; set; }
    public string? PolicyNo { get; set; }
    public bool IsActive { get; set; } = true;
}
