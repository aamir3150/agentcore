using System;

namespace MyMandiSystem.Core.Entities;

public class InsurancePolicy
{
    public int SrNo { get; set; }
    public string InsuranceCompanyName { get; set; } = string.Empty;
    public string CoverNote { get; set; } = string.Empty;
    public int InsuredValue { get; set; }
    public int NetPremium { get; set; }
    public DateTime FromDate { get; set; } = DateTime.Today;
    public DateTime ToDate { get; set; } = DateTime.Today.AddYears(1);
    public string? PolicyNo { get; set; }
    public string? Category { get; set; }

    // Computed UI helper properties
    public string Status
    {
        get
        {
            if (ToDate < DateTime.Today) return "EXPIRED";
            var days = (ToDate - DateTime.Today).Days;
            return $"{days} Days Left";
        }
    }

    public bool IsExpired => ToDate < DateTime.Today;
    public bool IsExpiringSoon => !IsExpired && (ToDate - DateTime.Today).Days <= 5;
}
