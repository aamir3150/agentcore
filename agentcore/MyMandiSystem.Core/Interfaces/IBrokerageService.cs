using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IBrokerageService
{
    Task<BrokerageInvoice?> GetBrokerageInvoiceByIdAsync(int id);
    Task<IEnumerable<BrokerageInvoice>> GetAllBrokerageInvoicesAsync();
    Task SaveBrokerageInvoiceAsync(BrokerageInvoice invoice);
    Task PostBrokerageInvoiceToGLAsync(int id);

    Task<Contract?> GetContractByIdAsync(int id);
    Task<IEnumerable<Contract>> GetAllContractsAsync();
    Task SaveContractAsync(Contract contract);
    Task<string> GetNextContractNoAsync(int financialYearId);

    Task<IEnumerable<GatePass>> GetAllGatePassesAsync();
    Task<IEnumerable<GatePass>> GetGatePassesByStatusAsync(GatePassStatus status);
    Task SaveGatePassAsync(GatePass gatePass);
    Task<string> GetNextGatePassNoAsync();
}
