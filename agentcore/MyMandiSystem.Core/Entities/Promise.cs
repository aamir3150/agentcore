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
    public int PartyId { get; set; }
    public virtual Party Party { get; set; } = null!;
    public DateTime PromiseDate { get; set; }
    public DateTime DueDate { get; set; }
    public decimal Amount { get; set; }
    public string? Reason { get; set; }
    public PromiseStatus Status { get; set; } = PromiseStatus.Pending;
}
