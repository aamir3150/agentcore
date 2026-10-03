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
    Task AddAccountGroupAsync(AccountGroup group);
    Task UpdateAccountGroupAsync(AccountGroup group);
    Task DeleteAccountGroupAsync(int groupId);
}
