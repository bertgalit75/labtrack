# SDD-03 EF Model Reconciliation — Initial 18-Table Scope

**Branch:** `feat/kestrel-windows-hosting-prep`  
**Scope decision:** SDD-03-ADR-001 — Initial 18-Table Persistence Scope  
**Baseline:** `database/LabTrack_Consolidated_Install.sql` and SDD-03  
**Verification type:** Static source comparison only. No build, test runner, MariaDB connection, SQL execution, or migration was run.

## Result

**INCOMPLETE — the EF model is not yet fully reconciled for the approved 18-table application scope.**

The initial application model is intentionally limited to exactly 18 tables: seven ASP.NET Core Identity tables and eleven application tables. The other 30 tables in the 48-table SQL installation baseline are intentionally excluded from the initial EF application model under ADR-001. Their exclusion is not a defect and must not be reported as missing implementation work for this phase.

This report records static source-level findings only. The current context declares eleven application DbSets plus the seven ASP.NET Core Identity entity types inherited from `IdentityDbContext`. A runtime metadata inventory test using `context.Model.GetEntityTypes()` is still required to prove the exact model table set.

## Approved 18-table model inventory

**Identity (7):** `AspNetRoles`, `AspNetUsers`, `AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserRoles`, `AspNetUserTokens`.

**Application (11):** `Patients`, `TestDefinitions`, `TestDefinitionVersions`, `TestParameterDefinitions`, `ReferenceIntervals`, `ResultValueOptions`, `Examinations`, `ExaminationTests`, `TestResults`, `TestResultRevisions`, `AuditLogs`.

## Static discrepancies within the approved scope

### Identity
- SQL `AspNetUsers` contains `IsActive TINYINT(1) NOT NULL DEFAULT 1` and `DeactivatedAtUtc DATETIME(6) NULL`.
- `ApplicationUser` exposes `IsActive`; `DeactivatedAtUtc` is absent in the source reviewed.
- The context maps the seven standard Identity tables by name, but exact SQL column lengths, nullable LONGTEXT columns, composite keys, named indexes, and delete behavior still require explicit metadata verification.
- Confirm exact signed `BIGINT` key mappings and all Identity table/column/index names in the EF metadata test.

### Patients
- The source-level review found missing SQL-backed properties, including `CivilStatus`, `Nationality`, `MobileNumber`, `EmailAddress`, `AddressLine1`, `AddressLine2`, `Barangay`, `CityMunicipality`, `Province`, `PostalCode`, `UpdatedAtUtc`, and `UpdatedByUserId`.
- User foreign-key navigations and the SQL-defined indexes / named foreign-key mappings require reconciliation.
- Use the data dictionary and SQL as the authority for the exact column inventory; do not rely on property counts from this static report.

### Test catalog
- `TestDefinitions`: `Description` and `UpdatedAtUtc` appear absent; SQL key/index names and category FK require verification.
- `TestDefinitionVersions`: `DefaultMethodId`, `Instructions`, `PublishedAtUtc`, and `PublishedByUserId` appear absent; composite unique key and FKs require configuration.
- `TestParameterDefinitions`: `DisplayOrder` and `IsActive` appear absent; unsigned `TINYINT`, composite unique index, unit FK, and ValueType CHECK require configuration.
- `ReferenceIntervals`: `TestMethodId` and `Priority` appear absent; unsigned age-day types, CHECK constraints, and selection/method/unit indexes require configuration.
- `ResultValueOptions`: `DisplayOrder` appears absent; composite unique index and parameter FK require configuration.

### Examination and results
- `Examinations`: named unique/index definitions and Patient/creator FKs require configuration.
- `ExaminationTests`: `CompletedAtUtc` and `ActualMethodId` appear absent; unsigned `SMALLINT`, composite unique index, and FKs require reconciliation.
- `TestResults`: unique constraint and FKs require configuration. The deferred FK for `CurrentRevisionId` needs explicit treatment consistent with the SDD invariant.
- `TestResultRevisions`: `MethodSnapshot` appears absent; precision, string lengths, FKs, named unique/indexes, and ValueType CHECK require configuration.
- `AuditLogs`: `BeforeData`, `AfterData`, `IpAddress`, `UserAgent`, and `CorrelationId` appear absent; actor FK and indexes require configuration.

These findings are a checklist for source reconciliation, not proof from executed EF metadata tests. Re-check each item against the current entity and SQL source before changing code.

## Explicitly excluded tables

The 30 remaining tables in the 48-table installation baseline are outside the initial EF application model by approved scope decision ADR-001. Do not:
- add entities, DbSets, or placeholder mappings for them in this phase;
- classify their absence from the EF model as a defect;
- remove or alter them in the consolidated SQL installation script; or
- claim that the physical database schema has been reduced from 48 tables to 18.

Any future addition to application persistence scope requires an approved SDD change.

## Provider compatibility gate

The repository retains `net10.0` and EF Core / Identity EF Core 10.0.0. The stable Pomelo 9.0.0 package previously evaluated targets EF Core 9.0.x and must not be added to this EF Core 10 graph. Do not downgrade EF Core, suppress dependency warnings, or use an unapproved preview/nightly package. Provider activation remains blocked pending a stable officially compatible provider release and review.

## Required next changes

1. Reconcile the entity properties and relationships for the 18 approved tables only.
2. Configure the 18 mapped entities explicitly, preserving SQL names, types, precision, lengths, defaults, nullability, keys, indexes, FKs, delete behavior, and CHECK constraints.
3. Add an EF metadata test that enumerates `context.Model.GetEntityTypes()` and asserts the exact 18-table allowlist, with no accidental mappings to the 30 excluded tables.
4. Compare every mapped table's metadata with the relevant SQL definitions. Excluded tables are not expected in the EF model.
5. Run the metadata tests without opening a database connection and without invoking migrations.
6. Revisit provider compatibility only when a stable Pomelo release officially supports EF Core 10. MariaDB qualification requires separate explicit authorization and a disposable database populated only with synthetic data.

## Safety verification

For this static review:
- No database connection was made.
- No SQL was executed.
- No migration was generated or applied.
- No production configuration or startup guard was changed.
- No package was installed, removed, or downgraded.
- The consolidated 48-table SQL installation baseline is to remain unchanged.
