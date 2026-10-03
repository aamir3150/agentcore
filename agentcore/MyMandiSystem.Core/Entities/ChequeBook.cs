using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class ChequeBook : AuditBase
{
    public int Id { get; set; }
    public int BankAccountId { get; set; }
    public virtual BankAccount BankAccount { get; set; } = null!;
    public int SeriesStart { get; set; }
    public int SeriesEnd { get; set; }
    public bool IsActive { get; set; } = true;
    public virtual ICollection<Cheque> Cheques { get; set; } = new List<Cheque>();
}
