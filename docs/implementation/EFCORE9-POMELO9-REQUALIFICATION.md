# EF Core 9 / Pomelo 9 qualification retarget

**Decision:** The user approved keeping the application target framework at .NET 10 while using EF Core 9 and the official Pomelo 9 provider.

## Current branch changes

- Infrastructure EF Core, Identity EF Core, EF Core InMemory, and EF Core Design package references are aligned to 9.0.14.
- The provider package is now `Pomelo.EntityFrameworkCore.MySql` 9.0.0.
- The API remains targeted at `net10.0`; ASP.NET Core OpenAPI and Windows Services remain on the .NET 10 line.
- The local `dotnet-ef` tool is aligned to 9.0.14.

## Required requalification (not yet complete)

The previously generated migration and model snapshot were produced with EF Core 10 / the Microting provider. They must not be treated as validated EF Core 9 / Pomelo artifacts. Regenerate the initial migration and snapshot with the pinned EF Core 9 tooling, review the complete SQL output, and rerun the model inventory tests. Do not apply the migration to any database.

Run a clean restore, Release solution build, and complete test suite. Confirm that the Pomelo provider reports MariaDB 10.11 metadata correctly and that all 18 approved table mappings, column types, defaults, indexes, constraints, key generation, and FK delete behavior remain consistent with the SQL installation baseline.

Then run provider integration tests only against a newly created disposable MariaDB 10.11.6.0 database/instance, after the user supplies its connection details and explicitly approves the test operation. Never use production or an existing local database.

## Release boundary

This retarget does not authorize enabling MariaDB in `Program.cs`, running production migrations, importing real patient data, deploying the application, or merging either draft PR. The existing fail-closed persistence guard must remain unchanged until all static and disposable-runtime qualification checks pass.
