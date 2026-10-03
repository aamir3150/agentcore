using System;
using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class CropSeason : AuditBase
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public int FinancialYearId { get; set; }
    public virtual FinancialYear FinancialYear { get; set; } = null!;
    public bool IsActive { get; set; } = true;
}
