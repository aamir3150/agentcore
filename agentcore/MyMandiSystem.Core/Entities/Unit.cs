using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class Unit : AuditBase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? ShortName { get; set; }
    public virtual ICollection<Product> Products { get; set; } = new List<Product>();
}
