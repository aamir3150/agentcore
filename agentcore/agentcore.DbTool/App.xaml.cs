using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Infrastructure;
using MyMandiSystem.Infrastructure.Interceptors;
using MyMandiSystem.Infrastructure.Services;
using agentcore.DbTool.ViewModels;
using System;
using System.IO;
using System.Windows;

namespace agentcore.DbTool;

// Using CurrentUserService from MyMandiSystem.Infrastructure.Services

public partial class App : Application
{
    public static ServiceProvider? ServiceProvider { get; private set; }

    protected override void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        this.DispatcherUnhandledException += (s, args) =>
        {
            try
            {
                File.AppendAllText("dbtool_crash.log", $"[{DateTime.Now}] Dispatcher Unhandled: {args.Exception}\n");
            }
            catch { }
            MessageBox.Show($"ایرر پیش آ گیا:\n{args.Exception.Message}\n\nتفصیل:\n{args.Exception}", "agentcore DB Tool Error", MessageBoxButton.OK, MessageBoxImage.Error);
            args.Handled = true;
        };

        AppDomain.CurrentDomain.UnhandledException += (s, args) =>
        {
            try
            {
                File.AppendAllText("dbtool_crash.log", $"[{DateTime.Now}] Domain Unhandled: {args.ExceptionObject}\n");
            }
            catch { }
            MessageBox.Show($"غیر متوقع خرابی:\n{args.ExceptionObject}", "agentcore DB Tool Fatal Error", MessageBoxButton.OK, MessageBoxImage.Error);
        };

        var logFile = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "startup_trace.log");
        File.WriteAllText(logFile, $"[{DateTime.Now}] Starting up agentcore.DbTool\n");

        try
        {
            var baseDir = AppDomain.CurrentDomain.BaseDirectory;
            var configPath = Path.Combine(baseDir, "appsettings.json");
            var basePath = File.Exists(configPath) ? baseDir : Directory.GetCurrentDirectory();
            File.AppendAllText(logFile, $"Base path: {basePath}\n");

            var configuration = new ConfigurationBuilder()
                .SetBasePath(basePath)
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .Build();

            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);

            var configService = new SystemConfigService(configuration);
            services.AddSingleton<ISystemConfigService>(configService);
            services.AddSingleton<ICurrentUserService, CurrentUserService>();
            services.AddSingleton<AuditInterceptor>();

            string year = configService.GetCurrentFinancialYear();
            string dbName = $"{configService.GetDatabasePrefix()}_{year}";
            string connectionString = configService.GetConnectionString(dbName);
            File.AppendAllText(logFile, $"DB: {dbName}, Conn: {connectionString}\n");

            services.AddDbContextFactory<AppDbContext>((sp, options) =>
            {
                var interceptor = sp.GetRequiredService<AuditInterceptor>();
                options.UseSqlServer(connectionString)
                       .AddInterceptors(interceptor);
            });

            services.AddSingleton<IDatabaseManagementService, DatabaseManagementService>();
            services.AddSingleton<IYearEndService, YearEndService>();
            services.AddTransient<DatabaseManagementViewModel>();
            services.AddTransient<MainWindow>();

            ServiceProvider = services.BuildServiceProvider();
            File.AppendAllText(logFile, "DI Built successfully\n");

            var mainWindow = ServiceProvider.GetRequiredService<MainWindow>();
            File.AppendAllText(logFile, "MainWindow instantiated\n");

            var vm = ServiceProvider.GetRequiredService<DatabaseManagementViewModel>();
            mainWindow.DataContext = vm;
            mainWindow.Loaded += async (s, args) => await vm.InitializeAsync(0);

            this.MainWindow = mainWindow;
            mainWindow.Show();
            File.AppendAllText(logFile, "MainWindow.Show called\n");
        }
        catch (Exception ex)
        {
            try { File.AppendAllText(logFile, $"[{DateTime.Now}] Startup Failed: {ex}\n"); } catch { }
            MessageBox.Show($"سٹارٹ اپ کے دوران خرابی:\n{ex.Message}\n\n{ex.StackTrace}", "agentcore DB Tool Startup Error", MessageBoxButton.OK, MessageBoxImage.Error);
            Shutdown();
        }
    }
}
