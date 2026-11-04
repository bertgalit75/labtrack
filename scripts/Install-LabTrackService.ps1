[CmdletBinding(SupportsShouldProcess=$true)]
param(
    [Parameter(Mandatory=$true)][string]$PublishPath,
    [string]$ServiceName = 'LabTrack',
    [string]$DisplayName = 'LabTrack Application Service',
    [string]$InstallPath = 'C:\LabTrack\app',
    [Parameter(Mandatory=$true)][PSCredential]$ServiceCredential,
    [switch]$ConfirmReleaseAuthorization
)
$ErrorActionPreference = 'Stop'
if (-not $ConfirmReleaseAuthorization) { throw 'Blocked: pass -ConfirmReleaseAuthorization only after separate written production release approval.' }
$marker = 'C:\LabTrack\release-authorization.txt'
if (-not (Test-Path $marker)) { throw "Blocked: approved release marker is missing: $marker" }
$src = (Resolve-Path $PublishPath).Path
$exe = Get-ChildItem $src -Filter 'LabTrack.WebApi.exe' -File | Select-Object -First 1
if (-not $exe) { throw 'Expected LabTrack.WebApi.exe not found. Confirm apphost publish output and project assembly name.' }
if (-not (Test-Path (Join-Path $src 'wwwroot\index.html'))) { throw 'React wwwroot/index.html missing from publish output.' }
if (-not (Test-Path 'C:\LabTrack\certs\labtrack.pfx')) { throw 'TLS PFX missing at C:\LabTrack\certs\labtrack.pfx.' }
if (-not [Environment]::GetEnvironmentVariable('Kestrel__Endpoints__Https__Certificate__Password','Machine')) { throw 'Machine certificate password variable is not configured. Use approved secret handling.' }
if (Get-Service -Name $ServiceName -ErrorAction SilentlyContinue) { throw "Service $ServiceName already exists. Use an approved upgrade/rollback procedure." }
if ($PSCmdlet.ShouldProcess($InstallPath, 'Copy approved publish output and create Windows Service')) {
    New-Item -ItemType Directory -Force -Path $InstallPath | Out-Null
    Copy-Item (Join-Path $src '*') $InstallPath -Recurse -Force
    $binary = Join-Path $InstallPath $exe.Name
    New-Service -Name $ServiceName -DisplayName $DisplayName -BinaryPathName "`"$binary`"" -StartupType Automatic -Credential $ServiceCredential -Description 'LabTrack ASP.NET Core Kestrel host'
    Write-Warning 'Service created but NOT started. Verify the supplied dedicated service account, ACLs, certificate private-key permissions, environment variables, logs and release approval before starting.'
}
