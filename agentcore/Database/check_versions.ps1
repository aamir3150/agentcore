$ddl = Get-Content 'd:\Projects\agentcore\agentcore\Database\ATUpdate_Full_Extracted_DDL.sql' -Raw

# Check for version indicators like 'Version' or '1.' or comments
$lines = $ddl -split "`r?`n"
$versionLines = $lines | Where-Object { $_ -match '(?i)version|\b1\.\d{3}\b' }
Write-Host "Lines mentioning version: $($versionLines.Count)"
$versionLines | Select-Object -First 30
