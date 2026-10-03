using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IInvoiceService
{
    Task<Invoice?> GetInvoiceByIdAsync(int id);
    Task<string> GetNextInvoiceNoAsync(InvoiceType type, int financialYearId);
    Task SaveInvoiceAsync(Invoice invoice);
    Task PostInvoiceToGLAsync(int invoiceId);
    Task<IEnumerable<Invoice>> GetInvoicesByPartyAsync(int partyId);
}
