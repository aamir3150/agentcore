using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class Town : AuditBase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<Party> Parties { get; set; } = new List<Party>();
    public virtual ICollection<Sector> Sectors { get; set; } = new List<Sector>();
}
