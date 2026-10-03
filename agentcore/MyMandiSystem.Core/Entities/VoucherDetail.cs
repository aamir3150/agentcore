using System;

namespace MyMandiSystem.Core.Entities;

public class VoucherDetail : AuditBase
{
    public int Id { get; set; }
    public int VoucherId { get; set; }
    public virtual Voucher Voucher { get; set; } = null!;
    public int LineNo { get; set; }
    public int AccountId { get; set; }
    public virtual Account Account { get; set; } = null!;
    public string? Narration { get; set; }
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string? ChequeNo { get; set; }
    public DateTime? ChequeDate { get; set; }
}
