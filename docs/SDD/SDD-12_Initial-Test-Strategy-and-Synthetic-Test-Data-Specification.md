# SDD-12 — Initial Test Strategy and Synthetic Test-Data Specification

**Version:** 0.1  
**Status:** Approved — documentation baseline only  
**Release:** 1  
**Date:** 3 November 2026  
**Depends on:** SDD-00 through SDD-11

## 1. Purpose

This document defines the proposed verification strategy for the initial LabTrack modules and the rules for creating safe synthetic test data. It establishes test levels, environments, coverage expectations, traceability, defect handling, and release evidence.

It is a planning and acceptance baseline only. It does not authorize application implementation, production database execution, or use of real patient information in test environments.

## 2. Scope

### 2.1 Initial modules covered

- Authentication, additional verification, logout, and session limits.
- Application shell, role-aware navigation, and dashboard summaries.
- Staff accounts, role assignment, and permission enforcement.
- Patient search, registration, controlled demographic correction, and merge controls.
- Test catalog draft editing, validation, publication, version immutability, and effective-date selection.
- Examination registration, worklists, details, cancellation, reopening, and completion rules.
- Shared API behavior: validation, Problem Details, pagination, concurrency, idempotency, audit events, and authorization.

### 2.2 Explicitly deferred test suites

Detailed test suites for result encoding, revision history, independent verification, critical-result communication, report signing/release, report amendment, report distribution, and audit export will be added when their screen/API contracts and remaining laboratory policies are specified. Their already-approved principles must still be preserved in architecture and test fixtures.

## 3. Test objectives

1. Verify that implemented behavior conforms to approved SDD requirements and contracts.
2. Prove that backend authorization is enforced independently of frontend visibility.
3. Verify separation-of-duty restrictions and controlled workflows.
4. Protect patient identity, examination history, published catalog versions, and auditability.
5. Detect transaction, concurrency, retry, and data-integrity defects.
6. Verify that the LAN-only deployment functions without internet connectivity for ordinary application operations.
7. Verify backup/restore and operational readiness before production acceptance.
8. Demonstrate acceptable performance for the expected 5–10 simultaneous users on the actual target server.

## 4. Test levels and responsibilities

| Test level | Main purpose | Typical owner | Required evidence |
|---|---|---|---|
| Static analysis and code review | Catch unsafe patterns, style issues, dependency risks | Developers/reviewer | CI reports, review record |
| Unit tests | Validate business rules and pure logic | Developers | Automated test results |
| API/service integration tests | Validate service, authorization, transactions, and persistence | Developers/QA | Test run and database assertions |
| Database validation | Validate MariaDB 10.11 DDL, constraints, mappings, and migrations | Developer/DB reviewer | Schema comparison and SQL test log |
| Frontend component tests | Validate controls, forms, navigation, and state handling | Frontend developer/QA | Component test report |
| End-to-end tests | Validate user journeys across browser/API/database | QA | Scenario results, screenshots/log references |
| Security tests | Validate authentication, authorization, session, input, and file protections | Security reviewer/QA | Findings and retest evidence |
| Performance tests | Validate response times and concurrent-user behavior | QA/operations | Workload profile and measurements |
| Backup/restore tests | Verify recoverability of DB and associated files | Operations/DB administrator | Restore log and reconciliation |
| User acceptance testing | Confirm laboratory workflow suitability | Authorized laboratory representatives | Signed UAT checklist |

Separation of duties should be applied to test approval where practical: the person implementing a control should not be the sole person accepting that control.

## 5. Test environments

### 5.1 Development

- Local developer environment with synthetic data only.
- Isolated database instance; no connection strings to production.
- Automated tests run on each relevant change.
- Secrets supplied through local secret management/environment configuration, never committed.

### 5.2 CI/test

- Reproducible environment with pinned dependency versions.
- MariaDB 10.11.x test instance matching the selected production-compatible patch version.
- Fresh database creation and migration execution on a disposable database.
- No internet dependency during application test execution except controlled dependency acquisition in the build pipeline.
- Synthetic data seeded deterministically from version-controlled scripts/factories.

### 5.3 Staging/UAT

- Separate LAN environment configured to resemble production.
- No production patient data unless a separately approved, lawful de-identification process is established; synthetic data is the default.
- Production-like authentication, HTTPS certificate trust, Windows service hosting, firewall rules, document storage, and backup procedures.
- Explicitly labeled non-production environment to prevent confusion.

### 5.4 Production

- No destructive or synthetic-data test execution against live records.
- Smoke checks limited to approved non-destructive checks and designated test accounts/records, if policy allows.
- No direct developer access to production database for routine testing.

## 6. Test data principles

- Use wholly fictional names, dates, identifiers, contact details, and clinical values.
- Never copy real patient records into development, CI, demonstration, or UAT.
- Do not use real national identifiers, phone numbers, email addresses, addresses, or credentials.
- Use reserved/example domains such as `example.invalid` for synthetic email addresses.
- Use visibly non-real identifiers with a `TEST-` or `SYN-` prefix where the field permits it.
- Synthetic records must be clearly marked as test data in all screens, exports, and generated documents.
- Never send synthetic critical-result notifications to real recipients or external channels.
- Seed deterministic datasets so a test can reproduce the same state.
- Keep test credentials separate from all production credentials; do not reuse privileged production passwords.
- Reset disposable test databases between test runs where feasible.
- Do not store secrets in fixtures, screenshots, logs, or repository history.

## 7. Synthetic data catalogue

The initial seed pack should contain the following fixture families. Values are examples and must be clearly marked synthetic.

| Fixture family | Minimum cases | Purpose |
|---|---:|---|
| Staff accounts | 1 per approved role category, plus disabled and locked accounts | RBAC, account lifecycle, session tests |
| Role combinations | Single-role users and selected multi-role combinations | Least privilege, conflicting-role checks |
| Patients | 30–50 synthetic patients | Search, pagination, duplicate detection, access scope |
| Patient edge cases | Similar names, same DOB, missing optional fields, identity correction, merged/canonical pair | Matching, controlled correction, merge behavior |
| Test definitions | Draft, invalid draft, valid published, superseded, future-effective, expired | Catalog validation and version selection |
| Examinations | Draft/incomplete, active, completed, cancelled, reopened | Worklists and state transitions |
| Concurrent operations | Shared numbering sequence and simultaneous edits | Uniqueness, transactions, concurrency |
| Audit events | Login, access change, patient correction, catalog publication, examination cancellation | Audit creation and filtering |
| Documents | Synthetic files of permitted types and invalid/oversized samples | Upload/download validation and storage security |

No actual patient data or clinically authoritative reference intervals are supplied by this document. Clinical values and reference ranges must be approved by the laboratory before use in clinical validation.

## 8. Core scenario catalogue

### 8.1 Authentication and session

- Valid sign-in, invalid sign-in, disabled account, locked account, throttling.
- Additional verification required for System Administrator and Authorized Signatory.
- Additional verification failure, expiry, retry, and offline/LAN behavior.
- Logout invalidates the session.
- 15-minute inactivity timeout is enforced.
- 8-hour absolute session limit is enforced even with activity.
- Antiforgery checks reject invalid state-changing requests.
- Session revocation prevents further protected API access.

### 8.2 Authorization and staff access

- Each role receives only explicitly approved permissions.
- Direct API calls without permission are denied regardless of UI state.
- Object-level scope is enforced for patient and examination records.
- System Administrator technical privileges do not automatically grant clinical-record access.
- Account activation/deactivation and role assignment are separately permissioned.
- Role changes follow the approved session-refresh/re-authentication behavior.
- Emergency recovery use is recorded and subject to post-use review.

### 8.3 Patient management

- Search by approved patient number/name/identifier.
- Pagination and filter combinations return stable, authorized results.
- Duplicate-aware registration warns or blocks according to approved matching policy.
- Routine demographic correction succeeds for permitted users.
- Identity-defining correction fails without elevated permission and reason.
- Patient number cannot be edited through UI or API.
- Merge is available only through the dedicated controlled command.
- Merge requester cannot approve their own merge.
- Merge cycles are rejected; historical examinations/reports remain attributable.
- Unauthorized ID enumeration does not reveal whether an out-of-scope patient exists.

### 8.4 Test catalog

- Draft creation and editing require catalog-edit permission.
- Publication requires separate publication permission.
- Invalid/incomplete definitions cannot be published.
- Published definitions are immutable.
- Corrections create a new version.
- Only published/effective versions are selectable for new examinations.
- Existing examination tests retain their selected definition version.
- Concurrent publication attempts cannot create conflicting active versions.

### 8.5 Examination management

- Registration requires an active canonical patient.
- Registration accepts only published/effective test versions.
- Examination and examination-test records commit atomically.
- Concurrent registrations never produce duplicate examination numbers.
- Idempotent retry does not create duplicate examinations.
- Unauthorized cancellation/reopening is denied.
- Cancellation/reopening requires reason and audit event.
- Completion is blocked until all tests are verified or properly cancelled.
- Reopening/cancellation after verified/released work follows elevated authorization and preserves history.

### 8.6 Common API behavior

- Invalid DTOs return the agreed Problem Details shape and field errors.
- BIGINT identifiers are represented as JSON strings.
- Pagination limits and invalid sort/filter values are validated.
- Stale concurrency tokens produce conflict responses without silent overwrite.
- Repeated idempotency key with same payload returns the defined prior outcome.
- Reuse of an idempotency key with a different payload is rejected.
- Sensitive data is not exposed in error details, logs, URLs, or correlation metadata.
- API behavior remains functional without external internet access.

## 9. Negative, boundary, and abuse testing

At minimum, test:
- Empty, missing, malformed, excessively long, and unexpected input values.
- Unicode and whitespace handling in names and search terms.
- Invalid date ranges and timezone boundary cases.
- Invalid or expired catalog effective dates.
- Duplicate requests and retry after timeout.
- Concurrent edits and stale versions.
- Direct object-reference manipulation and ID enumeration.
- Privilege escalation through role combinations or forged client claims.
- CSRF, session fixation, cookie flag configuration, and logout/session invalidation.
- SQL injection, stored/reflected XSS, path traversal, unsafe file names, and oversized uploads.
- Unauthorized document download and MIME/content-type mismatch.
- Database connection loss during transaction and application restart during write.
- Disk-full or storage-unavailable conditions for document operations.
- Audit-write failure behavior; high-risk operations must not silently proceed if required audit evidence cannot be recorded.

Security tests must be performed only in isolated non-production environments.

## 10. Database and migration validation

Before any production schema deployment:
1. Execute the consolidated SQL and/or approved migration chain on a disposable MariaDB 10.11.x instance.
2. Verify every expected table, column, index, primary key, unique constraint, foreign key, and CHECK behavior supported by the chosen MariaDB version.
3. Compare actual `SHOW CREATE TABLE` output with the approved schema baseline.
4. Verify EF Core model/provider mappings, including signed BIGINT keys, DATETIME(6), UTC handling, string lengths, nullability, and identity/authentication tables.
5. Verify migration from an empty database and upgrade from the previous supported schema version.
6. Test restrictive delete behavior and database privileges for append-only records.
7. Test cross-table invariants that are enforced by application services rather than simple foreign keys.
8. Verify backup and restore using both database and associated document storage.

A successful static SQL review is not a substitute for this runtime validation.

## 11. Performance and reliability testing

The expected workload is 5–10 simultaneous users. Test on the actual candidate server or a documented hardware-equivalent environment.

Measure:
- Sign-in and current-user request latency.
- Dashboard load and summary queries.
- Patient search and paginated lists.
- Test catalog search and version selection.
- Examination registration and worklist retrieval.
- Concurrent examination numbering and idempotent retries.
- Database CPU, memory, disk I/O, connection pool, and lock waits.
- Error rate, timeout rate, and recovery after service/database restart.

The SDD-06 targets remain provisional until validated:
- Ordinary navigation: under 2 seconds.
- Common list retrieval: under 2 seconds.
- Patient search: under 2 seconds.
- Standard form submission: under 3 seconds.
- Dashboard initial load: under 3 seconds.

Record dataset size, hardware, browser, concurrency profile, test duration, percentile latency, throughput, and errors. Do not report only averages.

## 12. Backup, restore, and operational verification

- Verify daily backup scheduling and retention configuration.
- Confirm backups are stored separately from the primary server storage.
- Confirm backup access is restricted and encryption is used where required by the approved security design.
- Restore the database and associated document files into an isolated environment.
- Verify record counts, document references, checksums, and application startup after restore.
- Measure actual recovery time against the approved target RTO of 8 hours.
- Determine whether the restore point meets the target RPO of 24 hours.
- Record restore-test date, operator, backup identifier, results, discrepancies, and corrective actions.
- A backup job reporting success is not proof that restoration works.

## 13. Defect severity and release handling

| Severity | Definition | Release handling |
|---|---|---|
| Critical | Data loss/corruption, unauthorized disclosure, privilege escalation, wrong patient association, unsafe workflow bypass | Release blocker; stop affected testing/deployment |
| High | Core workflow unusable, required audit missing, transaction integrity failure, security control bypass | Release blocker unless formally risk-accepted by authorized governance |
| Medium | Material function impaired with safe workaround | Resolve or document approved deferral before release |
| Low | Cosmetic or minor usability issue without material workflow/security impact | May be deferred with owner and target version |

Every defect should include reproducible steps, expected/actual behavior, environment/build, synthetic record IDs, evidence, severity, owner, and retest result. Never attach real patient data to defect reports.

## 14. Traceability and evidence

- Each approved requirement and acceptance criterion must map to one or more test cases.
- Test cases must identify preconditions, synthetic fixtures, steps, expected results, and cleanup.
- Automated tests must run in CI for relevant changes.
- Manual tests must record tester, date, build, environment, result, and evidence reference.
- Failed tests must link to defects; fixes require retest and regression testing.
- Security, database integrity, backup/restore, and critical workflow tests require retained evidence.
- Release approval requires review of unresolved defects and explicit acceptance of any permitted residual risk.

## 15. Entry and exit criteria

### Entry criteria

- Relevant SDD and API contract approved.
- Test environment isolated and reproducible.
- Synthetic data pack prepared.
- Build/deployment candidate identified.
- Test cases reviewed and traceable to requirements.
- Test accounts and permissions configured.

### Exit criteria

- All critical and high-severity tests pass, or an authorized governance body explicitly rejects/accepts a documented risk (security/data integrity blockers are not waivable by routine project sign-off).
- No unresolved critical/high defects.
- Authorization and separation-of-duty tests pass.
- Database runtime validation and migration tests pass.
- Backup/restore tests pass.
- Performance targets are met or formally revised based on measured hardware.
- UAT scenarios are signed by authorized laboratory representatives.
- Test evidence and release notes are complete.

## 16. Open decisions

1. Final target browsers and minimum supported versions.
2. Exact test automation framework and CI runner/hosting approach.
3. Exact synthetic demographic field set and approved duplicate-matching rules.
4. Laboratory-approved test catalog fixtures and non-clinical reference values for test environments.
5. Final role-to-test-account mapping and allowed role combinations.
6. Final API error-code catalog and correlation-ID format.
7. Exact security test scope and independent review responsibility.
8. Exact backup encryption/key custody and restore-test schedule.
9. Formal UAT approvers and release acceptance authority.
10. Final performance workload profile and dataset sizes.
11. Detailed test suites for results, verification, critical results, and reports.

## 17. Approval checklist

- [ ] Test levels, responsibilities, and evidence requirements approved.
- [ ] Environment separation and synthetic-only data policy approved.
- [ ] Synthetic fixture catalogue approved.
- [ ] Core scenario catalogue approved.
- [ ] Security, negative, boundary, and abuse test scope approved.
- [ ] MariaDB runtime and migration validation plan approved.
- [ ] Performance and reliability test approach approved.
- [ ] Backup/restore verification plan approved.
- [ ] Defect severity and release handling approved.
- [ ] Entry/exit criteria and UAT evidence approved.
- [ ] Open decisions resolved or explicitly deferred with interim policy.

## 18. Approval status

Draft for user review. Approval would accept this test strategy and synthetic-data policy baseline only. It would not mean tests have been executed or passed, would not validate the database schema at runtime, and would not authorize implementation, production database execution, or deployment. A separate explicit implementation-start approval remains required after all readiness gates are satisfied.

## 19. Approval addendum — 3–4 November 2026

**Status:** Approved — test strategy and synthetic-data documentation baseline.

The user approved SDD-12 on 3 November 2026. This approval accepts the test strategy, environment separation, synthetic-only data policy, scenario catalogue, evidence requirements, database/migration validation plan, performance/reliability plan, backup/restore plan, and release criteria as a documentation baseline only.

No tests have been executed or passed by virtue of this approval. The MariaDB schema has not been runtime-validated. The synthetic-data policy remains mandatory. Result encoding, result revision, verification, critical-result, and report workflows were explicitly deferred from the detailed initial test suite and require additional contract-aligned test cases before those features can be accepted.

On 4 November 2026, the user separately authorized development implementation. That authorization does not authorize production database execution, real patient-data loading, or deployment. Disposable non-production validation and synthetic fixtures are permitted only within the approved SDD-12 controls.
