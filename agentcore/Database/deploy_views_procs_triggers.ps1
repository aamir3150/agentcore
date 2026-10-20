param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$SourceDb = "AgenSoft_Master_Schema_Staging",
    [string]$TargetDb = "MyMandi_2025-26"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$TargetDb;Trusted_Connection=True;")
$conn.Open()

# 1. Deploy Views
$viewCmd = $conn.CreateCommand()
$viewCmd.CommandText = @"
SELECT 
    v.name,
    m.definition
FROM [$SourceDb].sys.views v
INNER JOIN [$SourceDb].sys.sql_modules m ON v.object_id = m.object_id
WHERE v.is_ms_shipped = 0
ORDER BY v.name;
"@

$vReader = $viewCmd.ExecuteReader()
$viewDefs = [System.Collections.Generic.List[PSCustomObject]]::new()
while ($vReader.Read()) {
    $viewDefs.Add([PSCustomObject]@{
        Name = $vReader["name"].ToString()
        Definition = $vReader["definition"].ToString()
    })
}
$vReader.Close()

Write-Host "Total Views to deploy: $($viewDefs.Count)"
$viewSuccess = 0
foreach ($v in $viewDefs) {
    try {
        # Drop existing view if any
        $dropCmd = $conn.CreateCommand()
        $dropCmd.CommandText = "IF OBJECT_ID('dbo.[$($v.Name)]', 'V') IS NOT NULL DROP VIEW [dbo].[$($v.Name)];"
        $dropCmd.ExecuteNonQuery() | Out-Null

        $createCmd = $conn.CreateCommand()
        $createCmd.CommandText = $v.Definition
        $createCmd.ExecuteNonQuery() | Out-Null
        $viewSuccess++
    } catch {
        Write-Host "Warning on View $($v.Name): $($_.Exception.Message)" -ForegroundColor Yellow
    }
}
Write-Host "Successfully deployed $viewSuccess / $($viewDefs.Count) Views!" -ForegroundColor Green

# 2. Deploy Stored Procedures
$procCmd = $conn.CreateCommand()
$procCmd.CommandText = @"
SELECT 
    p.name,
    m.definition
FROM [$SourceDb].sys.procedures p
INNER JOIN [$SourceDb].sys.sql_modules m ON p.object_id = m.object_id
WHERE p.is_ms_shipped = 0
ORDER BY p.name;
"@

$pReader = $procCmd.ExecuteReader()
$procDefs = [System.Collections.Generic.List[PSCustomObject]]::new()
while ($pReader.Read()) {
    $procDefs.Add([PSCustomObject]@{
        Name = $pReader["name"].ToString()
        Definition = $pReader["definition"].ToString()
    })
}
$pReader.Close()

Write-Host "`nTotal Procedures to deploy: $($procDefs.Count)"
$procSuccess = 0
foreach ($p in $procDefs) {
    try {
        $dropCmd = $conn.CreateCommand()
        $dropCmd.CommandText = "IF OBJECT_ID('dbo.[$($p.Name)]', 'P') IS NOT NULL DROP PROCEDURE [dbo].[$($p.Name)];"
        $dropCmd.ExecuteNonQuery() | Out-Null

        $createCmd = $conn.CreateCommand()
        $createCmd.CommandText = $p.Definition
        $createCmd.ExecuteNonQuery() | Out-Null
        $procSuccess++
    } catch {
        Write-Host "Warning on Procedure $($p.Name): $($_.Exception.Message)" -ForegroundColor Yellow
    }
}
Write-Host "Successfully deployed $procSuccess / $($procDefs.Count) Procedures!" -ForegroundColor Green

# 3. Deploy Triggers
$trigCmd = $conn.CreateCommand()
$trigCmd.CommandText = @"
SELECT 
    t.name,
    m.definition
FROM [$SourceDb].sys.triggers t
INNER JOIN [$SourceDb].sys.sql_modules m ON t.object_id = m.object_id
WHERE t.is_ms_shipped = 0
ORDER BY t.name;
"@

$tReader = $trigCmd.ExecuteReader()
$trigDefs = [System.Collections.Generic.List[PSCustomObject]]::new()
while ($tReader.Read()) {
    $trigDefs.Add([PSCustomObject]@{
        Name = $tReader["name"].ToString()
        Definition = $tReader["definition"].ToString()
    })
}
$tReader.Close()

Write-Host "`nTotal Triggers to deploy: $($trigDefs.Count)"
$trigSuccess = 0
foreach ($t in $trigDefs) {
    try {
        $dropCmd = $conn.CreateCommand()
        $dropCmd.CommandText = "IF OBJECT_ID('dbo.[$($t.Name)]', 'TR') IS NOT NULL DROP TRIGGER [dbo].[$($t.Name)];"
        $dropCmd.ExecuteNonQuery() | Out-Null

        $createCmd = $conn.CreateCommand()
        $createCmd.CommandText = $t.Definition
        $createCmd.ExecuteNonQuery() | Out-Null
        $trigSuccess++
    } catch {
        Write-Host "Warning on Trigger $($t.Name): $($_.Exception.Message)" -ForegroundColor Yellow
    }
}
Write-Host "Successfully deployed $trigSuccess / $($trigDefs.Count) Triggers!" -ForegroundColor Green

$conn.Close()
