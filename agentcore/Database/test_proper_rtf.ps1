$csharpCode = @"
using System;
using System.IO;
using System.Collections.Generic;
using System.Text;
using System.Text.RegularExpressions;

public class ProperRtfParser
{
    public static List<string> ExtractRtfBlocks(string filePath)
    {
        byte[] bytes = File.ReadAllBytes(filePath);
        int len = bytes.Length;
        List<string> blocks = new List<string>();

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
}
"@

Add-Type -TypeDefinition $csharpCode -Language CSharp -ReferencedAssemblies "System.Data"

$blocks = [ProperRtfParser]::ExtractRtfBlocks("D:\Projects\3F Technologies\3FT Shortcut\AgenSoft\ATUpdate.dll")
Write-Host "Total RTF blocks: $($blocks.Count)"

foreach ($idx in @(43, 139, 248)) {
    Write-Host "`n--- Testing Block $idx ---"
    $plain = [ProperRtfParser]::RtfToPlainText($blocks[$idx])
    Write-Host "Length: $($plain.Length)"
    Write-Host "Preview (first 150 chars):"
    Write-Host ($plain.Substring(0, [Math]::Min(150, $plain.Length)))
}
