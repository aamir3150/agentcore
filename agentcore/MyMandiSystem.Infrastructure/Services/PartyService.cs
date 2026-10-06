using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class PartyService : IPartyService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public PartyService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<Party?> GetPartyByNoAsync(string partyNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Parties.FirstOrDefaultAsync(p => p.PartyNo == partyNo);
    }

    public async Task<IEnumerable<Party>> GetAllPartiesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Parties
            .Include(p => p.Account)
            .Include(p => p.PartyGroup)
            .Include(p => p.Town)
            .Include(p => p.Sector)
            .Where(p => !p.IsDeleted)
            .ToListAsync();
    }

    public async Task<IEnumerable<PartyGroup>> GetPartyGroupsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.PartyGroups.Where(pg => !pg.IsDeleted).ToListAsync();
    }

    public async Task AddPartyAsync(Party party)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        // 1. Sanitize nullable foreign keys
        if (party.TownId.HasValue && party.TownId.Value <= 0)
        {
            party.TownId = null;
        }
        if (party.SectorId.HasValue && party.SectorId.Value <= 0)
        {
            party.SectorId = null;
        }

        // 2. Ensure PartyGroup exists and is valid
        if (party.PartyGroupId <= 0)
        {
            var defaultGroup = await context.PartyGroups.FirstOrDefaultAsync(pg => !pg.IsDeleted);
            if (defaultGroup == null)
            {
                defaultGroup = new PartyGroup 
                { 
                    Name = party.PartyType == PartyType.Vendor ? "Zamindaraan (Farmers)" : "General Customers" 
                };
                context.PartyGroups.Add(defaultGroup);
                await context.SaveChangesAsync();
            }
            party.PartyGroupId = defaultGroup.Id;
        }

        // 3. Auto-create Ledger Account
        if (party.Account == null)
        {
            var accountGroups = await context.AccountGroups.ToListAsync();
            AccountGroup? targetGroup = null;

            if (party.PartyType == PartyType.Customer)
                targetGroup = accountGroups.FirstOrDefault(g => g.GroupType == AccountType.Asset);
            else if (party.PartyType == PartyType.Vendor)
                targetGroup = accountGroups.FirstOrDefault(g => g.GroupType == AccountType.Liability);

            if (targetGroup == null)
            {
                var groupType = party.PartyType == PartyType.Vendor ? AccountType.Liability : AccountType.Asset;
                var groupName = party.PartyType == PartyType.Vendor ? "Accounts Payable (Zamindaraan)" : "Accounts Receivable (Customers)";
                targetGroup = new AccountGroup { Name = groupName, GroupType = groupType };
                context.AccountGroups.Add(targetGroup);
                await context.SaveChangesAsync();
            }

            party.Account = new Account
            {
                AccountNo = !string.IsNullOrWhiteSpace(party.PartyNo) ? party.PartyNo : "P-" + DateTime.Now.Ticks.ToString().Substring(10),
                Name = party.Name,
                UrduName = party.UrduName,
                AccountGroupId = targetGroup.Id,
                AccountType = targetGroup.GroupType,
                IsSystemAccount = true,
                IsActive = true
            };
        }

        context.Parties.Add(party);
        await context.SaveChangesAsync();

        // Update AccountNo to match PartyNo to maintain standard tracking
        if (party.PartyNo != null && party.Account != null && party.Account.AccountNo != party.PartyNo)
        {
            party.Account.AccountNo = party.PartyNo;
            await context.SaveChangesAsync();
        }
    }

    public async Task UpdatePartyAsync(Party party)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        if (party.TownId.HasValue && party.TownId.Value <= 0)
        {
            party.TownId = null;
        }
        if (party.SectorId.HasValue && party.SectorId.Value <= 0)
        {
            party.SectorId = null;
        }

        context.Parties.Update(party);
        
        if (party.AccountId > 0)
        {
            var account = await context.Accounts.FindAsync(party.AccountId);
            if (account != null)
            {
                account.Name = party.Name;
                account.UrduName = party.UrduName;
            }
        }

        await context.SaveChangesAsync();
    }

    public async Task DeletePartyAsync(int partyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var party = await context.Parties.Include(p => p.Account).FirstOrDefaultAsync(p => p.Id == partyId);
        if (party != null)
        {
            party.IsDeleted = true;
            if (party.Account != null) party.Account.IsDeleted = true; // Soft delete linked account
            await context.SaveChangesAsync();
        }
    }

    // --- Party Groups ---

    public async Task AddPartyGroupAsync(PartyGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.PartyGroups.Add(group);
        await context.SaveChangesAsync();
    }

    public async Task UpdatePartyGroupAsync(PartyGroup group)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.PartyGroups.Update(group);
        await context.SaveChangesAsync();
    }

    public async Task DeletePartyGroupAsync(int groupId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var group = await context.PartyGroups.FindAsync(groupId);
        if (group != null)
        {
            group.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }

    // --- Salesmen ---

    public async Task<IEnumerable<Salesman>> GetAllSalesmenAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Salesmen
            .Include(s => s.Account)
            .Where(s => !s.IsDeleted)
            .ToListAsync();
    }

    public async Task AddSalesmanAsync(Salesman salesman)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        
        if (salesman.Account == null)
        {
            var accountGroups = await context.AccountGroups.ToListAsync();
            int groupId = accountGroups.FirstOrDefault(g => g.GroupType == AccountType.Liability)?.Id ?? 1;

            salesman.Account = new Account
            {
                AccountNo = "S-" + DateTime.Now.Ticks.ToString().Substring(10),
                Name = salesman.Name + " (Commission Payable)",
                AccountGroupId = groupId,
                AccountType = AccountType.Liability,
                IsSystemAccount = true,
                IsActive = true
            };
        }

        context.Salesmen.Add(salesman);
        await context.SaveChangesAsync();
    }

    public async Task UpdateSalesmanAsync(Salesman salesman)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Salesmen.Update(salesman);
        await context.SaveChangesAsync();
    }

    public async Task DeleteSalesmanAsync(int salesmanId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var salesman = await context.Salesmen.Include(s => s.Account).FirstOrDefaultAsync(s => s.Id == salesmanId);
        if (salesman != null)
        {
            salesman.IsDeleted = true;
            if (salesman.Account != null) salesman.Account.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }
}
