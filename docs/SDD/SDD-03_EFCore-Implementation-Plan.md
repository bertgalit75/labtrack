# SDD-03 EF Core Implementation Plan

**Status:** Initial 18-table EF model, metadata tests, and CI workflow committed; execution and MariaDB provider qualification pending  
**Target framework:** .NET 10 / EF Core 10  
**Database target:** MariaDB 10.11.x  
**Provider direction:** Pomelo, pending a stable EF Core 10-compatible release  
**Branch:** `feat/kestrel-windows-hosting-prep`

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

The repository retains `net10.0` and EF Core / Identity EF Core 10.0.0. The stable Pomelo 9.0.0 release previously evaluated targets EF Core 9.0.x and must not be added to this EF Core 10 dependency graph. Do not downgrade EF Core, suppress dependency diagnostics, or use unapproved preview/nightly packages. Provider activation remains blocked until a stable compatible provider is reviewed and approved.

## 4. Implemented repository changes

- Completed missing SQL-backed entity properties for Patients, test catalog entities, examination/result entities, audit records, and the custom Identity user timestamp.
- Replaced the scaffold mapping in `LabTrackDbContext` with explicit Fluent API mappings for all 18 approved tables.
- Configured table/column names and store types, lengths, date/time and decimal precision, unsigned numeric types, defaults, nullability, keys, named indexes, in-scope FKs with RESTRICT delete behavior, and four declared CHECK constraints.
- Added `LabTrack.Infrastructure.Tests` to the solution.
- Added metadata tests for the exact 18-table allowlist, absence of the 30 excluded tables and Identity passkeys, all 18 column inventories, named indexes, CHECK constraints, critical store types, and RESTRICT behavior for EF-modeled FKs.
- Added `.github/workflows/backend-model-verification.yml` to restore, build, and run the metadata tests on pushes and pull requests to this branch.

## 5. Verification status

The test project and workflow have been committed, but **no successful build or test run has been observed yet**. The GitHub workflow run lookup returned no run for the workflow-creation commit. Do not describe the code as build-verified or the tests as passing until a run reports success.

Run from the repository root in PowerShell:

```powershell
dotnet restore .\LabTrack.sln
dotnet build .\LabTrack.sln --no-restore --configuration Release
dotnet test .\src\backend\LabTrack.Infrastructure.Tests\LabTrack.Infrastructure.Tests.csproj --no-build --configuration Release
```

These commands build the EF model using the InMemory provider; they do not connect to MariaDB or apply migrations.

## 6. Remaining qualification sequence

1. Obtain a successful solution build and metadata-test run; resolve every failure.
2. Reconcile any remaining differences between the model metadata and the 18 SQL table definitions.
3. Re-check official Pomelo release compatibility with EF Core 10. Do not install an incompatible version.
4. Only after separate explicit authorization, qualify provider behavior against a newly created disposable MariaDB 10.11 database populated with synthetic data.
5. Never use production or the user's existing local database for qualification. Do not apply migrations.
6. Request a separate explicit approval before changing the startup guard to enable MariaDB.

Until those gates are passed: no MariaDB connection, no migrations, no schema execution, and no production activation.
