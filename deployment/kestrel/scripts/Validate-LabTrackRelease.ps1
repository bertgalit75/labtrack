[CmdletBinding()]
param([string]$PublishPath = (Join-Path $PSScriptRoot '..\..\..\artifacts\kestrel\publish'))
$ErrorActionPreference = 'Stop'
$required = @('LabTrack.WebApi.exe','LabTrack.WebApi.dll','wwwroot\index.html')
$missing = @($required | Where-Object { -not (Test-Path (Join-Path $PublishPath $_)) })
if ($missing.Count -gt 0) { throw "Release package incomplete. Missing: $($missing -join ', ')" }
if (Test-Path (Join-Path $PublishPath 'appsettings.Development.json')) { throw 'Development configuration must not be included in release output.' }
$patterns = @('(?i)Password\s*=\s*[^;\s]+','(?i)"(Password|ClientSecret|ApiKey)"\s*:\s*"[^"]+"')
foreach ($file in Get-ChildItem $PublishPath -Recurse -File -Include '*.json','*.config') {
    $content = Get-Content $file.FullName -Raw
    foreach ($pattern in $patterns) {
        if ($content -match $pattern) { throw "Potential secret detected in release configuration: $($file.FullName)" }
    }
}
Write-Host 'Release structure and basic configuration scan passed. This is not a full secret scan or production approval.'
