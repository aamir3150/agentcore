using Microsoft.Data.SqlClient;
using MyMandiSystem.Core.Interfaces;
using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace MyMandiSystem.Infrastructure.Services;

public class DatabaseManagementService : IDatabaseManagementService
{
    private readonly ISystemConfigService _configService;

    public DatabaseManagementService(ISystemConfigService configService)
    {
        _configService = configService;
    }

    private string BuildConnectionString(string? dbName)
    {
        var targetDb = string.IsNullOrWhiteSpace(dbName) 
            ? $"{_configService.GetDatabasePrefix()}_{_configService.GetCurrentFinancialYear()}" 
            : dbName;
        return _configService.GetConnectionString(targetDb);
    }

    public async Task<string> GetServerNameAsync()
    {
        try
        {
            var masterConnStr = _configService.GetConnectionString("master");
            await using var conn = new SqlConnection(masterConnStr);
            await conn.OpenAsync();
            await using var cmd = new SqlCommand("SELECT @@SERVERNAME", conn);
            var serverName = await cmd.ExecuteScalarAsync();
            return serverName?.ToString() ?? @".\SQLEXPRESS";
        }
        catch
        {
            return @".\SQLEXPRESS";
        }
    }

    public async Task<List<DatabaseSummary>> GetDatabasesAsync()
    {
        var list = new List<DatabaseSummary>();
        var prefix = _configService.GetDatabasePrefix();
        var currentYear = _configService.GetCurrentFinancialYear();

        try
        {
            var masterConnStr = _configService.GetConnectionString("master");
            await using var conn = new SqlConnection(masterConnStr);
            await conn.OpenAsync();

            var sql = $@"
                SELECT name, is_read_only 
                FROM sys.databases 
                WHERE name LIKE '{prefix}%' 
                ORDER BY name DESC";

            await using var cmd = new SqlCommand(sql, conn);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var dbName = reader.GetString(0);
                var isReadOnly = reader.GetBoolean(1);

                // Extract year code (e.g. MyMandi_2025-26 -> 2025-26)
                var yearCode = dbName.StartsWith($"{prefix}_") 
                    ? dbName.Substring($"{prefix}_".Length) 
                    : dbName;

                var isCurrent = string.Equals(yearCode, currentYear, StringComparison.OrdinalIgnoreCase);

                list.Add(new DatabaseSummary
                {
                    DatabaseName = dbName,
                    YearCode = yearCode,
                    IsCurrentYear = isCurrent,
                    IsLocked = isReadOnly
                });
            }
        }
        catch
        {
            // Fallback to current year if sys.databases cannot be queried
            list.Add(new DatabaseSummary
            {
                DatabaseName = $"{prefix}_{currentYear}",
                YearCode = currentYear,
                IsCurrentYear = true,
                IsLocked = false
            });
        }

        return list;
    }

    public async Task<List<TableSummary>> GetTablesAsync(string? dbName = null)
    {
        var connStr = BuildConnectionString(dbName);
        var tables = new List<TableSummary>();

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        var sql = @"
            SELECT t.name AS TableName, SUM(p.rows) AS RowCounts
            FROM sys.tables t
            INNER JOIN sys.partitions p ON t.object_id = p.OBJECT_ID
            WHERE p.index_id IN (0,1) AND t.is_ms_shipped = 0
            GROUP BY t.name
            ORDER BY t.name ASC";

        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var name = reader.GetString(0);
            var count = reader.GetInt64(1);

            tables.Add(new TableSummary
            {
                TableName = name,
                RowCount = count,
                Category = ClassifyTable(name)
            });
        }

        return tables;
    }

    public async Task<DatabaseOverview> GetDatabaseOverviewAsync(string? dbName = null)
    {
        var targetDb = string.IsNullOrWhiteSpace(dbName) 
            ? $"{_configService.GetDatabasePrefix()}_{_configService.GetCurrentFinancialYear()}" 
            : dbName;
        var connStr = BuildConnectionString(targetDb);
        var overview = new DatabaseOverview { DatabaseName = targetDb };

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        var sql = @"
            SELECT 
                t.name AS TableName,
                SUM(p.rows) AS [RowCount],
                ROUND(((SUM(a.total_pages) * 8) / 1024.0), 2) AS TotalSpaceMB,
                ROUND(((SUM(a.used_pages) * 8) / 1024.0), 2) AS UsedSpaceMB,
                ROUND(((SUM(a.total_pages) - SUM(a.used_pages)) * 8 / 1024.0), 2) AS UnusedSpaceMB,
                MAX(t.create_date) AS CreateDate,
                MAX(t.modify_date) AS ModifyDate,
                (SELECT COUNT(*) FROM sys.columns c WHERE c.object_id = t.object_id) AS ColumnCount,
                CASE WHEN MAX(i.type_desc) = 'CLUSTERED' THEN 'Clustered Index' ELSE 'Heap' END AS EngineType
            FROM sys.tables t
            INNER JOIN sys.indexes i ON t.object_id = i.object_id
            INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
            INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
            WHERE t.is_ms_shipped = 0 AND i.index_id IN (0, 1)
            GROUP BY t.object_id, t.name
            ORDER BY t.name ASC";

        await using var cmd = new SqlCommand(sql, conn);
        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var tableName = reader.GetString(0);
            var rowCount = reader.GetInt64(1);
            var totalSpaceMb = Convert.ToDouble(reader.GetValue(2));
            var usedSpaceMb = Convert.ToDouble(reader.GetValue(3));
            var unusedSpaceMb = Convert.ToDouble(reader.GetValue(4));
            var createDate = reader.IsDBNull(5) ? (DateTime?)null : reader.GetDateTime(5);
            var modifyDate = reader.IsDBNull(6) ? (DateTime?)null : reader.GetDateTime(6);
            var colCount = reader.GetInt32(7);
            var engineType = reader.GetString(8);

            overview.Tables.Add(new TableDetailedSummary
            {
                TableName = tableName,
                RowCount = rowCount,
                ColumnCount = colCount,
                TotalSpaceMb = totalSpaceMb,
                UsedSpaceMb = usedSpaceMb,
                UnusedSpaceMb = unusedSpaceMb,
                EngineType = engineType,
                CreateDate = createDate,
                ModifyDate = modifyDate,
                Category = ClassifyTable(tableName)
            });
        }

        overview.TotalTables = overview.Tables.Count;
        overview.TotalRows = overview.Tables.Sum(t => t.RowCount);
        overview.TotalSpaceMb = overview.Tables.Sum(t => t.TotalSpaceMb);

        return overview;
    }

    public async Task<TableStructureDetails> GetTableStructureDetailsAsync(string tableName, string? dbName = null)
    {
        if (!Regex.IsMatch(tableName, @"^[a-zA-Z0-9_]+$"))
        {
            throw new ArgumentException("Invalid table name.");
        }

        var connStr = BuildConnectionString(dbName);
        var details = new TableStructureDetails { TableName = tableName };

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        // 1. Columns & Schema
        var colSql = @"
            SELECT 
                c.COLUMN_NAME AS ColumnName,
                c.ORDINAL_POSITION AS OrdinalPosition,
                c.DATA_TYPE AS DataType,
                CASE 
                    WHEN c.CHARACTER_MAXIMUM_LENGTH = -1 THEN c.DATA_TYPE + '(max)'
                    WHEN c.CHARACTER_MAXIMUM_LENGTH IS NOT NULL THEN c.DATA_TYPE + '(' + CAST(c.CHARACTER_MAXIMUM_LENGTH AS VARCHAR) + ')'
                    WHEN c.NUMERIC_PRECISION IS NOT NULL AND c.DATA_TYPE IN ('decimal', 'numeric') THEN c.DATA_TYPE + '(' + CAST(c.NUMERIC_PRECISION AS VARCHAR) + ',' + CAST(c.NUMERIC_SCALE AS VARCHAR) + ')'
                    ELSE c.DATA_TYPE
                END AS FullDataType,
                c.IS_NULLABLE AS IsNullable,
                ISNULL(c.COLUMN_DEFAULT, 'None') AS DefaultValue,
                ISNULL(c.COLLATION_NAME, '-') AS CollationName,
                CASE WHEN pk.COLUMN_NAME IS NOT NULL THEN 1 ELSE 0 END AS IsPrimaryKey,
                COLUMNPROPERTY(OBJECT_ID(c.TABLE_SCHEMA + '.' + c.TABLE_NAME), c.COLUMN_NAME, 'IsIdentity') AS IsIdentity
            FROM INFORMATION_SCHEMA.COLUMNS c
            LEFT JOIN (
                SELECT ku.TABLE_SCHEMA, ku.TABLE_NAME, ku.COLUMN_NAME
                FROM INFORMATION_SCHEMA.TABLE_CONSTRAINTS tc
                JOIN INFORMATION_SCHEMA.KEY_COLUMN_USAGE ku
                    ON tc.CONSTRAINT_NAME = ku.CONSTRAINT_NAME
                    AND tc.TABLE_SCHEMA = ku.TABLE_SCHEMA
                WHERE tc.CONSTRAINT_TYPE = 'PRIMARY KEY'
            ) pk ON c.TABLE_SCHEMA = pk.TABLE_SCHEMA 
                AND c.TABLE_NAME = pk.TABLE_NAME 
                AND c.COLUMN_NAME = pk.COLUMN_NAME
            WHERE c.TABLE_NAME = @TableName
            ORDER BY c.ORDINAL_POSITION;";

        await using (var cmd = new SqlCommand(colSql, conn))
        {
            cmd.Parameters.AddWithValue("@TableName", tableName);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var isNullable = string.Equals(reader.GetString(4), "YES", StringComparison.OrdinalIgnoreCase);
                var isPk = reader.GetInt32(7) == 1;
                var isIdentity = !reader.IsDBNull(8) && reader.GetInt32(8) == 1;

                details.Columns.Add(new ColumnStructure
                {
                    ColumnName = reader.GetString(0),
                    OrdinalPosition = reader.GetInt32(1),
                    DataType = reader.GetString(2),
                    FullDataType = reader.GetString(3),
                    IsNullable = isNullable,
                    DefaultValue = reader.GetString(5),
                    Collation = reader.GetString(6),
                    IsPrimaryKey = isPk,
                    IsIdentity = isIdentity
                });
            }
        }

        // 2. Indexes
        var idxSql = @"
            SELECT 
                i.name AS IndexName,
                i.type_desc AS IndexType,
                i.is_unique AS IsUnique,
                i.is_primary_key AS IsPrimaryKey,
                STRING_AGG(c.name, ', ') WITHIN GROUP (ORDER BY ic.key_ordinal) AS IndexColumns
            FROM sys.indexes i
            INNER JOIN sys.index_columns ic ON i.object_id = ic.object_id AND i.index_id = ic.index_id
            INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
            INNER JOIN sys.tables t ON i.object_id = t.object_id
            WHERE t.name = @TableName AND i.name IS NOT NULL
            GROUP BY i.name, i.type_desc, i.is_unique, i.is_primary_key
            ORDER BY i.is_primary_key DESC, i.name ASC;";

        await using (var cmd = new SqlCommand(idxSql, conn))
        {
            cmd.Parameters.AddWithValue("@TableName", tableName);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                details.Indexes.Add(new TableIndexSummary
                {
                    IndexName = reader.GetString(0),
                    IndexType = reader.GetString(1),
                    IsUnique = reader.GetBoolean(2),
                    IsPrimaryKey = reader.GetBoolean(3),
                    Columns = reader.IsDBNull(4) ? "" : reader.GetString(4)
                });
            }
        }

        // 3. Foreign Keys
        var fkSql = @"
            SELECT 
                fk.name AS ConstraintName,
                c1.name AS ColumnName,
                OBJECT_NAME(fk.referenced_object_id) AS ReferencedTable,
                c2.name AS ReferencedColumn,
                fk.delete_referential_action_desc AS OnDeleteAction
            FROM sys.foreign_keys fk
            INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
            INNER JOIN sys.columns c1 ON fkc.parent_object_id = c1.object_id AND fkc.parent_column_id = c1.column_id
            INNER JOIN sys.columns c2 ON fkc.referenced_object_id = c2.object_id AND fkc.referenced_column_id = c2.column_id
            INNER JOIN sys.tables t ON fk.parent_object_id = t.object_id
            WHERE t.name = @TableName
            ORDER BY fk.name ASC;";

        await using (var cmd = new SqlCommand(fkSql, conn))
        {
            cmd.Parameters.AddWithValue("@TableName", tableName);
            await using var reader = await cmd.ExecuteReaderAsync();
            while (await reader.ReadAsync())
            {
                var fkCol = reader.GetString(1);
                details.ForeignKeys.Add(new TableForeignKeySummary
                {
                    ConstraintName = reader.GetString(0),
                    ColumnName = fkCol,
                    ReferencedTable = reader.GetString(2),
                    ReferencedColumn = reader.GetString(3),
                    OnDeleteAction = reader.GetString(4)
                });

                var col = details.Columns.FirstOrDefault(c => c.ColumnName.Equals(fkCol, StringComparison.OrdinalIgnoreCase));
                if (col != null)
                {
                    col.IsForeignKey = true;
                }
            }
        }

        // 4. Row count & size
        var statSql = @"
            SELECT SUM(p.rows), ROUND(((SUM(a.total_pages) * 8) / 1024.0), 2)
            FROM sys.tables t
            INNER JOIN sys.indexes i ON t.object_id = i.object_id
            INNER JOIN sys.partitions p ON i.object_id = p.object_id AND i.index_id = p.index_id
            INNER JOIN sys.allocation_units a ON p.partition_id = a.container_id
            WHERE t.name = @TableName AND i.index_id IN (0,1)
            GROUP BY t.name;";

        await using (var cmd = new SqlCommand(statSql, conn))
        {
            cmd.Parameters.AddWithValue("@TableName", tableName);
            await using var reader = await cmd.ExecuteReaderAsync();
            if (await reader.ReadAsync())
            {
                details.RowCount = reader.IsDBNull(0) ? 0 : reader.GetInt64(0);
                details.TotalSizeMb = reader.IsDBNull(1) ? 0.0 : Convert.ToDouble(reader.GetValue(1));
            }
        }

        return details;
    }

    public async Task<DataTable> GetTableDataAsync(string tableName, string? dbName = null, int topRows = 1000, string? searchFilter = null)
    {
        // Sanitize table name against SQL injection: must only contain alphanumeric / underscore
        if (!Regex.IsMatch(tableName, @"^[a-zA-Z0-9_]+$"))
        {
            throw new ArgumentException("Invalid table name.");
        }

        var connStr = BuildConnectionString(dbName);
        var dt = new DataTable(tableName);

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        var sql = $"SELECT TOP ({topRows}) * FROM [{tableName}]";

        await using var cmd = new SqlCommand(sql, conn);
        using var adapter = new SqlDataAdapter(cmd);
        adapter.Fill(dt);

        return dt;
    }

    public async Task<DataTable> ExecuteQueryAsync(string sqlQuery, string? dbName = null)
    {
        var trimmed = sqlQuery.Trim();
        var connStr = BuildConnectionString(dbName);
        var dt = new DataTable("QueryResult");

        await using var conn = new SqlConnection(connStr);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(trimmed, conn)
        {
            CommandTimeout = 60
        };

        using var adapter = new SqlDataAdapter(cmd);
        adapter.Fill(dt);

        return dt;
    }

    public Task<bool> ExportTableToCsvAsync(DataTable table, string filePath)
    {
        return Task.Run(() =>
        {
            try
            {
                var sb = new StringBuilder();

                // Headers
                var columnNames = table.Columns.Cast<DataColumn>().Select(c => EscapeCsv(c.ColumnName));
                sb.AppendLine(string.Join(",", columnNames));

                // Rows
                foreach (DataRow row in table.Rows)
                {
                    var fields = table.Columns.Cast<DataColumn>().Select(col =>
                    {
                        var val = row[col];
                        if (val == DBNull.Value || val == null) return "";
                        if (val is DateTime dt) return dt.ToString("yyyy-MM-dd HH:mm:ss");
                        return EscapeCsv(val.ToString() ?? "");
                    });
                    sb.AppendLine(string.Join(",", fields));
                }

                File.WriteAllText(filePath, sb.ToString(), Encoding.UTF8);
                return true;
            }
            catch
            {
                return false;
            }
        });
    }

    private string EscapeCsv(string str)
    {
        if (str.Contains(',') || str.Contains('"') || str.Contains('\n') || str.Contains('\r'))
        {
            return $"\"{str.Replace("\"", "\"\"")}\"";
        }
        return str;
    }

    private string ClassifyTable(string tableName)
    {
        var lower = tableName.ToLowerInvariant();
        if (lower.Contains("account") || lower.Contains("ledger")) return "Chart of Accounts";
        if (lower.Contains("part") || lower.Contains("town") || lower.Contains("sector") || lower.Contains("salesman")) return "Parties & Directory";
        if (lower.Contains("voucher") || lower.Contains("cheque") || lower.Contains("bank")) return "Vouchers & Cash";
        if (lower.Contains("product") || lower.Contains("unit") || lower.Contains("stock") || lower.Contains("comp")) return "Products & Inventory";
        if (lower.Contains("invoice") || lower.Contains("contract") || lower.Contains("gatepass") || lower.Contains("brokerage")) return "Trading & Brokerage";
        if (lower.Contains("pestro")) return "Pestro Chemicals";
        return "System Foundation";
    }

    public async Task<bool> AttachDatabaseAsync(string databaseName, string mdfPath, string? ldfPath = null)
    {
        if (string.IsNullOrWhiteSpace(databaseName) || string.IsNullOrWhiteSpace(mdfPath) || !File.Exists(mdfPath))
        {
            throw new FileNotFoundException($"MDF database file not found at: {mdfPath}");
        }

        var masterConnStr = _configService.GetConnectionString("master");
        await using var conn = new SqlConnection(masterConnStr);
        await conn.OpenAsync();

        string sql;
        var cleanMdf = mdfPath.Replace("'", "''");
        if (!string.IsNullOrWhiteSpace(ldfPath) && File.Exists(ldfPath))
        {
            var cleanLdf = ldfPath.Replace("'", "''");
            sql = $"CREATE DATABASE [{databaseName}] ON (FILENAME = '{cleanMdf}'), (FILENAME = '{cleanLdf}') FOR ATTACH;";
        }
        else
        {
            sql = $"CREATE DATABASE [{databaseName}] ON (FILENAME = '{cleanMdf}') FOR ATTACH_REBUILD_LOG;";
        }

        await using var cmd = new SqlCommand(sql, conn);
        cmd.CommandTimeout = 120;
        await cmd.ExecuteNonQueryAsync();
        return true;
    }

    public async Task<bool> DetachDatabaseAsync(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("Database name is required.");

        var masterConnStr = _configService.GetConnectionString("master");
        await using var conn = new SqlConnection(masterConnStr);
        await conn.OpenAsync();

        var sql = $@"
ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
EXEC sp_detach_db '{databaseName.Replace("'", "''")}';";

        await using var cmd = new SqlCommand(sql, conn);
        cmd.CommandTimeout = 120;
        await cmd.ExecuteNonQueryAsync();
        return true;
    }

    public async Task<bool> BackupDatabaseAsync(string databaseName, string destinationFilePath)
    {
        if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("Database name is required.");
        if (string.IsNullOrWhiteSpace(destinationFilePath)) throw new ArgumentException("Destination file path is required.");

        var dir = Path.GetDirectoryName(destinationFilePath);
        if (!string.IsNullOrEmpty(dir) && !Directory.Exists(dir))
        {
            Directory.CreateDirectory(dir);
        }

        var masterConnStr = _configService.GetConnectionString("master");
        await using var conn = new SqlConnection(masterConnStr);
        await conn.OpenAsync();

        var sql = $"BACKUP DATABASE [{databaseName}] TO DISK = '{destinationFilePath.Replace("'", "''")}' WITH INIT, STATS = 10;";
        await using var cmd = new SqlCommand(sql, conn);
        cmd.CommandTimeout = 300;
        await cmd.ExecuteNonQueryAsync();
        return true;
    }

    public async Task<bool> RestoreDatabaseAsync(string databaseName, string sourceBakFilePath)
    {
        if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("Database name is required.");
        if (string.IsNullOrWhiteSpace(sourceBakFilePath) || !File.Exists(sourceBakFilePath))
        {
            throw new FileNotFoundException($"Backup file not found at: {sourceBakFilePath}");
        }

        var masterConnStr = _configService.GetConnectionString("master");
        await using var conn = new SqlConnection(masterConnStr);
        await conn.OpenAsync();

        var sql = $@"
IF EXISTS (SELECT name FROM sys.databases WHERE name = '{databaseName}')
BEGIN
    ALTER DATABASE [{databaseName}] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
END
RESTORE DATABASE [{databaseName}] FROM DISK = '{sourceBakFilePath.Replace("'", "''")}' WITH REPLACE;
ALTER DATABASE [{databaseName}] SET MULTI_USER;";

        await using var cmd = new SqlCommand(sql, conn);
        cmd.CommandTimeout = 300;
        await cmd.ExecuteNonQueryAsync();
        return true;
    }

    public async Task<bool> ShrinkDatabaseAsync(string databaseName)
    {
        if (string.IsNullOrWhiteSpace(databaseName)) throw new ArgumentException("Database name is required.");

        var masterConnStr = _configService.GetConnectionString("master");
        await using var conn = new SqlConnection(masterConnStr);
        await conn.OpenAsync();

        var sql = $"DBCC SHRINKDATABASE ([{databaseName}]);";
        await using var cmd = new SqlCommand(sql, conn);
        cmd.CommandTimeout = 180;
        await cmd.ExecuteNonQueryAsync();
        return true;
    }
}
