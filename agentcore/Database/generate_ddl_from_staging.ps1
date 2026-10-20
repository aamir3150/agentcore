param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$SourceDb = "AgenSoft_Master_Schema_Staging",
    [string]$TargetDb = "MyMandi_2025-26",
    [string]$OutFile = "d:\Projects\agentcore\agentcore\Database\Create_Missing_Tables_In_MyMandi.sql"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$SourceDb;Trusted_Connection=True;")
$conn.Open()

# Get missing tables
$cmd = $conn.CreateCommand()
$cmd.CommandText = @"
SELECT t.name 
FROM sys.tables t
WHERE t.is_ms_shipped = 0
  AND t.name NOT IN (SELECT name FROM [$TargetDb].sys.tables WHERE is_ms_shipped = 0)
ORDER BY t.name;
"@

$missingTables = [System.Collections.Generic.List[string]]::new()
$reader = $cmd.ExecuteReader()
while ($reader.Read()) {
    $missingTables.Add($reader["name"].ToString())
}
$reader.Close()

Write-Host "Missing tables to script: $($missingTables.Count)"

$sb = [System.Text.StringBuilder]::new()
$sb.AppendLine("-- ============================================================================") | Out-Null
$sb.AppendLine("-- AUTOMATIC SCHEMA SYNCHRONIZATION FROM ATUPDATE.DLL MASTER SPECIFICATION") | Out-Null
$sb.AppendLine("-- Target Database: [$TargetDb]") | Out-Null
$sb.AppendLine("-- Generated Date: $(Get-Date -Format 'yyyy-MM-dd HH:mm:ss')") | Out-Null
$sb.AppendLine("-- Total Tables to Create: $($missingTables.Count)") | Out-Null
$sb.AppendLine("-- ============================================================================") | Out-Null
$sb.AppendLine("USE [$TargetDb];") | Out-Null
$sb.AppendLine("GO") | Out-Null
$sb.AppendLine() | Out-Null

foreach ($tbl in $missingTables) {
    # Get columns
    $colCmd = $conn.CreateCommand()
    $colCmd.CommandText = @"
    SELECT 
        c.name AS ColumnName,
        tp.name AS TypeName,
        c.max_length AS MaxLength,
        c.precision AS [Precision],
        c.scale AS Scale,
        c.is_nullable AS IsNullable,
        c.is_identity AS IsIdentity,
        ic.seed_value AS IdentitySeed,
        ic.increment_value AS IdentityIncrement,
        dc.definition AS DefaultDefinition
    FROM sys.columns c
    INNER JOIN sys.types tp ON c.user_type_id = tp.user_type_id
    LEFT JOIN sys.identity_columns ic ON c.object_id = ic.object_id AND c.column_id = ic.column_id
    LEFT JOIN sys.default_constraints dc ON c.default_object_id = dc.object_id
    WHERE c.object_id = OBJECT_ID('$SourceDb.dbo.[$tbl]')
    ORDER BY c.column_id;
"@
    
    $colReader = $colCmd.ExecuteReader()
    $colDefs = [System.Collections.Generic.List[string]]::new()
    while ($colReader.Read()) {
        $cName = $colReader["ColumnName"].ToString()
        $tName = $colReader["TypeName"].ToString()
        $maxLen = [int]$colReader["MaxLength"]
        $prec = [int]$colReader["Precision"]
        $scale = [int]$colReader["Scale"]
        $isNullable = [bool]$colReader["IsNullable"]
        $isIdent = [bool]$colReader["IsIdentity"]
        $defVal = if ($colReader["DefaultDefinition"] -ne [DBNull]::Value) { $colReader["DefaultDefinition"].ToString() } else { $null }

        # Type string formatting
        $typeStr = $tName
        if ($tName -in @("varchar", "nvarchar", "char", "nchar", "varbinary", "binary")) {
            if ($maxLen -eq -1) {
                $typeStr = "$tName(max)"
            } else {
                $lenActual = if ($tName -in @("nvarchar", "nchar")) { $maxLen / 2 } else { $maxLen }
                $typeStr = "$tName($lenActual)"
            }
        } elseif ($tName -in @("decimal", "numeric")) {
            $typeStr = "$tName($prec, $scale)"
        }

        $identStr = if ($isIdent) {
            $seed = $colReader["IdentitySeed"]
            $inc = $colReader["IdentityIncrement"]
            " IDENTITY($seed,$inc)"
        } else { "" }

        $nullStr = if ($isNullable) { "NULL" } else { "NOT NULL" }
        $defaultStr = if (![string]::IsNullOrEmpty($defVal)) { " DEFAULT $defVal" } else { "" }

        $colDefs.Add("    [$cName] $typeStr$identStr $nullStr$defaultStr")
    }
    $colReader.Close()

    # Get Primary Key
    $pkCmd = $conn.CreateCommand()
    $pkCmd.CommandText = @"
    SELECT 
        kc.name AS ConstraintName,
        c.name AS ColumnName,
        ic.is_descending_key AS IsDesc
    FROM sys.key_constraints kc
    INNER JOIN sys.index_columns ic ON kc.parent_object_id = ic.object_id AND kc.unique_index_id = ic.index_id
    INNER JOIN sys.columns c ON ic.object_id = c.object_id AND ic.column_id = c.column_id
    WHERE kc.parent_object_id = OBJECT_ID('$SourceDb.dbo.[$tbl]') AND kc.type = 'PK'
    ORDER BY ic.key_ordinal;
"@
    $pkReader = $pkCmd.ExecuteReader()
    $pkCols = [System.Collections.Generic.List[string]]::new()
    $pkName = ""
    while ($pkReader.Read()) {
        $pkName = $pkReader["ConstraintName"].ToString()
        $desc = if ([bool]$pkReader["IsDesc"]) { " DESC" } else { " ASC" }
        $pkCols.Add("[$($pkReader['ColumnName'])]$desc")
    }
    $pkReader.Close()

    if ($pkCols.Count -gt 0) {
        $colDefs.Add("    CONSTRAINT [PK_$tbl] PRIMARY KEY CLUSTERED (" + ($pkCols -join ", ") + ")")
    }

    $sb.AppendLine("IF NOT EXISTS (SELECT * FROM sys.tables WHERE name = '$tbl')") | Out-Null
    $sb.AppendLine("BEGIN") | Out-Null
    $sb.AppendLine("    CREATE TABLE [dbo].[$tbl] (") | Out-Null
    $sb.AppendLine(($colDefs -join ",`r`n")) | Out-Null
    $sb.AppendLine("    );") | Out-Null
    $sb.AppendLine("    PRINT 'Created table: [$tbl]';") | Out-Null
    $sb.AppendLine("END") | Out-Null
    $sb.AppendLine("GO") | Out-Null
    $sb.AppendLine() | Out-Null
}

$conn.Close()

[System.IO.File]::WriteAllText($OutFile, $sb.ToString(), [System.Text.Encoding]::UTF8)
Write-Host "Generated complete DDL script: $OutFile ($($sb.Length) chars)"
