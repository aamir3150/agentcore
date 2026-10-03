using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IReportService
{
    Task<IEnumerable<dynamic>> GetLedgerReportAsync(int accountId, DateTime from, DateTime to);
    Task<IEnumerable<dynamic>> GetDayBookReportAsync(DateTime date);
    Task<IEnumerable<dynamic>> GetStockReportAsync(int? productId);
    Task<IEnumerable<dynamic>> GetPurchaseRegisterAsync(DateTime from, DateTime to);
    Task<IEnumerable<dynamic>> GetSaleRegisterAsync(DateTime from, DateTime to);

    // --- System Reports ---
    Task<IEnumerable<dynamic>> GetBrokerageCommissionAsync(DateTime from, DateTime to);
    Task<IEnumerable<dynamic>> GetGatePassRegisterAsync(DateTime from, DateTime to);
    Task<IEnumerable<dynamic>> GetPestroExpiryReportAsync(int daysToExpiry);
    Task<IEnumerable<dynamic>> GetProfitabilityReportAsync(DateTime from, DateTime to, string type);
    Task<IEnumerable<dynamic>> GetMasterListAsync(string entityName);
}
