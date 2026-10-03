using Microsoft.EntityFrameworkCore;
using MyMandiSystem.Core.Entities;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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

    public async Task<bool> StartNewYearAsync(string newYearCode)
    {
        if (_configService.IsReadOnly)
        {
            throw new InvalidOperationException("Cannot perform Year-End process in Read-Only (Archive) mode.");
        }

        try
        {
            var dataDir = _configService.GetDataDirectory();
            var currentYear = _configService.GetCurrentFinancialYear();
            var dbPrefix = _configService.GetDatabasePrefix();

            var currentDbPath = Path.Combine(dataDir, $"{dbPrefix}_{currentYear}.mdf");
            var newDbPath = Path.Combine(dataDir, $"{dbPrefix}_{newYearCode}.mdf");

            if (File.Exists(newDbPath)) throw new Exception("Target year database already exists.");

            // 1. Copy Database File
            File.Copy(currentDbPath, newDbPath);

            // 2. Clear Transactions and Carry Forward Balances in NEW DB
            // We use a temporary context pointing to the NEW database
            var optionsBuilder = new DbContextOptionsBuilder<AppDbContext>();
            string newConnString = $"Server=(localdb)\\mssqllocaldb;Database={dbPrefix}_{newYearCode};Trusted_Connection=True;TrustServerCertificate=True;";
            optionsBuilder.UseSqlServer(newConnString);

            using (var context = new AppDbContext(optionsBuilder.Options))
            {
                // Clear all transactional data
                context.VoucherDetails.ExecuteDelete();
                context.Vouchers.ExecuteDelete();
                context.InvoiceDetails.ExecuteDelete();
                context.Invoices.ExecuteDelete();
                context.StockTransactions.ExecuteDelete();
                context.GeneralLedger.ExecuteDelete();
                context.BrokerageInvoiceDetails.ExecuteDelete();
                context.BrokerageInvoices.ExecuteDelete();
                context.Contracts.ExecuteDelete();
                context.GatePasses.ExecuteDelete();
                context.PestroInvoiceDetails.ExecuteDelete();
                context.PestroInvoices.ExecuteDelete();
                context.Cheques.ExecuteDelete();
                context.BankDeposits.ExecuteDelete();

                // Carry forward Account Balances
                var accounts = await context.Accounts.ToListAsync();
                foreach (var acc in accounts)
                {
                    acc.OpeningBalance = acc.CurrentBalance;
                    // CurrentBalance remains as is for now, will be updated by new transactions
                }

                // Carry forward Product Stock
                var products = await context.Products.ToListAsync();
                foreach (var prod in products)
                {
                    prod.OpeningStock = prod.CurrentStock;
                }

                // Add New Financial Year Record
                var lastYear = await context.FinancialYears.OrderByDescending(y => y.EndDate).FirstOrDefaultAsync();
                var newYear = new FinancialYear
                {
                    YearCode = newYearCode,
                    StartDate = lastYear?.EndDate.AddDays(1) ?? DateTime.Now,
                    EndDate = lastYear?.EndDate.AddYears(1) ?? DateTime.Now.AddYears(1).AddDays(-1),
                    IsCurrent = true
                };
                
                if (lastYear != null) lastYear.IsCurrent = false;
                
                context.FinancialYears.Add(newYear);
                await context.SaveChangesAsync();
            }
            
            return true;
        }
        catch (Exception)
        {
            return false;
        }
    }

    public Task<IEnumerable<string>> GetAvailableYearsAsync()
    {
        var dataDir = _configService.GetDataDirectory();
        if (!Directory.Exists(dataDir)) return Task.FromResult(Enumerable.Empty<string>());

        var files = Directory.GetFiles(dataDir, $"{_configService.GetDatabasePrefix()}_*.mdf");
        var years = files.Select(f => Path.GetFileNameWithoutExtension(f).Split('_').Last());
        return Task.FromResult(years);
    }

    public Task<string> GetDatabasePathForYearAsync(string year)
    {
        var dataDir = _configService.GetDataDirectory();
        return Task.FromResult(Path.Combine(dataDir, $"{_configService.GetDatabasePrefix()}_{year}.mdf"));
    }
}
