$conn = New-Object System.Data.SqlClient.SqlConnection("Server=.\SQLEXPRESS;Database=AgenSoft_Master_Schema_Staging;Trusted_Connection=True;")
$conn.Open()
$cmd = $conn.CreateCommand()
$cmd.CommandText = "SELECT name FROM sys.tables WHERE is_ms_shipped = 0 ORDER BY name"
$reader = $cmd.ExecuteReader()
$stagingTables = [System.Collections.Generic.List[string]]::new()
while ($reader.Read()) {
    $stagingTables.Add($reader["name"].ToString())
}
$reader.Close()
$conn.Close()

Write-Host "Total user tables in AgenSoft_Master_Schema_Staging: $($stagingTables.Count)"
$i = 1
foreach ($t in $stagingTables) {
    Write-Host "$($i.ToString('D2')). $t"
    $i++
}
