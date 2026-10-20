param(
    [string]$Server = ".\SQLEXPRESS",
    [string]$SourceDb = "AgenSoft_Master_Schema_Staging",
    [string]$TargetDb = "MyMandi_2025-26"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=$Server;Database=$SourceDb;Trusted_Connection=True;")
$conn.Open()

$commonTables = @("Companies", "Parties", "PartyGroups", "ProductGroups", "Products", "Promises", "Salesmen", "Sectors", "Towns", "Units", "Users")

$sb = [System.Text.StringBuilder]::new()
$sb.AppendLine("-- Missing columns in common tables:")

foreach ($tbl in $commonTables) {
    $cmd = $conn.CreateCommand()
    $cmd.CommandText = @"
    SELECT 
        c.name AS ColumnName,
        tp.name AS TypeName,
        c.max_length AS MaxLength,
        c.precision AS [Precision],
        c.scale AS Scale,
        c.is_nullable AS IsNullable
    FROM [$SourceDb].sys.columns c
    INNER JOIN [$SourceDb].sys.types tp ON c.user_type_id = tp.user_type_id
    WHERE c.object_id = OBJECT_ID('$SourceDb.dbo.[$tbl]')
      AND c.name NOT IN (
          SELECT name FROM [$TargetDb].sys.columns WHERE object_id = OBJECT_ID('$TargetDb.dbo.[$tbl]')
      )
    ORDER BY c.column_id;
"@
    $reader = $cmd.ExecuteReader()
    $missingCols = [System.Collections.Generic.List[string]]::new()
    while ($reader.Read()) {
        $cName = $reader["ColumnName"].ToString()
        $tName = $reader["TypeName"].ToString()
        $maxLen = [int]$reader["MaxLength"]
        $prec = [int]$reader["Precision"]
        $scale = [int]$reader["Scale"]
        $isNullable = [bool]$reader["IsNullable"]

        $typeStr = $tName
        if ($tName -in @("varchar", "nvarchar", "char", "nchar", "varbinary", "binary")) {
            if ($maxLen -eq -1) {
                $typeStr = "$tName(max)"
            } else {
                $lenActual = if ($tName -in @("nvarchar", "nchar")) { $maxLen / 2 } else { $maxLen }
                $typeStr = "$tName($lenActual)"
            }
        } elseif ($tName -in @("decimal", "numeric")) {
            $typeStr = "$tName($prec, $scale)"
        }

        $nullStr = if ($isNullable) { "NULL" } else { "NOT NULL" }
        $missingCols.Add("ALTER TABLE [dbo].[$tbl] ADD [$cName] $typeStr $nullStr;")
    }
    $reader.Close()

    if ($missingCols.Count -gt 0) {
        Write-Host "Table $tbl has $($missingCols.Count) missing columns in MyMandi_2025-26!"
        foreach ($mc in $missingCols) {
            $sb.AppendLine($mc)
            Write-Host "   + $mc"
        }
    } else {
        Write-Host "Table $tbl has 0 missing columns (all columns match)."
    }
}

$conn.Close()

$outFile = "d:\Projects\agentcore\agentcore\Database\Add_Missing_Columns_To_Common_Tables.sql"
[System.IO.File]::WriteAllText($outFile, $sb.ToString(), [System.Text.Encoding]::UTF8)
Write-Host "Saved missing column script to: $outFile"
