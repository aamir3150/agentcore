$ddl = Get-Content 'd:\Projects\agentcore\agentcore\Database\ATUpdate_Full_Extracted_DDL.sql' -Raw

$tableRegex = [regex]'(?is)CREATE\s+TABLE\s+(?:\[?dbo\]?\.)?\[?([a-zA-Z0-9_#]+)\]?\s*\((.*?)\)\s*(?:ON\s+\[PRIMARY\])?'
$matches = $tableRegex.Matches($ddl)

$tables = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::OrdinalIgnoreCase)
foreach ($m in $matches) {
    $tblName = $m.Groups[1].Value
    if (!$tables.ContainsKey($tblName)) {
        $tables[$tblName] = $m.Value
    }
}

Write-Host "Total unique tables matched: $($tables.Count)"
$sorted = $tables.Keys | Sort-Object
$i = 1
foreach ($s in $sorted) {
    Write-Host "$($i.ToString('D2')). $s"
    $i++
}
