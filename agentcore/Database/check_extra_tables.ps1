$ddl = Get-Content 'd:\Projects\agentcore\agentcore\Database\ATUpdate_Full_Extracted_DDL.sql' -Raw

$testTables = @("ComRecords", "ComSettings", "ExpenseType")
foreach ($t in $testTables) {
    if ($ddl -match "(?i)CREATE\s+TABLE\s+[^`r`n]*$t") {
        Write-Host "Match for $t : $($Matches[0])"
    } else {
        Write-Host "No direct CREATE TABLE match for $t"
        # Check where $t appears
        $matchesAny = [regex]::Matches($ddl, "(?i)[^\r\n]*$t[^\r\n]*")
        Write-Host "Total lines containing $t : $($matchesAny.Count)"
        if ($matchesAny.Count -gt 0) {
            Write-Host "Sample line: $($matchesAny[0].Value)"
        }
    }
}
