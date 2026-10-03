using System;

namespace MyMandiSystem.Core.Entities;

public enum GatePassType
{
    In = 1,
    Out = 2
}

public enum GatePassStatus
{
    Open = 1,
    Linked = 2,
    Cancelled = 3
}

public class GatePass : AuditBase
{
    public int Id { get; set; }
    public string GatePassNo { get; set; } = string.Empty;
    public DateTime GatePassDate { get; set; }
    public GatePassType GatePassType { get; set; }
    public int PartyId { get; set; }
    public virtual Party Party { get; set; } = null!;
    public string? VehicleNo { get; set; }
    public string? DriverName { get; set; }
    public string? DriverCNIC { get; set; }
    public int? InvoiceId { get; set; } // Could link to Brokerage or General
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    public int Bags { get; set; }
    public decimal GrossWeight { get; set; }
    public decimal TareWeight { get; set; }
    public decimal NetWeight { get; set; }
    public GatePassStatus Status { get; set; } = GatePassStatus.Open;
}
