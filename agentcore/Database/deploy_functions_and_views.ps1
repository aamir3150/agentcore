param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$SourceDb = "AgenSoft_Master_Schema_Staging",
    [string]$TargetDb = "MyMandi_2025-26"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$TargetDb;Trusted_Connection=True;")
$conn.Open()

# 1. Deploy Functions
$fnCmd = $conn.CreateCommand()
$fnCmd.CommandText = @"
SELECT 
    o.name,
    m.definition
FROM [$SourceDb].sys.objects o
INNER JOIN [$SourceDb].sys.sql_modules m ON o.object_id = m.object_id
WHERE o.type IN ('FN', 'IF', 'TF') AND o.is_ms_shipped = 0
ORDER BY o.name;
"@

$fnReader = $fnCmd.ExecuteReader()
$fnDefs = [System.Collections.Generic.List[PSCustomObject]]::new()
while ($fnReader.Read()) {
    $fnDefs.Add([PSCustomObject]@{
        Name = $fnReader["name"].ToString()
        Definition = $fnReader["definition"].ToString()
    })
}
$fnReader.Close()

Write-Host "Total Functions to deploy: $($fnDefs.Count)"
$fnSuccess = 0
foreach ($fn in $fnDefs) {
    try {
        $dropCmd = $conn.CreateCommand()
        $dropCmd.CommandText = "IF OBJECT_ID('dbo.[$($fn.Name)]') IS NOT NULL DROP FUNCTION [dbo].[$($fn.Name)];"
        $dropCmd.ExecuteNonQuery() | Out-Null

        $createCmd = $conn.CreateCommand()
        $createCmd.CommandText = $fn.Definition
        $createCmd.ExecuteNonQuery() | Out-Null
        $fnSuccess++
    } catch {
        Write-Host "Warning on Function $($fn.Name): $($_.Exception.Message)" -ForegroundColor Yellow
    }
}
Write-Host "Successfully deployed $fnSuccess / $($fnDefs.Count) Functions!" -ForegroundColor Green

# 2. Deploy Views
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

Write-Host "`nTotal Views to deploy: $($viewDefs.Count)"
$viewSuccess = 0
# Run twice to resolve view-on-view dependencies
for ($pass = 1; $pass -le 2; $pass++) {
    foreach ($v in $viewDefs) {
        try {
            $dropCmd = $conn.CreateCommand()
            $dropCmd.CommandText = "IF OBJECT_ID('dbo.[$($v.Name)]', 'V') IS NOT NULL DROP VIEW [dbo].[$($v.Name)];"
            $dropCmd.ExecuteNonQuery() | Out-Null

            $createCmd = $conn.CreateCommand()
            $createCmd.CommandText = $v.Definition
            $createCmd.ExecuteNonQuery() | Out-Null
            if ($pass -eq 2) { $viewSuccess++ }
        } catch {
            if ($pass -eq 2) {
                Write-Host "Warning on View $($v.Name): $($_.Exception.Message)" -ForegroundColor Yellow
            }
        }
    }
}
Write-Host "Successfully deployed $viewSuccess / $($viewDefs.Count) Views!" -ForegroundColor Green

$conn.Close()
