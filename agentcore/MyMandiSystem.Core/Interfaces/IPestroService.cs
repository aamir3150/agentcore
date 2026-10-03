using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IPestroService
{
    Task<PestroInvoice?> GetPestroInvoiceByIdAsync(int id);
    Task<IEnumerable<PestroInvoice>> GetAllPestroInvoicesAsync();
    Task SavePestroInvoiceAsync(PestroInvoice invoice);
    Task PostPestroInvoiceToGLAsync(int id);

    Task<IEnumerable<PestroBatch>> GetAllBatchesAsync();
    Task<IEnumerable<PestroBatch>> GetActiveBatchesAsync(int productId);
    Task SaveBatchAsync(PestroBatch batch);
    Task<string> GetNextPestroInvoiceNoAsync(InvoiceType type, int financialYearId);
}
