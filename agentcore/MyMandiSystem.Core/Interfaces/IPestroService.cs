using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public class PestroStockBatchDto
{
    public string ProductCode { get; set; } = string.Empty;
    public string BatchNo { get; set; } = string.Empty;
    public System.DateTime ExpiryDate { get; set; }
    public decimal AvailableQty { get; set; }
    public decimal CostPrice { get; set; }
}

public interface IPestroService
{
    Task<PestroInvoice?> GetPestroInvoiceByIdAsync(int id);
    Task<PestroInvoice?> GetPestroInvoiceByNoAsync(string invoiceNo, InvoiceType type);
    Task<IEnumerable<PestroInvoice>> GetAllPestroInvoicesAsync();
    Task SavePestroInvoiceAsync(PestroInvoice invoice);
    Task DeletePestroInvoiceAsync(int id);
    Task PostPestroInvoiceToGLAsync(int id);

    Task<IEnumerable<PestroBatch>> GetAllBatchesAsync();
    Task<IEnumerable<PestroBatch>> GetActiveBatchesAsync(int productId);
    Task<IEnumerable<PestroStockBatchDto>> GetAvailableBatchesForProductAsync(string productCode);
    Task<List<PestroStockBatchDto>> AutoAllocateBatchesAsync(string productCode, decimal requiredQty);
    Task SaveBatchAsync(PestroBatch batch);
    Task<string> GetNextPestroInvoiceNoAsync(InvoiceType type, int financialYearId);
}
