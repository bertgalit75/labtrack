# Development MariaDB connection configuration

The development connection string is prefilled for the approved local Windows-server setup:

- Host: `127.0.0.1`
- Port: `3306`
- Database: `labtrack_dev`
- Character set: `utf8mb4`

## Set credentials without committing a password

From `src/backend/LabTrack.WebApi`, run:

```powershell
dotnet user-secrets init
dotnet user-secrets set "ConnectionStrings:LabTrack" "Server=127.0.0.1;Port=3306;Database=labtrack_dev;User ID=YOUR_DB_USER;Password=YOUR_DB_PASSWORD;CharSet=utf8mb4;SslMode=Preferred;Connection Timeout=15;Default Command Timeout=30;"
```

Replace `YOUR_DB_USER` and `YOUR_DB_PASSWORD` with the dedicated least-privilege development account credentials. Do not use the MariaDB root account for the application.

Alternatively, set the environment variable `ConnectionStrings__LabTrack` to the complete connection string. Environment variables override appsettings and user-secrets values.

## Important: connection is configured, but not activated yet

`appsettings.Development.json` intentionally retains `Persistence:Mode` as `DevelopmentInMemory`. The current `Program.cs` registers EF Core InMemory in that mode and deliberately refuses to start in another mode until a MariaDB EF Core provider and the mappings have passed the approved compatibility/validation gate. Adding a connection string alone does not switch EF Core to MariaDB.

Do not change the mode to `MariaDb` or remove the startup guard until the selected provider is validated against MariaDB 10.11 and the schema mappings are reconciled. No database connection or SQL script is executed by this configuration change.
