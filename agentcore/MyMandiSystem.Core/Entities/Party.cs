using System;
using System.ComponentModel.DataAnnotations;

namespace MyMandiSystem.Core.Entities;

public enum PartyType
{
    Customer = 1,
    Vendor = 2,
    Both = 3
}

public class Party : AuditBase
{
    public int Id { get; set; }
    public string PartyNo { get; set; } = string.Empty;
    public string Name { get; set; } = string.Empty;
    public string? UrduName { get; set; }
    public PartyType PartyType { get; set; }
    public int PartyGroupId { get; set; }
    public virtual PartyGroup PartyGroup { get; set; } = null!;
    public int AccountId { get; set; }
    public virtual Account Account { get; set; } = null!;
    public int? TownId { get; set; }
    public virtual Town? Town { get; set; }
    public int? SectorId { get; set; }
    public virtual Sector? Sector { get; set; }
    public string? Phone { get; set; }
    public string? CNIC { get; set; }
    public string? Address { get; set; }
    public decimal CreditLimit { get; set; }
    public decimal WHT_Percentage { get; set; }
    public decimal DefaultMarkupRate { get; set; }
    public bool IsActive { get; set; } = true;

    [Timestamp]
    public byte[] RowVersion { get; set; } = null!;
}
