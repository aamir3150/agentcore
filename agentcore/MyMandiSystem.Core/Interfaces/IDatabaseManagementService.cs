using System;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace MyMandiSystem.Core.Interfaces;

public class TableSummary
{
    public string TableName { get; set; } = string.Empty;
    public long RowCount { get; set; }
    public string Category { get; set; } = "General";
    public string DisplayTitle => $"{TableName} ({RowCount:N0} rows)";
}

public class DatabaseSummary
{
    public string DatabaseName { get; set; } = string.Empty;
    public string YearCode { get; set; } = string.Empty;
    public double SizeMb { get; set; }
    public bool IsCurrentYear { get; set; }
    public bool IsLocked { get; set; }
    public string StatusText => IsCurrentYear ? "Active / Current" : (IsLocked ? "Locked (Read-Only)" : "Archived");
}

public class TableDetailedSummary
{
    public string TableName { get; set; } = string.Empty;
    public long RowCount { get; set; }
    public int ColumnCount { get; set; }
    public double TotalSpaceMb { get; set; }
    public double UsedSpaceMb { get; set; }
    public double UnusedSpaceMb { get; set; }
    public string EngineType { get; set; } = "Clustered";
    public string Collation { get; set; } = "SQL_Latin1_General_CP1_CI_AS";
    public DateTime? CreateDate { get; set; }
    public DateTime? ModifyDate { get; set; }
    public string Category { get; set; } = "General";
    public string DisplaySize => TotalSpaceMb >= 1.0 ? $"{TotalSpaceMb:F2} MB" : $"{(TotalSpaceMb * 1024):F0} KB";
}

public class ColumnStructure
{
    public int OrdinalPosition { get; set; }
    public string ColumnName { get; set; } = string.Empty;
    public string DataType { get; set; } = string.Empty;
    public string FullDataType { get; set; } = string.Empty;
    public bool IsNullable { get; set; }
    public string NullableText => IsNullable ? "YES" : "NO";
    public string DefaultValue { get; set; } = "None";
    public string Collation { get; set; } = "-";
    public bool IsPrimaryKey { get; set; }
    public bool IsIdentity { get; set; }
    public bool IsForeignKey { get; set; }
    public string KeyBadge => IsPrimaryKey ? "PRIMARY 🔑" : (IsForeignKey ? "FOREIGN 🔗" : (IsIdentity ? "IDENTITY ⚡" : "-"));
    public string Extra => IsIdentity ? "auto_increment" : "";
}

public class TableIndexSummary
{
    public string IndexName { get; set; } = string.Empty;
    public string IndexType { get; set; } = string.Empty;
    public bool IsUnique { get; set; }
    public bool IsPrimaryKey { get; set; }
    public string Columns { get; set; } = string.Empty;
}

public class TableForeignKeySummary
{
    public string ConstraintName { get; set; } = string.Empty;
    public string ColumnName { get; set; } = string.Empty;
    public string ReferencedTable { get; set; } = string.Empty;
    public string ReferencedColumn { get; set; } = string.Empty;
    public string OnDeleteAction { get; set; } = "NO_ACTION";
}

public class TableStructureDetails
{
    public string TableName { get; set; } = string.Empty;
    public long RowCount { get; set; }
    public double TotalSizeMb { get; set; }
    public List<ColumnStructure> Columns { get; set; } = new();
    public List<TableIndexSummary> Indexes { get; set; } = new();
    public List<TableForeignKeySummary> ForeignKeys { get; set; } = new();
}

public class DatabaseOverview
{
    public string DatabaseName { get; set; } = string.Empty;
    public int TotalTables { get; set; }
    public long TotalRows { get; set; }
    public double TotalSpaceMb { get; set; }
    public List<TableDetailedSummary> Tables { get; set; } = new();
}

public interface IDatabaseManagementService
{
    Task<string> GetServerNameAsync();
    Task<List<DatabaseSummary>> GetDatabasesAsync();
    Task<List<TableSummary>> GetTablesAsync(string? dbName = null);
    Task<DatabaseOverview> GetDatabaseOverviewAsync(string? dbName = null);
    Task<TableStructureDetails> GetTableStructureDetailsAsync(string tableName, string? dbName = null);
    Task<DataTable> GetTableDataAsync(string tableName, string? dbName = null, int topRows = 1000, string? searchFilter = null);
    Task<DataTable> ExecuteQueryAsync(string sqlQuery, string? dbName = null);
    Task<bool> ExportTableToCsvAsync(DataTable table, string filePath);
    Task<bool> AttachDatabaseAsync(string databaseName, string mdfPath, string? ldfPath = null);
    Task<bool> DetachDatabaseAsync(string databaseName);
    Task<bool> BackupDatabaseAsync(string databaseName, string destinationFilePath);
    Task<bool> RestoreDatabaseAsync(string databaseName, string sourceBakFilePath);
    Task<bool> ShrinkDatabaseAsync(string databaseName);
}
