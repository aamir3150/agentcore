using System;

namespace MyMandiSystem.Core.Entities;

public enum DepositType
{
    Cash = 1,
    Cheque = 2
}

public class BankDeposit : AuditBase
{
    public int Id { get; set; }
    public int BankAccountId { get; set; }
    public virtual BankAccount BankAccount { get; set; } = null!;
    public DepositType DepositType { get; set; }
    public decimal Amount { get; set; }
    public DateTime DepositDate { get; set; }
    public string? ReferenceNo { get; set; }
    public int? SourceChequeId { get; set; }
    public virtual Cheque? SourceCheque { get; set; }
    public bool IsReconciled { get; set; }
    public DateTime? ReconciledDate { get; set; }
    public int? VoucherId { get; set; }
    public virtual Voucher? Voucher { get; set; }
}
