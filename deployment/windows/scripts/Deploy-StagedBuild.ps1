[CmdletBinding(SupportsShouldProcess=$true)]
param(
    [string]$Source = (Join-Path $PSScriptRoot '..\..\..\..\artifacts\windows'),
    [string]$Destination = 'C:\LabTrack'
)
$ErrorActionPreference = 'Stop'
$sourcePath = (Resolve-Path $Source).Path
foreach ($required in @('wwwroot','api')) {
    if (-not (Test-Path (Join-Path $sourcePath $required))) { throw "Missing staged folder: $required" }
}
if ($PSCmdlet.ShouldProcess($Destination, 'Copy staged frontend and API files')) {
    New-Item -ItemType Directory -Force -Path (Join-Path $Destination 'wwwroot'),(Join-Path $Destination 'api') | Out-Null
    Copy-Item (Join-Path $sourcePath 'wwwroot\*') (Join-Path $Destination 'wwwroot') -Recurse -Force
    Copy-Item (Join-Path $sourcePath 'api\*') (Join-Path $Destination 'api') -Recurse -Force
}
Write-Warning 'File copy only. No IIS app/binding activation, database operation, service restart, or production deployment authorization is performed.'
