using System;
using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class AccountGroup : AuditBase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public AccountType GroupType { get; set; }
    public int? ParentGroupId { get; set; }
    public virtual AccountGroup? ParentGroup { get; set; }
    public virtual ICollection<AccountGroup> SubGroups { get; set; } = new List<AccountGroup>();
    public virtual ICollection<Account> Accounts { get; set; } = new List<Account>();
}
