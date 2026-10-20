param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$MasterDb = "AgenSoft_Master_Schema_Staging",
    [string]$TargetDb = "MyMandi_2025-26"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$TargetDb;Trusted_Connection=True;")
$conn.Open()

# 1. Total tables in TargetDb
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT name FROM sys.tables WHERE is_ms_shipped = 0 ORDER BY name"
$reader = $cmd.ExecuteReader()
$targetTables = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
while ($reader.Read()) {
    $targetTables.Add($reader["name"].ToString()) | Out-Null
}
$reader.Close()

Write-Host "=================================================="
Write-Host "DATABASE AUDIT & CROSS-CHECK REPORT"
Write-Host "Target Database: $TargetDb"
Write-Host "Total user tables in target database: $($targetTables.Count)"
Write-Host "=================================================="

# 2. Total tables in MasterDb
$cmdMaster = $conn.CreateCommand()
$cmdMaster.CommandText = "SELECT name FROM [$MasterDb].sys.tables WHERE is_ms_shipped = 0 ORDER BY name"
$readerMaster = $cmdMaster.ExecuteReader()
$masterTables = [System.Collections.Generic.List[string]]::new()
while ($readerMaster.Read()) {
    $masterTables.Add($readerMaster["name"].ToString())
}
$readerMaster.Close()

Write-Host "Total master tables in AgenSoft specification: $($masterTables.Count)"

# 3. Check for any missing tables
$missingFromTarget = [System.Collections.Generic.List[string]]::new()
foreach ($m in $masterTables) {
    if (!$targetTables.Contains($m)) {
        $missingFromTarget.Add($m)
    }
}

if ($missingFromTarget.Count -eq 0) {
    Write-Host "`n[PASS] ZERO MISSING TABLES! All $($masterTables.Count) master tables exist in $TargetDb." -ForegroundColor Green
} else {
    Write-Host "`n[FAIL] Missing tables count: $($missingFromTarget.Count)" -ForegroundColor Red
    foreach ($mis in $missingFromTarget) {
        Write-Host " - $mis"
    }
}

# 4. Detailed column and structure validation for created tables
Write-Host "`n=================================================="
Write-Host "STRUCTURE & COLUMN AUDIT FOR ALL MASTER TABLES"
Write-Host "=================================================="

$structureReport = [System.Collections.Generic.List[PSCustomObject]]::new()
$hasIssues = $false

foreach ($tbl in $masterTables) {
    # Compare columns
    $colCmd = $conn.CreateCommand()
    $colCmd.CommandText = @"
    SELECT 
        m.name AS ColName,
        tp_m.name AS MasterType,
        m.max_length AS MasterLen,
        m.precision AS MasterPrec,
        m.scale AS MasterScale,
        m.is_nullable AS MasterNull,
        t.name AS TargetCol,
        tp_t.name AS TargetType,
        t.max_length AS TargetLen,
        t.is_nullable AS TargetNull
    FROM [$MasterDb].sys.columns m
    INNER JOIN [$MasterDb].sys.types tp_m ON m.user_type_id = tp_m.user_type_id
    LEFT JOIN [$TargetDb].sys.columns t ON t.object_id = OBJECT_ID('$TargetDb.dbo.[$tbl]') AND m.name = t.name
    LEFT JOIN [$TargetDb].sys.types tp_t ON t.user_type_id = tp_t.user_type_id
    WHERE m.object_id = OBJECT_ID('$MasterDb.dbo.[$tbl]')
    ORDER BY m.column_id;
"@
    $colReader = $colCmd.ExecuteReader()
    $totalCols = 0
    $matchedCols = 0
    $unmatchedCols = [System.Collections.Generic.List[string]]::new()

    while ($colReader.Read()) {
        $totalCols++
        $cName = $colReader["ColName"].ToString()
        if ($colReader["TargetCol"] -ne [DBNull]::Value) {
            $matchedCols++
        } else {
            $unmatchedCols.Add($cName)
        }
    }
    $colReader.Close()

    # Check Primary Key
    $pkCmd = $conn.CreateCommand()
    $pkCmd.CommandText = "SELECT COUNT(*) FROM [$TargetDb].sys.key_constraints WHERE parent_object_id = OBJECT_ID('$TargetDb.dbo.[$tbl]') AND type = 'PK'"
    $pkCount = [int]$pkCmd.ExecuteScalar()

    $status = if ($unmatchedCols.Count -eq 0) { "OK" } else { "MISMATCH" }
    if ($status -ne "OK") { $hasIssues = $true }

    $structureReport.Add([PSCustomObject]@{
        Table = $tbl
        MasterColumns = $totalCols
        TargetMatched = $matchedCols
        Status = $status
        HasPK = ($pkCount -gt 0)
        MissingCols = ($unmatchedCols -join ", ")
    })
}

$conn.Close()

Write-Host "Sample of Structure Audit Results (First 30 tables):"
$structureReport | Select-Object -First 30 | Format-Table Table, MasterColumns, TargetMatched, HasPK, Status -AutoSize

$issuesList = $structureReport | Where-Object { $_.Status -ne "OK" }
if ($issuesList.Count -eq 0) {
    Write-Host "`n[PASS] ALL $($masterTables.Count) TABLES HAVE 100% IDENTICAL COLUMN STRUCTURE!" -ForegroundColor Green
} else {
    Write-Host "`n[FAIL] Found structure issues in $($issuesList.Count) tables:" -ForegroundColor Yellow
    $issuesList | Format-Table Table, MasterColumns, TargetMatched, MissingCols -AutoSize
}

Write-Host "`n=================================================="
Write-Host "FINAL SUMMARY:"
Write-Host " - Master Tables Specified: $($masterTables.Count)"
Write-Host " - Master Tables Present in MyMandi_2025-26: $($masterTables.Count - $missingFromTarget.Count) / $($masterTables.Count)"
Write-Host " - Total Tables Now in MyMandi_2025-26: $($targetTables.Count)"
Write-Host "=================================================="
