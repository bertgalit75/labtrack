# SDD-03-ADR-001 — Initial 18-Table Persistence Scope

- **Status:** Accepted
- **Decision date:** 2026-11-04
- **Applies to:** Initial EF Core application persistence model
- **Related documents:** `SDD-03_Data-Dictionary-and-Database-Constraints.md`, `SDD-03_EFCore-Implementation-Plan.md`, `SDD-03_EF-Model-Reconciliation-Report.md`

## Context

The consolidated MariaDB installation baseline defines 48 tables. The initial EF Core context already focuses on seven ASP.NET Core Identity tables and eleven application tables. Expanding the initial application model to all 48 tables would increase implementation and verification work beyond the desired initial system scope.

The project prioritizes a simpler initial application persistence layer.

## Decision

The initial EF Core application persistence model is limited to exactly these 18 tables.

### ASP.NET Core Identity (7)

1. `AspNetRoles`
2. `AspNetUsers`
3. `AspNetRoleClaims`
4. `AspNetUserClaims`
5. `AspNetUserLogins`
6. `AspNetUserRoles`
7. `AspNetUserTokens`

### Application domain (11)

1. `Patients`
2. `TestDefinitions`
3. `TestDefinitionVersions`
4. `TestParameterDefinitions`
5. `ReferenceIntervals`
6. `ResultValueOptions`
7. `Examinations`
8. `ExaminationTests`
9. `TestResults`
10. `TestResultRevisions`
11. `AuditLogs`

The remaining 30 tables in the consolidated SQL baseline are intentionally excluded from the initial EF Core application model. They are not implementation defects for this phase and must not be scaffolded, mapped, or represented by placeholder entities as part of this scope.

## Consequences and constraints

1. **The SQL installation baseline remains unchanged.** All 48 tables and their definitions remain in `database/LabTrack_Consolidated_Install.sql`. This ADR does not authorize dropping, renaming, altering, or omitting any of those tables from the installation script.
2. **The 18-table EF model is not the complete physical schema.** It is the initial application's persistence boundary over a larger installation baseline.
3. **Model verification uses an explicit allowlist.** EF metadata tests must assert that exactly the 18 named tables are mapped and that none of the 30 excluded tables is accidentally mapped.
4. **In-scope fidelity remains mandatory.** All columns, keys, indexes, foreign keys, delete behaviors, defaults, nullability, precision, and CHECK constraints applicable to the 18 in-scope tables must be reconciled against the approved SQL and data dictionary.
5. **No placeholder mappings.** Do not add empty entities or shadow mappings merely to represent the excluded tables.
6. **Future expansion requires a new decision.** Adding any excluded table to application persistence requires an approved SDD change that identifies its use case and implementation/test scope.
7. **Security and deployment safeguards are unaffected.** This decision does not authorize a database connection, SQL execution, migration generation/application, MariaDB provider activation, or any production startup change. The existing fail-closed startup behavior remains mandatory.

## Acceptance criteria

- The EF model metadata test passes with exactly the 18-table allowlist.
- The 18 in-scope tables are reconciled to the SQL baseline.
- The 30 excluded tables remain present in the consolidated installation SQL and are not mapped by the initial application model.
- No database connection or migration is required to validate the static EF model.
