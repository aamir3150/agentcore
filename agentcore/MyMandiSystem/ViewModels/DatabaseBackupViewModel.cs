using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Data.SqlClient;
using MyMandiSystem.Core.Interfaces;
using MyMandiSystem.Helpers;
using System;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Windows;

namespace MyMandiSystem.ViewModels;

public partial class DatabaseBackupViewModel : ObservableObject
{
    private readonly ISystemConfigService _configService;

    [ObservableProperty] private string _databaseName = "";
    [ObservableProperty] private string _backupFilePath = "";
    [ObservableProperty] private string _estimatedSize = "Calculated on start...";
    [ObservableProperty] private string _statusMessage = "Ready to create SQL database backup / بیک اپ بنانے کے لیے تیار";
    [ObservableProperty] private double _progressValue;
    [ObservableProperty] private bool _isBackingUp;
    [ObservableProperty] private bool _isCompleted;
    [ObservableProperty] private string _completedFileSize = "";

    public DatabaseBackupViewModel(ISystemConfigService configService)
    {
        _configService = configService;
        InitializeBackupInfo();
    }

    private void InitializeBackupInfo()
    {
        var year = _configService.GetCurrentFinancialYear();
        DatabaseName = $"MyMandi_{year}";

        var defaultFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "agentcore_Backups");
        if (!Directory.Exists(defaultFolder))
        {
            try { Directory.CreateDirectory(defaultFolder); } catch {}
        }

        var fileName = $"{DatabaseName}_Backup_{DateTime.Now:yyyyMMdd_HHmmss}.bak";
        BackupFilePath = Path.Combine(defaultFolder, fileName);

        _ = LoadDatabaseSizeAsync();
    }

    private async Task LoadDatabaseSizeAsync()
    {
        try
        {
            var connStr = _configService.GetConnectionString(DatabaseName);
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            var sql = "SELECT SUM(size) * 8 / 1024 FROM sys.database_files";
            await using var cmd = new SqlCommand(sql, conn);
            var sizeMb = await cmd.ExecuteScalarAsync();
            if (sizeMb != null && sizeMb != DBNull.Value)
            {
                EstimatedSize = $"Approx. {sizeMb} MB";
            }
        }
        catch
        {
            EstimatedSize = "Approx. 25 MB";
        }
    }

    [RelayCommand]
    public void BrowsePath()
    {
        var dialog = new Microsoft.Win32.SaveFileDialog
        {
            Title = "Select Backup File Destination / بیک اپ محفوظ کرنے کی جگہ منتخب کریں",
            Filter = "SQL Server Backup (*.bak)|*.bak|All Files (*.*)|*.*",
            FileName = Path.GetFileName(BackupFilePath),
            InitialDirectory = Path.GetDirectoryName(BackupFilePath)
        };

        if (dialog.ShowDialog() == true)
        {
            BackupFilePath = dialog.FileName;
        }
    }

    [RelayCommand]
    public async Task StartBackupAsync()
    {
        if (string.IsNullOrWhiteSpace(BackupFilePath))
        {
            MessageBox.Show("Baraye meherbani backup file ka path muntakhab karein.", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        try
        {
            IsBackingUp = true;
            IsCompleted = false;
            ProgressValue = 25;
            StatusMessage = "Connecting to SQL Server and preparing backup device...";

            var dir = Path.GetDirectoryName(BackupFilePath);
            if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
            {
                Directory.CreateDirectory(dir);
            }

            await Task.Delay(300);
            ProgressValue = 55;
            StatusMessage = $"Backing up database [{DatabaseName}] to disk...";

            // Use master connection to execute BACKUP DATABASE
            var masterConnStr = _configService.GetConnectionString("master");
            await using var conn = new SqlConnection(masterConnStr);
            await conn.OpenAsync();

            var cleanPath = BackupFilePath.Replace("'", "''");
            var backupSql = $@"
                BACKUP DATABASE [{DatabaseName}] 
                TO DISK = '{cleanPath}' 
                WITH FORMAT, INIT, NAME = N'{DatabaseName}-Full Database Backup', SKIP, NOREWIND, NOUNLOAD, STATS = 10;";

            await using var cmd = new SqlCommand(backupSql, conn)
            {
                CommandTimeout = 180
            };

            await cmd.ExecuteNonQueryAsync();

            ProgressValue = 100;
            IsCompleted = true;

            if (File.Exists(BackupFilePath))
            {
                var fi = new FileInfo(BackupFilePath);
                var sizeMb = (fi.Length / (1024.0 * 1024.0)).ToString("N2");
                CompletedFileSize = $"{sizeMb} MB";
                StatusMessage = $"کامیابی! ڈیٹا بیس کا مکمل بیک اپ ({sizeMb} MB) محفوظ ہو چکا ہے۔";
            }
            else
            {
                StatusMessage = "ڈیٹا بیس کا مکمل بیک اپ کامیابی سے تیار ہو گیا۔";
            }

            MessageBox.Show($"ڈیٹا بیس بیک اپ کامیابی سے مکمل ہو گیا!\n\nفائل: {BackupFilePath}\nسائز: {CompletedFileSize}", 
                "کامیابی / Backup Complete", MessageBoxButton.OK, MessageBoxImage.Information);
        }
        catch (Exception ex)
        {
            FriendlyErrorHandler.ShowError(ex, "Database Backup Operation");
            StatusMessage = $"نقص: {ex.Message}";
        }
        finally
        {
            IsBackingUp = false;
        }
    }

    [RelayCommand]
    public void OpenBackupFolder()
    {
        try
        {
            if (File.Exists(BackupFilePath))
            {
                Process.Start("explorer.exe", $"/select,\"{BackupFilePath}\"");
            }
            else
            {
                var dir = Path.GetDirectoryName(BackupFilePath);
                if (Directory.Exists(dir)) Process.Start("explorer.exe", $"\"{dir}\"");
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Could not open folder: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Warning);
        }
    }

    [RelayCommand]
    private void Close(Window? window)
    {
        window?.Close();
    }
}
