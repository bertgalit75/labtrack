# SDD-03 EF Model Reconciliation — Initial 18-Table Scope

**Branch:** `feat/kestrel-windows-hosting-prep`  
**Scope decision:** SDD-03-ADR-001 — Initial 18-Table Persistence Scope  
**Baseline:** `database/LabTrack_Consolidated_Install.sql` and SDD-03  
**Verification status:** Source changes committed; automated tests authored but not executed in this environment. No MariaDB connection, SQL execution, or migration was performed.

## Current result

**IMPLEMENTED — initial 18-table EF mapping and metadata test suite are present; qualification remains pending.**

The application model is intentionally limited to seven ASP.NET Core Identity tables and eleven application tables. The other 30 tables in the 48-table installation baseline remain intentionally excluded from the EF application model. Their exclusion is not a defect.

## Approved table allowlist

**Identity (7):** `AspNetRoles`, `AspNetUsers`, `AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserRoles`, `AspNetUserTokens`.

**Application (11):** `Patients`, `TestDefinitions`, `TestDefinitionVersions`, `TestParameterDefinitions`, `ReferenceIntervals`, `ResultValueOptions`, `Examinations`, `ExaminationTests`, `TestResults`, `TestResultRevisions`, `AuditLogs`.

## Implemented

- Added the missing SQL-backed fields to the Patient, test catalog, examination/result, and audit entities.
- Added `ApplicationUser.DeactivatedAtUtc` to match the SQL `AspNetUsers` definition.
- Replaced the initial scaffold mapping in `LabTrackDbContext` with explicit mappings for all 18 approved tables.
- Configured SQL table/column names, relevant store types, lengths, precision, defaults, nullability, primary/composite keys, named unique and secondary indexes, in-scope foreign keys with RESTRICT delete behavior, and four CHECK constraints.
- Added a dedicated test project to the solution.

## Deliberate EF relationship boundary

The SQL schema includes foreign keys from in-scope tables to excluded principals such as `TestCategories`, `TestMethods`, and `Units`. The dependent FK columns remain mapped as scalar properties in EF. EF relationships are not configured for those excluded principal types because doing so would require mapping entities outside the approved 18-table allowlist.

Those constraints remain present in the unchanged SQL installation baseline and are enforced by the database schema. This ADR-001 trade-off must be retained unless a future scope decision changes it.

## Automated tests authored

`src/backend/LabTrack.Infrastructure.Tests/InitialPersistenceModelTests.cs` currently checks:

1. Exactly the approved 18 table names are mapped.
2. None of the 30 intentionally excluded table names is mapped.
3. Each in-scope table has the expected SQL column inventory.
4. Named indexes are present for all 18 tables.
5. The four SQL CHECK constraints are represented.
6. Selected critical store types include unsigned integers, decimal precision, DATE, LONGTEXT, and DATETIME(6).
7. All EF-modeled foreign keys use RESTRICT delete behavior.

**These tests have not yet been run.** Do not describe the build or tests as passing until the user runs them or an execution environment reports a successful result.

Run from the repository root in PowerShell:

```powershell
dotnet restore .\LabTrack.sln
dotnet build .\LabTrack.sln --no-restore
dotnet test .\src\backend\LabTrack.Infrastructure.Tests\LabTrack.Infrastructure.Tests.csproj --no-build
```

The model tests use EF Core InMemory and do not require a database connection.

## Provider and deployment gates

The repository remains on .NET 10 / EF Core 10. The stable Pomelo 9.0.0 release previously evaluated targets EF Core 9.0.x and must not be added to this EF Core 10 graph. Provider activation remains blocked until a stable officially compatible release is reviewed and approved.

The production fail-closed startup guard in `Program.cs` was not changed. The consolidated 48-table SQL installation script was not changed.

## Safety verification

- No database connection was made.
- No SQL was executed.
- No migration was generated or applied.
- No production configuration or startup guard was changed.
- No provider package was installed, removed, or downgraded.
- The SQL baseline remains the original 48-table installation schema.
