$connStr = "Server=.\SQLEXPRESS;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($connStr)
try {
    $conn.Open()
    Write-Host "Successfully connected to SQL Server: $($conn.ServerVersion)"
    
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = "SELECT name FROM sys.databases WHERE name IN ('master', 'MyMandi_2025-26')"
    $reader = $cmd.ExecuteReader()
    while ($reader.Read()) {
        Write-Host "Database found: $($reader['name'])"
    }
    $reader.Close()
} catch {
    Write-Host "Connection error: $_"
} finally {
    if ($conn.State -eq [System.Data.ConnectionState]::Open) {
        $conn.Close()
    }
}
