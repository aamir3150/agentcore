$ddl = Get-Content 'd:\Projects\agentcore\agentcore\Database\ATUpdate_Full_Extracted_DDL.sql' -Raw

# Find all ALTER TABLE statements
$alterRegex = [regex]'(?is)ALTER\s+TABLE\s+(?:\[?dbo\]?\.)?\[?([a-zA-Z0-9_#]+)\]?\s+(ADD|ALTER\s+COLUMN)\s+([^;\r\n]+)'
$matches = $alterRegex.Matches($ddl)

Write-Host "Total ALTER TABLE statements found: $($matches.Count)"

$tablesAltered = [System.Collections.Generic.Dictionary[string, System.Collections.Generic.List[string]]]::new([System.StringComparer]::OrdinalIgnoreCase)

foreach ($m in $matches) {
    $tbl = $m.Groups[1].Value
    if (!$tablesAltered.ContainsKey($tbl)) {
        $tablesAltered[$tbl] = [System.Collections.Generic.List[string]]::new()
    }
    $tablesAltered[$tbl].Add($m.Value.Trim())
}

Write-Host "Tables with ALTER statements: $($tablesAltered.Count)"
foreach ($k in ($tablesAltered.Keys | Sort-Object)) {
    Write-Host "$k : $($tablesAltered[$k].Count) statements"
}
