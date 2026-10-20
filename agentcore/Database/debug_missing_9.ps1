param(
    [string]$DllPath = "D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll"
)

$csharpCode = @"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class BlockItem2
{
    public int Index { get; set; }
    public decimal Version { get; set; }
    public string RawVersion { get; set; }
    public string Sql { get; set; }
}

public class MigrationParser2
{
    public static List<BlockItem2> ParseBlocks(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        int len = bytes.Length;
        List<BlockItem2> list = new List<BlockItem2>();

        int idx = 0;
        for (int i = 0; i < len - 5; i++)
        {
            if (bytes[i] == 0x7B && bytes[i+1] == 0x5C && bytes[i+2] == 0x72 && bytes[i+3] == 0x74 && bytes[i+4] == 0x66)
            {
                int depth = 1;
                int j = i + 5;
                while (j < len && depth > 0)
                {
                    if (bytes[j] == 0x7B) depth++;
                    else if (bytes[j] == 0x7D) depth--;
                    j++;
                }
                int count = j - i;
                string rtf = Encoding.ASCII.GetString(bytes, i, count);

                string sql = Regex.Replace(rtf, @"\\par[ \r\n]?", "\r\n");
                sql = Regex.Replace(sql, @"\\tab[ ]?", "    ");
                sql = sql.Replace(@"\{", "{").Replace(@"\}", "}").Replace(@"\\", "\\");
                sql = Regex.Replace(sql, @"\\[a-zA-Z]+(-?\d+)?[ ]?", "");
                sql = sql.Replace("{", "").Replace("}", "").Trim();

                Match m = Regex.Match(sql, @"(?:SET\s+Value\s*=\s*'|VALUES\s*\([^,]+,[^,]+,[^,]+,')(\d+\.\d{3})'");
                decimal ver = 0;
                string rawVer = "0.000";
                if (m.Success)
                {
                    rawVer = m.Groups[1].Value;
                    decimal.TryParse(rawVer, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out ver);
                }

                list.Add(new BlockItem2 {
                    Index = idx++,
                    Version = ver,
                    RawVersion = rawVer,
                    Sql = sql
                });

                i = j - 1;
            }
        }
        return list;
    }
}
"@

Add-Type -TypeDefinition $csharpCode -Language CSharp
$blocks = [MigrationParser2]::ParseBlocks($DllPath)

$targetTables = @(
    "BrokerageGatePass",
    "BrokrageShortage",
    "CropsWeighmentsSupplyModes",
    "ExpenseType",
    "Insurance",
    "MultiBody",
    "OpeningBatchPestro",
    "PartyGroups",
    "ZNarration"
)

$conn = New-Object System.Data.SqlClient.SqlConnection("Server=.\SQLEXPRESS;Database=AgenSoft_Master_Schema_Staging;Trusted_Connection=True;")
$conn.Open()

foreach ($tbl in $targetTables) {
    Write-Host "`n=================================================="
    Write-Host "Searching for CREATE TABLE $tbl..."
    $found = $false
    foreach ($b in $blocks) {
        if ($b.Sql -match "(?is)CREATE\s+TABLE\s+(?:\[?dbo\]?\.)?\[?$tbl\]?\s*\(") {
            $found = $true
            Write-Host "Found in Block Index: $($b.Index), Version: $($b.RawVersion), Length: $($b.Sql.Length)"
            
            $batches = [System.Text.RegularExpressions.Regex]::Split($b.Sql, '^\s*GO\s*$', [System.Text.RegularExpressions.RegexOptions]::Multiline -bor [System.Text.RegularExpressions.RegexOptions]::IgnoreCase)
            foreach ($batch in $batches) {
                if ($batch -match "(?is)CREATE\s+TABLE\s+(?:\[?dbo\]?\.)?\[?$tbl\]?\s*\(") {
                    Write-Host "Batch preview: $($batch.Substring(0, [Math]::Min(120, $batch.Length)).Replace("`r", " ").Replace("`n", " "))"
                    try {
                        $cmd = $conn.CreateCommand()
                        $cmd.CommandText = $batch
                        $cmd.ExecuteNonQuery()
                        Write-Host "SUCCESS: Executed batch for $tbl!"
                    } catch {
                        Write-Host "ERROR executing batch for $tbl : $($_.Exception.Message)"
                    }
                }
            }
        }
    }
    if (!$found) {
        Write-Host "WARNING: No CREATE TABLE statement found for $tbl in any block!"
    }
}

$conn.Close()
