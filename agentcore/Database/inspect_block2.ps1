$csharpCode = @"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class BlockInspector
{
    public static string GetBlock(string filePath, int targetIndex)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        int len = bytes.Length;
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
                if (idx == targetIndex)
                {
                    int count = j - i;
                    string rtf = Encoding.ASCII.GetString(bytes, i, count);
                    string sql = Regex.Replace(rtf, @"\\par[ \r\n]?", "\r\n");
                    sql = Regex.Replace(sql, @"\\tab[ ]?", "    ");
                    sql = sql.Replace(@"\{", "{").Replace(@"\}", "}").Replace(@"\\", "\\");
                    sql = Regex.Replace(sql, @"\\[a-zA-Z]+(-?\d+)?[ ]?", "");
                    sql = sql.Replace("{", "").Replace("}", "").Trim();
                    return sql;
                }
                idx++;
                i = j - 1;
            }
        }
        return null;
    }
}
"@

Add-Type -TypeDefinition $csharpCode -Language CSharp
$b2 = [BlockInspector]::GetBlock("D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll", 2)
[System.IO.File]::WriteAllText("d:\Projects\agentcore\agentcore\Database\Block2_BaseTables.sql", $b2, [System.Text.Encoding]::UTF8)
Write-Host "Saved Block 2 to Block2_BaseTables.sql ($($b2.Length) chars)"

# Check what tables are created in Block 2
$tableRegex = [regex]'(?is)CREATE\s+TABLE\s+(?:\[?dbo\]?\.)?\[?([a-zA-Z0-9_#]+)\]?'
$matches = $tableRegex.Matches($b2)
Write-Host "Tables created in Block 2: $($matches.Count)"
$tbls = $matches | ForEach-Object { $_.Groups[1].Value } | Select-Object -Unique | Sort-Object
Write-Host "Unique tables in Block 2: $($tbls.Count)"
$tbls -join ', '
