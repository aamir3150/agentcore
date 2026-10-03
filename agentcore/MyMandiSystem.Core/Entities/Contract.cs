using System;
using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public enum ContractType
{
    Purchase = 1,
    Sale = 2
}

public enum ContractStatus
{
    Draft = 1,
    Active = 2,
    PartiallyFulfilled = 3,
    Completed = 4,
    Cancelled = 5
}

public class Contract : AuditBase
{
    public int Id { get; set; }
    public string ContractNo { get; set; } = string.Empty;
    public ContractType ContractType { get; set; }
    public DateTime ContractDate { get; set; }
    public int FinancialYearId { get; set; }
    public virtual FinancialYear FinancialYear { get; set; } = null!;
    public int CropSeasonId { get; set; }
    public virtual CropSeason CropSeason { get; set; } = null!;
    public int PartyId { get; set; }
    public virtual Party Party { get; set; } = null!;
    public int ProductId { get; set; }
    public virtual Product Product { get; set; } = null!;
    
    public decimal Quantity { get; set; }
    public decimal Rate { get; set; }
    public decimal TotalAmount { get; set; }
    public decimal CommissionRate { get; set; }
    public decimal CommissionAmount { get; set; }
    public string? VehicleNo { get; set; }
    public string? DriverName { get; set; }
    public ContractStatus Status { get; set; } = ContractStatus.Draft;
    public decimal FulfilledQty { get; set; }

    public virtual ICollection<BrokerageInvoice> BrokerageInvoices { get; set; } = new List<BrokerageInvoice>();
}
