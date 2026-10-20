param(
    [string]$DllPath = "D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll"
)

$csharpCode = @"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class BlockInfo
{
    public int Index { get; set; }
    public string Version { get; set; }
    public int Length { get; set; }
    public string SqlPreview { get; set; }
}

public class BlockAnalyzer
{
    public static List<BlockInfo> Analyze(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        int len = bytes.Length;
        List<BlockInfo> list = new List<BlockInfo>();

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
                string ver = m.Success ? m.Groups[1].Value : "Unknown";

                list.Add(new BlockInfo {
                    Index = idx++,
                    Version = ver,
                    Length = sql.Length,
                    SqlPreview = sql.Length > 100 ? sql.Substring(0, 100).Replace("\r", " ").Replace("\n", " ") : sql
                });

                i = j - 1;
            }
        }
        return list;
    }
}
"@

Add-Type -TypeDefinition $csharpCode -Language CSharp

$blocks = [BlockAnalyzer]::Analyze($DllPath)
Write-Host "Total blocks analyzed: $($blocks.Count)"

$knownVer = $blocks | Where-Object { $_.Version -ne "Unknown" }
Write-Host "Blocks with explicit DBVersion: $($knownVer.Count)"
$unknownVer = $blocks | Where-Object { $_.Version -eq "Unknown" }
Write-Host "Blocks without explicit DBVersion: $($unknownVer.Count)"

Write-Host "`nFirst 10 blocks:"
$blocks | Select-Object -First 10 | Format-Table Index, Version, Length, SqlPreview -AutoSize

Write-Host "`nSample of Unknown version blocks:"
$unknownVer | Select-Object -First 10 | Format-Table Index, Version, Length, SqlPreview -AutoSize
