# LabTrack Windows / IIS deployment preparation

**Target profile**
- Hosting: IIS + ASP.NET Core Module (ANCM)
- Server role: dedicated Windows host
- Planned LAN address: `192.168.0.116` (configure as a DHCP reservation or static address outside the DHCP pool)
- Browser endpoint: `https://192.168.0.116/` for initial LAN validation; production should use an internal DNS name and a certificate whose SAN matches that name.
- Frontend: React/Vite static build in the IIS site root
- API: ASP.NET Core Web API as an IIS child application at `/api`
- Database: MariaDB 10.11, loopback-only/local to the server unless an approved separate DB host is later specified.

## Important readiness boundary

This is deployment **preparation only**. No deployment, service installation, IIS configuration, database connection, schema execution, migration, account creation, firewall modification, or production data operation has been performed.

The current `Program.cs` intentionally throws outside Development + `Persistence:Mode=DevelopmentInMemory`. Do not bypass or edit that guard for deployment. The MariaDB EF Core provider and mappings remain unqualified. Consequently the current API cannot be launched as a production IIS application. Scripts below create folders/build artifacts and provide an explicitly gated IIS configuration template; they do not claim a working production deployment.

Other release gates remain: SDD-09 authorization matrix approval/implementation; completion of SDD-12 test execution and UAT; provider qualification; reviewed DB deployment plan; backup/restore drill; supported and serviced Windows edition; HTTPS certificate/trust plan; separate written authorization for production DB and deployment. SDD-11 and SDD-12 approvals are design baselines only, not deployment approvals.

## Layout

On the dedicated host:

```
C:\LabTrack\
  wwwroot\       # React dist files (IIS site root)
  api\            # dotnet publish output (IIS child application /api)
  logs\           # app logs if/when configured; restrict ACLs
  backups\        # encrypted backups; not web-accessible
```

Use a dedicated IIS app pool identity. Grant it read/execute to `api` and `wwwroot`; grant write only to explicitly approved application data/log directories. Never grant write to the entire deployment tree. Do not put secrets in source control or `appsettings*.json`; use environment variables or a managed secret store after the approved configuration is defined.

## Build and stage (developer/build machine)

Run from repository root:

```powershell
.\deployment\windows\scripts\Build-LabTrack.ps1
```

This runs `dotnet restore`, `dotnet build -c Release`, `npm install`, `npm run build`, and `dotnet publish -c Release -r win-x64 --self-contained false`. It writes to `artifacts\windows\wwwroot` and `artifacts\windows\api`. It does not run EF migrations or SQL.

A package-lock file should be committed before adopting reproducible `npm ci` builds. Until then the script uses `npm install` and the resolved dependency tree may vary.

## IIS host preparation

1. Use a supported, fully patched Windows edition. A dedicated Windows 11 Pro machine is the current desktop OS target; Windows Server is also suitable. Windows 10 is out of support and is not a production target.
2. Install IIS Web Server, IIS Management Console, Static Content, Default Document, HTTP Errors, and Request Filtering.
3. Install the .NET 10 Hosting Bundle **after** IIS. It installs the ASP.NET Core Module and runtime needed by framework-dependent publish. If installed before IIS, repair/re-run it after IIS.
4. Reboot if requested.
5. From an elevated PowerShell session, run `scripts\Initialize-IisHost.ps1`. This only creates the directory structure, app pool and site skeleton. It deliberately does not bind HTTPS or configure a database.
6. Copy build artifacts with `scripts\Deploy-StagedBuild.ps1` only after release authorization. This script is a file-copy helper and does not start or enable the application.

Microsoft IIS hosting guidance: https://learn.microsoft.com/aspnet/core/host-and-deploy/iis/?view=aspnetcore-10.0

## IIS site / child app model

- Site `LabTrack` points to `C:\LabTrack\wwwroot`.
- App pool `LabTrackApiPool`: No Managed Code, 64-bit, dedicated identity.
- Child application `/api` points to `C:\LabTrack\api` and uses `LabTrackApiPool`.
- The API publish output's generated `web.config` is required for ANCM process startup. Do not replace it with the SPA `web.config`.
- `iis\spa-web.config` is for the React root only. It provides client-side route fallback to `index.html` while excluding `/api` and existing files.
- Before production, verify API route prefixes in controllers and configure frontend API base URL consistently. Current frontend/API integration and deployment path have not been verified; do not assume `/api` is already implemented.

## HTTPS

Preferred: create an internal DNS record such as `labtrack.<your-internal-domain>` pointing to `192.168.0.116`, issue a certificate from the organization's trusted internal CA with that DNS name in SAN, distribute the CA trust to workstations, then bind the certificate to the IIS site on TCP 443. Avoid relying on a bare IP certificate unless the certificate explicitly contains the IP SAN and clients trust its issuer.

Do not use a self-signed certificate for routine multi-workstation production use unless its trust chain is securely distributed and managed. Redirect HTTP to HTTPS only after the HTTPS binding and certificate are verified. The current API sets secure cookies and HTTPS redirection.

## Firewall / network

- Allow inbound TCP 443 to `192.168.0.116` from the approved laboratory LAN subnet(s) only.
- Optionally allow TCP 80 from those same subnet(s) only for HTTP-to-HTTPS redirection or certificate validation; otherwise leave closed.
- Do **not** open TCP 3306 to workstation networks. Keep MariaDB bound to loopback if database and API share the server. If DB is moved to a separate approved host, allow 3306 only from the application host's fixed IP and use TLS/least-privilege DB credentials.
- Do not expose RDP, SMB, SQL, or management ports to the general LAN/Internet. Restrict administration to a management subnet/VPN and named administrators.
- No router port forwarding or public Internet exposure.

Example rule (run elevated; replace subnet with the actual approved CIDR before use):

```powershell
New-NetFirewallRule -DisplayName 'LabTrack HTTPS from Lab LAN' -Direction Inbound -Action Allow -Protocol TCP -LocalAddress 192.168.0.116 -LocalPort 443 -RemoteAddress '192.168.0.0/24' -Profile Domain,Private
```

This is an example only; the script does not create firewall rules. Confirm the real subnet and network profile with the network administrator.

## Windows Service alternative

Not selected: IIS is the approved target for this preparation. A Windows Service/Kestrel deployment would require explicit service-host integration (`Microsoft.Extensions.Hosting.WindowsServices` or an approved service wrapper), service identity/ACLs, reverse-proxy or direct Kestrel TLS configuration, health/restart policy, and a separate frontend web server. The current project has not been configured for Windows Service hosting. Do not install a service as a workaround for the MariaDB startup guard.

## Production authorization boundary

Never execute `dotnet ef database update`, SQL install/seed scripts, migration bundles, or deployment scripts against a production database without a separate explicit authorization. Do not load real patient data into development. Synthetic data only. Do not remove the provider guard, fabricate role assignments, or treat demo accounts as production identities.
