using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class CatalogService : ICatalogService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public CatalogService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    // --- Companies ---
    public async Task<IEnumerable<Company>> GetCompaniesAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Companies.Where(c => !c.IsDeleted).ToListAsync();
    }

    public async Task AddCompanyAsync(Company company)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Companies.Add(company);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCompanyAsync(Company company)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Companies.Update(company);
        await context.SaveChangesAsync();
    }

    public async Task DeleteCompanyAsync(int companyId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var company = await context.Companies.FindAsync(companyId);
        if (company != null)
        {
            company.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }

    // --- Units ---
    public async Task<IEnumerable<Unit>> GetUnitsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Units.Where(u => !u.IsDeleted).ToListAsync();
    }

    public async Task AddUnitAsync(Unit unit)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Units.Add(unit);
        await context.SaveChangesAsync();
    }

    public async Task UpdateUnitAsync(Unit unit)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Units.Update(unit);
        await context.SaveChangesAsync();
    }

    public async Task DeleteUnitAsync(int unitId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var unit = await context.Units.FindAsync(unitId);
        if (unit != null)
        {
            unit.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }

    // --- Towns ---
    public async Task<IEnumerable<Town>> GetTownsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Towns.Where(t => !t.IsDeleted).ToListAsync();
    }

    public async Task AddTownAsync(Town town)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Towns.Add(town);
        await context.SaveChangesAsync();
    }

    public async Task UpdateTownAsync(Town town)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Towns.Update(town);
        await context.SaveChangesAsync();
    }

    public async Task DeleteTownAsync(int townId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var town = await context.Towns.FindAsync(townId);
        if (town != null)
        {
            town.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }

    // --- Sectors ---
    public async Task<IEnumerable<Sector>> GetSectorsAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.Sectors.Include(s => s.Town).Where(s => !s.IsDeleted).ToListAsync();
    }

    public async Task AddSectorAsync(Sector sector)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Sectors.Add(sector);
        await context.SaveChangesAsync();
    }

    public async Task UpdateSectorAsync(Sector sector)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        context.Sectors.Update(sector);
        await context.SaveChangesAsync();
    }

    public async Task DeleteSectorAsync(int sectorId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var sector = await context.Sectors.FindAsync(sectorId);
        if (sector != null)
        {
            sector.IsDeleted = true;
            await context.SaveChangesAsync();
        }
    }
}
