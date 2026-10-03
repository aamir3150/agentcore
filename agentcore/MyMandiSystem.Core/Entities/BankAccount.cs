using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class BankAccount : AuditBase
{
    public int Id { get; set; }
    public int AccountId { get; set; }
    public virtual Account Account { get; set; } = null!;
    public string BankName { get; set; } = string.Empty;
    public string BranchName { get; set; } = string.Empty;
    public string AccountNumber { get; set; } = string.Empty;
    public decimal CurrentBalance { get; set; }
    public virtual ICollection<ChequeBook> ChequeBooks { get; set; } = new List<ChequeBook>();
}
