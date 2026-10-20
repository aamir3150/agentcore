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

$modernTables = [System.Collections.Generic.List[string]]::new()
foreach ($t in $activeTables) {
    if (!$stagingTables.Contains($t)) {
        $modernTables.Add($t)
    }
}

Write-Host "Modern tables in MyMandi_2025-26 to preserve untouched: $($modernTables.Count)"
$i = 1
foreach ($m in $modernTables) {
    Write-Host "$($i.ToString('D2')). $m"
    $i++
}
