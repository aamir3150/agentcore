using System;

namespace MyMandiSystem.Core.Entities;

public enum SourceType
{
    Voucher = 1,
    GenInvoice = 2,
    BrkInvoice = 3,
    PestroInvoice = 4
}

public class GeneralLedger : AuditBase
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public int AccountId { get; set; }
    public virtual Account Account { get; set; } = null!;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public decimal Balance { get; set; }
    public string? Narration { get; set; }
    public SourceType SourceType { get; set; }
    public int SourceId { get; set; }
    public int FinancialYearId { get; set; }
    public virtual FinancialYear FinancialYear { get; set; } = null!;
    public int? CropSeasonId { get; set; }
    public virtual CropSeason? CropSeason { get; set; }
    public bool IsReversed { get; set; }
    public int? ReversedById { get; set; }
    public virtual GeneralLedger? ReversedBy { get; set; }
}
