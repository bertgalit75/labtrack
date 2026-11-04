# SDD-03 EF Core Implementation Plan

**Status:** Implementation plan approved; provider activation and complete model qualification blocked  
**Target framework:** .NET 10 / EF Core 10 (retained by decision)  
**Database target:** MariaDB 10.11.x  
**Provider direction:** Pomelo, pending a stable EF Core 10-compatible release  
**Branch:** `feat/kestrel-windows-hosting-prep`

## 1. Scope and safety boundary

This plan authorizes repository-side model and configuration work against the approved SDD-03 dictionary and `database/LabTrack_Consolidated_Install.sql`. It does not authorize connecting to any database, executing SQL, applying or generating migrations, changing database contents, or enabling production startup.

The existing `Program.cs` startup guard is a required safety control. Keep the only active persistence mode as `DevelopmentInMemory` for Development. All other environments must continue to fail closed until the provider, complete model, and disposable-database qualification gates are explicitly passed.

## 2. Compatibility decision: .NET 10 retained

The repository currently targets `net10.0` and uses EF Core / Identity EF Core 10.0.0. The official Pomelo compatibility matrix currently lists Pomelo 9.0.0 for EF Core 9.0.x; the EF Core 10 support work remains open upstream. Therefore Pomelo 9.0.0 must not be added to this EF Core 10 dependency graph, and package-version warnings must not be suppressed.

**Implementation rule:** retain .NET 10 and EF Core 10. Do not add an incompatible stable Pomelo package, a preview/nightly package, or an unreviewed third-party fork. Re-check the official Pomelo release and compatibility matrix before adding the provider. Until then, MariaDB provider activation is intentionally blocked and the startup guard remains in force.

## 3. Approved source of truth

Reconcile the EF model only against:

1. `docs/SDD/SDD-03_Data-Dictionary-and-Database-Constraints.md`
2. `database/LabTrack_Consolidated_Install.sql`
3. Approved architecture and identity decisions in SDD-01 / SDD-02.

SDD-03 currently records a static baseline of 48 tables, 428 columns, 48 primary keys, 33 unique keys, 58 secondary indexes, 98 foreign keys, and 8 CHECK constraints. These are source-text counts, not runtime database observations.

Do not silently “correct” the SQL baseline through EF conventions. Preserve declared names, nullability, lengths, decimal precision/scale, defaults, UTC datetime precision, composite keys, indexes, delete restrictions, and CHECK expressions. Record every intentional deviation as an explicit SDD decision before implementation.

## 4. Implementation sequence

### Phase A — Compatibility and dependency gate
- Keep all current .NET 10 / EF Core 10 package versions aligned.
- Track the official Pomelo EF Core 10 release as a blocking dependency.
- Do not change target frameworks or install Pomelo 9.0.0 into the EF Core 10 graph.
- Once a stable compatible release exists, add the provider to Infrastructure and verify restore/build without suppressing dependency diagnostics.

### Phase B — Entity and Identity reconciliation
- Inventory all 48 SQL tables against existing domain entities and ASP.NET Core Identity entities.
- Add or revise entities only where the SDD-approved domain model supports them.
- Map the seven Identity tables explicitly, including the custom `BIGINT` keys, exact table/column names, normalized-name indexes, and the additional `AspNetUsers.IsActive` / `DeactivatedAtUtc` fields.
- Record unmapped tables and columns as gaps; do not use EF shadow properties as a substitute for missing domain design without a documented decision.

### Phase C — Fluent mapping and constraints
- Use focused `IEntityTypeConfiguration<T>` classes grouped by domain area.
- Reconcile all SQL column types and EF CLR types, including `DATE`, `DATETIME(6)`, `DECIMAL(p,s)`, unsigned integers, nullable values, and LONGTEXT/JSON text.
- Reproduce all primary keys, composite keys, unique constraints, secondary indexes, foreign keys, delete behaviors, and CHECK constraints with source-defined names.
- Treat self-references and deferred foreign keys (including `TestResults.CurrentRevisionId` and `StaffAuthorizations.EvidenceFileId`) explicitly.
- Do not invent cascade deletes. The SQL baseline specifies `ON DELETE RESTRICT` for its declared foreign keys.

### Phase D — Static model verification
- Add automated tests that build the EF model without opening a database connection.
- Assert table/column names, key and index names, precision, lengths, nullability, defaults, foreign-key targets/delete behaviors, and CHECK SQL.
- Compare the model inventory to the 48-table SQL inventory and fail tests on missing or extra mappings.
- Keep tests independent of migrations and database availability.

### Phase E — Disposable MariaDB qualification (separate authorization required)
- Use only a newly created, disposable MariaDB 10.11 test instance/database with synthetic data.
- Verify provider initialization, generated SQL, type mappings, constraints, transactions, and Identity store behavior.
- Never point tests at the existing local MariaDB instance, staging data, or production.
- Do not apply migrations. Any schema provisioning must use a reviewed disposable-test procedure and separate explicit authorization.
- Record test evidence and close all discrepancies before requesting approval to enable MariaDB startup.

## 5. Current repository baseline

- `LabTrack.Infrastructure` targets `net10.0` and references EF Core 10.0.0, Identity EF Core 10.0.0, and EF Core InMemory 10.0.0.
- `LabTrackDbContext` currently exposes a partial set of domain DbSets and contains a scaffold mapping; its own comment correctly says exact mapping and runtime testing remain outstanding.
- `ApplicationUser` currently extends `IdentityUser<long>` and includes `IsActive`; SDD-03 also declares `DeactivatedAtUtc`, which still requires model reconciliation.
- `Program.cs` registers InMemory only for Development + `Persistence:Mode=DevelopmentInMemory`, and throws for every other mode. Preserve this exact fail-closed behavior.
- Windows Service hosting is already configured in the Web API project; it is independent of provider qualification.

## 6. Exit criteria

MariaDB provider activation is not complete until all of the following are true:

- A stable provider version officially compatible with EF Core 10 is selected and dependency restore is clean.
- All 48 tables and 428 columns are reconciled with no undocumented gaps.
- All 48 primary keys, 33 unique keys, 58 secondary indexes, 98 foreign keys, and 8 CHECK constraints are represented or explicitly dispositioned by an approved SDD change.
- Model metadata tests pass.
- Disposable MariaDB 10.11 qualification passes with synthetic data.
- A separate approval explicitly authorizes changing the startup configuration.

Until then: no MariaDB connection, no migrations, no schema execution, and no production activation.
