using System;
using System.ComponentModel.DataAnnotations;

namespace MyMandiSystem.Core.Entities;

public class Product : AuditBase
{
    public int Id { get; set; }
    public string ProductCode { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? UrduName { get; set; }
    public int CompanyId { get; set; }
    public virtual Company Company { get; set; } = null!;
    public int UnitId { get; set; }
    public virtual Unit Unit { get; set; } = null!;
    public int? GroupId { get; set; }
    public virtual ProductGroup? ProductGroup { get; set; }
    public decimal PurchasePrice { get; set; }
    public decimal SalePrice { get; set; }
    public string? BarcodeNo { get; set; }
    public bool IsPestro { get; set; }
    public bool ExpiryTracking { get; set; }
    public int ReorderLevel { get; set; }
    public string? HSCode { get; set; }
    public bool IsActive { get; set; } = true;
    public decimal OpeningStock { get; set; }
    public decimal CurrentStock { get; set; }

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
