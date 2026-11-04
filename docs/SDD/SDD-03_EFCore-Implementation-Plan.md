# SDD-03 EF Core Implementation Plan

**Status:** 18-table EF model and static metadata tests implemented; execution and MariaDB provider qualification pending  
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

## 2. Important boundary: foreign keys to excluded principals

Some in-scope SQL tables reference principals that are intentionally excluded from the EF model, including `TestCategories`, `TestMethods`, and `Units`. The in-scope dependent columns are mapped as scalar properties (for example `TestCategoryId`, `DefaultMethodId`, `UnitId`, and `ActualMethodId`), but EF does not create relationships to entity types that are deliberately outside the 18-table allowlist.

Those foreign-key constraints remain defined in the unchanged consolidated SQL installation baseline and remain database-enforced. This is an explicit scope trade-off, not a request to remove those constraints. EF relationships are configured for foreign keys whose principal and dependent tables are both within the 18-table scope.

## 3. Safety and compatibility boundary

Repository-side model and test work does not authorize a database connection, SQL execution, migration generation/application, database-content changes, or production startup activation.

The existing `Program.cs` guard remains unchanged: only Development with `Persistence:Mode=DevelopmentInMemory` is permitted. All other modes fail closed.

The repository retains `net10.0` and EF Core / Identity EF Core 10.0.0. The stable Pomelo 9.0.0 release previously evaluated targets EF Core 9.0.x and must not be added to this EF Core 10 dependency graph. Do not downgrade EF Core, suppress dependency diagnostics, or use unapproved preview/nightly packages. Provider activation remains blocked until a stable compatible provider is reviewed and approved.

## 4. Implemented repository changes

- Completed missing SQL-backed entity properties for Patients, test catalog entities, examination/result entities, audit records, and the custom Identity user timestamp.
- Replaced the scaffold mapping in `LabTrackDbContext` with explicit Fluent API mappings for all 18 approved tables.
- Configured table and column names/types, lengths, date/time precision, decimal precision, unsigned numeric types, defaults, nullability, keys, named indexes, in-scope foreign keys with RESTRICT delete behavior, and four declared CHECK constraints.
- Added `LabTrack.Infrastructure.Tests` to the solution.
- Added model tests for the exact 18-table allowlist, excluded-table absence, all 18 column inventories, named indexes, CHECK constraints, selected critical store types, and RESTRICT behavior of EF-modeled foreign keys.

The model uses scalar FK properties, not EF relationships, for the SQL constraints whose principal tables are excluded by ADR-001. The SQL baseline remains authoritative for those database-level constraints.

## 5. Verification status

The test project has been committed but **has not yet been executed** in this environment. Static repository edits are not equivalent to a successful build or passing test run. Run the solution build and test suite locally before treating the model as qualified:

```powershell
dotnet restore .\LabTrack.sln
dotnet build .\LabTrack.sln --no-restore
dotnet test .\src\backend\LabTrack.Infrastructure.Tests\LabTrack.Infrastructure.Tests.csproj --no-build
```

These commands build the EF model using the InMemory provider; they do not connect to MariaDB or apply migrations. If compilation or assertions fail, correct those failures before proceeding.

## 6. Remaining qualification sequence

1. Run the solution build and model tests above; review all failures.
2. Reconcile any differences between the model metadata and the 18 SQL table definitions.
3. Re-check official Pomelo release compatibility with EF Core 10. Do not install an incompatible version.
4. Only after separate explicit authorization, qualify provider behavior against a newly created disposable MariaDB 10.11 database populated with synthetic data.
5. Never use production or the user's existing local database for qualification. Do not apply migrations.
6. Request a separate explicit approval before changing the startup guard to enable MariaDB.

Until those gates are passed: no MariaDB connection, no migrations, no schema execution, and no production activation.
