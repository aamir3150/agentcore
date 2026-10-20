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

Write-Host "Tables in AgenSoft_Master_Schema_Staging: $($stagingTables.Count)"

# Read the 89 tables list from test_improved_regex
$ddl = Get-Content 'd:\Projects\agentcore\agentcore\Database\ATUpdate_Full_Extracted_DDL.sql' -Raw
$tableRegex = [regex]'(?is)CREATE\s+TABLE\s+(?:\[?dbo\]?\.)?\[?([a-zA-Z0-9_#]+)\]?'
$matches = $tableRegex.Matches($ddl)
$regexTables = [System.Collections.Generic.HashSet[string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($m in $matches) {
    $tbl = $m.Groups[1].Value
    if ($tbl -notmatch '^#') {
        $regexTables.Add($tbl) | Out-Null
    }
}

Write-Host "Total non-temp tables in regex: $($regexTables.Count)"

# Check which ones are in regexTables but NOT in stagingTables
$missingInStaging = [System.Collections.Generic.List[string]]::new()
foreach ($t in ($regexTables | Sort-Object)) {
    if (!$stagingTables.Contains($t)) {
        $missingInStaging.Add($t)
    }
}

Write-Host "Tables in regex but missing in Staging DB: $($missingInStaging.Count)"
foreach ($m in $missingInStaging) {
    Write-Host " - $m"
}
