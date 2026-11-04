# SDD-03 EF Model Reconciliation — Static Test Report

**Branch:** `feat/kestrel-windows-hosting-prep`  
**Baseline:** `database/LabTrack_Consolidated_Install.sql` and SDD-03 v0.2  
**Scope:** Static source comparison only. No build, test runner, MariaDB connection, SQL execution, or migration was run.

## Result

**FAIL — EF model is not reconciled to the approved 48-table SQL baseline.**

This is a source-level audit of the checked-in `LabTrackDbContext` and domain entities, not an executed EF Core metadata test. The current context declares 11 application DbSets plus the seven ASP.NET Core Identity entity types inherited from `IdentityDbContext`. It does not declare or configure the remaining SDD-03 tables. DbSet count is not a complete table coverage test: Identity entities are inherited, and EF can discover other entity types through navigation properties. A runtime metadata inventory test is still required.

## Confirmed discrepancies

### Identity

- SQL `AspNetUsers` contains `IsActive TINYINT(1) NOT NULL DEFAULT 1` and `DeactivatedAtUtc DATETIME(6) NULL`.
- `ApplicationUser` exposes `IsActive` only; `DeactivatedAtUtc` is absent.
- The context maps the seven standard Identity tables by name but does not explicitly reconcile all SQL column lengths, nullable LONGTEXT columns, composite keys, named indexes, or the SQL `RESTRICT` delete behavior.
- The default Identity model must be checked for exact `BIGINT` signed keys and table/column/index naming.

### Patients

SQL `Patients` has 26 columns. The current `Patient` entity exposes only 10 properties:
`PatientId`, `PatientNumber`, `FirstName`, `MiddleName`, `LastName`, `Suffix`, `DateOfBirth`, `SexAtRegistration`, `PatientStatus`, `CanonicalPatientId`, `RegisteredAtUtc`, `RegisteredByUserId`, and `RowVersion` (13 properties; three are present beyond the initially partial set).
Missing SQL-backed properties include `CivilStatus`, `Nationality`, `MobileNumber`, `EmailAddress`, `AddressLine1`, `AddressLine2`, `Barangay`, `CityMunicipality`, `Province`, `PostalCode`, `UpdatedAtUtc`, and `UpdatedByUserId`. The existing entity has no navigation for the two user FKs. Its current mapping omits the SQL-defined composite name/date index and explicit named FK/index definitions. Note: the SQL dictionary is authoritative; property count above should be verified by compiling the metadata inventory.

### Test catalog

- `TestDefinitions`: entity omits `Description` and `UpdatedAtUtc`; the SQL key/index names and category FK are not configured.
- `TestDefinitionVersions`: entity omits `DefaultMethodId`, `Instructions`, `PublishedAtUtc`, and `PublishedByUserId`; composite unique key and four FKs are not configured.
- `TestParameterDefinitions`: entity omits `DisplayOrder` and `IsActive`; its unsigned `TINYINT` mapping, composite unique index, unit FK, and ValueType CHECK are not configured.
- `ReferenceIntervals`: entity omits `TestMethodId` and `Priority`; `AgeMinimumDays` / `AgeMaximumDays` are SQL `INT UNSIGNED` (not arbitrary CLR unsigned widths); two CHECK constraints and selection/method/unit indexes are not configured.
- `ResultValueOptions`: entity omits `DisplayOrder`; composite unique index and parameter FK are not configured.

### Examination and results

- `Examinations`: entity's SQL-backed properties exist, but named unique/index definitions and Patient/creator FKs are absent.
- `ExaminationTests`: entity omits `CompletedAtUtc` and `ActualMethodId`; SQL uses `SMALLINT UNSIGNED` for `TestInstanceNumber`. Composite unique index and all FKs are absent.
- `TestResults`: entity's three core references exist but unique constraint and FKs are absent. SQL defines a deferred FK for `CurrentRevisionId`; it must be configured explicitly after checking its target and the SDD's same-result invariant.
- `TestResultRevisions`: entity omits `MethodSnapshot`; precision, string lengths, all FKs, named unique/indexes and ValueType CHECK are not configured.
- `AuditLogs`: entity omits `BeforeData`, `AfterData`, `IpAddress`, `UserAgent`, and `CorrelationId`; actor FK and all four indexes are absent.

## Table coverage gap

The SQL baseline has 48 tables. Only the following 18 table types are represented in the currently visible context via seven Identity tables and eleven domain DbSets:

`AspNetRoles`, `AspNetUsers`, `AspNetRoleClaims`, `AspNetUserClaims`, `AspNetUserLogins`, `AspNetUserRoles`, `AspNetUserTokens`, `Patients`, `TestDefinitions`, `TestDefinitionVersions`, `TestParameterDefinitions`, `ReferenceIntervals`, `ResultValueOptions`, `Examinations`, `ExaminationTests`, `TestResults`, `TestResultRevisions`, `AuditLogs`.

**At least 30 of 48 SQL tables are not explicitly represented by a DbSet or an explicitly configured entity in the current context source.** Confirm the exact EF model table set using `context.Model.GetEntityTypes()`; the source-level list is not a substitute for that test.

## Provider compatibility gate

The repository targets `net10.0` and EF Core / Identity EF Core `10.0.0`. The selected stable Pomelo release previously evaluated, `Pomelo.EntityFrameworkCore.MySql 9.0.0`, targets EF Core 9.0.x. It is not a compatible package to add to this EF Core 10 graph. Do not downgrade EF Core independently, suppress NU1202/NU1605 warnings, or use an unapproved nightly/preview package. Retain .NET 10 and EF Core 10 as directed. Provider activation remains blocked pending an officially compatible stable Pomelo release or a separately approved compatibility decision.

## Required next changes

1. Reconcile all existing entity properties to the SQL declarations before adding mappings.
2. Add the missing domain entity types only where the approved domain design and SDD provide semantics; do not generate empty placeholder entities solely to inflate coverage.
3. Configure every mapped entity through explicit Fluent API configurations, preserving SQL names, types, precision, lengths, defaults, nullability, PK/unique/secondary indexes, FKs, delete restrictions and CHECK constraints.
4. Add an EF metadata test that enumerates `context.Model.GetEntityTypes()`, compares the complete mapped table/column/key/index/FK/check-constraint inventory with a machine-readable extraction of the approved SQL baseline, and reports missing/extra items.
5. Run that metadata test without opening a connection. Do not invoke migrations.
6. Once stable Pomelo EF Core 10 support is available, add the provider and run separate tests against a disposable MariaDB 10.11 instance only after explicit authorization.

## Safety verification

- No database connection was made.
- No SQL was executed.
- No migration was generated or applied.
- No production configuration or startup guard was changed.
- No package was installed, removed, or downgraded.
