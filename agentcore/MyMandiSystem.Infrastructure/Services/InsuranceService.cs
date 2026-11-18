using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class InsuranceService : IInsuranceService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public InsuranceService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<IEnumerable<InsurancePolicy>> GetAllPoliciesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.InsurancePolicies
            .OrderByDescending(p => p.ToDate)
            .ToListAsync();
    }

    public async Task<InsurancePolicy?> GetPolicyBySrNoAsync(int srNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.InsurancePolicies.FindAsync(srNo);
    }

    public async Task AddPolicyAsync(InsurancePolicy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.InsurancePolicies.Add(policy);
        await context.SaveChangesAsync();
    }

    public async Task UpdatePolicyAsync(InsurancePolicy policy)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.InsurancePolicies.Update(policy);
        await context.SaveChangesAsync();
    }

    public async Task DeletePolicyAsync(int srNo)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.InsurancePolicies.FindAsync(srNo);
        if (existing != null)
        {
            context.InsurancePolicies.Remove(existing);
            await context.SaveChangesAsync();
        }
    }
}
