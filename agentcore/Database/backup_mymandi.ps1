param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$TargetDb = "MyMandi_2025-26",
    [string]$BackupPath = "d:\Projects\agentcore\agentcore\Database\MyMandi_2025-26_PreOverwrite.bak"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=master;Trusted_Connection=True;")
$conn.Open()

Write-Host "Creating full database backup of $TargetDb..."
$cmd = $conn.CreateCommand()
$cmd.CommandText = "BACKUP DATABASE [$TargetDb] TO DISK = '$BackupPath' WITH FORMAT, INIT;"
$cmd.CommandTimeout = 120
$cmd.ExecuteNonQuery()

Write-Host "Backup created successfully at: $BackupPath"
$conn.Close()
