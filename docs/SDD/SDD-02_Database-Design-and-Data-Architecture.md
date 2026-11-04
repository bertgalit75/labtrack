# SDD-02 — Database Design and Data Architecture

**Version:** 0.2 (consolidated baseline)  
**Status:** Approved  
**Release:** 1

## 1. Database baseline

- MariaDB 10.11.x.
- InnoDB.
- utf8mb4.
- Signed BIGINT identifiers.
- UTC DATETIME(6) timestamps.
- New database; no production-data migration is currently required.
- Initial schema changes are to be controlled and versioned, with EF Core migrations proposed.

## 2. Schema scope

The accepted Release 1 review baseline contains **48 tables**:

- Identity/staff/access: 13
- Patient management: 9
- Test catalog: 8
- Examination management: 3
- Results and verification: 6
- Reports/documents: 6
- Audit/numbering: 3

The 51-table count in one assumptions/review note conflicts with the static report and actual consolidated SQL CREATE TABLE count. The accepted baseline is 48; do not add three tables based on that discrepancy.

## 3. Important data relationships

- A patient may have examinations.
- An examination contains examination tests.
- Each examination test references a specific published test-definition version.
- Results belong to examination tests and parameter definitions.
- Result revisions belong to results.
- Submissions capture specific result revisions for verification.
- Reports contain examination test/submission references and preserved snapshots.
- Signatures and distribution events are recorded against report releases.
- Audit and workflow events preserve operational history.

## 4. Integrity requirements

Application/service transactions must enforce cross-table invariants, including:

- A result parameter belongs to the selected test-definition version.
- A result revision belongs to its result.
- The current revision belongs to the same result.
- A submission item's result and revision belong to the submission's examination test.
- Report release, examination test, and submission refer to the same examination.
- Signing staff, credentials, and authorizations are correctly related and valid at signing time.
- Patient merge and report amendment chains cannot form cycles.
- Status transitions are controlled.
- Historical records are protected from ordinary mutation.
- Number allocation is transaction-safe.

## 5. Implementation validation required

The consolidated SQL is a review baseline, not a production-ready or execution-validated schema. Before implementation:

1. Execute on a disposable MariaDB 10.11 instance.
2. Inspect tables, indexes, foreign keys, and `SHOW CREATE TABLE`.
3. Verify ASP.NET Identity and EF Core provider mappings.
4. Compare EF Core model snapshots/migrations with the reviewed SQL.
5. Test concurrency, integrity, and cycle prevention.
6. Test backup and restore, including report/document files.
7. Review audit payload privacy and authorization evidence handling.

## 6. Approval decisions

- 48 tables accepted as Release 1 baseline.
- Database is new; no production data migration required.
- MariaDB 10.11.x confirmed.
- Schema versioning and controlled migrations proposed.
