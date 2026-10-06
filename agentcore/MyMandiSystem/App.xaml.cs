using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Infrastructure;
using MyMandiSystem.Infrastructure.Interceptors;
using MyMandiSystem.Infrastructure.Services;
using MyMandiSystem.ViewModels;
using System;
using System.IO;
using System.Linq;
using System.Windows;

namespace MyMandiSystem;

public partial class App : System.Windows.Application
{
    public static ServiceProvider? ServiceProvider { get; private set; }

    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        // Global Exception Handler
        this.DispatcherUnhandledException += (s, args) =>
        {
            var message = args.Exception.Message;
            if (args.Exception.InnerException != null)
                message += $"\nInner Error: {args.Exception.InnerException.Message}\nStackTrace: {args.Exception.StackTrace}";
                
            try { File.WriteAllText(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_error.log"), message); } catch { }
            System.Windows.MessageBox.Show($"An unexpected error occurred:\n{message}", "System Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        // 1. Show Splash Screen
        var splash = new Views.SplashScreenWindow();
        splash.Show();

        splash.UpdateProgress(20, "Loading configuration & environment...");

        // 2. Build Configuration
        var baseDir = AppDomain.CurrentDomain.BaseDirectory;
        var configPath = Path.Combine(baseDir, "appsettings.json");
        var basePath = File.Exists(configPath) ? baseDir : Directory.GetCurrentDirectory();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(basePath)
            .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
            .Build();

        // 3. Parse Arguments (Agensoft Pattern)
        string? requestedYear = null;
        bool isReadOnly = false;
        for (int i = 0; i < e.Args.Length; i++)
        {
            if (e.Args[i] == "--year" && i + 1 < e.Args.Length)
            {
                requestedYear = e.Args[i + 1];
                isReadOnly = true;
            }
        }

        splash.UpdateProgress(45, "Registering dependency services...");

        // 4. Register Services
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        ConfigureServices(serviceCollection, configuration, requestedYear, isReadOnly);
        ServiceProvider = serviceCollection.BuildServiceProvider();

        splash.UpdateProgress(75, "Verifying database connection & schemas...");
        await System.Threading.Tasks.Task.Run(() =>
        {
            InitializeDatabase();
        });

        splash.UpdateProgress(95, "Starting agentcore ERP...");

        // 5. Launch UI
        var mainWindow = new MainWindow();
        var mainVm = ServiceProvider.GetRequiredService<MainViewModel>();
        mainWindow.DataContext = mainVm;

        this.MainWindow = mainWindow;
        mainWindow.Show();
        splash.Close();
    }

    private void InitializeDatabase()
    {
        var contextFactory = ServiceProvider!.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var context = contextFactory.CreateDbContext();
        
        try
        {
            if (context.Database.CanConnect())
            {
                var pendingMigrations = context.Database.GetPendingMigrations();
                if (pendingMigrations.Any())
                {
                    context.Database.Migrate();
                    SeedMasterData(context);
                }
            }
            else
            {
                context.Database.Migrate();
                SeedMasterData(context);
            }
        }
        catch
        {
            context.Database.Migrate();
            SeedMasterData(context);
        }
    }

    private void SeedMasterData(AppDbContext context)
    {
        // 1. Seed System User (Id 1)
        if (!context.Users.Any())
        {
            context.Users.Add(new Core.Entities.User 
            { 
                Username = "admin", 
                FullName = "System Administrator", 
                PasswordHash = "admin",
                Role = Core.Entities.UserRole.Admin,
                IsActive = true,
                CreatedAt = DateTime.Now,
                CreatedBy = 1 
            });
            context.SaveChanges();
        }

        // 2. Seed Financial Year
        if (!context.FinancialYears.Any())
        {
            var year = new Core.Entities.FinancialYear
            {
                YearCode = "2025-26",
                StartDate = new DateTime(2025, 7, 1),
                EndDate = new DateTime(2026, 6, 30),
                IsCurrent = true
            };
            context.FinancialYears.Add(year);
            context.SaveChanges();

            context.CropSeasons.Add(new Core.Entities.CropSeason
            {
                Name = "Kharif 2025",
                StartDate = year.StartDate,
                EndDate = year.EndDate.AddMonths(-6),
                FinancialYearId = year.Id,
                IsActive = true
            });
            context.SaveChanges();
        }

        // 3. Seed Default Account Groups
        if (!context.AccountGroups.Any(g => g.GroupType == Core.Entities.AccountType.Asset))
        {
            context.AccountGroups.Add(new Core.Entities.AccountGroup 
            { 
                Name = "Accounts Receivable (Customers)", 
                GroupType = Core.Entities.AccountType.Asset 
            });
        }
        if (!context.AccountGroups.Any(g => g.GroupType == Core.Entities.AccountType.Liability))
        {
            context.AccountGroups.Add(new Core.Entities.AccountGroup 
            { 
                Name = "Accounts Payable (Zamindaraan)", 
                GroupType = Core.Entities.AccountType.Liability 
            });
        }
        if (!context.AccountGroups.Any(g => g.GroupType == Core.Entities.AccountType.Revenue))
        {
            context.AccountGroups.Add(new Core.Entities.AccountGroup 
            { 
                Name = "Mandi Commission & Income", 
                GroupType = Core.Entities.AccountType.Revenue 
            });
        }
        if (!context.AccountGroups.Any(g => g.GroupType == Core.Entities.AccountType.Expense))
        {
            context.AccountGroups.Add(new Core.Entities.AccountGroup 
            { 
                Name = "Operating Expenses", 
                GroupType = Core.Entities.AccountType.Expense 
            });
        }
        context.SaveChanges();

        // 4. Seed Cash Account
        if (!context.Accounts.Any(a => a.AccountNo == "1001"))
        {
            var assetGroup = context.AccountGroups.FirstOrDefault(g => g.GroupType == Core.Entities.AccountType.Asset);
            if (assetGroup != null)
            {
                context.Accounts.Add(new Core.Entities.Account
                {
                    AccountNo = "1001",
                    Name = "Cash in Hand",
                    UrduName = "روکڑ / کیش ان ہینڈ",
                    AccountGroupId = assetGroup.Id,
                    AccountType = Core.Entities.AccountType.Asset,
                    IsSystemAccount = true,
                    IsActive = true
                });
                context.SaveChanges();
            }
        }

        // 5. Seed Default Party Groups
        if (!context.PartyGroups.Any())
        {
            context.PartyGroups.AddRange(
                new Core.Entities.PartyGroup { Name = "General Customers" },
                new Core.Entities.PartyGroup { Name = "Zamindaraan (Farmers)" },
                new Core.Entities.PartyGroup { Name = "Beopari / Traders" }
            );
            context.SaveChanges();
        }

        // 6. Seed Default Units
        if (!context.Units.Any())
        {
            context.Units.AddRange(
                new Core.Entities.Unit { ShortName = "BAG", Name = "Bag (بوری)" },
                new Core.Entities.Unit { ShortName = "KG", Name = "Kilogram (کلو)" },
                new Core.Entities.Unit { ShortName = "MND", Name = "Mound / Mann (من)" }
            );
            context.SaveChanges();
        }

        // 7. Seed Default Company for Printing
        if (!context.Companies.Any())
        {
            context.Companies.Add(new Core.Entities.Company
            {
                Name = "City Computers Marot",
                ShortName = "CCM",
                IsActive = true
            });
            context.SaveChanges();
        }

        // Setup PrintContext
        var company = context.Companies.FirstOrDefault(c => c.IsActive);
        if (company != null)
        {
            Printing.PrintContext.Company.Name = company.Name;
            Printing.PrintContext.Company.NameUrdu = "سٹی کمپیوٹرز مروٹ";
            Printing.PrintContext.Company.Address = "Grain Market, Marot";
            Printing.PrintContext.Company.Phone = "0344-7436314";
        }
    }

    private void ConfigureServices(IServiceCollection services, IConfiguration configuration, string? yearArg, bool readOnlyArg)
    {
        // Setup Config Service
        var configService = new SystemConfigService(configuration);
        configService.IsReadOnly = readOnlyArg;
        if (!string.IsNullOrEmpty(yearArg)) configService.SetCurrentFinancialYear(yearArg);
        
        services.AddSingleton<ISystemConfigService>(configService);
        services.AddSingleton<ICurrentUserService, CurrentUserService>();

        // Build Dynamic Connection String
        string year = yearArg ?? configService.GetCurrentFinancialYear();
        string dbName = $"MyMandi_{year}";
        string connectionString = configService.GetConnectionString(dbName);

        services.AddSingleton<AuditInterceptor>();
        
        services.AddDbContextFactory<AppDbContext>((sp, options) =>
        {
            var interceptor = sp.GetRequiredService<AuditInterceptor>();
            options.UseSqlServer(connectionString)
                   .AddInterceptors(interceptor);
        });

        // 12 Service Layer Registrations
        services.AddSingleton<IAccountService, AccountService>();
        services.AddSingleton<IPartyService, PartyService>();
        services.AddSingleton<IProductService, ProductService>();
        services.AddSingleton<IVoucherService, VoucherService>();
        services.AddSingleton<IAuthService, AuthService>();
        services.AddSingleton<IStockService, StockService>();
        services.AddSingleton<IInvoiceService, InvoiceService>();
        services.AddSingleton<IBrokerageService, BrokerageService>();
        services.AddScoped<IPestroService, PestroService>();
        services.AddScoped<IReportService, ReportService>();
        services.AddScoped<ISystemService, SystemService>();
        services.AddSingleton<ICatalogService, CatalogService>();
        services.AddSingleton<IYearEndService, YearEndService>();

        // Print Engine Services
        services.AddSingleton<MyMandiSystem.Core.Printing.IPrintService, MyMandiSystem.Printing.PrintService>();
        services.AddSingleton<MyMandiSystem.Core.Printing.IPrintModelFactory, MyMandiSystem.Infrastructure.Printing.PrintModelFactory>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<CashReceivingVoucherViewModel>();
        services.AddTransient<JournalVoucherViewModel>();
        services.AddTransient<CashPaymentVoucherViewModel>();
        services.AddTransient<ChartOfAccountsViewModel>();
        services.AddTransient<MasterDataViewModel>();
        services.AddTransient<PartyViewModel>();
        services.AddTransient<ProductViewModel>();
        services.AddTransient<OpeningBalancesViewModel>();
        services.AddTransient<GeneralInvoiceViewModel>();
        services.AddTransient<BrokerageViewModel>();
        services.AddTransient<PestroViewModel>();
        services.AddTransient<ReportingViewModel>();
        services.AddTransient<AccountLedgerExtendedViewModel>();
        services.AddTransient<AccountPayablesViewModel>();
        services.AddTransient<BankStatementViewModel>();
        services.AddTransient<CashBookViewModel>();
        services.AddTransient<DailyVouchersDetailViewModel>();
        services.AddTransient<TrialBalanceViewModel>();
        services.AddTransient<ProfitLossStatementViewModel>();
        services.AddTransient<BalanceSheetViewModel>();
        services.AddTransient<BrokeragePurchaseInvoiceViewModel>();
        services.AddTransient<BrokerageSaleInvoiceViewModel>();
        services.AddTransient<BrokerageMultiInvoiceViewModel>();
    }
}
