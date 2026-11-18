using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System.Linq;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class SystemService : ISystemService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;

    public SystemService(IDbContextFactory<AppDbContext> contextFactory)
    {
        _contextFactory = contextFactory;
    }

    public async Task<FinancialYear?> GetCurrentYearAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var y = await context.FinancialYears.FirstOrDefaultAsync(y => y.IsCurrent);
            if (y != null) return y;
        }
        catch { }

        // Fallback for native AgenSoft database
        return new FinancialYear
        {
            Id = 1,
            YearCode = "2025-26",
            IsCurrent = true,
            StartDate = new DateTime(2025, 7, 1),
            EndDate = new DateTime(2026, 6, 30)
        };
    }

    public async Task<string> GetNextDocumentNoAsync(DocumentType type, int yearId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var sequence = await context.DocumentSequences
            .FirstOrDefaultAsync(s => s.DocumentType == type && s.FinancialYearId == yearId);

        if (sequence == null)
        {
            // Create default sequence if not found
            sequence = new DocumentSequence
            {
                DocumentType = type,
                FinancialYearId = yearId,
                Prefix = type.ToString().Substring(0, 3).ToUpper(),
                CurrentNumber = 0
            };
            context.DocumentSequences.Add(sequence);
        }

        sequence.CurrentNumber++;
        await context.SaveChangesAsync();

        return $"{sequence.Prefix}-{sequence.CurrentNumber:D5}";
    }

    public async Task<string> GetConfigValueAsync(string key)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var config = await context.SystemConfigs.FirstOrDefaultAsync(c => c.ConfigKey == key);
        return config?.ConfigValue ?? string.Empty;
    }

    public async Task SetConfigValueAsync(string key, string value)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var config = await context.SystemConfigs.FirstOrDefaultAsync(c => c.ConfigKey == key);
        if (config == null)
        {
            config = new SystemConfig { ConfigKey = key, ConfigValue = value };
            context.SystemConfigs.Add(config);
        }
        else
        {
            config.ConfigValue = value;
        }
        await context.SaveChangesAsync();
    }

    public async Task<System.Collections.Generic.List<CropSeason>> GetActiveSeasonsAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var list = await context.CropSeasons.Where(s => s.IsActive && !s.IsDeleted).OrderBy(s => s.StartDate).ToListAsync();
            if (list.Any()) return list;
        }
        catch { }

        // Fallback for native AgenSoft database (Crops from Products table or defaults)
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var crops = await context.Database.SqlQueryRaw<string>(
                "IF OBJECT_ID('Products', 'U') IS NOT NULL SELECT ProductName AS Value FROM Products WHERE IsCrop = 1 ELSE SELECT CAST('cotton' AS VARCHAR(50)) AS Value"
            ).ToListAsync();

            if (crops.Any())
            {
                int id = 1;
                return crops.Select(c => new CropSeason { Id = id++, Name = c, IsActive = true }).ToList();
            }
        }
        catch { }

        return new System.Collections.Generic.List<CropSeason>
        {
            new() { Id = 1, Name = "cotton", IsActive = true },
            new() { Id = 2, Name = "wheat", IsActive = true }
        };
    }

    public async Task<System.Collections.Generic.List<CropSeason>> GetAllSeasonsAsync()
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var list = await context.CropSeasons.Where(s => !s.IsDeleted).OrderBy(s => s.StartDate).ToListAsync();
            if (list.Any()) return list;
        }
        catch { }

        return await GetActiveSeasonsAsync();
    }

    public async Task AddCropSeasonAsync(CropSeason season)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        if (season.FinancialYearId <= 0)
        {
            var currentYear = await context.FinancialYears.FirstOrDefaultAsync(y => y.IsCurrent && !y.IsDeleted) 
                              ?? await context.FinancialYears.FirstOrDefaultAsync(y => !y.IsDeleted);
            if (currentYear != null)
            {
                season.FinancialYearId = currentYear.Id;
            }
        }
        season.CreatedAt = DateTime.Now;
        season.CreatedBy = 1;
        context.CropSeasons.Add(season);
        await context.SaveChangesAsync();
    }

    public async Task UpdateCropSeasonAsync(CropSeason season)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.CropSeasons.FindAsync(season.Id);
        if (existing != null)
        {
            existing.Name = season.Name;
            existing.StartDate = season.StartDate;
            existing.EndDate = season.EndDate;
            existing.IsActive = season.IsActive;
            existing.UpdatedAt = DateTime.Now;
            existing.UpdatedBy = 1;
            await context.SaveChangesAsync();
        }
    }

    public async Task DeleteCropSeasonAsync(int seasonId)
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var existing = await context.CropSeasons.FindAsync(seasonId);
        if (existing != null)
        {
            existing.IsDeleted = true;
            existing.UpdatedAt = DateTime.Now;
            await context.SaveChangesAsync();
        }
    }
}
