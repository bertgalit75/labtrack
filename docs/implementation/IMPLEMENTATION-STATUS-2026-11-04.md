# LabTrack Implementation Status — 4 November 2026

## Authorization and boundaries

- User explicitly authorized development implementation on 4 November 2026.
- SDD-11 is an approved API-contract baseline (3 November 2026), with open details preserved.
- SDD-12 is an approved test-strategy and synthetic-data baseline (3 November 2026). Approval is documentation-only; no tests are claimed executed or passed.
- No production database execution, real patient-data loading, or deployment is authorized.
- Synthetic-only data is required in development and test environments.

## Confirmed implementation choices

- Result value support: numerical and qualitative.
- Initial host OS: existing Windows server PC; future Linux portability remains a design objective.
- Frontend: React + TypeScript.
- Backend: ASP.NET Core Web API and EF Core.
- Database: MariaDB 10.11.x; no change to this approved database requirement.
- Architecture: Clean Architecture, modular monolith, LAN-only centralized host, same-origin frontend/API preferred.
- Expected workload: 5–10 concurrent users, subject to measured testing.

## Work started in this iteration

- Created initial Domain, Application, Infrastructure, and Web API project structure.
- Added initial domain types for patients, test catalog versions, examination/test records, result revisions, and audit records.
- Added a pure result interpretation function for numerical and qualitative values. Numerical ranges are supplied by the published catalog; the code does not invent clinical ranges. Qualitative interpretation is driven by configured catalog options.
- Added an ASP.NET Core Identity user type and initial cookie-authentication controller skeleton.
- Added a development-only EF InMemory registration guard. It is not a MariaDB substitute and is not suitable for persistence, clinical use, or production.
- Added a React/TypeScript desktop-first interface prototype with synthetic demonstration values and the supplied official logo.
- Added approval addenda to SDD-11 and SDD-12 to reconcile document headers/status text with the approved baselines and current implementation authorization.

## Blocking technical qualification

As of 4 November 2026, the official EF Core provider matrix lists Pomelo for MariaDB/MySQL through EF Core 9, while .NET 10 is the active LTS line. Oracle's Connector/NET documentation describes EF Core 10 support as preview-only for its documented package and lists MySQL Server 8.0+ rather than validating MariaDB 10.11. No unverified provider has been selected. The MariaDB EF provider, exact package versions, MariaDB 10.11 mapping, and migrations remain a qualification gate.

The current source deliberately refuses non-development startup until a qualified MariaDB provider configuration is supplied. The InMemory provider is only for isolated developer checks.

## Contract and policy gaps intentionally not invented

- SDD-11 covers authentication/session, dashboard, staff/access, patients, test catalog, and examination registration/worklists. Its open DTO and workflow details remain open.
- SDD-11 does not define result-entry, result-revision, submission, verification, critical-result, report-signing/release, amendment, or print API contracts. Those must be specified and approved before implementing those API workflows.
- User's choice of both numerical and qualitative results is recorded as a product decision. It does not by itself approve clinical reference ranges, qualitative option interpretations, critical thresholds, or report-release policy.
- SDD-12 deferred detailed result/report test suites. Additional test cases must be added through a controlled test-strategy addendum before result/report acceptance.
- SDD-03's 48-table SQL baseline remains a documentation baseline until disposable MariaDB 10.11 runtime validation is completed.
- Result verification independence, immutable revisions, audit evidence, critical-result communication, and authorized signatory rules remain mandatory design constraints.

## Next development sequence

1. Complete a contract traceability pass against SDD-11 and SDD-03 before expanding controllers or EF mappings.
2. Qualify the EF Core/MariaDB 10.11 provider and target Windows runtime combination; document the exact supported package matrix.
3. Add persistence mappings only after comparing every relevant entity with the approved SQL and data dictionary.
4. Implement SDD-11 authentication/session, staff/access, patient, catalog, and examination API contracts.
5. Prepare and approve result-entry/revision/submission/verification/report API and UI contracts before implementing those workflows.
6. Add SDD-12-aligned unit, integration, API, database, frontend, security, performance, and restore tests using synthetic fixtures.
7. Execute schema/migration and backup/restore validation only on disposable non-production infrastructure until separate production authorization is granted.

## Verification state

No .NET SDK is installed in the current build environment. The C# projects have not been restored or compiled. No automated tests have been run. The frontend prototype has not been dependency-installed or built. No database has been connected or modified. No deployment has occurred.
