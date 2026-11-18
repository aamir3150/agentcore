using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace MyMandiSystem.Core.Entities;

public enum InvoiceType
{
    GenPurchase = 1,
    GenPurchaseReturn = 2,
    GenSale = 3,
    GenSaleReturn = 4,
    PestroPurchase = 5,
    PestroPurchaseReturn = 6,
    PestroSale = 7,
    PestroSaleReturn = 8,
    BrkPurchase = 9,
    BrkSale = 10
}

public enum InvoiceStatus
{
    Draft = 1,
    Posted = 2,
    Reversed = 3
}

public class Invoice : AuditBase
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
    
    public decimal SubTotal { get; set; }
    public decimal DiscountAmount { get; set; }
    public decimal DiscountPercentage { get; set; }
    public decimal TaxAmount { get; set; }
    public decimal TaxPercentage { get; set; }
    public decimal FreightCharges { get; set; }
    public decimal LaborCharges { get; set; }
    public decimal MarketCommitteeFee { get; set; }
    public decimal NetAmount { get; set; }

    public bool IsPosted { get; set; }
    public DateTime? PostedAt { get; set; }
    public InvoiceStatus Status { get; set; } = InvoiceStatus.Draft;
    
    public int? LinkedInvoiceId { get; set; }
    public virtual Invoice? LinkedInvoice { get; set; }

    // AgenSoft Legacy Compatibility Fields
    [NotMapped] public string? BillNo { get; set; }
    [NotMapped] public DateTime? BillDate { get; set; }
    [NotMapped] public decimal ExpPlus { get; set; }
    [NotMapped] public decimal ExpMinus { get; set; }
    [NotMapped] public decimal ComRatio { get; set; }
    [NotMapped] public decimal ComAmount { get; set; }
    [NotMapped] public decimal PaidAmount { get; set; }
    [NotMapped] public decimal ReceivedAmount { get; set; }
    [NotMapped] public decimal SpecialDiscount { get; set; }
    [NotMapped] public decimal BankTax { get; set; }
    [NotMapped] public bool IsMulti { get; set; }
    [NotMapped] public decimal Freight { get; set; }
    [NotMapped] public bool IsFreightPlus { get; set; }
    [NotMapped] public string? VehicleNo { get; set; }
    [NotMapped] public string? Narration { get; set; }
    [NotMapped] public string? NarrationUrdu { get; set; }
    [NotMapped] public decimal WHTRatio { get; set; }
    [NotMapped] public decimal WHTAmount { get; set; }
    [NotMapped] public bool WHTPlus { get; set; }
    [NotMapped] public int? AssociatedSaleId { get; set; }
    [NotMapped] public int? AssociatedPurchaseId { get; set; }
    [NotMapped] public string? CustomerIDCom { get; set; }
    [NotMapped] public decimal CustomerComRatio { get; set; }
    [NotMapped] public int? SectorId { get; set; }

    public virtual ICollection<InvoiceDetail> Details { get; set; } = new List<InvoiceDetail>();
}
