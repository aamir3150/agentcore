using MyMandiSystem.Core.Printing;

namespace MyMandiSystem.Core.PrintModels;

/// <summary>
/// English-only print model for AgenSoft CrpChequeDepost / CrpChequeDepositMulti parity.
/// </summary>
public sealed class ChequeDepositSlip : IPrintable
{
    public string Title { get; init; } = "Cheque Deposit";
    public string CompanyName { get; init; } = string.Empty;
    public string CompanyAddress { get; init; } = string.Empty;
    public string CompanyPhone { get; init; } = string.Empty;
    public string CompanyEmail { get; init; } = string.Empty;
    public string FooterText { get; init; } = string.Empty;
    public bool HasBranchOverride => !string.IsNullOrWhiteSpace(CompanyName);

    public long VoucherNo { get; init; }
    public long IdBranch { get; init; }
    public long DisplayVoucherNo => IdBranch > 0 ? IdBranch : VoucherNo;
    public DateTime VoucherDate { get; init; }
    public DateTime? VoucherTime { get; init; }

    public string ChequeNo { get; init; } = string.Empty;
    public DateTime? ChequeDate { get; init; }
    public string SlipNo { get; init; } = string.Empty;
    public DateTime? DepositedDate { get; init; }
    public DateTime? ReconcileDate { get; init; }
    public bool IsReconciled => ReconcileDate.HasValue;
    public bool IsPosted { get; init; }

    public string BankAccountNo { get; init; } = string.Empty;
    public string BankName { get; init; } = string.Empty;
    public string AccountNo { get; init; } = string.Empty;
    public string PayToName { get; init; } = string.Empty;

    public decimal Amount { get; init; }
    public string AmountInWords { get; init; } = string.Empty;
    public string DepositedBy { get; init; } = string.Empty;
    public string Narration { get; init; } = string.Empty;
    public string CropSeason { get; init; } = string.Empty;
    public string UserName { get; init; } = string.Empty;

    public byte[]? FPData { get; init; }
    public bool HasThumb => FPData is { Length: > 0 };

    public bool IsMultiMode { get; init; }
    public IReadOnlyList<ChequeDepositSlipItem> MultiItems { get; init; } = Array.Empty<ChequeDepositSlipItem>();
    public decimal MultiTotalAmount => MultiItems.Sum(x => x.Amount);

    public PrintOptions DefaultOptions => IsMultiMode
        ? new(PaperKind.A4, Landscape: false, Title: Title, Copies: 1)
        : new(PaperKind.A5, Landscape: true, Title: Title, Copies: 1);
}

public sealed record ChequeDepositSlipItem(
    long VoucherNo,
    string ChequeNo,
    DateTime? ChequeDate,
    string SlipNo,
    DateTime? DepositedDate,
    string BankAccountNo,
    string BankName,
    string AccountNo,
    string PayToName,
    string DepositedBy,
    decimal Amount);

