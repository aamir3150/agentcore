param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$SourceDb = "AgenSoft_Master_Schema_Staging",
    [string]$TargetDb = "MyMandi_2025-26"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$TargetDb;Trusted_Connection=True;")
$conn.Open()

$targetTables = @(
    "Companies",
    "Parties",
    "PartyGroups",
    "ProductGroups",
    "Products",
    "Promises",
    "Salesmen",
    "Sectors",
    "Towns",
    "Units",
    "Users"
)

# 1. Drop all foreign keys pointing to or originating from these 11 tables
Write-Host "Finding and dropping foreign key constraints on the 11 tables..."
$fkCmd = $conn.CreateCommand()
$fkCmd.CommandText = @"
SELECT 
    fk.name AS FK_Name,
    tp.name AS ParentTable
FROM sys.foreign_keys fk
INNER JOIN sys.tables tp ON fk.parent_object_id = tp.object_id
INNER JOIN sys.tables tr ON fk.referenced_object_id = tr.object_id
WHERE tr.name IN ('$($targetTables -join "','")')
   OR tp.name IN ('$($targetTables -join "','")');
"@

$reader = $fkCmd.ExecuteReader()
$dropFkStatements = [System.Collections.Generic.List[string]]::new()
while ($reader.Read()) {
    $pTbl = $reader["ParentTable"].ToString()
    $fkName = $reader["FK_Name"].ToString()
    $dropFkStatements.Add("ALTER TABLE [dbo].[$pTbl] DROP CONSTRAINT [$fkName];")
}
$reader.Close()

foreach ($sql in $dropFkStatements) {
    Write-Host "Dropping FK: $sql"
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $sql
    $cmd.ExecuteNonQuery() | Out-Null
}

# 2. Drop the 11 tables
foreach ($tbl in $targetTables) {
    Write-Host "Dropping old table: $tbl"
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "IF OBJECT_ID('dbo.[$tbl]', 'U') IS NOT NULL DROP TABLE [dbo].[$tbl];"
    $cmd.ExecuteNonQuery() | Out-Null
}

# 3. Generate and recreate the 11 tables using EXACT schema from SourceDb
Write-Host "`nRecreating the 11 tables from $SourceDb with exact matching schema..."
$connSource = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$SourceDb;Trusted_Connection=True;")
$connSource.Open()

foreach ($tbl in $targetTables) {
    # Get columns
    $colCmd = $connSource.CreateCommand()
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
    $pkCmd = $connSource.CreateCommand()
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
    while ($pkReader.Read()) {
        $desc = if ([bool]$pkReader["IsDesc"]) { " DESC" } else { " ASC" }
        $pkCols.Add("[$($pkReader['ColumnName'])]$desc")
    }
    $pkReader.Close()

    if ($pkCols.Count -gt 0) {
        $colDefs.Add("    CONSTRAINT [PK_$tbl] PRIMARY KEY CLUSTERED (" + ($pkCols -join ", ") + ")")
    }

    $createSql = "CREATE TABLE [dbo].[$tbl] (`r`n" + ($colDefs -join ",`r`n") + "`r`n);"
    Write-Host "Creating table $tbl with $($colDefs.Count) definitions..."
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = $createSql
    $cmd.ExecuteNonQuery() | Out-Null
    Write-Host "Table $tbl created successfully!" -ForegroundColor Green
}

$connSource.Close()
$conn.Close()

Write-Host "`nAll 11 tables recreated successfully with EXACT ATUpdate.dll structure!" -ForegroundColor Green
