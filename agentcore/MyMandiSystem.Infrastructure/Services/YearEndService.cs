using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class YearEndService : IYearEndService
{
    private readonly IDbContextFactory<AppDbContext> _contextFactory;
    private readonly ISystemConfigService _configService;

    public YearEndService(IDbContextFactory<AppDbContext> contextFactory, ISystemConfigService configService)
    {
        _contextFactory = contextFactory;
        _configService = configService;
    }

    public async Task<YearEndPreCheckResult> PerformPreCheckAsync()
    {
        using var context = await _contextFactory.CreateDbContextAsync();
        var currentYear = _configService.GetCurrentFinancialYear();

        var accounts = await context.Accounts.AsNoTracking().ToListAsync();
        var vouchersCount = await context.Vouchers.CountAsync();
        var productsCount = await context.Products.CountAsync();
        var partiesCount = await context.Parties.CountAsync();

        decimal totalDebit = accounts.Where(a => a.CurrentBalance > 0).Sum(a => a.CurrentBalance);
        decimal totalCredit = accounts.Where(a => a.CurrentBalance < 0).Sum(a => Math.Abs(a.CurrentBalance));

        // Calculate suggested next year code (e.g. 2025-26 -> 2026-27)
        string suggestedNextYear = CalculateNextYearCode(currentYear);

        // Calculate suggested dates
        var curFinYear = await context.FinancialYears.FirstOrDefaultAsync(y => y.IsCurrent);
        DateTime startDate = curFinYear?.EndDate.AddDays(1) ?? new DateTime(DateTime.Now.Year, 7, 1);
        DateTime endDate = startDate.AddYears(1).AddDays(-1);

        return new YearEndPreCheckResult
        {
            CurrentYearCode = currentYear,
            SuggestedNewYearCode = suggestedNextYear,
            TotalAccounts = accounts.Count,
            TotalDebitBalance = totalDebit,
            TotalCreditBalance = totalCredit,
            TotalVouchers = vouchersCount,
            TotalProducts = productsCount,
            TotalParties = partiesCount,
            SuggestedStartDate = startDate,
            SuggestedEndDate = endDate
        };
    }

    public async Task<YearEndExecutionResult> StartNewFinancialYearAsync(YearEndRequest request, IProgress<string>? progress = null)
    {
        var result = new YearEndExecutionResult();
        var prefix = _configService.GetDatabasePrefix();
        var targetDbName = $"{prefix}_{request.NewYearCode}";
        result.TargetDatabaseName = targetDbName;

        try
        {
            progress?.Report($"1/7: Validating target financial year {request.NewYearCode}...");
            result.ExecutionLogs.Add($"Target database: {targetDbName}");

            // Verify target does not already exist
            var targetConnStr = _configService.GetConnectionString(targetDbName);
            var masterConnStr = _configService.GetConnectionString("master");

            await using (var masterConn = new SqlConnection(masterConnStr))
            {
                await masterConn.OpenAsync();
                await using var checkCmd = new SqlCommand($"SELECT DB_ID('{targetDbName}')", masterConn);
                var dbId = await checkCmd.ExecuteScalarAsync();
                if (dbId != null && dbId != DBNull.Value)
                {
                    throw new InvalidOperationException($"Target database '{targetDbName}' already exists on SQL Server.");
                }
            }

            // Step 1: Initialize target DB via EF Core Migrations
            progress?.Report($"2/7: Creating database [{targetDbName}] and executing schema migrations...");
            result.ExecutionLogs.Add("Executing EF Core Database.MigrateAsync() on new database...");

            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            optionsBuilder.UseSqlServer(targetConnStr);

            using (var newContext = new AppDbContext(optionsBuilder.Options))
            {
                await newContext.Database.MigrateAsync();
                result.ExecutionLogs.Add("Database schema and tables created successfully.");

                // Step 2: Read Master Data from Current Context
                progress?.Report("3/7: Reading master data and accounts from active database...");
                using var curContext = await _contextFactory.CreateDbContextAsync();

                var companies = await curContext.Companies.AsNoTracking().ToListAsync();
                var units = await curContext.Units.AsNoTracking().ToListAsync();
                var prodGroups = await curContext.ProductGroups.AsNoTracking().ToListAsync();
                var products = await curContext.Products.AsNoTracking().ToListAsync();
                var accGroups = await curContext.AccountGroups.AsNoTracking().ToListAsync();
                var accounts = await curContext.Accounts.AsNoTracking().ToListAsync();
                var partyGroups = await curContext.PartyGroups.AsNoTracking().ToListAsync();
                var towns = await curContext.Towns.AsNoTracking().ToListAsync();
                var sectors = await curContext.Sectors.AsNoTracking().ToListAsync();
                var parties = await curContext.Parties.AsNoTracking().ToListAsync();
                var salesmen = await curContext.Salesmen.AsNoTracking().ToListAsync();
                var configs = await curContext.SystemConfigs.AsNoTracking().ToListAsync();
                var users = await curContext.Users.AsNoTracking().ToListAsync();
                var bankAccounts = await curContext.BankAccounts.AsNoTracking().ToListAsync();

                // Step 3: Carry Forward Balances
                progress?.Report("4/7: Calculating closing balances and assigning opening balances...");
                foreach (var acc in accounts)
                {
                    acc.OpeningBalance = acc.CurrentBalance;
                    // Current balance starts equal to opening balance in new year
                    acc.CurrentBalance = acc.CurrentBalance;
                    acc.LedgerEntries.Clear();
                    acc.SubAccounts.Clear();
                }

                foreach (var prod in products)
                {
                    prod.OpeningStock = prod.CurrentStock;
                    prod.CurrentStock = prod.CurrentStock;
                }

                // Step 4: Transfer Master Data into New DB using single open connection with IDENTITY_INSERT
                progress?.Report("5/7: Transferring master data to new financial year database...");
                await InsertWithIdentityAsync(newContext, "Companies", companies);
                await InsertWithIdentityAsync(newContext, "Units", units);
                await InsertWithIdentityAsync(newContext, "ProductGroups", prodGroups);
                await InsertWithIdentityAsync(newContext, "Products", products);
                await InsertWithIdentityAsync(newContext, "AccountGroups", accGroups);
                await InsertWithIdentityAsync(newContext, "Accounts", accounts);
                await InsertWithIdentityAsync(newContext, "PartyGroups", partyGroups);
                await InsertWithIdentityAsync(newContext, "Towns", towns);
                await InsertWithIdentityAsync(newContext, "Sectors", sectors);
                await InsertWithIdentityAsync(newContext, "Parties", parties);
                await InsertWithIdentityAsync(newContext, "Salesmen", salesmen);
                await InsertWithIdentityAsync(newContext, "SystemConfigs", configs);
                await InsertWithIdentityAsync(newContext, "Users", users);
                await InsertWithIdentityAsync(newContext, "BankAccounts", bankAccounts);

                result.AccountsCarriedForward = accounts.Count;
                result.ProductsCarriedForward = products.Count;
                result.ExecutionLogs.Add($"Migrated {accounts.Count} accounts and {products.Count} products.");

                // Step 5: Setup New Financial Year Record & Sequences
                progress?.Report("6/7: Initializing new financial year record and fresh document sequences...");
                var newYear = new FinancialYear
                {
                    YearCode = request.NewYearCode,
                    StartDate = request.NewStartDate,
                    EndDate = request.NewEndDate,
                    IsCurrent = true,
                    IsLocked = false,
                    DatabaseFileName = targetDbName
                };
                newContext.FinancialYears.Add(newYear);
                await newContext.SaveChangesAsync();

                // Generate fresh clean sequences restarting from 1
                foreach (DocumentType dt in Enum.GetValues<DocumentType>())
                {
                    newContext.DocumentSequences.Add(new DocumentSequence
                    {
                        DocumentType = dt,
                        FinancialYearId = newYear.Id,
                        Prefix = dt.ToString().Substring(0, 3).ToUpper(),
                        CurrentNumber = 0,
                        ResetOnNewYear = true
                    });
                }
                await newContext.SaveChangesAsync();
                result.ExecutionLogs.Add("Voucher & invoice sequences reset for the new financial year.");

                // Step 6: Lock Previous Year if requested
                if (request.LockPreviousYear)
                {
                    progress?.Report($"Locking previous financial year {request.CurrentYearCode}...");
                    var oldYear = await curContext.FinancialYears.FirstOrDefaultAsync(y => y.YearCode == request.CurrentYearCode);
                    if (oldYear != null)
                    {
                        oldYear.IsLocked = true;
                        oldYear.IsCurrent = false;
                        await curContext.SaveChangesAsync();
                        result.ExecutionLogs.Add($"Previous financial year {request.CurrentYearCode} marked as LOCKED.");
                    }
                }

                // Step 7: Update AppSettings to point to the new financial year
                progress?.Report("7/7: Updating application settings to new financial year...");
                _configService.SetCurrentFinancialYear(request.NewYearCode);
                result.ExecutionLogs.Add($"Configuration updated: CurrentFinancialYear = {request.NewYearCode}.");
            }

            result.Success = true;
            result.Message = $"Financial Year {request.NewYearCode} successfully created! Previous year {request.CurrentYearCode} is locked.";
            return result;
        }
        catch (Exception ex)
        {
            result.Success = false;
            result.Message = $"Year-End rollover failed: {ex.Message}";
            result.ExecutionLogs.Add($"ERROR: {ex.Message}");
            return result;
        }
    }

    private async Task InsertWithIdentityAsync<T>(AppDbContext context, string tableName, List<T> entities) where T : class
    {
        if (entities == null || entities.Count == 0) return;

        var conn = context.Database.GetDbConnection();
        if (conn.State != ConnectionState.Open)
        {
            await conn.OpenAsync();
        }

        await context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] ON;");
        context.Set<T>().AddRange(entities);
        await context.SaveChangesAsync();
        await context.Database.ExecuteSqlRawAsync($"SET IDENTITY_INSERT [{tableName}] OFF;");
    }

    public async Task<bool> LockFinancialYearAsync(string yearCode)
    {
        try
        {
            using var context = await _contextFactory.CreateDbContextAsync();
            var year = await context.FinancialYears.FirstOrDefaultAsync(y => y.YearCode == yearCode);
            if (year != null)
            {
                year.IsLocked = true;
                await context.SaveChangesAsync();
                return true;
            }
            return false;
        }
        catch
        {
            return false;
        }
    }

    public Task<bool> SwitchActiveFinancialYearAsync(string targetYearCode)
    {
        try
        {
            _configService.SetCurrentFinancialYear(targetYearCode);
            return Task.FromResult(true);
        }
        catch
        {
            return Task.FromResult(false);
        }
    }

    public async Task<IEnumerable<string>> GetAvailableYearsAsync()
    {
        var list = new List<string>();
        var prefix = _configService.GetDatabasePrefix();
        var masterConnStr = _configService.GetConnectionString("master");

        try
        {
            await using var conn = new SqlConnection(masterConnStr);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand($"SELECT name FROM sys.databases WHERE name LIKE '{prefix}%' ORDER BY name DESC", conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dbName = reader.GetString(0);
                var year = dbName.StartsWith($"{prefix}_") ? dbName.Substring($"{prefix}_".Length) : dbName;
                list.Add(year);
            }
        }
        catch
        {
            list.Add(_configService.GetCurrentFinancialYear());
        }

        return list;
    }

    public Task<string> GetDatabasePathForYearAsync(string year)
    {
        var prefix = _configService.GetDatabasePrefix();
        return Task.FromResult($"{prefix}_{year}");
    }

    private string CalculateNextYearCode(string currentYear)
    {
        // Try parsing "2025-26" pattern
        var match = Regex.Match(currentYear, @"^(\d{4})-(\d{2})$");
        if (match.Success)
        {
            int startYr = int.Parse(match.Groups[1].Value);
            int endYr = int.Parse(match.Groups[2].Value);
            return $"{startYr + 1}-{(endYr + 1):D2}";
        }

        // Try single 4-digit year e.g. 2025 -> 2026
        if (int.TryParse(currentYear, out int yr))
        {
            return (yr + 1).ToString();
        }

        return $"{currentYear}_Next";
    }
}
