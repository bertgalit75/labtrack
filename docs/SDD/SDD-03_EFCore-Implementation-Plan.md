# SDD-03 EF Core Implementation Plan

**Status:** Initial 18-table application persistence scope approved; provider activation and model qualification remain blocked  
**Target framework:** .NET 10 / EF Core 10  
**Database target:** MariaDB 10.11.x  
**Provider direction:** Pomelo, pending a stable EF Core 10-compatible release  
**Branch:** `feat/kestrel-windows-hosting-prep`

## 1. Approved initial persistence scope

Per **SDD-03-ADR-001**, the initial application persistence model is intentionally limited to exactly 18 tables:

- **Seven ASP.NET Core Identity tables:** `AspNetRoles`, `AspNetUsers`, `AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserRoles`, and `AspNetUserTokens`.
- **Eleven application tables:** `Patients`, `TestDefinitions`, `TestDefinitionVersions`, `TestParameterDefinitions`, `ReferenceIntervals`, `ResultValueOptions`, `Examinations`, `ExaminationTests`, `TestResults`, `TestResultRevisions`, and `AuditLogs`.

The other 30 tables in the 48-table SQL installation baseline are intentionally outside this initial EF Core application model. Their exclusion is a scope decision, not an implementation defect. Do not scaffold entities, add DbSets, or create placeholder mappings for those 30 tables as part of this phase. Reconsideration requires a future approved SDD change.

**Important distinction:** the SQL installation baseline remains intact and continues to define the database installation schema. This decision does not delete, rename, or alter any of its 48 tables, columns, constraints, indexes, or seed data. The 18-table application model is a deliberately narrower view of that schema, not a claim that the physical database contains only 18 tables.

## 2. Scope and safety boundary

This plan authorizes repository-side model and configuration work for the 18 in-scope tables against the approved SDD-03 dictionary and `database/LabTrack_Consolidated_Install.sql`. It does not authorize connecting to any database, executing SQL, applying or generating migrations, changing database contents, or enabling production startup.

The existing `Program.cs` startup guard is a required safety control. Keep the only active persistence mode as `DevelopmentInMemory` for Development. All other environments must continue to fail closed until provider and model qualification gates are passed and a separate explicit approval authorizes activation.

## 3. Compatibility decision: .NET 10 retained

The repository currently targets `net10.0` and uses EF Core / Identity EF Core 10.0.0. The stable Pomelo 9.0.0 release previously evaluated targets EF Core 9.0.x and must not be added to this EF Core 10 dependency graph.

**Implementation rule:** retain .NET 10 and EF Core 10. Do not add an incompatible stable Pomelo package, a preview/nightly package, or an unreviewed third-party fork. Re-check the official Pomelo release and compatibility matrix before adding the provider. Until a stable compatible release is available and approved, MariaDB provider activation remains blocked and the startup guard stays in force.

## 4. Approved sources of truth

Reconcile the 18 in-scope EF entities only against:

1. `docs/SDD/SDD-03_Data-Dictionary-and-Database-Constraints.md`
2. `database/LabTrack_Consolidated_Install.sql`
3. Approved architecture and identity decisions in SDD-01 / SDD-02.

Preserve the SQL baseline exactly. Do not “correct” or simplify the installation SQL to match the smaller application model. For each of the 18 mapped tables, preserve declared names, nullability, lengths, decimal precision/scale, defaults, UTC datetime precision, keys, indexes, foreign keys, delete restrictions, and CHECK expressions where applicable.

The 30 out-of-scope tables are not part of the initial application-model completeness gate. They remain in the installation baseline and are not to be mapped in this phase.

## 5. Implementation sequence

### Phase A — Compatibility and dependency gate
- Keep all current .NET 10 / EF Core 10 package versions aligned.
- Track official Pomelo EF Core 10 compatibility.
- Do not add Pomelo 9.0.0 to the EF Core 10 graph, downgrade EF Core, suppress dependency diagnostics, or use preview/nightly packages.
- Add a provider only after a stable compatible release is reviewed and approved.

### Phase B — Reconcile the 18 in-scope tables
- Map exactly the seven Identity tables and eleven application tables listed in Section 1.
- Complete the entity properties and navigation relationships required by the SQL definitions for those 18 tables.
- Configure Identity table names, custom `BIGINT` keys, exact column lengths, normalized-name indexes, and custom user fields including `IsActive` and `DeactivatedAtUtc`.
- Do not add the other 30 SQL tables to the EF model.
- Do not use placeholder entities or shadow properties to conceal missing domain design.

### Phase C — Fluent mapping and constraints
- Use focused `IEntityTypeConfiguration<T>` classes grouped by domain area.
- Reconcile SQL column types and EF CLR types, including `DATE`, `DATETIME(6)`, `DECIMAL(p,s)`, unsigned integers, nullable values, and LONGTEXT/JSON text.
- Reproduce the declared primary keys, composite keys, unique constraints, secondary indexes, foreign keys, delete behaviors, and CHECK constraints for the 18 in-scope tables.
- Treat self-references and deferred foreign keys explicitly where they occur within the in-scope model.
- Do not invent cascade deletes; preserve the SQL-defined delete behavior.

### Phase D — Static model verification
- Add automated tests that build the EF model without opening a database connection.
- Assert that the model contains exactly the 18 approved table mappings: the seven Identity tables and eleven application tables.
- Assert that none of the 30 excluded tables is accidentally mapped.
- For the 18 mapped tables, verify column names/types/nullability/lengths/precision/defaults, key and index names, foreign-key targets/delete behavior, and CHECK SQL against the approved baseline.
- Treat an accidental mapping outside the 18-table allowlist as a test failure. Do not treat the 30 intentionally excluded SQL tables as missing-model defects.
- Keep tests independent of migrations and database availability.

### Phase E — Disposable MariaDB qualification (separate authorization required)
- Use only a newly created, disposable MariaDB 10.11 test instance/database with synthetic data.
- Verify provider initialization, generated SQL, type mappings, constraints, transactions, and Identity store behavior for the approved 18-table application scope.
- Never point tests at the existing local MariaDB instance, staging data, or production.
- Do not apply migrations. Any schema provisioning must use a reviewed disposable-test procedure and separate explicit authorization.
- Record evidence and close discrepancies before requesting approval to enable MariaDB startup.

## 6. Current repository baseline

- `LabTrack.Infrastructure` targets `net10.0` and references EF Core 10.0.0, Identity EF Core 10.0.0, and EF Core InMemory 10.0.0.
- `LabTrackDbContext` currently exposes eleven application DbSets, with seven Identity entity types inherited from `IdentityDbContext`.
- The current entity model is still partial even within the approved 18-table scope; see `SDD-03_EF-Model-Reconciliation-Report.md`.
- `Program.cs` registers InMemory only for Development + `Persistence:Mode=DevelopmentInMemory`, and fails closed for every other mode. Preserve this behavior.
- Windows Service / Kestrel hosting is independent of provider qualification.

## 7. Exit criteria

The initial EF application model is ready for provider qualification only when:

- The EF model contains exactly the 18 approved table mappings and no mapping for the 30 excluded tables.
- Every column, key, unique constraint, index, foreign key, delete behavior, and CHECK constraint applicable to those 18 tables is reconciled with the unchanged SQL baseline.
- Automated EF metadata tests pass.
- A stable provider officially compatible with EF Core 10 is selected and dependency restore is clean.
- Disposable MariaDB 10.11 qualification passes with synthetic data after separate authorization.
- A separate approval explicitly authorizes changing startup configuration.

Until then: no MariaDB connection, no migrations, no schema execution, and no production activation.
