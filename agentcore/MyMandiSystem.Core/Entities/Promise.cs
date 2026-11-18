using System;

namespace MyMandiSystem.Core.Entities;

public enum PromiseStatus
{
    Pending = 1,
    Fulfilled = 2,
    Overdue = 3,
    Cancelled = 4
}

public class Promise : AuditBase
{
    public int Id { get; set; }
    public int PromiseNo { get; set; }
    public int PartyId { get; set; }
    public virtual Party Party { get; set; } = null!;
    public DateTime PromiseDate { get; set; } = DateTime.Today;
    public DateTime DueDate { get; set; } = DateTime.Today.AddDays(7);
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public bool IsPayment { get; set; } // false: Receive Promise (وصولی), true: Payment Promise (ادائیگی)
    public PromiseStatus Status { get; set; } = PromiseStatus.Pending;

    // Helper UI properties
    public bool IsFulfilled => Status == PromiseStatus.Fulfilled;
    public bool IsOverdue => Status != PromiseStatus.Fulfilled && DueDate < DateTime.Today;
    public string PromiseTypeDescription => IsPayment ? "Payment (ادائیگی)" : "Receiving (وصولی)";
}
