# Patient Management API — Initial implementation notes

## Implemented on `feature/patient-management-api`

- `GET /api/v1/patients`: search by patient number or name, optional status filter, page/pageSize, allow-listed sort.
- `POST /api/v1/patients`: server-generated patient number and patient registration.
- `GET /api/v1/patients/{patientId}`: patient detail.
- `PATCH /api/v1/patients/{patientId}`: routine demographics only; identity-defining fields are excluded.
- Patient DTOs use decimal-string identifiers. EF entities are not returned from controllers.
- Registration and demographic updates write audit events in the same relational database transaction.
- Updates use a row-version concurrency token exposed as an ETag. PATCH requires `If-Match`; stale values return 412 and missing values return 428.
- Cookie-authenticated writes require antiforgery validation.

## Explicit interim boundaries

1. SDD-09 role grants remain unapproved. No roles or permission grants are seeded. Every patient endpoint therefore defaults to deny unless a trusted Identity user/role claim supplies the exact `permission` claim.
2. Read and demographic-update endpoints additionally require `patient_scope=all`. This is a conservative interim gate, not a substitute for the final department/assignment/object-level scope model. Do not issue this claim until the user has approved the access scope.
3. The patient-number policy remains open in SDD-11. The initial generator currently emits `PT-` plus 26 uppercase hexadecimal characters from a GUID (29 characters total), within the SQL column limit. This is a provisional technical format, not a user-approved numbering policy.
4. PATCH treats null as unchanged and an empty string as clearing an optional field. The routine update surface excludes first/middle/last name, suffix, date of birth, and sex-at-registration.
5. Search fields and sort fields are intentionally fixed in code. Exact normalization, date filters, and more granular record-scope behavior remain open.
6. Idempotency-Key replay handling is not implemented in this first slice and remains required before retry-safe registration is accepted.
7. No MariaDB provider was enabled, no migrations were generated, and no database was connected or modified. The existing non-development startup guard remains unchanged.

## Verification status

A new InMemory-backed patient service test project covers registration/audit creation, search/pagination, routine updates, and stale row-version rejection. These tests have been authored but have **not** been executed in this environment. Build/test results must be recorded after running the solution with the .NET 10 SDK. InMemory tests do not qualify MariaDB mappings or provider behavior.
