# MariaDB runtime configuration

LabTrack supports two explicit persistence modes:

- `DevelopmentInMemory`: permitted only when the ASP.NET Core environment is `Development`. Data is lost when the process stops. This mode is for UI and API development only.
- `MariaDb`: configures the Pomelo EF Core provider for the qualified MariaDB 10.11 target and requires a persistent connection string.

The repository's default `appsettings.json` remains in `DevelopmentInMemory` mode. Do not change the committed default to contain a database endpoint or credential.

## Configure a local qualification connection

Use a dedicated, least-privilege MariaDB account and a disposable qualification database. Do not use the MariaDB root account, `labtrack_dev`, or a production database for initial qualification.

From `src/backend/LabTrack.WebApi`, credentials can be stored in .NET user-secrets for local development:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:LabTrack" "Server=127.0.0.1;Port=YOUR_PORT;Database=YOUR_QUALIFICATION_DATABASE;User ID=YOUR_APP_USER;Password=YOUR_PASSWORD;CharSet=utf8mb4;SslMode=Preferred;Connection Timeout=15;Default Command Timeout=30;"
```

Alternatively, provide the complete connection string through the process environment:

```powershell
$env:ConnectionStrings__LabTrack = "Server=127.0.0.1;Port=YOUR_PORT;Database=YOUR_QUALIFICATION_DATABASE;User ID=YOUR_APP_USER;Password=YOUR_PASSWORD;CharSet=utf8mb4;SslMode=Preferred;Connection Timeout=15;Default Command Timeout=30;"
$env:Persistence__Mode = "MariaDb"
```

For a non-development host, set `Persistence__Mode=MariaDb` and `ConnectionStrings__LabTrack` in the service's protected environment or approved secret store. Do not put credentials in appsettings files, source control, deployment scripts, logs, or support tickets.

## Startup behavior

When `Persistence:Mode` is `MariaDb`, the API:

1. Requires `ConnectionStrings:LabTrack`.
2. Configures Pomelo with an explicit MariaDB 10.11.6 server version. It does not use server auto-detection.
3. Attempts a database connection before accepting requests.
4. Refuses to start if the database cannot be reached.
5. Refuses to start if EF Core reports pending migrations.
6. Does **not** apply migrations automatically and does not create or seed a database.
7. Exposes `/health/live` for process liveness and `/health/ready` for database connectivity.

This separation is intentional. The runtime application account should not need schema-modification privileges. Database migrations must be reviewed, backed up, and applied as a separate release operation using a deployment identity with the required DDL permissions.

## Qualification and production restrictions

The MariaDB runtime path is an integration step, not by itself production approval. Before production use, complete and record all of the following:

- Confirm that all EF Core runtime, design, Identity, and provider packages are on a compatible version set. Pomelo 9.0.0 is an EF Core 9 provider; do not combine it with EF Core 10 packages.
- Regenerate or otherwise formally reconcile the migration and model snapshot using the selected compatible EF Core toolchain. The current checked-in migration metadata must be verified against the package graph before database release.
- Compare the EF model, generated migration SQL, and approved MariaDB 10.11 schema, including the workflow and numbering tables.
- Run provider integration tests against the exact MariaDB 10.11 patch release and verify CRUD, Identity, transactions, constraints, concurrency, and restart persistence.
- Use a separate production database and a restricted runtime account. Keep schema deployment credentials separate.
- Complete authorization, audit, HTTPS, backup/restore, monitoring, incident recovery, UAT, and host operating-system support reviews.

Do not use real patient information until the full production acceptance and security review is complete.
