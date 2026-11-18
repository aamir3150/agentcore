using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using MyMandiSystem.Core.Entities;

namespace MyMandiSystem.Core.Interfaces;

public interface IReportService
{
    Task<IEnumerable<dynamic>> GetLedgerReportAsync(int accountId, DateTime? from = null, DateTime? to = null, bool withoutOpening = false, bool isUrdu = false);
    Task<IEnumerable<dynamic>> GetDayBookReportAsync(DateTime date);
    Task<IEnumerable<dynamic>> GetStockReportAsync(int? productId);
    Task<IEnumerable<dynamic>> GetPurchaseRegisterAsync(DateTime from, DateTime to);
    Task<IEnumerable<dynamic>> GetSaleRegisterAsync(DateTime from, DateTime to);
    Task<IEnumerable<dynamic>> GetProfitabilityReportAsync(DateTime from, DateTime to, string type);

    // --- System Reports ---
    Task<IEnumerable<dynamic>> GetBrokerageCommissionAsync(DateTime from, DateTime to);
    Task<IEnumerable<dynamic>> GetGatePassRegisterAsync(DateTime from, DateTime to);
    Task<IEnumerable<dynamic>> GetPestroExpiryReportAsync(int daysToExpiry);
    // --- Account Reports (AgenSoft Specification) ---
    Task<PayablesFormSettings> GetPayablesFormSettingsAsync();
    Task<IReadOnlyList<LookupOption>> GetPayablesCropSeasonsAsync();
    Task<IReadOnlyList<LookupOption>> GetPayablePartyTypesAsync();
    Task<IReadOnlyList<LookupOption>> GetPartyGroupsLookupAsync();
    Task<IReadOnlyList<string>> GetPartyCitiesLookupAsync();
    Task<IReadOnlyList<LookupOption>> GetTownsLookupAsync();
    Task<IReadOnlyList<LookupOption>> GetSectorsByTownLookupAsync(int? townId);
    Task<List<AccountPayableBalanceRecord>> GetAccountPayablesRecordsAsync(AccountPayablesRequest request);
    Task<int> SendPayablesSmsAsync(IEnumerable<AccountPayableBalanceRecord> records, string companyName, string companyPhone, string ownerMobile);
    Task<IEnumerable<dynamic>> GetAccountPayablesReportAsync(DateTime? from, DateTime? to, string partyType, string partyGroup, string city, string town, string sector, decimal minAmount, bool showOnlyFinalBalance, bool excludeZeroBalance, bool sortByAccountName);
    Task<IEnumerable<dynamic>> GetAccountReceivablesReportAsync(DateTime? from, DateTime? to, string partyType, string partyGroup, string city, string town, string sector, decimal minAmount, bool showOnlyFinalBalance, bool excludeZeroBalance, bool sortByAccountName);
    Task<IEnumerable<dynamic>> GetBankStatementReportAsync(int bankAccountId, DateTime? from, DateTime? to, bool includeOtherVouchers);
    Task<IEnumerable<dynamic>> GetCashBookReportAsync(DateTime? from, DateTime? to, bool isDefaultCashBookView);
    Task<IEnumerable<dynamic>> GetDailyVouchersDetailReportAsync(DateTime? from, DateTime? to, string voucherType, int? fromVoucherNo, int? toVoucherNo, bool excludeAccountsVouchers, bool includeInvoices);
    Task<IEnumerable<dynamic>> GetTrialBalanceReportAsync(DateTime? from, DateTime? to, bool showTransactionAccountsOnly, bool showGroupsOnly, bool excludeZeroBalance, decimal minAmount, int partiesScope, bool sortByAccountName);
    Task<IEnumerable<dynamic>> GetProfitLossStatementReportAsync(DateTime? from, DateTime? to, bool withoutOpening, bool includeStock, bool includeStockCrops, bool includeStockPestro);

    Task<IEnumerable<dynamic>> GetMasterListAsync(string entityName);
}
