# SDD-05 — API Contracts and Application Service Design

**Version:** 0.1 (consolidated baseline)  
**Status:** Approved  
**Release:** 1

## 1. API baseline

- Base path: `/api/v1`.
- Same-origin deployment preferred, e.g. `https://labtrack.local/api/v1`.
- JSON UTF-8.
- ISO 8601 timestamps; UTC in persisted data.
- BIGINT identifiers serialized as strings in JSON to avoid JavaScript precision loss.
- DTOs are independent of EF Core entities.
- Standardized Problem Details-style errors.
- Pagination/filtering for list endpoints.
- Secure HttpOnly cookie authentication and antiforgery protection for state-changing requests.

## 2. Endpoint design

Endpoint groups cover:

- Authentication and current user.
- Staff, roles, permissions, credentials, and authorizations.
- Patients, duplicate review, change requests, and merge.
- Test catalog, versions, parameters, reference intervals, and options.
- Examinations and examination tests.
- Result encoding, revisions, submissions, and verification.
- Critical-result communication.
- Reports, signatures, releases, amendments, and distribution.
- Audit and workflow history.

Exact endpoint paths and DTO schemas are proposed contracts and must be maintained in a detailed API specification before implementation.

## 3. Application architecture

Proposed modular monolith:

- API/controllers.
- Application services and use cases.
- Domain rules and entities.
- Infrastructure/repositories/database/file storage.
- Automated tests.

Controllers are the approved direction. Workflow changes should use explicit commands such as `/verify`, `/approve`, and `/release`, not generic unrestricted status updates.

## 4. Reliability and concurrency

- Use explicit concurrency handling for RowVersion/ETag-style updates.
- BIGINT RowVersion requires explicit mapping and increment logic.
- Use idempotency keys for retry-sensitive operations such as examination registration, submission, and report release.
- Use transactions for numbering and workflow state/history/event writes.
- File storage and database commits are not atomic; use staging, checksums, and recovery/cleanup procedures.

## 5. Approved conventions

- String serialization for BIGINT IDs.
- Controller-based API.
- Swagger/OpenAPI restricted in production.
- Problem Details-style error responses.
- Idempotency keys for retry-sensitive operations.
- Explicit workflow command endpoints.

## 6. Dependency note

Exact .NET, EF Core, MariaDB provider, and package versions must be selected as a compatible set during implementation planning.
