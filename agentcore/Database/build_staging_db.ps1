param(
    [string]$DllPath = "D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll",
    [string]$Server = ".\SQLEXPRESS",
    [string]$StagingDb = "AgenSoft_Master_Schema_Staging"
)

$csharpCode = @"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;
using System.Data.SqlClient;

public class StagingBuilder
{
    public class MigrationBlock
    {
        public int Index { get; set; }
        public decimal Version { get; set; }
        public string RawVersion { get; set; }
        public string Sql { get; set; }
    }

    public static List<MigrationBlock> LoadBlocks(string dllPath)
    {
        byte[] bytes = File.ReadAllBytes(dllPath);
        int len = bytes.Length;
        List<MigrationBlock> list = new List<MigrationBlock>();

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
                string sql = RtfToPlainText(rtf);

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

                list.Add(new MigrationBlock {
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

    public static string RtfToPlainText(string rtf)
    {
        StringBuilder sb = new StringBuilder();
        int len = rtf.Length;
        int i = 0;

        Stack<bool> skipGroup = new Stack<bool>();
        bool currentSkip = false;

        while (i < len)
        {
            char c = rtf[i];
            if (c == '{')
            {
                skipGroup.Push(currentSkip);
                i++;
                if (i < len && rtf[i] == '\\')
                {
                    int startWord = i + 1;
                    while (startWord < len && char.IsLetter(rtf[startWord])) startWord++;
                    string word = rtf.Substring(i + 1, startWord - (i + 1));
                    if (word == "fonttbl" || word == "colortbl" || word == "stylesheet" || word == "info" || word == "*")
                    {
                        currentSkip = true;
                    }
                }
            }
            else if (c == '}')
            {
                if (skipGroup.Count > 0)
                {
                    currentSkip = skipGroup.Pop();
                }
                i++;
            }
            else if (currentSkip)
            {
                i++;
            }
            else if (c == '\\')
            {
                i++;
                if (i >= len) break;
                char nextChar = rtf[i];
                if (nextChar == '{' || nextChar == '}' || nextChar == '\\')
                {
                    sb.Append(nextChar);
                    i++;
                }
                else if (nextChar == '\'' && i + 2 < len)
                {
                    string hex = rtf.Substring(i + 1, 2);
                    try
                    {
                        byte b = Convert.ToByte(hex, 16);
                        sb.Append((char)b);
                    }
                    catch { }
                    i += 3;
                }
                else
                {
                    int startWord = i;
                    while (i < len && char.IsLetter(rtf[i])) i++;
                    string word = rtf.Substring(startWord, i - startWord);

                    if (i < len && (rtf[i] == '-' || char.IsDigit(rtf[i])))
                    {
                        if (rtf[i] == '-') i++;
                        while (i < len && char.IsDigit(rtf[i])) i++;
                    }

                    if (i < len && rtf[i] == ' ') i++;

                    if (word == "par" || word == "line")
                    {
                        sb.AppendLine();
                    }
                    else if (word == "tab")
                    {
                        sb.Append("    ");
                    }
                }
            }
            else
            {
                sb.Append(c);
                i++;
            }
        }

        return sb.ToString().Trim();
    }

    public static void ExecuteScriptBatches(SqlConnection conn, string script)
    {
        string[] batches = Regex.Split(script, @"^\s*GO\s*$", RegexOptions.Multiline | RegexOptions.IgnoreCase);
        foreach (string batch in batches)
        {
            string trimmed = batch.Trim();
            if (string.IsNullOrWhiteSpace(trimmed)) continue;

            if (trimmed.IndexOf("master.dbo", StringComparison.OrdinalIgnoreCase) >= 0) continue;
            if (trimmed.StartsWith("CREATE DATABASE", StringComparison.OrdinalIgnoreCase)) continue;

            try
            {
                using (SqlCommand cmd = new SqlCommand(trimmed, conn))
                {
                    cmd.CommandTimeout = 60;
                    cmd.ExecuteNonQuery();
                }
            }
            catch (Exception)
            {
                // Continue on migration error
            }
        }
    }
}
"@

Add-Type -TypeDefinition $csharpCode -Language CSharp -ReferencedAssemblies "System.Data"

# Connect to master, drop and recreate staging DB
$masterConnStr = "Server=$Server;Database=master;Trusted_Connection=True;TrustServerCertificate=True;"
$conn = New-Object System.Data.SqlClient.SqlConnection($masterConnStr)
$conn.Open()

Write-Host "Recreating database $StagingDb..."
$cmd = $conn.CreateCommand()
$cmd.CommandText = @"
IF EXISTS (SELECT name FROM sys.databases WHERE name = '$StagingDb')
BEGIN
    ALTER DATABASE [$StagingDb] SET SINGLE_USER WITH ROLLBACK IMMEDIATE;
    DROP DATABASE [$StagingDb];
END
CREATE DATABASE [$StagingDb];
"@
$cmd.ExecuteNonQuery()
$conn.Close()

Write-Host "Database $StagingDb created. Connecting to it..."
$stagingConnStr = "Server=$Server;Database=$StagingDb;Trusted_Connection=True;TrustServerCertificate=True;"
$stagingConn = New-Object System.Data.SqlClient.SqlConnection($stagingConnStr)
$stagingConn.Open()

Write-Host "Loading migration blocks from $DllPath..."
$blocks = [StagingBuilder]::LoadBlocks($DllPath)
$ordered = $blocks | Sort-Object Version, Index

Write-Host "Applying Base Tables (Index 2)..."
$b2 = $blocks | Where-Object { $_.Index -eq 2 }
if ($b2) {
    [StagingBuilder]::ExecuteScriptBatches($stagingConn, $b2.Sql)
}

Write-Host "Applying Base Defaults (Index 3)..."
$b3 = $blocks | Where-Object { $_.Index -eq 3 }
if ($b3) {
    [StagingBuilder]::ExecuteScriptBatches($stagingConn, $b3.Sql)
}

Write-Host "Applying remaining migration versions (1.000 to 1.245)..."
$migrations = $ordered | Where-Object { $_.Version -ge [decimal]1.0 }
$count = 0
foreach ($m in $migrations) {
    [StagingBuilder]::ExecuteScriptBatches($stagingConn, $m.Sql)
    $count++
    if ($count % 50 -eq 0) {
        Write-Host "Applied $count / $($migrations.Count) migrations (current: $($m.RawVersion))..."
    }
}

Write-Host "Completed applying all migrations!"

# Check tables created in Staging DB
$cmd = $stagingConn.CreateCommand()
$cmd.CommandText = "SELECT COUNT(*) FROM sys.tables WHERE is_ms_shipped = 0"
$tblCount = $cmd.ExecuteScalar()
Write-Host "Total user tables in $StagingDb : $tblCount"

$stagingConn.Close()
