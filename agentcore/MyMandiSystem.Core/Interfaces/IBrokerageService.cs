using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IBrokerageService
{
    Task<BrokerageInvoice?> GetBrokerageInvoiceByIdAsync(int id);
    Task<BrokerageInvoice?> GetBrokerageInvoiceByNoAsync(string invoiceNo, InvoiceType invoiceType);
    Task<IEnumerable<BrokerageInvoice>> GetAllBrokerageInvoicesAsync();
    Task SaveBrokerageInvoiceAsync(BrokerageInvoice invoice);
    Task DeleteBrokerageInvoiceAsync(int id);
    Task PostBrokerageInvoiceToGLAsync(int id);

    Task SyncLegacyMultiInvoiceAsync(string multiId, BrokerageInvoice? purchaseInvoice, BrokerageInvoice? saleInvoice, decimal profitLoss, string plsAccount = "40001");
    Task<(BrokerageInvoice? Purchase, BrokerageInvoice? Sale, string PlsAccount, decimal ProfitLoss)> GetMultiInvoiceDataAsync(string multiId);
    Task<List<BrokerageMultiInvoiceSummaryDto>> GetAllMultiInvoicesAsync();
    Task DeleteMultiInvoiceAsync(string multiId);

    Task<Contract?> GetContractByIdAsync(int id);
    Task<IEnumerable<Contract>> GetAllContractsAsync();
    Task SaveContractAsync(Contract contract);
    Task<string> GetNextContractNoAsync(int financialYearId);

    Task<IEnumerable<GatePass>> GetAllGatePassesAsync();
    Task<IEnumerable<GatePass>> GetGatePassesByStatusAsync(GatePassStatus status);
    Task SaveGatePassAsync(GatePass gatePass);
    Task<string> GetNextGatePassNoAsync();
}
