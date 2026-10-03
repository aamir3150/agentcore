using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class Company : AuditBase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
