using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class PartyGroup : AuditBase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public virtual ICollection<Party> Parties { get; set; } = new List<Party>();
}
