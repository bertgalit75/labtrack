# LabTrack Kestrel + Windows Service deployment preparation

**Target:** Windows dedicated host, LAN `192.168.0.116`, ASP.NET Core Kestrel, Windows Service, React static frontend, MariaDB 10.11.

## Safety / readiness boundary

This package prepares a deployment workflow. It does not install/start a service, alter firewall rules, install certificates, connect to MariaDB, execute SQL/migrations/seeds, create users/roles, or deploy to production.

The existing API production startup guard must remain intact. MariaDB EF Core provider/mappings are not yet qualified. Do not bypass that guard. Do not install/start the service until the release gates in `RELEASE-GATES.md` are met and separately authorized. Build success is not runtime readiness.

The repository was not available in the current working session, so these files are a standalone deployment overlay based on the previously identified layout (`src/backend/LabTrack.WebApi` and `src/frontend/labtrack-web`). Review paths and integrate the hosting code described below before use.

## Target topology

- Browser: `https://labtrack.<internal-domain>` (initial LAN test can use `https://192.168.0.116` only if the certificate includes the IP SAN and clients trust it).
- Kestrel: HTTPS listener on `0.0.0.0:443`.
- React: built `dist` copied to the API publish directory's `wwwroot`, served by the same ASP.NET Core process. This gives same-origin API calls and avoids CORS.
- MariaDB: local service, preferably bound to `127.0.0.1:3306`; never open 3306 to workstation LAN.
- Service account: dedicated low-privilege local account or gMSA; grant read/execute to app directory and only required write access to logs/data directories.

## Repository layout assumptions

```
LabTrack.sln
src/backend/LabTrack.WebApi/LabTrack.WebApi.csproj
src/frontend/labtrack-web/package.json
```

## Build and stage

From repository root on a build machine with .NET SDK 10 and Node.js installed:

```powershell
.\deployment\kestrel\scripts\Build-LabTrack.ps1
```

Artifacts: `artifacts\kestrel\publish\`. The script copies React `dist` into the publish `wwwroot` directory. It does not run EF migrations or SQL.

Commit `package-lock.json` and switch to `npm ci` for reproducible builds before release.

## Required application integration (must be reviewed in source)

1. Add the `Microsoft.Extensions.Hosting.WindowsServices` package matching the application's target framework.
2. In the host builder, enable Windows Service lifetime, e.g. `builder.Host.UseWindowsService();` (or the equivalent for the actual hosting pattern). Do not add this blindly without inspecting current `Program.cs`.
3. Ensure static-file middleware serves `wwwroot` and that SPA fallback only handles non-API routes. Preserve existing API routes, authentication, authorization, audit middleware, exception handling, and security headers. Typical ordering is static files, routing, authentication, authorization, mapped API endpoints, then a fallback to `index.html` for non-API client routes; adapt to the actual app.
4. Set the frontend API base URL to same-origin relative `/api` (or the actual API prefix). Verify the backend route prefix before build.
5. Ensure the app uses `ContentRoot`/`WebRoot` correctly when running as a Windows Service; service working directory is not the project directory.
6. Keep the existing production startup guard. The package's service installation script requires a manually created release-authorization marker and refuses to install without it.

## Windows setup

- Use a supported, serviced Windows release. Windows 10 reached end of support on 2025-10-14 for ordinary editions; it should be treated as a development/test host, not a production PHI server, unless a qualifying ESU/LTSC lifecycle and security plan is documented. Prefer supported Windows 11 or Windows Server for production.
- Install .NET 10 runtime if framework-dependent. Install Node.js only on the build machine.
- Install MariaDB 10.11 on the same host; configure loopback-only binding and a dedicated least-privilege application DB account after provider qualification. Do not expose port 3306 to client PCs.
- Provision an internal DNS record, e.g. `labtrack.<internal-domain>` -> `192.168.0.116`.
- Obtain a certificate from the trusted internal CA. SAN must match the DNS name (or the IP if accessing by IP). Export as PFX with a strong password; secure the PFX and password separately. The service identity must have read access to the certificate private key.
- Copy publish output to `C:\LabTrack\app`; logs to `C:\LabTrack\logs`; backups outside web root and encrypted.

## Configure Kestrel HTTPS

Set environment variables for the Windows Service (machine-level environment variables are visible to all services, so protect secrets carefully):

```powershell
[Environment]::SetEnvironmentVariable('ASPNETCORE_ENVIRONMENT','Production','Machine')
[Environment]::SetEnvironmentVariable('Kestrel__Endpoints__Https__Url','https://0.0.0.0:443','Machine')
[Environment]::SetEnvironmentVariable('Kestrel__Endpoints__Https__Certificate__Path','C:\LabTrack\certs\labtrack.pfx','Machine')
# Set Kestrel__Endpoints__Https__Certificate__Password via an approved secret-management process.
```

Do not place the PFX password or MariaDB password in source control, scripts, screenshots, or plain-text deployment files. Prefer a managed secret store; if environment variables are used temporarily, restrict server administration and document rotation. Configure HTTP-to-HTTPS redirection only after HTTPS is tested. No public port forwarding.

## Firewall

Only after approved DNS/certificate and LAN subnet confirmation, allow inbound TCP 443 from the actual lab subnet to `192.168.0.116`. Optional TCP 80 can be allowed only if a deliberate HTTP-to-HTTPS redirect endpoint is configured. Do not open TCP 3306 to workstations. No internet exposure or router port forwarding.

Example only (verify actual CIDR before executing):

```powershell
New-NetFirewallRule -DisplayName 'LabTrack HTTPS from Lab LAN' -Direction Inbound -Action Allow -Protocol TCP -LocalAddress 192.168.0.116 -LocalPort 443 -RemoteAddress '192.168.0.0/24' -Profile Domain,Private
```

## Install service (future, gated)

After source integration, provider qualification, security testing, UAT, certificate/secret provisioning and separate written release authorization:

1. Place the exact published output in `C:\LabTrack\app`.
2. Create `C:\LabTrack\release-authorization.txt` through the approved release process (do not create it now).
3. Run elevated PowerShell:

```powershell
.\deployment\kestrel\scripts\Install-LabTrackService.ps1 -PublishPath 'C:\LabTrack\app' -ServiceName 'LabTrack' -ServiceCredential (Get-Credential) -ConfirmReleaseAuthorization
```

The script checks for the authorization marker, executable, React output and TLS material before creating the service. Supply a dedicated low-privilege service account via `Get-Credential`; it does not use LocalSystem by default. It is not a substitute for approval. Review service account rights and ACL policy before use. Do not run it during preparation.

## Linux migration path

Keep application code OS-neutral: no Windows registry, drive-letter paths, Windows-only file APIs, or IIS-specific assumptions. Use environment-based configuration and relative/content-root paths. On Linux, publish for `linux-x64`, copy React files to `wwwroot`, run under a dedicated non-root account with systemd, and use Kestrel behind a TLS reverse proxy (Caddy/Nginx) or direct Kestrel TLS if operationally approved. Store secrets in root-readable environment/secret files or a secret manager. MariaDB configuration and EF migrations remain provider-specific and must be tested independently.

A sample systemd unit is in `linux/labtrack.service.example`. Do not enable it until all release gates pass.
