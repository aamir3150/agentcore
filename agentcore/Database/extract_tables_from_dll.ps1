param(
    [string]$DllPath = "D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll",
    [string]$OutDir = "d:\Projects\agentcore\agentcore\Database"
)

$csharpCode = @"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class RtfExtractor
{
    public static List<string> ExtractRtfBlocks(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        int len = bytes.Length;
        List<string> blocks = new List<string>();

        // Find "{\rtf"
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
                blocks.Add(rtf);
                i = j - 1;
            }
        }
        return blocks;
    }

    public static string RtfToSql(string rtf)
    {
        // Replace \par with \r\n
        string sql = Regex.Replace(rtf, @"\\par[ \r\n]?", "\r\n");
        // Replace \tab with 4 spaces
        sql = Regex.Replace(sql, @"\\tab[ ]?", "    ");
        // Escaped chars
        sql = sql.Replace(@"\{", "{").Replace(@"\}", "}").Replace(@"\\", "\\");
        // Control words
        sql = Regex.Replace(sql, @"\\[a-zA-Z]+(-?\d+)?[ ]?", "");
        // Remove braces
        sql = sql.Replace("{", "").Replace("}", "");
        return sql;
    }
}
"@

Add-Type -TypeDefinition $csharpCode -Language CSharp

Write-Host "Extracting RTF blocks using C#..."
$sw = [System.Diagnostics.Stopwatch]::StartNew()
$blocks = [RtfExtractor]::ExtractRtfBlocks($DllPath)
$sw.Stop()
Write-Host "Extracted $($blocks.Count) blocks in $($sw.ElapsedMilliseconds) ms."

$allSqlStatements = [System.Collections.Generic.List[string]]::new()
foreach ($b in $blocks) {
    $sql = [RtfExtractor]::RtfToSql($b)
    $allSqlStatements.Add($sql)
}

$fullScript = $allSqlStatements -join "`r`nGO`r`n"
$fullScriptPath = Join-Path $OutDir "ATUpdate_Full_Extracted_DDL.sql"
[System.IO.File]::WriteAllText($fullScriptPath, $fullScript, [System.Text.Encoding]::UTF8)
Write-Host "Full script saved to: $fullScriptPath ($($fullScript.Length) chars)"

# Parse CREATE TABLE
$tableRegex = [regex]'(?is)CREATE\s+TABLE\s+(\[dbo\]\.)?\[?([a-zA-Z0-9_#]+)\]?\s*\((.*?)\)\s*(ON\s+\[PRIMARY\])?'
$tablesDict = [System.Collections.Generic.Dictionary[string, string]]::new([System.StringComparer]::OrdinalIgnoreCase)

$matches = $tableRegex.Matches($fullScript)
foreach ($m in $matches) {
    $tblName = $m.Groups[2].Value
    if (!$tablesDict.ContainsKey($tblName)) {
        $tablesDict[$tblName] = $m.Value.Trim()
    }
}

Write-Host "Total unique CREATE TABLE found: $($tablesDict.Count)"

$tablesJson = Join-Path $OutDir "ATUpdate_Tables.json"
$tablesDict | ConvertTo-Json -Depth 2 | Set-Content -Path $tablesJson -Encoding UTF8
Write-Host "Saved table definitions to $tablesJson"
