using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class Salesman : AuditBase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Phone { get; set; }
    public decimal CommissionRate { get; set; }
    public int AccountId { get; set; }
    public virtual Account Account { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
