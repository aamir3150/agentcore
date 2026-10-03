using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class AccountService : IAccountService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public AccountService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Account?> GetAccountByNoAsync(string accountNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Accounts.FirstOrDefaultAsync(a => a.AccountNo == accountNo);
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Accounts.ToListAsync();
    }

    public async Task<IEnumerable<AccountGroup>> GetAccountGroupsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.AccountGroups.ToListAsync();
    }

    public async Task AddAccountAsync(Account account)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAccountAsync(Account account)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Accounts.Update(account);
        await context.SaveChangesAsync();
    }

    public async Task<decimal> GetAccountBalanceAsync(int accountId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var account = await context.Accounts.FindAsync(accountId);
        if (account == null) return 0;
        
        var ledgerSum = await context.GeneralLedger
            .Where(gl => gl.AccountId == accountId)
            .SumAsync(gl => gl.Debit - gl.Credit);
            
        return account.OpeningBalance + ledgerSum;
    }

    public async Task DeleteAccountAsync(int accountId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var account = await context.Accounts.FindAsync(accountId);
        if (account != null && !account.IsSystemAccount)
        {
            account.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task AddAccountGroupAsync(AccountGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.AccountGroups.Add(group);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAccountGroupAsync(AccountGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.AccountGroups.Update(group);
        await context.SaveChangesAsync();
    }

    public async Task DeleteAccountGroupAsync(int groupId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var group = await context.AccountGroups.FindAsync(groupId);
        if (group != null)
        {
            group.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }

    public async Task SetOpeningBalanceAsync(int accountId, decimal amount)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var account = await context.Accounts.FindAsync(accountId);
        if (account != null)
        {
            account.OpeningBalance = amount;
            await context.SaveChangesAsync();
        }
    }
}
