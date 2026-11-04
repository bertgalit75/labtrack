# LabTrack Kestrel deployment (Windows-first)

Selected architecture: ASP.NET Core Kestrel hosted as a Windows Service, serving the React/Vite build from the same application origin.

Target development/test host: Windows 10 Pro, LAN address 192.168.0.116. Ordinary Windows 10 reached end of support on 2025-10-14; do not treat it as a production PHI server without a qualifying ESU/LTSC lifecycle and documented security plan. Prefer supported Windows 11 Pro or Windows Server for production.

## Safety boundary

Preparation only. The API startup restriction remains: only Development plus Persistence:Mode=DevelopmentInMemory is currently permitted. MariaDB 10.11 provider and mappings are not qualified. This work does not authorize production startup, service installation/start, firewall changes, database connection, SQL, migrations, seeding, real patient data, or deployment to 192.168.0.116.

Approved SDD-03, SDD-09, SDD-11 and SDD-12 baselines remain authoritative. Design approvals do not constitute implementation verification, UAT, or production authorization.

## Build

Requirements: .NET 10 SDK, Node.js compatible with Vite 7, npm, and PowerShell.

From repository root:

    .\deployment\kestrel\scripts\Build-LabTrack.ps1

The build script requires the committed frontend package-lock.json, uses npm ci, and stages output under artifacts\kestrel\publish. React dist is copied to publish wwwroot. No database, service, firewall, SQL, migration, or deployment action is performed.

## Hosting behavior

The API project enables Windows Service lifetime. Static files are served from wwwroot. Unknown /api routes return 404 rather than the SPA document; non-API client routes can fall back to index.html. Existing controller routes use /api/v1.

Use same-origin relative API URLs from React. Verify the frontend API client configuration before release.

## Windows host preparation

- Use a dedicated low-privilege service identity; never LocalSystem.
- Keep application, logs and data in separate directories with least-privilege ACLs.
- Obtain a certificate from a trusted internal CA. SAN must match the internal DNS name, or the IP if clients use IP and the certificate includes its IP SAN.
- Keep PFX passwords and database credentials outside source control using approved secret management.
- Keep MariaDB loopback-only when co-located. Never open TCP 3306 to workstation networks.
- Allow inbound TCP 443 only from the network administrator-approved lab subnet. No public exposure or router port forwarding.
- Run Test-LabTrackHost.ps1 and Validate-LabTrackRelease.ps1 for read-only checks.

See config/appsettings.Production.example.json for a secret-free Kestrel HTTPS example.

## Gated service installation

Do not run Install-LabTrackService.ps1 until provider qualification, SDD-09 authorization enforcement, SDD-12 test execution, UAT, TLS/secret handling, supported OS review, backup/restore, monitoring and rollback are complete and separate written release authorization is granted.

The script requires both -ConfirmReleaseAuthorization and a manually provisioned C:\LabTrack\release-authorization.txt marker. It creates a service in Manual startup mode and does not start it. Do not create the marker during preparation.

## Gated firewall configuration

Do not run Set-LabTrackFirewall.ps1 until the actual LAN CIDR is confirmed and approved by the network administrator. It limits HTTPS to the supplied CIDR and never opens MariaDB port 3306.

## Linux migration

Keep application code OS-neutral. For Linux, publish for linux-x64, run under a dedicated non-root account with systemd, and use a TLS reverse proxy or approved direct Kestrel TLS. Use secret files with restrictive permissions or a secret manager. Requalify service management, paths, certificates and MariaDB behavior independently.
