using System;
using System.Collections.Generic;

namespace MyMandiSystem.Core.Entities;

public class FinancialYear : AuditBase
{
    public int Id { get; set; }
    public string YearCode { get; set; } = string.Empty; // e.g. "2025-26"
    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }
    public bool IsCurrent { get; set; }
    public bool IsLocked { get; set; }
    public string? DatabaseFileName { get; set; }

    public virtual ICollection<CropSeason> CropSeasons { get; set; } = new List<CropSeason>();
}
