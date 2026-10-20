using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public class YearEndPreCheckResult
{
    public string CurrentYearCode { get; set; } = string.Empty;
    public string SuggestedNewYearCode { get; set; } = string.Empty;
    public int TotalAccounts { get; set; }
    public decimal TotalDebitBalance { get; set; }
    public decimal TotalCreditBalance { get; set; }
    public int TotalVouchers { get; set; }
    public int TotalProducts { get; set; }
    public int TotalParties { get; set; }
    public bool IsBalanced => Math.Abs(TotalDebitBalance - TotalCreditBalance) < 0.01m;
    public DateTime SuggestedStartDate { get; set; }
    public DateTime SuggestedEndDate { get; set; }
}

public class YearEndRequest
{
    public string CurrentYearCode { get; set; } = string.Empty;
    public string NewYearCode { get; set; } = string.Empty;
    public DateTime NewStartDate { get; set; }
    public DateTime NewEndDate { get; set; }
    public bool LockPreviousYear { get; set; } = true;
}

public class YearEndExecutionResult
{
    public bool Success { get; set; }
    public string Message { get; set; } = string.Empty;
    public string TargetDatabaseName { get; set; } = string.Empty;
    public int AccountsCarriedForward { get; set; }
    public int ProductsCarriedForward { get; set; }
    public List<string> ExecutionLogs { get; set; } = new();
}

public interface IYearEndService
{
    Task<YearEndPreCheckResult> PerformPreCheckAsync();
    Task<YearEndExecutionResult> StartNewFinancialYearAsync(YearEndRequest request, IProgress<string>? progress = null);
    Task<bool> LockFinancialYearAsync(string yearCode);
    Task<bool> SwitchActiveFinancialYearAsync(string targetYearCode);
    Task<IEnumerable<string>> GetAvailableYearsAsync();
    Task<string> GetDatabasePathForYearAsync(string year);
}
