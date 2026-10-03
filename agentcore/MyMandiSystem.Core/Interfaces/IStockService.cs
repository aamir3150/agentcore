using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IStockService
{
    Task<decimal> GetCurrentStockAsync(int productId);
    Task<IEnumerable<StockTransaction>> GetStockLedgerAsync(int productId, System.DateTime fromDate, System.DateTime toDate);
    Task AddStockTransactionAsync(StockTransaction transaction);
    Task<decimal> CalculateMovingAverageCostAsync(int productId);
    Task SetOpeningStockAsync(int productId, decimal qty, decimal rate);
    Task RecalculateStockAsync(int? productId);
}
