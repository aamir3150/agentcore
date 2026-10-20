$conn = New-Object System.Data.SqlClient.SqlConnection("Server=.\SQLEXPRESS;Database=MyMandi_2025-26;Trusted_Connection=True;")
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT name FROM sys.tables WHERE is_ms_shipped = 0 ORDER BY name"
$reader = $cmd.ExecuteReader()
$activeTables = [System.Collections.Generic.List[string]]::new()
while ($reader.Read()) {
    $activeTables.Add($reader["name"].ToString())
}
$reader.Close()
$conn.Close()

Write-Host "Total user tables in MyMandi_2025-26: $($activeTables.Count)"

# Read staging tables
$connStaging = New-Object System.Data.SqlClient.SqlConnection("Server=.\SQLEXPRESS;Database=AgenSoft_Master_Schema_Staging;Trusted_Connection=True;")
$connStaging.Open()
$cmdStaging = $connStaging.CreateCommand()
$cmdStaging.CommandText = "SELECT name FROM sys.tables WHERE is_ms_shipped = 0 ORDER BY name"
$readerStaging = $cmdStaging.ExecuteReader()
$stagingTables = [System.Collections.Generic.List[string]]::new()
while ($readerStaging.Read()) {
    $stagingTables.Add($readerStaging["name"].ToString())
}
$readerStaging.Close()
$connStaging.Close()

$common = [System.Collections.Generic.List[string]]::new()
$missingInActive = [System.Collections.Generic.List[string]]::new()

foreach ($t in $stagingTables) {
    if ($activeTables.Contains($t)) {
        $common.Add($t)
    } else {
        $missingInActive.Add($t)
    }
}

Write-Host "Common tables (already in both): $($common.Count)"
Write-Host "Tables missing in MyMandi_2025-26 that need to be created: $($missingInActive.Count)"
$i = 1
foreach ($m in $missingInActive) {
    Write-Host "$($i.ToString('D2')). $m"
    $i++
}
