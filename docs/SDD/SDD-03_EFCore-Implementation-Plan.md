# SDD-03 EF Core Implementation Plan

**Status:** Initial 18-table EF model and metadata tests committed; Microting EF Core 10 provider statically qualified; MariaDB runtime qualification remains open  
**Target framework:** .NET 10 / EF Core 10  
**Database target:** MariaDB 10.11.x  
**Provider direction:** Microting-maintained EF Core 10 fork of Pomelo, qualification candidate only  
**Qualification branch:** `qualify/microting-efcore10-mariadb1011`  
**Base branch:** `feat/kestrel-windows-hosting-prep`

## 1. Approved initial persistence scope

Per **SDD-03-ADR-001**, the initial EF application model maps exactly 18 tables:

- **Seven ASP.NET Core Identity tables:** `AspNetRoles`, `AspNetUsers`, `AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserRoles`, and `AspNetUserTokens`.
- **Eleven application tables:** `Patients`, `TestDefinitions`, `TestDefinitionVersions`, `TestParameterDefinitions`, `ReferenceIntervals`, `ResultValueOptions`, `Examinations`, `ExaminationTests`, `TestResults`, `TestResultRevisions`, and `AuditLogs`.

The other 30 tables in the 48-table SQL installation baseline are intentionally outside the initial EF application model. Their exclusion is not an implementation defect. Do not scaffold entities, add DbSets, or create placeholder mappings for them in this phase.

**The SQL installation baseline remains unchanged.** This is a narrower EF application model over the existing 48-table installation schema; it does not reduce the physical database to 18 tables.

.NET 10 Identity schema version 3 adds an `AspNetUserPasskeys` entity/table. That table is not part of the approved seven-table Identity scope or the SQL baseline. `LabTrackDbContext` explicitly ignores `IdentityUserPasskey<long>` so runtime Identity options cannot silently expand the EF model beyond the approved 18 tables.

## 2. Foreign keys to excluded principals

Some in-scope SQL tables reference principals that are intentionally excluded from the EF model, including `TestCategories`, `TestMethods`, and `Units`. Their dependent FK columns remain mapped as scalar properties (for example `TestCategoryId`, `DefaultMethodId`, `UnitId`, and `ActualMethodId`), but EF relationships are not created to entity types outside the 18-table allowlist.

Those constraints remain defined in the unchanged consolidated SQL installation baseline and remain database-enforced. EF relationships are configured for FKs whose principal and dependent tables are both within the 18-table scope. This is an explicit scope trade-off, not authorization to remove SQL constraints.

## 3. Safety and compatibility boundary

Repository-side model and test work does not authorize a database connection, SQL execution, migration generation/application, database-content changes, or production startup activation.

The existing `Program.cs` guard remains unchanged: only Development with `Persistence:Mode=DevelopmentInMemory` is permitted. All other modes fail closed.

The official `Pomelo.EntityFrameworkCore.MySql` 9.0.0 release targets EF Core 9.0.x and is not compatible with the repository's EF Core 10 dependency graph. Do not add it, downgrade EF Core, suppress dependency diagnostics, or use preview/nightly packages.

The current qualification candidate is the stable `Microting.EntityFrameworkCore.MySql` 10.0.14 package, a company-maintained fork of Pomelo. Its published package targets `net10.0`, requires `Microsoft.EntityFrameworkCore.Relational >= 10.0.14` and `MySqlConnector >= 2.6.2`, and documents MariaDB 10.11 LTS among its tested server lines. The fork renamed its package, assembly, and provider namespaces from Pomelo to Microting; its `UseMySql` extension remains in `Microsoft.EntityFrameworkCore`. This is not the official Pomelo package, and qualification must confirm its exact behavior before adoption.

The qualification branch aligns the EF Core, Identity EF Core, and InMemory package references to 10.0.14 and adds a provider-model test configured for MariaDB 10.11.0 metadata. The test supplies no connection string and does not open a connection. It verifies provider identity, the 18-table relational model, and critical relational store types. The user confirmed a clean working tree, package resolution at 10.0.14, a successful .NET 10 Release solution build, and 7/7 passing tests. GitHub Actions run 41804193323 also completed successfully. These results establish static model/provider compatibility only; they do not establish runtime database behavior or complete SQL-baseline equivalence.

## 4. Implemented repository changes

- Completed missing SQL-backed entity properties for Patients, test catalog entities, examination/result entities, audit records, and the custom Identity user timestamp.
- Replaced the scaffold mapping in `LabTrackDbContext` with explicit Fluent API mappings for all 18 approved tables.
- Configured table/column names and store types, lengths, date/time and decimal precision, unsigned numeric types, defaults, nullability, keys, named indexes, in-scope FKs with RESTRICT delete behavior, and four declared CHECK constraints.
- Added `LabTrack.Infrastructure.Tests` to the solution.
- Added metadata tests for the exact 18-table allowlist, absence of the 30 excluded tables and Identity passkeys, all 18 column inventories, named indexes, CHECK constraints, critical store types, and RESTRICT behavior for EF-modeled FKs.
- Added `.github/workflows/backend-model-verification.yml` to restore, build, and run the metadata tests on pushes and pull requests to the feature branch.

## 5. Verification status

On base commit `3cbac6f`, the user confirmed:

- `dotnet build .\LabTrack.sln`: passed.
- `dotnet test .\LabTrack.sln`: passed, 6 tests, 0 failures.
- GitHub Actions run `41802561391` (Backend model verification): passed.

On qualification commit `b45f942322114dd566f63ca16babe4731ef3ae81`, the user confirmed:

- `dotnet restore .\\LabTrack.sln`: passed.
- `dotnet build .\\LabTrack.sln --no-restore --configuration Release`: passed.
- `dotnet test .\\LabTrack.sln --no-build --configuration Release`: passed, 7 tests, 0 failures.
- GitHub Actions run `41804193323` (Backend model verification): passed.
- Local working tree was clean and all four top-level EF/Identity/provider packages resolved to 10.0.14.

Static provider qualification is complete for model construction using MariaDB 10.11.0 metadata. Runtime database behavior and full SQL-baseline equivalence are not yet qualified.

## 6. Remaining qualification sequence

1. Complete a static, table-by-table comparison of the provider relational model against the unchanged `database/LabTrack_Consolidated_Install.sql` baseline, covering columns, nullability, defaults, primary/unique/secondary indexes, CHECK constraints, identity key generation, and FK behavior. Record any intentional differences and resolve unintended mismatches.
2. Confirm the exact MariaDB 10.11 patch version intended for qualification.
3. Only after separate explicit authorization, run a narrowly scoped integration qualification against a newly created disposable MariaDB database/instance. Do not use production or the user's existing local database. Do not apply migrations or change schema; test only explicitly approved non-destructive operations and metadata behavior.
4. Review the results and obtain explicit approval before changing the startup guard to enable MariaDB.
5. Keep the qualification PR unmerged until static comparison, authorized runtime qualification, and review gates are complete.

Until those gates are passed: no MariaDB connection, no migrations, no schema execution, and no production activation.
