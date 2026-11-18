$connStr = 'Server=.\SQLEXPRESS;Database=MyMandi_2025-26;Integrated Security=True;TrustServerCertificate=True;'
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
$conn.Open()

$tables = @('Purchase', 'PurchaseBody', 'Sales', 'SalesBody')
foreach ($tbl in $tables) {
    Write-Host "=== Table: $tbl ==="
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT COLUMN_NAME, DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '$tbl' ORDER BY ORDINAL_POSITION"
    $reader = $cmd.ExecuteReader()
    $cols = @()
    while ($reader.Read()) {
        $cols += $reader[0] + " (" + $reader[1] + ")"
    }
    $reader.Close()
    Write-Host ($cols -join ", ")
}

$conn.Close()

