using System;
using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class BrokerageInvoice : AuditBase
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; } // reuse enum
    public DateTime InvoiceDate { get; set; }
    public int FinancialYearId { get; set; }
    public virtual FinancialYear FinancialYear { get; set; } = null!;
    public int CropSeasonId { get; set; }
    public virtual CropSeason CropSeason { get; set; } = null!;
    public int? ContractId { get; set; }
    public virtual Contract? Contract { get; set; }
    public int PartyId { get; set; }
    public virtual Party Party { get; set; } = null!;
    
    public decimal GrossAmount { get; set; }
    public decimal CommissionAmount { get; set; }
    public decimal WHT_Amount { get; set; }
    public decimal Soodh_Amount { get; set; }
    public decimal LaborCharges { get; set; }
    public decimal MarketCommitteeFee { get; set; }
    public decimal NetAmount { get; set; }
    public string? VehicleNo { get; set; }

    public bool IsPosted { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;

    public virtual ICollection<BrokerageInvoiceDetail> Details { get; set; } = new List<BrokerageInvoiceDetail>();
}
