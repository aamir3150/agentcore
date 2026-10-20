using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Win32;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.ObjectModel;
using System.Data;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace agentcore.DbTool.ViewModels;

public partial class DatabaseManagementViewModel : ObservableObject
{
    private readonly IDatabaseManagementService _dbService;
    private readonly IYearEndService _yearEndService;
    private readonly ISystemConfigService _configService;

    [ObservableProperty] private string _serverName = "Loading...";
    [ObservableProperty] private string _currentActiveDatabase = "";
    [ObservableProperty] private string _currentFinancialYear = "";
    [ObservableProperty] private int _selectedTabIndex = 0;

    // Databases & Tables
    [ObservableProperty] private ObservableCollection<DatabaseSummary> _databases = new();
    [ObservableProperty] private DatabaseSummary? _selectedDatabase;
    [ObservableProperty] private ObservableCollection<TableSummary> _tables = new();
    [ObservableProperty] private ObservableCollection<TableSummary> _filteredTables = new();
    [ObservableProperty] private TableSummary? _selectedTable;
    [ObservableProperty] private string _tableSearchText = "";
    [ObservableProperty] private string _rowFilterText = "";

    // phpMyAdmin Database & Table Structure View
    [ObservableProperty] private DatabaseOverview? _databaseOverview;
    [ObservableProperty] private ObservableCollection<TableDetailedSummary> _detailedTables = new();
    [ObservableProperty] private ObservableCollection<TableDetailedSummary> _filteredDetailedTables = new();
    [ObservableProperty] private string _structureSearchText = "";
    [ObservableProperty] private bool _isViewingSingleTable;
    [ObservableProperty] private TableStructureDetails? _currentTableStructure;
    [ObservableProperty] private ObservableCollection<ColumnStructure> _tableColumns = new();
    [ObservableProperty] private ObservableCollection<TableIndexSummary> _tableIndexes = new();
    [ObservableProperty] private ObservableCollection<TableForeignKeySummary> _tableForeignKeys = new();
    [ObservableProperty] private bool _isLoadingStructure;

    // Data Viewer
    [ObservableProperty] private DataView? _tableDataView;
    private DataTable? _currentLoadedTable;
    [ObservableProperty] private string _dataStatusText = "Select a table to view records.";
    [ObservableProperty] private bool _isLoadingData;

    // Custom SQL Query Runner
    [ObservableProperty] private string _customSqlQuery = "SELECT TOP 100 * FROM Accounts";
    [ObservableProperty] private DataView? _queryResultView;
    [ObservableProperty] private string _queryStatusText = "Ready to execute SQL query.";
    [ObservableProperty] private bool _isExecutingQuery;

    // Year-End Closing
    [ObservableProperty] private YearEndPreCheckResult? _preCheck;
    [ObservableProperty] private string _newYearCode = "";
    [ObservableProperty] private DateTime _newStartDate = DateTime.Now;
    [ObservableProperty] private DateTime _newEndDate = DateTime.Now.AddYears(1);
    [ObservableProperty] private bool _lockPreviousYear = true;
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsYearEndNotRunning))]
    private bool _isYearEndRunning;
    public bool IsYearEndNotRunning => !IsYearEndRunning;

    [ObservableProperty] private string _yearEndProgressStatus = "";
    [ObservableProperty] private ObservableCollection<string> _yearEndLogs = new();

    public DatabaseManagementViewModel(
        IDatabaseManagementService dbService,
        IYearEndService yearEndService,
        ISystemConfigService configService)
    {
        _dbService = dbService;
        _yearEndService = yearEndService;
        _configService = configService;
    }

    public async Task InitializeAsync(int initialTab = 0)
    {
        SelectedTabIndex = initialTab;
        CurrentFinancialYear = _configService.GetCurrentFinancialYear();
        CurrentActiveDatabase = $"{_configService.GetDatabasePrefix()}_{CurrentFinancialYear}";

        try
        {
            ServerName = await _dbService.GetServerNameAsync();
            await LoadDatabasesAsync();
            await LoadPreCheckAsync();
        }
        catch (Exception ex)
        {
            DataStatusText = $"Error initializing: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task RefreshDatabasesAndTablesAsync()
    {
        await LoadDatabasesAsync();
    }

    private async Task LoadDatabasesAsync()
    {
        try
        {
            var dbList = await _dbService.GetDatabasesAsync();
            Databases.Clear();
            foreach (var db in dbList) Databases.Add(db);

            SelectedDatabase = Databases.FirstOrDefault(d => d.IsCurrentYear) ?? Databases.FirstOrDefault();
            if (SelectedDatabase != null)
            {
                await LoadTablesForDatabaseAsync(SelectedDatabase.DatabaseName);
            }
        }
        catch (Exception ex)
        {
            DataStatusText = $"Error loading databases: {ex.Message}";
        }
    }

    async partial void OnSelectedDatabaseChanged(DatabaseSummary? value)
    {
        if (value != null)
        {
            await LoadTablesForDatabaseAsync(value.DatabaseName);
        }
    }

    private async Task LoadTablesForDatabaseAsync(string dbName)
    {
        try
        {
            IsLoadingData = true;
            DataStatusText = $"Loading tables for [{dbName}]...";

            var tableList = await _dbService.GetTablesAsync(dbName);
            Tables.Clear();
            foreach (var t in tableList) Tables.Add(t);

            ApplyTableFilter();

            if (FilteredTables.Any())
            {
                SelectedTable = FilteredTables.FirstOrDefault(t => t.TableName == "Accounts") 
                             ?? FilteredTables.FirstOrDefault(t => t.TableName == "Parties")
                             ?? FilteredTables.First();
            }
            else
            {
                TableDataView = null;
                DataStatusText = "No tables found in this database.";
            }

            // Load phpMyAdmin database structure overview
            await LoadDatabaseOverviewAsync(dbName);
        }
        catch (Exception ex)
        {
            DataStatusText = $"Error loading tables: {ex.Message}";
        }
        finally
        {
            IsLoadingData = false;
        }
    }

    [RelayCommand]
    public async Task LoadDatabaseOverviewAsync(string? dbName = null)
    {
        try
        {
            IsLoadingStructure = true;
            var targetDb = dbName ?? SelectedDatabase?.DatabaseName;
            var overview = await _dbService.GetDatabaseOverviewAsync(targetDb);
            DatabaseOverview = overview;

            DetailedTables.Clear();
            foreach (var t in overview.Tables) DetailedTables.Add(t);

            ApplyStructureFilter();
            IsViewingSingleTable = false;
        }
        catch (Exception ex)
        {
            DataStatusText = $"Error loading database structure overview: {ex.Message}";
        }
        finally
        {
            IsLoadingStructure = false;
        }
    }

    partial void OnStructureSearchTextChanged(string value)
    {
        ApplyStructureFilter();
    }

    private void ApplyStructureFilter()
    {
        FilteredDetailedTables.Clear();
        var query = StructureSearchText?.Trim().ToLowerInvariant() ?? "";
        var results = string.IsNullOrWhiteSpace(query)
            ? DetailedTables
            : DetailedTables.Where(t => t.TableName.ToLowerInvariant().Contains(query) || t.Category.ToLowerInvariant().Contains(query));

        foreach (var r in results) FilteredDetailedTables.Add(r);
    }

    [RelayCommand]
    public async Task ViewTableStructureAsync(string? tableName = null)
    {
        var target = tableName ?? SelectedTable?.TableName;
        if (string.IsNullOrWhiteSpace(target)) return;

        try
        {
            IsLoadingStructure = true;
            SelectedTabIndex = 0; // Structure tab
            IsViewingSingleTable = true;

            var dbName = SelectedDatabase?.DatabaseName;
            var details = await _dbService.GetTableStructureDetailsAsync(target, dbName);
            CurrentTableStructure = details;

            TableColumns.Clear();
            foreach (var col in details.Columns) TableColumns.Add(col);

            TableIndexes.Clear();
            foreach (var idx in details.Indexes) TableIndexes.Add(idx);

            TableForeignKeys.Clear();
            foreach (var fk in details.ForeignKeys) TableForeignKeys.Add(fk);

            // Synchronize SelectedTable
            var match = Tables.FirstOrDefault(t => t.TableName.Equals(target, StringComparison.OrdinalIgnoreCase));
            if (match != null) SelectedTable = match;
        }
        catch (Exception ex)
        {
            MessageBox.Show($"خرابی برائے ٹیبل سٹرکچر [{target}]:\n{ex.Message}", "Structure Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsLoadingStructure = false;
        }
    }

    [RelayCommand]
    public void BackToDatabaseOverview()
    {
        IsViewingSingleTable = false;
    }

    [RelayCommand]
    public async Task BrowseTableDataAsync(string? tableName = null)
    {
        var target = tableName ?? SelectedTable?.TableName;
        if (string.IsNullOrWhiteSpace(target)) return;

        var match = Tables.FirstOrDefault(t => t.TableName.Equals(target, StringComparison.OrdinalIgnoreCase));
        if (match != null) SelectedTable = match;

        SelectedTabIndex = 1; // Browse Tab
        await LoadSelectedTableDataAsync(target);
    }

    [RelayCommand]
    public void OpenQueryForTable(string? tableName = null)
    {
        var target = tableName ?? SelectedTable?.TableName ?? "Accounts";
        CustomSqlQuery = $"SELECT TOP 100 * FROM [{target}]";
        SelectedTabIndex = 2; // SQL Tab
    }

    [RelayCommand]
    public void ApplySqlPreset(string preset)
    {
        var table = SelectedTable?.TableName ?? "Accounts";
        CustomSqlQuery = preset switch
        {
            "SELECT" => $"SELECT TOP 100 *\nFROM [{table}];",
            "COUNT" => $"SELECT COUNT(*) AS [TotalRecords]\nFROM [{table}];",
            "INSERT" => $"-- Example INSERT template\nINSERT INTO [{table}] (/* ColumnNames */)\nVALUES (/* Values */);",
            "UPDATE" => $"-- Example UPDATE template\nUPDATE [{table}]\nSET /* Column = Value */\nWHERE /* Condition */;",
            "DELETE" => $"-- CAUTION: Delete query\n-- DELETE FROM [{table}] WHERE /* Condition */;",
            _ => $"SELECT TOP 100 * FROM [{table}];"
        };
    }

    partial void OnTableSearchTextChanged(string value)
    {
        ApplyTableFilter();
    }

    private void ApplyTableFilter()
    {
        FilteredTables.Clear();
        var query = TableSearchText?.Trim().ToLowerInvariant() ?? "";
        var results = string.IsNullOrWhiteSpace(query)
            ? Tables
            : Tables.Where(t => t.TableName.ToLowerInvariant().Contains(query) || t.Category.ToLowerInvariant().Contains(query));

        foreach (var r in results) FilteredTables.Add(r);
    }

    async partial void OnSelectedTableChanged(TableSummary? value)
    {
        if (value != null)
        {
            await LoadSelectedTableDataAsync(value.TableName);
        }
    }

    [RelayCommand]
    public async Task LoadSelectedTableDataAsync(string? tableName = null)
    {
        var targetTable = tableName ?? SelectedTable?.TableName;
        if (string.IsNullOrWhiteSpace(targetTable)) return;

        try
        {
            IsLoadingData = true;
            DataStatusText = $"Loading rows from [{targetTable}]...";

            var dbName = SelectedDatabase?.DatabaseName;
            var dt = await _dbService.GetTableDataAsync(targetTable, dbName, topRows: 1500);

            _currentLoadedTable = dt;
            TableDataView = dt.DefaultView;
            DataStatusText = $"Table: [{targetTable}] | Columns: {dt.Columns.Count} | Total Rows: {dt.Rows.Count:N0}";
            ApplyRowFilter();
        }
        catch (Exception ex)
        {
            DataStatusText = $"Error loading table [{targetTable}]: {ex.Message}";
        }
        finally
        {
            IsLoadingData = false;
        }
    }

    partial void OnRowFilterTextChanged(string value)
    {
        ApplyRowFilter();
    }

    private void ApplyRowFilter()
    {
        if (_currentLoadedTable == null || TableDataView == null) return;

        var filter = RowFilterText?.Trim();
        if (string.IsNullOrWhiteSpace(filter))
        {
            TableDataView.RowFilter = string.Empty;
            DataStatusText = $"Table: [{_currentLoadedTable.TableName}] | Columns: {_currentLoadedTable.Columns.Count} | Total Rows: {_currentLoadedTable.Rows.Count:N0}";
            return;
        }

        try
        {
            var stringCols = _currentLoadedTable.Columns.Cast<DataColumn>()
                .Where(c => c.DataType == typeof(string))
                .ToList();

            if (stringCols.Any())
            {
                var escaped = filter.Replace("'", "''");
                var clauses = stringCols.Select(c => $"[{c.ColumnName}] LIKE '%{escaped}%'");
                TableDataView.RowFilter = string.Join(" OR ", clauses);
                DataStatusText = $"Filtered Rows: {TableDataView.Count:N0} of {_currentLoadedTable.Rows.Count:N0} (Matching: \"{filter}\")";
            }
        }
        catch
        {
            TableDataView.RowFilter = string.Empty;
        }
    }

    [RelayCommand]
    public async Task ExportCurrentTableAsync()
    {
        if (_currentLoadedTable == null || _currentLoadedTable.Rows.Count == 0)
        {
            MessageBox.Show("No data available to export.", "Export Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var dialog = new SaveFileDialog
        {
            FileName = $"{_currentLoadedTable.TableName}_{DateTime.Now:yyyyMMdd_HHmm}.csv",
            Filter = "CSV Files (*.csv)|*.csv|All Files (*.*)|*.*",
            Title = $"Export [{_currentLoadedTable.TableName}] Data"
        };

        if (dialog.ShowDialog() == true)
        {
            try
            {
                var success = await _dbService.ExportTableToCsvAsync(_currentLoadedTable, dialog.FileName);
                if (success)
                {
                    var res = MessageBox.Show($"Successfully exported {_currentLoadedTable.Rows.Count} rows to:\n{dialog.FileName}\n\nDo you want to open the file?", 
                        "Export Complete", MessageBoxButton.YesNo, MessageBoxImage.Information);
                    if (res == MessageBoxResult.Yes)
                    {
                        Process.Start(new ProcessStartInfo(dialog.FileName) { UseShellExecute = true });
                    }
                }
                else
                {
                    MessageBox.Show("Failed to export table data.", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error exporting: {ex.Message}", "Export Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }

    [RelayCommand]
    public async Task ExecuteCustomQueryAsync()
    {
        if (string.IsNullOrWhiteSpace(CustomSqlQuery)) return;

        try
        {
            IsExecutingQuery = true;
            QueryStatusText = "Executing query...";

            var sw = Stopwatch.StartNew();
            var dt = await _dbService.ExecuteQueryAsync(CustomSqlQuery, SelectedDatabase?.DatabaseName);
            sw.Stop();

            QueryResultView = dt.DefaultView;
            QueryStatusText = $"Success! Returned {dt.Rows.Count:N0} row(s) in {sw.ElapsedMilliseconds} ms.";
        }
        catch (Exception ex)
        {
            QueryStatusText = $"Query Error: {ex.Message}";
        }
        finally
        {
            IsExecutingQuery = false;
        }
    }

    [RelayCommand]
    public async Task LoadPreCheckAsync()
    {
        try
        {
            PreCheck = await _yearEndService.PerformPreCheckAsync();
            NewYearCode = PreCheck.SuggestedNewYearCode;
            NewStartDate = PreCheck.SuggestedStartDate;
            NewEndDate = PreCheck.SuggestedEndDate;
        }
        catch (Exception ex)
        {
            YearEndProgressStatus = $"Error loading Year-End audit: {ex.Message}";
        }
    }

    [RelayCommand]
    public async Task ExecuteYearEndRolloverAsync()
    {
        if (string.IsNullOrWhiteSpace(NewYearCode))
        {
            MessageBox.Show("Please specify a valid Target New Year Code (e.g. 2026-27).", "Validation Error", MessageBoxButton.OK, MessageBoxImage.Warning);
            return;
        }

        var confirmMsg = $"⚠️ سال کا اختتام اور نیا مالی سال بنانا\n\n" +
            $"موجودہ مالی سال: {PreCheck?.CurrentYearCode}\n" +
            $"نیا مالی سال: {NewYearCode}\n" +
            $"ٹارگٹ ڈیٹا بیس: {_configService.GetDatabasePrefix()}_{NewYearCode}\n" +
            $"پرانا سال لاک کریں: {(LockPreviousYear ? "جی ہاں (Read-Only)" : "نہیں")}\n\n" +
            $"کیا آپ واقعی نیا مالی سال بنانا چاہتے ہیں؟\n" +
            $"اس عمل سے تمام کھاتہ جات اور پروڈکٹس کے کلوزنگ بیلنسز نئے سال میں اوپننگ بیلنس بن جائیں گے، اور نئے سال کا روزنامچہ صفر سے شروع ہو گا۔";

        var confirm = MessageBox.Show(confirmMsg, "Confirm Year-End Closing", MessageBoxButton.YesNo, MessageBoxImage.Question);
        if (confirm != MessageBoxResult.Yes) return;

        try
        {
            IsYearEndRunning = true;
            YearEndLogs.Clear();
            YearEndProgressStatus = "Starting Year-End rollover...";

            var progress = new Progress<string>(msg =>
            {
                YearEndProgressStatus = msg;
                YearEndLogs.Add($"[{DateTime.Now:HH:mm:ss}] {msg}");
            });

            var request = new YearEndRequest
            {
                CurrentYearCode = PreCheck?.CurrentYearCode ?? _configService.GetCurrentFinancialYear(),
                NewYearCode = NewYearCode.Trim(),
                NewStartDate = NewStartDate,
                NewEndDate = NewEndDate,
                LockPreviousYear = LockPreviousYear
            };

            var result = await _yearEndService.StartNewFinancialYearAsync(request, progress);

            if (result.Success)
            {
                MessageBox.Show(
                    $"مبارک ہو! نیا مالی سال ({NewYearCode}) کامیابی سے بن گیا ہے۔\n\n" +
                    $"• منتقل شدہ اکاؤنٹس: {result.AccountsCarriedForward}\n" +
                    $"• منتقل شدہ پروڈکٹس: {result.ProductsCarriedForward}\n" +
                    $"• نیا ڈیٹا بیس: {result.TargetDatabaseName}\n\n" +
                    $"سافٹ ویئر کو نئے مالی سال کے ساتھ استعمال کرنے کے لیے مرکزی ایپلیکیشن شروع کریں۔",
                    "Year-End Rollover Successful", MessageBoxButton.OK, MessageBoxImage.Information);

                await InitializeAsync(initialTab: 0);
            }
            else
            {
                MessageBox.Show($"Year-End Rollover Failed:\n{result.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
        catch (Exception ex)
        {
            MessageBox.Show($"Unexpected error: {ex.Message}", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
        }
        finally
        {
            IsYearEndRunning = false;
        }
    }

    [RelayCommand]
    public async Task SwitchFinancialYearAsync(DatabaseSummary? target)
    {
        if (target == null) return;

        if (target.IsCurrentYear)
        {
            MessageBox.Show($"مالی سال {target.YearCode} پہلے سے فعال (Active) ہے۔", "Notice", MessageBoxButton.OK, MessageBoxImage.Information);
            return;
        }

        var confirm = MessageBox.Show(
            $"کیا آپ مالی سال تبدیل کر کے '{target.YearCode}' میں جانا چاہتے ہیں؟\n\n" +
            $"ڈیٹا بیس: {target.DatabaseName}\n" +
            $"حیثیت: {target.StatusText}\n\n" +
            $"نوٹ: اگر یہ سال لاک ہے تو صارفین صرف پرانا ریکارڈ اور رپورٹس دیکھ سکیں گے، نیا اندراج نہیں ہو سکے گا۔",
            "Switch Financial Year", MessageBoxButton.YesNo, MessageBoxImage.Question);

        if (confirm == MessageBoxResult.Yes)
        {
            await _yearEndService.SwitchActiveFinancialYearAsync(target.YearCode);
            MessageBox.Show($"مرکزی سسٹم کا مالی سال {target.YearCode} پر سیٹ کر دیا گیا ہے۔", "Success", MessageBoxButton.OK, MessageBoxImage.Information);
            await InitializeAsync(initialTab: 2);
        }
    }
}
