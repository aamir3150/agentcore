using System;
using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public enum BatchMode
{
    Auto = 1,
    Manual = 2
}

public class PestroInvoice : AuditBase
{
    public int Id { get; set; }
    public string InvoiceNo { get; set; } = string.Empty;
    public InvoiceType InvoiceType { get; set; }
    public DateTime InvoiceDate { get; set; }
    public int FinancialYearId { get; set; }
    public virtual FinancialYear FinancialYear { get; set; } = null!;
    public int CropSeasonId { get; set; }
    public virtual CropSeason CropSeason { get; set; } = null!;
    public int PartyId { get; set; }
    public virtual Party Party { get; set; } = null!;
    public int? SalesmanId { get; set; }
    public virtual Salesman? Salesman { get; set; }

    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal NetAmount { get; set; }
    public BatchMode BatchMode { get; set; } = BatchMode.Auto;
    
    public bool IsPosted { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    public int? LinkedInvoiceId { get; set; }
    public virtual PestroInvoice? LinkedInvoice { get; set; }

    public virtual ICollection<PestroInvoiceDetail> Details { get; set; } = new List<PestroInvoiceDetail>();
}
