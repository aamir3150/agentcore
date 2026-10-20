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
        if (string.IsNullOrWhiteSpace(accountNo)) return null;
        using var context = await _contextFactory.CreateDbContextAsync();
        var trimmed = accountNo.Trim();
        return await context.Accounts.FirstOrDefaultAsync(a => !a.IsDeleted && a.AccountNo == trimmed);
    }

    public async Task<IEnumerable<Account>> GetAllAccountsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Accounts
            .Include(a => a.AccountGroup)
            .Where(a => !a.IsDeleted)
            .OrderBy(a => a.AccountNo)
            .ToListAsync();
    }

    public async Task<IEnumerable<AccountGroup>> GetAccountGroupsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.AccountGroups.ToListAsync();
    }

    public async Task AddAccountAsync(Account account)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        account.CurrentBalance = account.OpeningBalance;
        context.Accounts.Add(account);
        await context.SaveChangesAsync();
    }

    public async Task UpdateAccountAsync(Account account)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var ledgerSum = await context.GeneralLedger
            .Where(gl => gl.AccountId == account.Id)
            .SumAsync(gl => (decimal?)(gl.Debit - gl.Credit)) ?? 0m;
        account.CurrentBalance = account.OpeningBalance + ledgerSum;
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
            var ledgerSum = await context.GeneralLedger
                .Where(gl => gl.AccountId == accountId)
                .SumAsync(gl => (decimal?)(gl.Debit - gl.Credit)) ?? 0m;
            account.CurrentBalance = account.OpeningBalance + ledgerSum;
            await context.SaveChangesAsync();
        }
    }

    public async Task RecalculateAllAccountBalancesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var accounts = await context.Accounts.ToListAsync();
        var ledgerSums = await context.GeneralLedger
            .GroupBy(gl => gl.AccountId)
            .Select(g => new { AccountId = g.Key, Net = g.Sum(x => x.Debit - x.Credit) })
            .ToDictionaryAsync(x => x.AccountId, x => x.Net);

        foreach (var acc in accounts)
        {
            decimal net = ledgerSums.TryGetValue(acc.Id, out var sum) ? sum : 0m;
            acc.CurrentBalance = acc.OpeningBalance + net;
        }

        await context.SaveChangesAsync();
    }
}
