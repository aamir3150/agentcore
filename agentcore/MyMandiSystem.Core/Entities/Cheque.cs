using System;

namespace MyMandiSystem.Core.Entities;

public enum ChequeStatus
{
    Blank = 1,
    Issued = 2,
    Cleared = 3,
    Bounced = 4,
    Lost = 5,
    Cancelled = 6
}

public class Cheque : AuditBase
{
    public int Id { get; set; }
    public int ChequeBookId { get; set; }
    public virtual ChequeBook ChequeBook { get; set; } = null!;
    public string ChequeNo { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public int? IssuedToPartyId { get; set; }
    public virtual Party? IssuedToParty { get; set; }
    public DateTime? IssueDate { get; set; }
    public DateTime? ClearanceDate { get; set; }
    public ChequeStatus Status { get; set; }
    public int? VoucherId { get; set; }
    public virtual Voucher? Voucher { get; set; }
    public string? BounceReason { get; set; }
}
