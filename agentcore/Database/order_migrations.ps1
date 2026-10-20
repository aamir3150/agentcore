$csharpCode = @"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class BlockItem
{
    public int Index { get; set; }
    public decimal Version { get; set; }
    public string RawVersion { get; set; }
    public string Sql { get; set; }
}

public class MigrationParser
{
    public static List<BlockItem> ParseBlocks(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        int len = bytes.Length;
        List<BlockItem> list = new List<BlockItem>();

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
                else if (idx == 2)
                {
                    rawVer = "0.001-BaseTables";
                    ver = 0.001m;
                }
                else if (idx == 3)
                {
                    rawVer = "0.002-BaseDefaults";
                    ver = 0.002m;
                }

                list.Add(new BlockItem {
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
$items = [MigrationParser]::ParseBlocks("D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll")
Write-Host "Total blocks parsed: $($items.Count)"

$ordered = $items | Sort-Object Version, Index
Write-Host "Ordered range: $($ordered[0].RawVersion) to $($ordered[-1].RawVersion)"

# Save order summary to inspect
$summary = $ordered | Select-Object Index, RawVersion, Version, @{Name="Length"; Expression={$_.Sql.Length}}
$summary | Select-Object -First 25 | Format-Table -AutoSize
$summary | Select-Object -Last 15 | Format-Table -AutoSize
