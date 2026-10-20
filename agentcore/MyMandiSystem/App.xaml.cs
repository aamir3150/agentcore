using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
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

    public App()
    {
        // Enforce Software Rendering globally to prevent GPU DirectX/D3D swapchain blank white screen failures on client displays
        System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
    }

    protected override void OnStartup(StartupEventArgs e)
    {
        System.Windows.Media.RenderOptions.ProcessRenderMode = System.Windows.Interop.RenderMode.SoftwareOnly;
        this.ShutdownMode = ShutdownMode.OnMainWindowClose;

        base.OnStartup(e);

        Log("1. OnStartup initiated.");

        // Global Exception Handlers
        this.DispatcherUnhandledException += (s, args) =>
        {
            Log($"[DispatcherUnhandledException] {args.Exception}");
            FriendlyErrorHandler.ShowError(args.Exception, "WPF Dispatcher");
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            if (args.ExceptionObject is Exception ex)
            {
                Log($"[AppDomain Unhandled] {ex}");
                FriendlyErrorHandler.ShowError(ex, "AppDomain Unhandled");
            }
        };

        this.Exit += (s, args) =>
        {
            Log($"[App.Exit] Application exiting with code: {args.ApplicationExitCode}");
        };

        try
        {
            // 1. Show Splash Screen
            Log("2. Creating SplashScreenWindow...");
            var splash = new Views.SplashScreenWindow();
            splash.Show();
            Log("3. SplashScreenWindow shown.");

            splash.UpdateProgress(20, "Loading configuration & environment...");

            // 2. Build Configuration
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(baseDir, "appsettings.json");
            var basePath = File.Exists(configPath) ? baseDir : Directory.GetCurrentDirectory();

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();
            Log("4. Configuration built.");

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
            Log("5. ServiceProvider built.");

            splash.UpdateProgress(75, "Verifying database connection & schemas...");
            Log("6. InitializeDatabase starting...");
            InitializeDatabase();
            Log("7. InitializeDatabase finished.");

            splash.UpdateProgress(95, "Starting agentcore ERP...");

            // 5. Launch UI
            Log("8. Creating MainWindow...");
            var mainWindow = new MainWindow();
            Log("9. MainWindow instantiated.");

            mainWindow.Closed += (s, args) => Log("[MainWindow] Closed event fired.");
            mainWindow.Closing += (s, args) => Log($"[MainWindow] Closing event fired. Cancel={args.Cancel}");

            Log("10. Resolving MainViewModel...");
            MainViewModel mainVm;
            try
            {
                mainVm = ServiceProvider.GetRequiredService<MainViewModel>();
                Log("11. MainViewModel resolved.");
            }
            catch (Exception ex)
            {
                Log($"Resolve MainViewModel EXCEPTION: {ex}");
                throw;
            }

            mainWindow.DataContext = mainVm;
            this.MainWindow = mainWindow;

            Log("12. Showing MainWindow...");
            mainWindow.Show();
            mainWindow.Activate();
            mainWindow.Focus();
            Log("13. MainWindow.Show completed.");

            try
            {
                Log("13.5. Calling splash.Close()...");
                splash.Close();
                Log("14. SplashScreenWindow closed.");
            }
            catch (Exception exSplash)
            {
                Log($"splash.Close EXCEPTION: {exSplash}");
            }

            Log("15. Startup sequence completed successfully!");
        }
        catch (Exception ex)
        {
            Log($"[FATAL in OnStartup] {ex}");
            MessageBox.Show($"Startup Error: {ex.Message}\n\n{ex.StackTrace}", "agentcore Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
    }

    private static void Log(string msg)
    {
        try {
            File.AppendAllText(@"D:\Projects\agentcore\agentcore\startup.log", $"[{DateTime.Now:HH:mm:ss.fff}] {msg}\r\n");
        } catch {}
    }

    private void InitializeDatabase()
    {
        Log("InitDb: Getting IDbContextFactory...");
        var contextFactory = ServiceProvider!.GetRequiredService<IDbContextFactory<AppDbContext>>();
        Log("InitDb: Creating DbContext...");
        using var context = contextFactory.CreateDbContext();
        Log("InitDb: DbContext created. Checking CanConnect...");
        
        try
        {
            if (context.Database.CanConnect())
            {
                Log("InitDb: CanConnect is true. Calling EnsureLegacyCompatibility...");
                EnsureLegacyCompatibility(context);
                Log("InitDb: EnsureLegacyCompatibility done. Checking PendingMigrations...");

                var pendingMigrations = context.Database.GetPendingMigrations().ToList();
                Log($"InitDb: PendingMigrations count: {pendingMigrations.Count}");
                if (pendingMigrations.Any())
                {
                    Log("InitDb: Running Migrate()...");
                    context.Database.Migrate();
                    Log("InitDb: Migrate done.");
                }
                Log("InitDb: Calling SeedMasterData...");
                SeedMasterData(context);
                Log("InitDb: SeedMasterData completed.");
            }
            else
            {
                Log("InitDb: CanConnect is false. Running Migrate()...");
                context.Database.Migrate();
                EnsureLegacyCompatibility(context);
                SeedMasterData(context);
                Log("InitDb: Fallback done.");
            }
        }
        catch (Exception ex)
        {
            Log($"InitDb EXCEPTION: {ex}");
            System.Diagnostics.Debug.WriteLine($"Database initialization warning: {ex.Message}");
            try
            {
                EnsureLegacyCompatibility(context);
                SeedMasterData(context);
            }
            catch (Exception ex2)
            {
                Log($"InitDb secondary EXCEPTION: {ex2}");
            }
        }
    }

    private void EnsureLegacyCompatibility(AppDbContext context)
    {
        try
        {
            var sql = @"
IF OBJECT_ID('dbo.Users') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'FullName' AND object_id = OBJECT_ID('dbo.Users'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.Users')) ALTER TABLE dbo.Users ADD [Id] int IDENTITY(1,1);
    ALTER TABLE dbo.Users ADD [FullName] nvarchar(100) NULL;
    ALTER TABLE dbo.Users ADD [PasswordHash] nvarchar(100) NULL;
    ALTER TABLE dbo.Users ADD [Role] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Users ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Users ADD [LastLoginAt] datetime2 NULL;
    ALTER TABLE dbo.Users ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Users ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Users ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Users ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Users ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END;

IF OBJECT_ID('dbo.Companies') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Companies'))
    BEGIN
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.Companies')) ALTER TABLE dbo.Companies ADD [Id] int IDENTITY(1,1);
        ALTER TABLE dbo.Companies ADD [Code] nvarchar(20) NULL;
        ALTER TABLE dbo.Companies ADD [Name] nvarchar(150) NULL;
        ALTER TABLE dbo.Companies ADD [IsActive] bit NOT NULL DEFAULT 1;
        ALTER TABLE dbo.Companies ADD [RowVersion] rowversion NULL;
        ALTER TABLE dbo.Companies ADD [CreatedBy] int NOT NULL DEFAULT 1;
        ALTER TABLE dbo.Companies ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
        ALTER TABLE dbo.Companies ADD [UpdatedBy] int NULL;
        ALTER TABLE dbo.Companies ADD [UpdatedAt] datetime2 NULL;
        ALTER TABLE dbo.Companies ADD [IsDeleted] bit NOT NULL DEFAULT 0;
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'ShortName' AND object_id = OBJECT_ID('dbo.Companies')) ALTER TABLE dbo.Companies ADD [ShortName] nvarchar(50) NULL;
    END;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'ShortName' AND object_id = OBJECT_ID('dbo.Companies')) ALTER TABLE dbo.Companies ADD [ShortName] nvarchar(50) NULL;
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Companies_CompanyID')
        ALTER TABLE dbo.Companies ADD CONSTRAINT DF_Companies_CompanyID DEFAULT ('') FOR [CompanyID];
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Companies_CompanyName')
        ALTER TABLE dbo.Companies ADD CONSTRAINT DF_Companies_CompanyName DEFAULT ('') FOR [CompanyName];
END;

IF OBJECT_ID('dbo.Units') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Units'))
    BEGIN
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.Units')) ALTER TABLE dbo.Units ADD [Id] int IDENTITY(1,1);
        ALTER TABLE dbo.Units ADD [Name] nvarchar(50) NULL;
        ALTER TABLE dbo.Units ADD [ShortName] nvarchar(20) NULL;
        ALTER TABLE dbo.Units ADD [IsActive] bit NOT NULL DEFAULT 1;
        ALTER TABLE dbo.Units ADD [RowVersion] rowversion NULL;
        ALTER TABLE dbo.Units ADD [CreatedBy] int NOT NULL DEFAULT 1;
        ALTER TABLE dbo.Units ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
        ALTER TABLE dbo.Units ADD [UpdatedBy] int NULL;
        ALTER TABLE dbo.Units ADD [UpdatedAt] datetime2 NULL;
        ALTER TABLE dbo.Units ADD [IsDeleted] bit NOT NULL DEFAULT 0;
    END;
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Units_UnitID')
        ALTER TABLE dbo.Units ADD CONSTRAINT DF_Units_UnitID DEFAULT (0) FOR [UnitID];
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Units_UnitName')
        ALTER TABLE dbo.Units ADD CONSTRAINT DF_Units_UnitName DEFAULT ('') FOR [UnitName];
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_Units_Multiplier')
        ALTER TABLE dbo.Units ADD CONSTRAINT DF_Units_Multiplier DEFAULT (1) FOR [Multiplier];
END;

IF OBJECT_ID('dbo.Towns') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Towns'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.Towns')) ALTER TABLE dbo.Towns ADD [Id] int IDENTITY(1,1);
    ALTER TABLE dbo.Towns ADD [Name] nvarchar(100) NULL;
    ALTER TABLE dbo.Towns ADD [UrduName] nvarchar(100) NULL;
    ALTER TABLE dbo.Towns ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Towns ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.Towns ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Towns ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Towns ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Towns ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Towns ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END;

IF OBJECT_ID('dbo.Sectors') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Sectors'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.Sectors')) ALTER TABLE dbo.Sectors ADD [Id] int IDENTITY(1,1);
    ALTER TABLE dbo.Sectors ADD [Name] nvarchar(100) NULL;
    ALTER TABLE dbo.Sectors ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Sectors ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.Sectors ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Sectors ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Sectors ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Sectors ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Sectors ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END;

IF OBJECT_ID('dbo.PartyGroups') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.PartyGroups'))
    BEGIN
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.PartyGroups')) ALTER TABLE dbo.PartyGroups ADD [Id] int IDENTITY(1,1);
        ALTER TABLE dbo.PartyGroups ADD [Name] nvarchar(100) NULL;
        ALTER TABLE dbo.PartyGroups ADD [IsActive] bit NOT NULL DEFAULT 1;
        ALTER TABLE dbo.PartyGroups ADD [RowVersion] rowversion NULL;
        ALTER TABLE dbo.PartyGroups ADD [CreatedBy] int NOT NULL DEFAULT 1;
        ALTER TABLE dbo.PartyGroups ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
        ALTER TABLE dbo.PartyGroups ADD [UpdatedBy] int NULL;
        ALTER TABLE dbo.PartyGroups ADD [UpdatedAt] datetime2 NULL;
        ALTER TABLE dbo.PartyGroups ADD [IsDeleted] bit NOT NULL DEFAULT 0;
    END;
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_PartyGroups_PartyGroupID')
        ALTER TABLE dbo.PartyGroups ADD CONSTRAINT DF_PartyGroups_PartyGroupID DEFAULT (0) FOR [PartyGroupID];
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_PartyGroups_GroupName')
        ALTER TABLE dbo.PartyGroups ADD CONSTRAINT DF_PartyGroups_GroupName DEFAULT ('') FOR [GroupName];
END;

IF OBJECT_ID('dbo.ProductGroups') IS NOT NULL
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.ProductGroups'))
    BEGIN
        IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.ProductGroups')) ALTER TABLE dbo.ProductGroups ADD [Id] int IDENTITY(1,1);
        ALTER TABLE dbo.ProductGroups ADD [Name] nvarchar(100) NULL;
        ALTER TABLE dbo.ProductGroups ADD [IsActive] bit NOT NULL DEFAULT 1;
        ALTER TABLE dbo.ProductGroups ADD [RowVersion] rowversion NULL;
        ALTER TABLE dbo.ProductGroups ADD [CreatedBy] int NOT NULL DEFAULT 1;
        ALTER TABLE dbo.ProductGroups ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
        ALTER TABLE dbo.ProductGroups ADD [UpdatedBy] int NULL;
        ALTER TABLE dbo.ProductGroups ADD [UpdatedAt] datetime2 NULL;
        ALTER TABLE dbo.ProductGroups ADD [IsDeleted] bit NOT NULL DEFAULT 0;
    END;
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ProductGroups_CompanyID')
        ALTER TABLE dbo.ProductGroups ADD CONSTRAINT DF_ProductGroups_CompanyID DEFAULT ('') FOR [CompanyID];
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ProductGroups_GroupID')
        ALTER TABLE dbo.ProductGroups ADD CONSTRAINT DF_ProductGroups_GroupID DEFAULT (0) FOR [GroupID];
    IF NOT EXISTS (SELECT * FROM sys.default_constraints WHERE name = 'DF_ProductGroups_GroupName')
        ALTER TABLE dbo.ProductGroups ADD CONSTRAINT DF_ProductGroups_GroupName DEFAULT ('') FOR [GroupName];
END;

IF OBJECT_ID('dbo.Salesmen') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Salesmen'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.Salesmen')) ALTER TABLE dbo.Salesmen ADD [Id] int IDENTITY(1,1);
    ALTER TABLE dbo.Salesmen ADD [Name] nvarchar(100) NULL;
    ALTER TABLE dbo.Salesmen ADD [Phone] nvarchar(50) NULL;
    ALTER TABLE dbo.Salesmen ADD [AccountId] int NULL;
    ALTER TABLE dbo.Salesmen ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Salesmen ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.Salesmen ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Salesmen ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Salesmen ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Salesmen ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Salesmen ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END;

IF OBJECT_ID('dbo.Promises') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Remarks' AND object_id = OBJECT_ID('dbo.Promises'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.Promises')) ALTER TABLE dbo.Promises ADD [Id] int IDENTITY(1,1);
    ALTER TABLE dbo.Promises ADD [PartyId] int NULL;
    ALTER TABLE dbo.Promises ADD [Remarks] nvarchar(255) NULL;
    ALTER TABLE dbo.Promises ADD [IsFulfilled] bit NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Promises ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.Promises ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Promises ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Promises ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Promises ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Promises ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END;

IF OBJECT_ID('dbo.Parties') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Parties'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.Parties')) ALTER TABLE dbo.Parties ADD [Id] int IDENTITY(1,1);
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'PartyNo' AND object_id = OBJECT_ID('dbo.Parties')) ALTER TABLE dbo.Parties ADD [PartyNo] nvarchar(50) NULL;
    ALTER TABLE dbo.Parties ADD [Name] nvarchar(150) NULL;
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'PartyType' AND object_id = OBJECT_ID('dbo.Parties')) ALTER TABLE dbo.Parties ADD [PartyType] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Parties ADD [AccountId] int NULL;
    ALTER TABLE dbo.Parties ADD [TownId] int NULL;
    ALTER TABLE dbo.Parties ADD [Phone] nvarchar(50) NULL;
    ALTER TABLE dbo.Parties ADD [CNIC] nvarchar(30) NULL;
    ALTER TABLE dbo.Parties ADD [CreditLimit] decimal(18,2) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Parties ADD [WHT_Percentage] decimal(18,2) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Parties ADD [DefaultMarkupRate] decimal(18,2) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Parties ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Parties ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.Parties ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Parties ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Parties ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Parties ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Parties ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END;

IF OBJECT_ID('dbo.Products') IS NOT NULL AND NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Name' AND object_id = OBJECT_ID('dbo.Products'))
BEGIN
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'Id' AND object_id = OBJECT_ID('dbo.Products')) ALTER TABLE dbo.Products ADD [Id] int IDENTITY(1,1);
    IF NOT EXISTS (SELECT * FROM sys.columns WHERE name = 'ProductCode' AND object_id = OBJECT_ID('dbo.Products')) ALTER TABLE dbo.Products ADD [ProductCode] nvarchar(50) NULL;
    ALTER TABLE dbo.Products ADD [Name] nvarchar(150) NULL;
    ALTER TABLE dbo.Products ADD [ProductGroupId] int NULL;
    ALTER TABLE dbo.Products ADD [CostPrice] decimal(18,2) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Products ADD [OpeningStock] decimal(18,3) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Products ADD [CurrentStock] decimal(18,3) NOT NULL DEFAULT 0;
    ALTER TABLE dbo.Products ADD [IsActive] bit NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Products ADD [RowVersion] rowversion NULL;
    ALTER TABLE dbo.Products ADD [CreatedBy] int NOT NULL DEFAULT 1;
    ALTER TABLE dbo.Products ADD [CreatedAt] datetime2 NOT NULL DEFAULT GETDATE();
    ALTER TABLE dbo.Products ADD [UpdatedBy] int NULL;
    ALTER TABLE dbo.Products ADD [UpdatedAt] datetime2 NULL;
    ALTER TABLE dbo.Products ADD [IsDeleted] bit NOT NULL DEFAULT 0;
END;
";
            context.Database.ExecuteSqlRaw(sql);

            try
            {
                context.Database.ExecuteSqlRaw(@"
IF OBJECT_ID('dbo.TR_Units_AutoLegacyKey', 'TR') IS NOT NULL DROP TRIGGER dbo.TR_Units_AutoLegacyKey;
IF OBJECT_ID('dbo.TR_Companies_AutoLegacyKey', 'TR') IS NOT NULL DROP TRIGGER dbo.TR_Companies_AutoLegacyKey;
IF OBJECT_ID('dbo.TR_ProductGroups_AutoLegacyKey', 'TR') IS NOT NULL DROP TRIGGER dbo.TR_ProductGroups_AutoLegacyKey;

IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Units' AND parent_object_id = OBJECT_ID('dbo.Units')) ALTER TABLE dbo.Units DROP CONSTRAINT PK_Units;
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_Companies' AND parent_object_id = OBJECT_ID('dbo.Companies')) ALTER TABLE dbo.Companies DROP CONSTRAINT PK_Companies;
IF EXISTS (SELECT * FROM sys.key_constraints WHERE name = 'PK_ProductGroups' AND parent_object_id = OBJECT_ID('dbo.ProductGroups')) ALTER TABLE dbo.ProductGroups DROP CONSTRAINT PK_ProductGroups;

ALTER TABLE dbo.Units ALTER COLUMN UnitID tinyint NULL;
ALTER TABLE dbo.Units ALTER COLUMN UnitName varchar(50) NULL;
ALTER TABLE dbo.Units ALTER COLUMN Multiplier decimal(18,2) NULL;

ALTER TABLE dbo.Companies ALTER COLUMN CompanyID varchar(50) NULL;
ALTER TABLE dbo.Companies ALTER COLUMN CompanyName varchar(150) NULL;

ALTER TABLE dbo.ProductGroups ALTER COLUMN CompanyID varchar(50) NULL;
ALTER TABLE dbo.ProductGroups ALTER COLUMN GroupID tinyint NULL;
ALTER TABLE dbo.ProductGroups ALTER COLUMN GroupName varchar(100) NULL;

ALTER TABLE dbo.PartyGroups ALTER COLUMN PartyGroupID int NULL;
ALTER TABLE dbo.PartyGroups ALTER COLUMN GroupName varchar(100) NULL;

IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('dbo.Units') AND type = 'PK') ALTER TABLE dbo.Units ADD CONSTRAINT PK_Units_Id PRIMARY KEY CLUSTERED (Id);
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('dbo.Companies') AND type = 'PK') ALTER TABLE dbo.Companies ADD CONSTRAINT PK_Companies_Id PRIMARY KEY CLUSTERED (Id);
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('dbo.ProductGroups') AND type = 'PK') ALTER TABLE dbo.ProductGroups ADD CONSTRAINT PK_ProductGroups_Id PRIMARY KEY CLUSTERED (Id);
IF NOT EXISTS (SELECT * FROM sys.key_constraints WHERE parent_object_id = OBJECT_ID('dbo.PartyGroups') AND type = 'PK') ALTER TABLE dbo.PartyGroups ADD CONSTRAINT PK_PartyGroups_Id PRIMARY KEY CLUSTERED (Id);
");
            }
            catch (Exception exFix)
            {
                System.Diagnostics.Debug.WriteLine($"Schema modernizer notice: {exFix.Message}");
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"EnsureLegacyCompatibility notice: {ex.Message}");
        }
    }

    private void SeedMasterData(AppDbContext context)
    {
        Log("Seed: starting...");

        // 1. Seed System User (Id 1)
        try
        {
            Log("Seed: 1. Checking Users...");
            if (!context.Users.Any())
            {
                Log("Seed: 1. Adding admin user...");
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
                Log("Seed: 1. Admin user saved.");
            }
        }
        catch (Exception ex) { Log($"Seed: 1. User error: {ex.Message}"); }

        // 2. Seed Financial Year
        try
        {
            Log("Seed: 2. Checking FinancialYears...");
            if (!context.FinancialYears.Any())
            {
                Log("Seed: 2. Adding FinancialYear...");
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
                Log("Seed: 2. FinancialYear saved.");
            }
        }
        catch (Exception ex) { Log($"Seed: 2. FinancialYear error: {ex.Message}"); }

        // 3. Seed Default Account Groups
        try
        {
            Log("Seed: 3. Checking AccountGroups...");
            bool changed = false;
            if (!context.AccountGroups.Any(g => g.GroupType == Core.Entities.AccountType.Asset))
            {
                context.AccountGroups.Add(new Core.Entities.AccountGroup 
                { 
                    Name = "Accounts Receivable (Customers)", 
                    GroupType = Core.Entities.AccountType.Asset 
                });
                changed = true;
            }
            if (!context.AccountGroups.Any(g => g.GroupType == Core.Entities.AccountType.Liability))
            {
                context.AccountGroups.Add(new Core.Entities.AccountGroup 
                { 
                    Name = "Accounts Payable (Zamindaraan)", 
                    GroupType = Core.Entities.AccountType.Liability 
                });
                changed = true;
            }
            if (!context.AccountGroups.Any(g => g.GroupType == Core.Entities.AccountType.Revenue))
            {
                context.AccountGroups.Add(new Core.Entities.AccountGroup 
                { 
                    Name = "Mandi Commission & Income", 
                    GroupType = Core.Entities.AccountType.Revenue 
                });
                changed = true;
            }
            if (!context.AccountGroups.Any(g => g.GroupType == Core.Entities.AccountType.Expense))
            {
                context.AccountGroups.Add(new Core.Entities.AccountGroup 
                { 
                    Name = "Operating Expenses", 
                    GroupType = Core.Entities.AccountType.Expense 
                });
                changed = true;
            }
            if (changed)
            {
                context.SaveChanges();
                Log("Seed: 3. AccountGroups saved.");
            }
        }
        catch (Exception ex) { Log($"Seed: 3. AccountGroups error: {ex.Message}"); }

        // 4. Seed Cash Account
        try
        {
            Log("Seed: 4. Checking Cash Account...");
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
                    Log("Seed: 4. Cash Account saved.");
                }
            }
        }
        catch (Exception ex) { Log($"Seed: 4. Cash Account error: {ex.Message}"); }

        // 5. Seed Default Party Groups
        try
        {
            Log("Seed: 5. Checking PartyGroups...");
            if (!context.PartyGroups.Any())
            {
                context.PartyGroups.AddRange(
                    new Core.Entities.PartyGroup { Name = "General Customers" },
                    new Core.Entities.PartyGroup { Name = "Zamindaraan (Farmers)" },
                    new Core.Entities.PartyGroup { Name = "Beopari / Traders" }
                );
                context.SaveChanges();
                Log("Seed: 5. PartyGroups saved.");
            }
        }
        catch (Exception ex) { Log($"Seed: 5. PartyGroups error: {ex.Message}"); }

        // 6. Seed Default Units
        try
        {
            Log("Seed: 6. Checking Units...");
            if (!context.Units.Any())
            {
                context.Units.AddRange(
                    new Core.Entities.Unit { ShortName = "BAG", Name = "Bag (بوری)" },
                    new Core.Entities.Unit { ShortName = "KG", Name = "Kilogram (کلو)" },
                    new Core.Entities.Unit { ShortName = "MND", Name = "Mound / Mann (من)" }
                );
                context.SaveChanges();
                Log("Seed: 6. Units saved.");
            }
        }
        catch (Exception ex) { Log($"Seed: 6. Units error: {ex.InnerException?.Message ?? ex.Message}"); }

        // 7. Seed Default Companies
        try
        {
            Log("Seed: 7. Checking Companies...");
            if (!context.Companies.Any())
            {
                context.Companies.AddRange(
                    new Core.Entities.Company { Name = "General / Local (لوکل / عام)", ShortName = "GEN", IsActive = true },
                    new Core.Entities.Company { Name = "City Computers Marot", ShortName = "CCM", IsActive = true },
                    new Core.Entities.Company { Name = "Engro Fertilizers", ShortName = "ENGRO", IsActive = true },
                    new Core.Entities.Company { Name = "Fauji Fertilizer Company (FFC)", ShortName = "FFC", IsActive = true },
                    new Core.Entities.Company { Name = "Syngenta Pakistan", ShortName = "SYNGENTA", IsActive = true },
                    new Core.Entities.Company { Name = "Bayer CropScience", ShortName = "BAYER", IsActive = true }
                );
                context.SaveChanges();
                Log("Seed: 7. Companies saved.");
            }
        }
        catch (Exception ex) { Log($"Seed: 7. Companies error: {ex.InnerException?.Message ?? ex.Message}"); }

        // 8. Seed Default Product Groups
        try
        {
            Log("Seed: 8. Checking ProductGroups...");
            if (!context.ProductGroups.Any())
            {
                context.ProductGroups.AddRange(
                    new Core.Entities.ProductGroup { Name = "Grains & Crops (اجناس و فصلیں)" },
                    new Core.Entities.ProductGroup { Name = "Fertilizers (کھادیں)" },
                    new Core.Entities.ProductGroup { Name = "Pesticides & Chemicals (زرعی ادویات)" },
                    new Core.Entities.ProductGroup { Name = "Seeds (بیج)" },
                    new Core.Entities.ProductGroup { Name = "General Products (جنرل آئٹمز)" }
                );
                context.SaveChanges();
                Log("Seed: 8. ProductGroups saved.");
            }
        }
        catch (Exception ex) { Log($"Seed: 8. ProductGroups error: {ex.InnerException?.Message ?? ex.Message}"); }

        // Setup PrintContext and seed/load business configs
        try
        {
            Log("Seed: 9. Checking SystemConfigs...");
            var configs = context.SystemConfigs.Where(c => c.ConfigKey.StartsWith("Business_")).ToList();
            var cfgName = configs.FirstOrDefault(c => c.ConfigKey == "Business_Name")?.ConfigValue;
            var cfgUrdu = configs.FirstOrDefault(c => c.ConfigKey == "Business_NameUrdu")?.ConfigValue;
            var cfgProp = configs.FirstOrDefault(c => c.ConfigKey == "Business_Proprietor")?.ConfigValue;
            var cfgAddr = configs.FirstOrDefault(c => c.ConfigKey == "Business_Address")?.ConfigValue;
            var cfgPhone = configs.FirstOrDefault(c => c.ConfigKey == "Business_Phone")?.ConfigValue;
            var cfgReg = configs.FirstOrDefault(c => c.ConfigKey == "Business_RegistrationNo")?.ConfigValue;

            if (string.IsNullOrWhiteSpace(cfgName))
            {
                var defaultCompany = context.Companies.FirstOrDefault(c => c.IsActive)?.Name ?? "City Computers Marot";
                context.SystemConfigs.AddRange(
                    new Core.Entities.SystemConfig { ConfigKey = "Business_Name", ConfigValue = defaultCompany, Description = "Business/Firm Name English" },
                    new Core.Entities.SystemConfig { ConfigKey = "Business_NameUrdu", ConfigValue = "سٹی کمپیوٹرز مروٹ", Description = "Business/Firm Name Urdu" },
                    new Core.Entities.SystemConfig { ConfigKey = "Business_Proprietor", ConfigValue = "", Description = "Proprietor / Malik Name" },
                    new Core.Entities.SystemConfig { ConfigKey = "Business_Address", ConfigValue = "Grain Market, Marot (غلہ منڈی مروٹ)", Description = "Business Address" },
                    new Core.Entities.SystemConfig { ConfigKey = "Business_Phone", ConfigValue = "0344-7436314", Description = "Business Phone / Mobile" },
                    new Core.Entities.SystemConfig { ConfigKey = "Business_RegistrationNo", ConfigValue = "", Description = "License / Reg No" }
                );
                context.SaveChanges();

                Printing.PrintContext.Company.Name = defaultCompany;
                Printing.PrintContext.Company.NameUrdu = "سٹی کمپیوٹرز مروٹ";
                Printing.PrintContext.Company.Address = "Grain Market, Marot (غلہ منڈی مروٹ)";
                Printing.PrintContext.Company.Phone = "0344-7436314";
            }
            else
            {
                Printing.PrintContext.Company.Name = cfgName;
                if (!string.IsNullOrWhiteSpace(cfgUrdu)) Printing.PrintContext.Company.NameUrdu = cfgUrdu;
                if (!string.IsNullOrWhiteSpace(cfgProp)) Printing.PrintContext.Company.Proprietor = cfgProp;
                if (!string.IsNullOrWhiteSpace(cfgAddr)) Printing.PrintContext.Company.Address = cfgAddr;
                if (!string.IsNullOrWhiteSpace(cfgPhone)) Printing.PrintContext.Company.Phone = cfgPhone;
                if (!string.IsNullOrWhiteSpace(cfgReg)) Printing.PrintContext.Company.RegistrationNo = cfgReg;
            }
            Log("Seed: 9. SystemConfigs processed.");
        }
        catch (Exception ex) { Log($"Seed: 9. SystemConfigs error: {ex.Message}"); }
        
        Log("Seed: ALL steps finished successfully!");
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
        services.AddSingleton<IPestroService, PestroService>();
        services.AddSingleton<IReportService, ReportService>();
        services.AddSingleton<ISystemService, SystemService>();
        services.AddSingleton<ICatalogService, CatalogService>();
        services.AddSingleton<IYearEndService, YearEndService>();
        services.AddSingleton<IDatabaseManagementService, DatabaseManagementService>();

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
        services.AddTransient<BusinessProfileViewModel>();
    }
}
