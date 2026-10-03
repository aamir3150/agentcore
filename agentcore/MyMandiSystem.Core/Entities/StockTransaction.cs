using System;

namespace MyMandiSystem.Core.Entities;

public enum StockTransactionType
{
    Opening = 1,
    Purchase = 2,
    PurchaseReturn = 3,
    Sale = 4,
    SaleReturn = 5,
    Adjustment = 6,
    Transfer = 7
}

public enum SourceModule
{
    General = 1,
    Brokerage = 2,
    Pestro = 3,
    Manual = 4
}

public class StockTransaction : AuditBase
{
    public int Id { get; set; }
    public DateTime TransactionDate { get; set; }
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public int FinancialYearId { get; set; }
    public virtual FinancialYear FinancialYear { get; set; } = null!;
    
    public StockTransactionType TransactionType { get; set; }
    public SourceModule SourceModule { get; set; }
    public int? SourceInvoiceId { get; set; }
    
    public decimal QtyIn { get; set; }
    public decimal QtyOut { get; set; }
    public decimal Rate { get; set; }
    public decimal MovingAvgCost { get; set; }
    public int? BatchId { get; set; }
    public virtual PestroBatch? Batch { get; set; }
    public string? Narration { get; set; }
}
