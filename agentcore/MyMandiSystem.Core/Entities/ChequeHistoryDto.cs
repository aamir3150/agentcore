using System;

namespace MyMandiSystem.Core.Entities;

public class ChequeHistoryDto
{
    public string AccountNo { get; set; } = string.Empty;
    public string ChequeNo { get; set; } = string.Empty;
    public DateTime EntryDate { get; set; }
    public string AccountName { get; set; } = string.Empty;
    public string? Remarks { get; set; }
    public bool IsCancelled { get; set; }
    public DateTime? CancelDate { get; set; }
    public string Status { get; set; } = "Available";
    public long? VoucherNo { get; set; }
    public decimal? Amount { get; set; }
    public string? Payee { get; set; }
}

public class BankChequeReconciliationItemDto
{
    public long VoucherNo { get; set; }
    public DateTime VoucherDate { get; set; }
    public string ChequeNo { get; set; } = string.Empty;
    public DateTime? ChequeDate { get; set; }
    public string BankAccountNo { get; set; } = string.Empty;
    public string BankAccountName { get; set; } = string.Empty;
    public string PartyAccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
    public DateTime? ReconcileDate { get; set; }
    public bool IsLost { get; set; }
    public bool IsReconciled { get; set; }
}

public class BankDepositReconciliationItemDto
{
    public long VoucherNo { get; set; }
    public DateTime VoucherDate { get; set; }
    public string SlipNo { get; set; } = string.Empty;
    public DateTime? DepositedDate { get; set; }
    public string BankAccountNo { get; set; } = string.Empty;
    public string BankAccountName { get; set; } = string.Empty;
    public string AccountNo { get; set; } = string.Empty;
    public string AccountName { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public string Narration { get; set; } = string.Empty;
    public DateTime? ReconcileDate { get; set; }
    public bool IsCheque { get; set; }
    public string? ChequeNo { get; set; }
    public bool IsReconciled { get; set; }
}
