using System;

namespace MyMandiSystem.Core.Entities;

public sealed class LookupOption
{
    public string Code { get; init; } = string.Empty;
    public int? Id { get; init; }
    public string Name { get; init; } = string.Empty;

    public override string ToString() => Name;
}

public sealed class PayablesFormSettings
{
    public bool ShowUrduPrint { get; init; }
    public bool EnableCropSeason { get; init; }
    public DateTime FinancialYearStartDate { get; init; }
}

public enum PayablesDateMode
{
    AllDates,
    TillDate,
    DateRange
}

public sealed class AccountPayablesRequest
{
    public bool IsReceivable { get; init; }
    public PayablesDateMode DateMode { get; init; } = PayablesDateMode.AllDates;
    public DateTime? SingleDate { get; init; }
    public DateTime? FromDate { get; init; }
    public DateTime? ToDate { get; init; }
    public string CropSeasonCode { get; init; } = "0";
    public string? PartyTypeAccountNo { get; init; }
    public string PartyTypeName { get; init; } = "------- All Parties -------";
    public int? PartyGroupId { get; init; }
    public string PartyGroupName { get; init; } = "--- ALL ---";
    public string? CityName { get; init; }
    public string CityDisplayName { get; init; } = "------- All Cities -------";
    public int? TownId { get; init; }
    public string TownName { get; init; } = "--- ALL TOWNS ---";
    public int? SectorId { get; init; }
    public string SectorName { get; init; } = "--- ALL SECTORS ---";
    public decimal MinAmount { get; init; }
    public bool SortByAccountName { get; init; } = true;
    public bool ShowOnlyFinalBalance { get; init; }
    public bool ShowFinalBalances { get; init; }
    public bool IsUrduPrint { get; init; }
    public bool SendSms { get; init; }
}

/// <summary>
/// Exact 22-column ADODB Recordset parity for AgenSoft RptAccountPayables.SetReport.
/// </summary>
public sealed class AccountPayableBalanceRecord
{
    public string AccountNo { get; init; } = string.Empty;
    public string AccountName { get; init; } = string.Empty;
    public string AccountNameUrdu { get; init; } = string.Empty;
    public string ContactNo { get; init; } = string.Empty;
    public string AccountType { get; init; } = string.Empty;
    public int IsDetailed { get; init; }
    public string PLFlag { get; init; } = string.Empty;
    public int BALFlag { get; init; }
    public decimal OpeningDebit { get; init; }
    public decimal OpeningCredit { get; init; }
    public decimal OpeningBal { get; init; }
    public string OpeningBalType { get; init; } = "Dr";
    public decimal Debit { get; init; }
    public decimal Credit { get; init; }
    public decimal Bal { get; init; }
    public string BalType { get; init; } = "Dr";
    public decimal ClosingDebit { get; init; }
    public decimal ClosingCredit { get; init; }
    public decimal ClosingBal { get; init; }
    public string ClosingBalType { get; init; } = "Cr";
    public decimal TodayBal { get; init; }
    public string TodayBalType { get; init; } = "Dr";
}

