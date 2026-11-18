using MyMandiSystem.Core.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public interface IAccountService
{
    Task<Account?> GetAccountByNoAsync(string accountNo);
    Task<IEnumerable<Account>> GetAllAccountsAsync();
    Task<IEnumerable<AccountGroup>> GetAccountGroupsAsync();
    Task AddAccountAsync(Account account);
    Task UpdateAccountAsync(Account account);
    Task<decimal> GetAccountBalanceAsync(int accountId);
    Task DeleteAccountAsync(int accountId);
    Task SetOpeningBalanceAsync(int accountId, decimal amount);
    Task SetOpeningBalanceAsync(int accountId, decimal debit, decimal credit);
    Task AddAccountGroupAsync(AccountGroup group);
    Task UpdateAccountGroupAsync(AccountGroup group);
    Task DeleteAccountGroupAsync(int groupId);
    Task RecalculateAllAccountBalancesAsync();

    // Report Settings (P&L, Balance Sheet, Expense Report - AgenSoft Exact Parity)
    Task<IEnumerable<AccountReportSettingDto>> GetAccountReportSettingsAsync();
    Task UpdateProfitLossSettingsAsync(IEnumerable<AccountReportSettingDto> settings);
    Task UpdateBalanceSheetSettingsAsync(IEnumerable<AccountReportSettingDto> settings);
    Task UpdateExpenseReportSettingsAsync(IEnumerable<AccountReportSettingDto> settings);

    // Accounts Opening Balances (AgenSoft Exact Parity)
    Task<List<string>> GetOpeningBalanceAccountTypesAsync();
    Task<List<CropSeasonItemDto>> GetOpeningCropSeasonsAsync();
    Task<List<AccountOpeningBalanceDto>> GetAccountsOpeningBalancesAsync(string accountType);
    Task SaveAccountsOpeningBalancesAsync(IEnumerable<AccountOpeningBalanceDto> items);
}
