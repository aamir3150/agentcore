using System;

namespace MyMandiSystem.Core.Entities;

public class LedgerReportItem
{
    public string VoucherNo { get; set; } = string.Empty;
    public DateTime TransactionDate { get; set; }
    public string Narration { get; set; } = string.Empty;
    public decimal Debit { get; set; }
    public decimal Credit { get; set; }
    public string Balance { get; set; } = string.Empty;

    public string Reference
    {
        get => VoucherNo;
        set => VoucherNo = value;
    }
}
