[CmdletBinding(SupportsShouldProcess=$true,ConfirmImpact='High')]
param(
    [Parameter(Mandatory=$true)][ValidatePattern('^\d{1,3}(\.\d{1,3}){3}/\d{1,2}$')][string]$ApprovedLanCidr,
    [string]$LanIp='192.168.0.116'
)
$ErrorActionPreference='Stop'
if ($PSCmdlet.ShouldProcess("$LanIp TCP 443 from $ApprovedLanCidr",'Create restricted LabTrack HTTPS firewall rule')) {
    New-NetFirewallRule -DisplayName 'LabTrack HTTPS from approved LAN' -Direction Inbound -Action Allow -Protocol TCP -LocalAddress $LanIp -LocalPort 443 -RemoteAddress $ApprovedLanCidr -Profile Domain,Private | Out-Null
}
Write-Warning 'No HTTP or MariaDB firewall rule is created. Never expose TCP 3306 to workstation networks.'
