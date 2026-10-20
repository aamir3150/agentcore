param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$TargetDb = "MyMandi_2025-26",
    [string]$SqlFile = "d:\Projects\agentcore\agentcore\Database\Create_Missing_Tables_In_MyMandi.sql"
)

Write-Host "Connecting to $TargetDb on $Server..."
$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$TargetDb;Trusted_Connection=True;")
$conn.Open()

$sqlText = [System.IO.File]::ReadAllText($SqlFile)
$batches = [System.Text.RegularExpressions.Regex]::Split($sqlText, '^\s*GO\s*$', [System.Text.RegularExpressions.RegexOptions]::Multiline -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)

Write-Host "Total batches in script: $($batches.Count)"

$successCount = 0
$errorCount = 0
$errors = [System.Collections.Generic.List[string]]::new()

foreach ($batch in $batches) {
    $trimmed = $batch.Trim()
    if ([string]::IsNullOrWhiteSpace($trimmed)) { continue }
    if ($trimmed.StartsWith("USE ", [System.StringComparison]::OrdinalIgnoreCase)) { continue }

    try {
        $cmd = $conn.CreateCommand()
        $cmd.CommandText = $trimmed
        $cmd.CommandTimeout = 60
        $cmd.ExecuteNonQuery() | Out-Null
        $successCount++
    } catch {
        $errorCount++
        $errMsg = "Error executing batch: $($_.Exception.Message)`nBatch: $($trimmed.Substring(0, [Math]::Min(100, $trimmed.Length)))"
        $errors.Add($errMsg)
        Write-Host $errMsg -ForegroundColor Red
    }
}

$conn.Close()

Write-Host "`n=================================================="
Write-Host "Execution Finished!"
Write-Host "Successful batches: $successCount"
Write-Host "Failed batches: $errorCount"
if ($errorCount -eq 0) {
    Write-Host "ALL TABLES CREATED WITH ZERO ERRORS!" -ForegroundColor Green
} else {
    Write-Host "Errors encountered: $($errors.Count)" -ForegroundColor Yellow
}
