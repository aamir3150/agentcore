$json = Get-Content 'd:\Projects\agentcore\agentcore\Database\ATUpdate_Tables.json' -Raw | ConvertFrom-Json
$props = $json.psobject.properties.Name | Sort-Object
Write-Host "Total tables extracted: $($props.Count)"
$i = 1
foreach ($p in $props) {
    Write-Host "$($i.ToString('D2')). $p"
    $i++
}
