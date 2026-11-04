# SDD-03 EF Model Reconciliation — Initial 18-Table Scope

**Branch:** `feat/kestrel-windows-hosting-prep`  
**Scope decision:** SDD-03-ADR-001 — Initial 18-Table Persistence Scope  
**Baseline:** `database/LabTrack_Consolidated_Install.sql` and SDD-03  
**Verification status:** EF mapping, metadata tests, and CI workflow committed. No successful build/test result has been observed yet. No MariaDB connection, SQL execution, or migration was performed.

## Current result

**IMPLEMENTED, PENDING EXECUTION —** the source now contains explicit EF mappings and metadata tests for exactly the approved 18-table scope. Do not treat the model as qualified until the solution builds and the tests pass.

The other 30 tables in the 48-table installation baseline remain intentionally excluded from the EF application model. Their absence is not a defect.

## Approved table allowlist

**Identity (7):** `AspNetRoles`, `AspNetUsers`, `AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserRoles`, `AspNetUserTokens`.

**Application (11):** `Patients`, `TestDefinitions`, `TestDefinitionVersions`, `TestParameterDefinitions`, `ReferenceIntervals`, `ResultValueOptions`, `Examinations`, `ExaminationTests`, `TestResults`, `TestResultRevisions`, `AuditLogs`.

## Implemented source changes

- Added missing SQL-backed properties to Patient, test catalog, examination/result, and audit entities.
- Added `ApplicationUser.DeactivatedAtUtc`.
- Replaced the scaffold configuration with explicit table, column, type, length, precision, default, nullability, key, index, in-scope FK, and CHECK constraint mappings.
- Configured EF-modeled foreign keys to use RESTRICT delete behavior.
- Explicitly ignored `IdentityUserPasskey<long>`. EF Core 10 Identity schema version 3 adds an `AspNetUserPasskeys` entity, which is outside the approved 18-table model and absent from the SQL installation baseline. The ignore keeps the table allowlist stable.
- Added the `LabTrack.Infrastructure.Tests` project to `LabTrack.sln`.
- Added a GitHub Actions workflow to restore, build, and run the model tests for this branch.

## Foreign keys to excluded principal tables

The SQL baseline contains foreign keys from in-scope tables to out-of-scope principals such as `TestCategories`, `TestMethods`, and `Units`. The corresponding dependent columns are mapped as scalar EF properties, but EF relationships are intentionally not configured because the principal entity types are excluded by ADR-001.

Those FK constraints remain in the unchanged SQL installation baseline and are enforced by the database. The EF metadata tests validate the 18-table model boundary and the relationships represented inside that boundary; they do not claim that EF metadata models every SQL FK whose principal is excluded.

## Metadata tests authored

`src/backend/LabTrack.Infrastructure.Tests/InitialPersistenceModelTests.cs` currently verifies:

1. Exactly the approved 18 table names are mapped.
2. None of the 30 excluded SQL tables is mapped.
3. The EF Core 10 Identity passkey table is not mapped.
4. Each in-scope table has the expected column inventory.
5. Named indexes are present for all 18 tables.
6. The four declared CHECK constraints are represented.
7. Selected critical store types include unsigned integers, decimal precision, DATE, LONGTEXT, and DATETIME(6).
8. All EF-modeled foreign keys use RESTRICT delete behavior.

The tests are committed but **have not yet been executed successfully** in the available environment. The workflow run lookup returned no run for the workflow-creation commit, so no CI result is claimed.

Run from the repository root in PowerShell:

```powershell
dotnet restore .\LabTrack.sln
dotnet build .\LabTrack.sln --no-restore --configuration Release
dotnet test .\src\backend\LabTrack.Infrastructure.Tests\LabTrack.Infrastructure.Tests.csproj --no-build --configuration Release
```

The model tests use EF Core InMemory and do not require a database connection.

## Remaining gates

- Successful build and test execution, with failures fixed.
- Review the EF metadata against all 18 in-scope SQL definitions.
- Stable official Pomelo support for EF Core 10, then dependency review.
- Separate authorization before any disposable MariaDB qualification.
- Separate authorization before changing the production fail-closed startup guard.

## Safety verification

- No database connection was made.
- No SQL was executed.
- No migration was generated or applied.
- No production configuration or startup guard was changed.
- No provider package was installed, removed, or downgraded.
- The consolidated 48-table SQL installation script remains unchanged.
