[CmdletBinding(SupportsShouldProcess=$true)]
param(
    [string]$SiteName = 'LabTrack',
    [string]$AppPoolName = 'LabTrackApiPool',
    [string]$RootPath = 'C:\LabTrack',
    [string]$LanIp = '192.168.0.116'
)
$ErrorActionPreference = 'Stop'
Import-Module WebAdministration
$features = @('IIS-WebServerRole','IIS-WebServer','IIS-CommonHttpFeatures','IIS-StaticContent','IIS-DefaultDocument','IIS-HttpErrors','IIS-RequestFiltering','IIS-WebServerManagementTools','IIS-ManagementConsole')
foreach ($feature in $features) {
    $state = Get-WindowsOptionalFeature -Online -FeatureName $feature -ErrorAction SilentlyContinue
    if ($state -and $state.State -ne 'Enabled') {
        if ($PSCmdlet.ShouldProcess($feature, 'Enable Windows optional feature')) { Enable-WindowsOptionalFeature -Online -FeatureName $feature -All -NoRestart | Out-Null }
    }
}
$www = Join-Path $RootPath 'wwwroot'
$api = Join-Path $RootPath 'api'
$logs = Join-Path $RootPath 'logs'
$backups = Join-Path $RootPath 'backups'
foreach ($path in @($www,$api,$logs,$backups)) { if ($PSCmdlet.ShouldProcess($path, 'Create directory')) { New-Item -ItemType Directory -Force -Path $path | Out-Null } }
if (-not (Test-Path "IIS:\AppPools\$AppPoolName")) {
    if ($PSCmdlet.ShouldProcess($AppPoolName, 'Create IIS application pool')) { New-WebAppPool -Name $AppPoolName | Out-Null }
}
Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name managedRuntimeVersion -Value ''
Set-ItemProperty "IIS:\AppPools\$AppPoolName" -Name enable32BitAppOnWin64 -Value $false
if (-not (Test-Path "IIS:\Sites\$SiteName")) {
    if ($PSCmdlet.ShouldProcess($SiteName, 'Create IIS site skeleton')) {
        New-Website -Name $SiteName -PhysicalPath $www -IPAddress $LanIp -Port 80 -HostHeader 'labtrack.invalid' -ApplicationPool 'DefaultAppPool' | Out-Null
    }
}
Write-Warning 'This creates an HTTP placeholder binding only. Replace labtrack.invalid with the approved internal DNS name and configure a trusted HTTPS certificate before use.'
Write-Warning 'Do not create the /api child application until the API passes provider qualification and receives release authorization.'
