[CmdletBinding(SupportsShouldProcess=$true)]
param(
    [Parameter(Mandatory=$true)][string]$ApprovedLanCidr,
    [string]$LanIp = '192.168.0.116',
    [switch]$AllowHttpRedirect
)
$ErrorActionPreference = 'Stop'
if ($ApprovedLanCidr -notmatch '^\d{1,3}(\.\d{1,3}){3}/\d{1,2}$') { throw 'Supply the network administrator-approved IPv4 CIDR, e.g. 192.168.0.0/24.' }
if ($PSCmdlet.ShouldProcess("$LanIp TCP 443 from $ApprovedLanCidr", 'Create restricted inbound firewall rule')) {
    New-NetFirewallRule -DisplayName 'LabTrack HTTPS from approved LAN' -Direction Inbound -Action Allow -Protocol TCP -LocalAddress $LanIp -LocalPort 443 -RemoteAddress $ApprovedLanCidr -Profile Domain,Private | Out-Null
    if ($AllowHttpRedirect) {
        New-NetFirewallRule -DisplayName 'LabTrack HTTP redirect from approved LAN' -Direction Inbound -Action Allow -Protocol TCP -LocalAddress $LanIp -LocalPort 80 -RemoteAddress $ApprovedLanCidr -Profile Domain,Private | Out-Null
    }
}
Write-Warning 'This script does not open MariaDB TCP 3306. Keep DB access local-only.'
