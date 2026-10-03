using System;
using System.Collections.Generic;
using MyMandiSystem.Core.Enums;

namespace MyMandiSystem.Core.Entities;

public enum VoucherStatus
{
    Draft = 1,
    Posted = 2,
    Reversed = 3
}

public class Voucher : AuditBase
{
    public int Id { get; set; }
    public string VoucherNo { get; set; } = string.Empty;
    public VoucherType VoucherType { get; set; }
    public DateTime VoucherDate { get; set; } = DateTime.Today;
    public int FinancialYearId { get; set; }
    public virtual FinancialYear FinancialYear { get; set; } = null!;
    public int? CropSeasonId { get; set; }
    public virtual CropSeason? CropSeason { get; set; }
    
    public decimal TotalDebit { get; set; }
    public decimal TotalCredit { get; set; }
    public string? NarrationEnglish { get; set; }
    public string? NarrationUrdu { get; set; }
    public VoucherStatus Status { get; set; } = VoucherStatus.Draft;
    public bool IsPosted { get; set; }
    public DateTime? PostedAt { get; set; }

    // Denominations
    public int Count5000 { get; set; }
    public int Count1000 { get; set; }
    public int Count500 { get; set; }
    public int Count100 { get; set; }
    public int Count50 { get; set; }
    public int Count20 { get; set; }
    public int Count10 { get; set; }
    public int Count5 { get; set; }
    public int Count2 { get; set; }
    public int Count1 { get; set; }

    public virtual ICollection<VoucherDetail> Details { get; set; } = new List<VoucherDetail>();
}
