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

        splash.UpdateProgress(15, "Loading configuration & environment...");
        await System.Threading.Tasks.Task.Delay(250);

        // 2. Build Configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
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

        splash.UpdateProgress(35, "Registering dependency services...");
        await System.Threading.Tasks.Task.Delay(200);

        // 4. Register Services
        var serviceCollection = new ServiceCollection();
        serviceCollection.AddSingleton<IConfiguration>(configuration);
        ConfigureServices(serviceCollection, configuration, requestedYear, isReadOnly);
        ServiceProvider = serviceCollection.BuildServiceProvider();

        splash.UpdateProgress(60, "Verifying database migrations & schemas...");
        await System.Threading.Tasks.Task.Run(() =>
        {
            InitializeDatabase();
        });

        splash.UpdateProgress(85, "Preparing agentcore workspace & dashboard...");
        await System.Threading.Tasks.Task.Delay(250);

        // 5. Launch UI
        var mainWindow = new MainWindow();
        var mainVm = ServiceProvider.GetRequiredService<MainViewModel>();
        mainWindow.DataContext = mainVm;

        splash.UpdateProgress(100, "Starting agentcore ERP...");
        await System.Threading.Tasks.Task.Delay(200);

        this.MainWindow = mainWindow;
        mainWindow.Show();
        splash.Close();
    }

    private void InitializeDatabase()
    {
        var contextFactory = ServiceProvider!.GetRequiredService<IDbContextFactory<AppDbContext>>();
        using var context = contextFactory.CreateDbContext();
        
        // Ensure database is up to date
        context.Database.Migrate();

        // Seed Core Data (Idempotent)
        SeedMasterData(context);
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

        // 3. Seed Default Account Group
        if (!context.AccountGroups.Any())
        {
            context.AccountGroups.Add(new Core.Entities.AccountGroup 
            { 
                Name = "General Assets", 
                GroupType = Core.Entities.AccountType.Asset 
            });
            context.SaveChanges();
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
        string connTemplate = configuration.GetConnectionString("DefaultConnection") 
            ?? "Server=(localdb)\\mssqllocaldb;Database={0};Trusted_Connection=True;TrustServerCertificate=True;";
        string connectionString = string.Format(connTemplate, dbName);

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
