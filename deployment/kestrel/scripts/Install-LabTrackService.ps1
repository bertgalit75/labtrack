[CmdletBinding(SupportsShouldProcess=$true)]
param(
    [Parameter(Mandatory=$true)][string]$PublishPath,
    [string]$ServiceName='LabTrack',
    [string]$DisplayName='LabTrack Application Service',
    [string]$InstallPath='C:\LabTrack\app',
    [Parameter(Mandatory=$true)][PSCredential]$ServiceCredential,
    [switch]$ConfirmReleaseAuthorization
)
$ErrorActionPreference='Stop'
if (-not $ConfirmReleaseAuthorization) { throw 'Blocked: separate written release approval is required.' }
$marker='C:\LabTrack\release-authorization.txt'
if (-not (Test-Path $marker)) { throw "Blocked: release authorization marker is missing: $marker" }
$src=(Resolve-Path $PublishPath).Path
foreach ($required in @('LabTrack.WebApi.exe','wwwroot\index.html')) {
    if (-not (Test-Path (Join-Path $src $required))) { throw "Required publish item missing: $required" }
}
if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) { throw "Service $ServiceName already exists; use an approved upgrade procedure." }
if ($PSCmdlet.ShouldProcess($InstallPath,'Copy approved publish output and create Windows Service')) {
    New-Item -ItemType Directory -Force -Path $InstallPath | Out-Null
    Copy-Item (Join-Path $src '*') $InstallPath -Recurse -Force
    $binary=Join-Path $InstallPath 'LabTrack.WebApi.exe'
    New-Service -Name $ServiceName -DisplayName $DisplayName -BinaryPathName ('"{0}"' -f $binary) -StartupType Manual -Credential $ServiceCredential -Description 'LabTrack ASP.NET Core Kestrel host'
    Write-Warning 'Service created in Manual startup mode and NOT started. Verify service identity, ACLs, TLS private-key access and secrets.'
}
