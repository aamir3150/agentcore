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
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.FinancialYears.FirstOrDefaultAsync(y => y.IsCurrent);
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
        using var context = await _contextFactory.CreateDbContextAsync();
        return await context.CropSeasons.Where(s => s.IsActive).ToListAsync();
    }
}
