namespace MyMandiSystem.Core.Entities;

public enum DocumentType
{
    CashReceivingVoucher = 1,
    CashPaymentVoucher = 2,
    JournalVoucher = 3,
    GenPurchaseInvoice = 4,
    GenSaleInvoice = 5,
    BrokerageInvoice = 6,
    PestroInvoice = 7,
    Contract = 8,
    GatePass = 9
}

public class DocumentSequence : AuditBase
{
    public int Id { get; set; }
    public DocumentType DocumentType { get; set; }
    public string Prefix { get; set; } = string.Empty;
    public int CurrentNumber { get; set; }
    public int FinancialYearId { get; set; }
    public virtual FinancialYear FinancialYear { get; set; } = null!;
    public bool ResetOnNewYear { get; set; }
}
