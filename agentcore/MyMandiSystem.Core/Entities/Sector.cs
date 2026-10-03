using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class Sector : AuditBase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int TownId { get; set; }
    public virtual Town Town { get; set; } = null!;
    public virtual ICollection<Party> Parties { get; set; } = new List<Party>();
}
