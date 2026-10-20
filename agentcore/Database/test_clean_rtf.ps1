$sample = "{\rtf1\ansi\ansicpg1252\deff0\deflang1033{\fonttbl{\f0\fnil\fcharset0 MS Sans Serif;}}\viewkind4\uc1\pard\f0\fs17 MS Sans Serif;    dIF NOT EXISTS (SELECT * FROM dbo.sysobjects WHERE id = OBJECT_ID(N'[dbo].[CropsWeighmentsSupplyModes]')) CREATE TABLE ..."

# Function to clean RTF
function Clean-RtfToSql($rtf) {
    # Match the start of actual SQL code
    # Find first occurrence of standard SQL start: IF, CREATE, ALTER, DROP, INSERT, UPDATE, DELETE, /******
    $m = [System.Text.RegularExpressions.Regex]::Match($rtf, "(?i)\b(IF|CREATE|ALTER|DROP|INSERT|UPDATE|DELETE|SET|SELECT|DECLARE|BEGIN|\/\*)\b")
    if ($m.Success) {
        $clean = $rtf.Substring($m.Index)
        # Now remove any RTF formatting
        $clean = [System.Text.RegularExpressions.Regex]::Replace($clean, '\\par[ \r\n]?', "`r`n")
        $clean = [System.Text.RegularExpressions.Regex]::Replace($clean, '\\tab[ ]?', "    ")
        $clean = $clean.Replace("\{", "{").Replace("\}", "}").Replace("\\", "\")
        $clean = [System.Text.RegularExpressions.Regex]::Replace($clean, '\\[a-zA-Z]+(-?\d+)?[ ]?', "")
        $clean = $clean.Replace("{", "").Replace("}", "").Trim()
        return $clean
    }
    return $rtf
}

Write-Host "Cleaned:"
$cleaned = Clean-RtfToSql $sample
$cleaned.Substring(0, [Math]::Min(150, $cleaned.Length))
