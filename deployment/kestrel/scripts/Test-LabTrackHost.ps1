[CmdletBinding()]
param([string]$PublishPath = (Join-Path $PSScriptRoot '..\..\..\..\artifacts\kestrel\publish'))
$ErrorActionPreference = 'Stop'
Write-Host 'Installed .NET runtimes:'
dotnet --list-runtimes
if ($LASTEXITCODE -ne 0) { throw '.NET runtime inspection failed.' }
$checks = @(
    [pscustomobject]@{ Check='Publish directory'; Passed=(Test-Path $PublishPath) },
    [pscustomobject]@{ Check='API executable'; Passed=(Test-Path (Join-Path $PublishPath 'LabTrack.WebApi.exe')) },
    [pscustomobject]@{ Check='React index'; Passed=(Test-Path (Join-Path $PublishPath 'wwwroot\index.html')) }
)
$checks | Format-Table -AutoSize
if ($checks | Where-Object { -not $_.Passed }) { throw 'One or more host/publish checks failed.' }
Write-Host 'Read-only inspection complete. No service, firewall, certificate, or database changes were made.'
