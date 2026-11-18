$agenDir = "D:\Projects\3F Technologies\3FT Shortcut\AgenSoft"
$purDll = Join-Path $agenDir "ATPurchaseForms.dll"
$saleDll = Join-Path $agenDir "ATSalesForms.dll"
$brkDll = Join-Path $agenDir "ATBrokrageForms.dll"
$mainExe = Join-Path $agenDir "AgenSoft.exe"

Write-Host "Loading assemblies..."
[System.Reflection.Assembly]::LoadFrom($purDll) | Out-Null
[System.Reflection.Assembly]::LoadFrom($saleDll) | Out-Null
[System.Reflection.Assembly]::LoadFrom($brkDll) | Out-Null
[System.Reflection.Assembly]::LoadFrom($mainExe) | Out-Null

Write-Host "`n=== Forms in ATPurchaseForms.dll ==="
[System.Reflection.Assembly]::LoadFrom($purDll).GetTypes() | Where-Object { $_.BaseType.Name -match "Form" } | ForEach-Object { $_.FullName }

Write-Host "`n=== Forms in ATSalesForms.dll ==="
[System.Reflection.Assembly]::LoadFrom($saleDll).GetTypes() | Where-Object { $_.BaseType.Name -match "Form" } | ForEach-Object { $_.FullName }

Write-Host "`n=== Forms in ATBrokrageForms.dll ==="
[System.Reflection.Assembly]::LoadFrom($brkDll).GetTypes() | Where-Object { $_.BaseType.Name -match "Form" } | ForEach-Object { $_.FullName }

