# SDD-11 — Detailed API Contracts for Initial Modules

**Version:** 0.1  
**Status:** Approved with documented open details  
**Release:** 1  
**Date:** 3 November 2026  
**Depends on:** SDD-00 through SDD-10, especially SDD-04, SDD-05, SDD-07, SDD-09, and SDD-10

## 1. Purpose and contract status

This document converts the approved initial screen and permission designs into proposed HTTP contracts for authentication/session, dashboard, staff/access, patients, test catalog, and examination registration/worklists. It is a specification draft, not implemented or runtime-tested API behavior.

The API remains a controller-based ASP.NET Core Web API under `/api/v1`, using JSON UTF-8, secure cookie authentication, antiforgery protection for state-changing requests, BIGINT IDs serialized as decimal strings, and Problem Details-style errors. DTOs are independent of EF Core entities. Workflow changes use explicit commands rather than generic status updates.

### 1.1 Contract conventions

- Base path: `/api/v1`.
- Same-origin deployment is preferred.
- JSON property names: camelCase.
- IDs: decimal strings, never JSON numbers.
- Timestamps: ISO 8601 UTC with `Z`; date-only values use `YYYY-MM-DD`.
- Pagination: `page` (1-based) and `pageSize`; server enforces a configured maximum.
- Sort/filter parameters are allow-listed per endpoint; arbitrary SQL/property expressions are never accepted.
- Request bodies reject unknown or unsupported enum values where practical.
- No EF entity serialization.
- No patient identifiers or sensitive clinical data in URL query strings except approved search terms; never place secrets in URLs.
- Cookie-authenticated state-changing requests require antiforgery validation.
- Responses must not expose internal stack traces, SQL, filesystem paths, credentials, or secret configuration.

## 2. Authentication and session contracts

| Method and path | Purpose | Authorization |
|---|---|---|
| `POST /auth/login` | Authenticate username/password; begin required additional verification when policy demands it | Anonymous, rate-limited |
| `POST /auth/login/verify` | Complete privileged-user additional verification | Login challenge only |
| `POST /auth/logout` | End current session | Authenticated + antiforgery |
| `GET /auth/me` | Return current user profile, roles, permissions, and session metadata safe for UI | Authenticated |
| `GET /auth/session` | Return expiry/renewal state for current session | Authenticated |

### 2.1 Login request

```json
{
  "username": "string",
  "password": "string"
}
```

### 2.2 Login response variants

Successful session establishment returns `200 OK` with safe user summary and sets the server-managed cookie. If additional verification is required, return `202 Accepted` with a short-lived challenge reference and allowed verification method metadata; do not establish the fully privileged session until verification succeeds.

```json
{
  "user": {
    "id": "123",
    "displayName": "Example User",
    "roles": ["Laboratory Staff"],
    "permissions": ["patients.read", "examinations.read"]
  },
  "session": {
    "idleTimeoutSeconds": 900,
    "absoluteTimeoutSeconds": 28800,
    "expiresAtUtc": "2026-11-03T14:00:00Z"
  }
}
```

`expiresAtUtc` is illustrative. Exact challenge format, offline MFA mechanism, lockout thresholds, and cookie renewal rules remain to be finalized in the security implementation specification.

### 2.3 Authentication security requirements

- Use generic authentication failure responses to prevent account enumeration.
- Rate-limit login and verification attempts.
- Enforce 15-minute inactivity and 8-hour absolute session limits server-side.
- System Administrators and Authorized Signatories must complete the approved additional verification mechanism.
- Cookies are HttpOnly, Secure, and SameSite according to the approved same-origin deployment; antiforgery token delivery/validation is specified in the security implementation plan.
- Never return password hashes, MFA secrets, recovery codes, or session cookie values.

## 3. Common response and error models

### 3.1 Paged response

```json
{
  "items": [],
  "page": 1,
  "pageSize": 25,
  "totalItems": 0,
  "totalPages": 0
}
```

### 3.2 Problem Details-style error

```json
{
  "type": "about:blank",
  "title": "Validation failed",
  "status": 400,
  "detail": "One or more fields are invalid.",
  "instance": "/api/v1/patients",
  "traceId": "correlation-id",
  "errors": {
    "legalName": ["This field is required."]
  }
}
```

`errors` is an application extension for field-level validation. Sensitive values must not be echoed into error responses.

### 3.3 Status-code conventions

| HTTP | Use |
|---|---|
| 200 | Successful read/update/command with response body |
| 201 | Resource created; include `Location` header |
| 202 | Asynchronous/challenge workflow accepted but not completed |
| 204 | Successful operation with no response body |
| 400 | Malformed request or field validation failure |
| 401 | Missing/invalid/expired authentication |
| 403 | Authenticated but permission denied |
| 404 | Resource absent or not visible in caller's scope; avoid existence leakage |
| 409 | State conflict, duplicate/idempotency conflict, or concurrency conflict |
| 412 | Failed `If-Match` precondition when ETag is used |
| 429 | Rate limit exceeded |
| 500 | Unexpected server error with safe trace ID only |
| 503 | Service temporarily unavailable/maintenance |

## 4. Concurrency, idempotency, and audit contract

- Mutable resources expose an opaque `ETag` response header; updates/commands that require optimistic concurrency send `If-Match`.
- Missing required `If-Match` returns `428 Precondition Required` if adopted by the implementation; this exact status behavior must be confirmed in the API error catalog.
- Stale ETag returns `412 Precondition Failed` with a safe conflict response; clients must reload/reconcile rather than overwrite.
- Retry-sensitive creation/commands accept `Idempotency-Key` with a bounded length and endpoint-specific retention window. Reusing a key with a different normalized request returns `409 Conflict`.
- Idempotency records and the business operation must be committed consistently; concurrent requests with the same key must not duplicate the operation.
- Every successful state-changing operation records actor, timestamp, action, target, outcome, and reason where required in audit/workflow history.
- Do not write passwords, tokens, full patient demographics, or unnecessary clinical content to logs/audit payloads.

Exact ETag representation, idempotency retention, and audit event taxonomy remain implementation-planning decisions.

## 5. Dashboard contracts

| Method and path | Purpose | Permission/scope |
|---|---|---|
| `GET /dashboard/summary` | Return permitted summary cards/counts | Module-specific permissions and record scope |
| `GET /dashboard/work-items` | Return current user's permitted work queue | Work-item-specific read permission and assignment scope |
| `GET /dashboard/activity` | Return permitted recent workflow events | Workflow-history read permission and scope |

All endpoints use server-side authorization and aggregation. The dashboard must not infer access from client-supplied role names or construct totals from paginated result sets. Response fields are limited to what the user needs for the assigned duty.

## 6. Staff and access contracts

| Method and path | Purpose | Permission |
|---|---|---|
| `GET /staff` | Search/list staff accounts | `staff.accounts.read` |
| `POST /staff` | Create staff profile/account | `staff.accounts.manage` plus applicable account-create permission |
| `GET /staff/{staffId}` | Read staff profile | `staff.accounts.read` and scope |
| `PATCH /staff/{staffId}` | Update permitted profile fields | `staff.accounts.manage` |
| `POST /staff/{staffId}/activate` | Activate account | Dedicated account activation permission |
| `POST /staff/{staffId}/deactivate` | Deactivate account | Dedicated account deactivation permission |
| `GET /access/roles` | List assignable roles | Role administration/read permission |
| `PUT /staff/{staffId}/roles` | Replace assigned roles | `access.roles.assign`; server validates assignability and separation of duties |
| `GET /access/permissions` | Read permission catalog for administrative UI | Specific access-policy read permission |

### 6.1 Staff DTOs

Create request (proposed):

```json
{
  "staffNumber": "optional-policy-generated",
  "givenName": "string",
  "middleName": null,
  "familyName": "string",
  "displayName": "string",
  "username": "string",
  "initialCredentialDelivery": "administrator-set-or-approved-invite"
}
```

Staff response (proposed):

```json
{
  "id": "123",
  "staffNumber": "ST-000123",
  "displayName": "Example User",
  "username": "example.user",
  "isActive": true,
  "roles": ["Laboratory Staff"],
  "createdAtUtc": "2026-11-03T08:00:00Z",
  "updatedAtUtc": "2026-11-03T08:00:00Z"
}
```

Staff-number format, profile fields, initial credential activation/recovery workflow, and whether roles are returned as names or stable IDs remain to be finalized. Do not expose credential material in these DTOs.

## 7. Patient contracts

| Method and path | Purpose | Permission/scope |
|---|---|---|
| `GET /patients` | Search/list patients | `patients.read` plus record scope |
| `POST /patients` | Register patient | `patients.register` |
| `GET /patients/{patientId}` | Read patient detail | `patients.read` plus record scope |
| `PATCH /patients/{patientId}` | Update routine permitted demographics | `patients.demographics.update`; field-level policy |
| `POST /patients/{patientId}/identity-change-requests` | Request identity-defining correction | Elevated identity-change permission; reason required |
| `POST /patients/merge-requests` | Create merge request | `patients.merge.request` |
| `GET /patients/merge-requests/{requestId}` | Read merge request | Merge workflow scope |
| `POST /patients/merge-requests/{requestId}/approve` | Approve merge request | `patients.merge.approve`; requester cannot approve own request |
| `POST /patients/merge-requests/{requestId}/execute` | Execute approved merge | `patients.merge.execute`; cycle-safe dedicated command |

### 7.1 Patient list query

Proposed query parameters: `search`, `status`, `registeredFrom`, `registeredTo`, `page`, `pageSize`, `sort`. Searchable fields, matching behavior, and date semantics must be explicitly allow-listed. Do not accept arbitrary field names or return data outside caller scope.

### 7.2 Patient create DTO

```json
{
  "legalName": "string",
  "dateOfBirth": "YYYY-MM-DD",
  "sex": "policy-defined-code",
  "contactDetails": {},
  "otherDemographics": {}
}
```

The exact demographic schema is open. This example is illustrative only; it is not a final approved field list. Patient number is generated by the server and is never client-assigned.

### 7.3 Patient update and identity change

- Routine demographic update accepts only explicitly permitted fields.
- Identity-defining fields are excluded from the routine PATCH contract.
- Identity-change request requires a reason and explicit field/value proposal; elevated authorization and review are enforced server-side.
- Audit history preserves prior values under the privacy/redaction policy.
- Merge is never implemented as a generic patient PATCH or DELETE.

## 8. Test catalog contracts

| Method and path | Purpose | Permission |
|---|---|---|
| `GET /catalog/tests` | List/search test definitions and current published version | `catalog.tests.read` |
| `POST /catalog/tests` | Create draft test definition | `catalog.tests.edit` |
| `GET /catalog/tests/{testId}` | Read test definition summary | `catalog.tests.read` |
| `GET /catalog/tests/{testId}/versions` | List versions | `catalog.tests.read` |
| `POST /catalog/tests/{testId}/versions` | Create a new draft version | `catalog.tests.edit` |
| `GET /catalog/test-versions/{versionId}` | Read version and parameters | `catalog.tests.read` |
| `PUT /catalog/test-versions/{versionId}` | Update an unpublished draft version | `catalog.tests.edit`; draft-only |
| `POST /catalog/test-versions/{versionId}/validate` | Validate draft completeness/consistency | `catalog.tests.read` or explicit validate permission (to confirm) |
| `POST /catalog/test-versions/{versionId}/publish` | Publish a valid version | `catalog.tests.publish`; effective-date and approval checks |
| `POST /catalog/test-versions/{versionId}/deactivate` | End future use according to policy | `catalog.tests.deactivate` |

### 8.1 Version DTO (proposed)

```json
{
  "id": "456",
  "testId": "123",
  "versionNumber": "2",
  "status": "Draft",
  "effectiveFromUtc": null,
  "effectiveToUtc": null,
  "code": "CBC",
  "name": "Complete Blood Count",
  "parameters": []
}
```

The exact parameter schema, reference interval model, value types, units, effective-date semantics, and publication approval separation must align with the validated SDD-03 schema and be finalized before implementation. Published versions are immutable; changes create a new version.

## 9. Examination contracts

| Method and path | Purpose | Permission/scope |
|---|---|---|
| `GET /examinations` | Search/list examinations | `examinations.read` plus record/worklist scope |
| `POST /examinations` | Register examination and requested tests | `examinations.register` |
| `GET /examinations/{examinationId}` | Read examination details | `examinations.read` plus scope |
| `GET /examinations/{examinationId}/tests` | List examination tests and selected definition versions | `examinations.read` plus scope |
| `PATCH /examinations/{examinationId}` | Update permitted registration metadata | `examinations.update`; field/status restrictions |
| `POST /examinations/{examinationId}/reopen` | Reopen under approved transition rules | Dedicated reopen permission; reason and audit required |
| `POST /examinations/{examinationId}/cancel` | Cancel examination | Dedicated cancel permission; reason and elevated checks |
| `POST /examinations/{examinationId}/tests/{examinationTestId}/cancel` | Cancel one test | Dedicated cancel permission; reason and state checks |

### 9.1 Examination registration request

```json
{
  "patientId": "123",
  "requestedAtUtc": null,
  "notes": null,
  "tests": [
    { "testDefinitionVersionId": "456" }
  ]
}
```

Server behavior:
- Resolve and validate the patient as active and canonical.
- Validate every selected test-definition version is published and effective for the applicable examination time.
- Generate the examination number server-side using transaction-safe allocation.
- Snapshot/reference the exact test-definition version for each examination test.
- Create examination and all requested test rows atomically.
- Enforce idempotency for retry-sensitive registration requests.
- Return `201 Created` with the examination resource and `Location` header.

The meaning of `requestedAtUtc`, whether clients may supply a requested date/time, required registration metadata, and exact examination-number format remain policy decisions. Client must not choose or override the generated examination number.

### 9.2 Examination response (proposed)

```json
{
  "id": "789",
  "examinationNumber": "EX-YYYYMM-000001",
  "patient": {
    "id": "123",
    "displayName": "Example Patient"
  },
  "status": "Registered",
  "registeredAtUtc": "2026-11-03T08:15:00Z",
  "tests": [
    {
      "id": "790",
      "testDefinitionVersionId": "456",
      "testCode": "CBC",
      "testName": "Complete Blood Count",
      "status": "Pending"
    }
  ]
}
```

Status strings are illustrative pending the finalized SDD-04 status dictionary. Response patient fields are minimized to the caller's duty and scope.

## 10. Authorization contract

- Backend authorization is authoritative for every endpoint and every resource identifier.
- Permission codes are stable identifiers; UI role names are not authorization inputs.
- Enforce object-level record scope, assignment, status, and field-level restrictions.
- A `403` response is used for authenticated users lacking permission; `404` may be used where revealing resource existence would leak protected information.
- The System Administrator does not automatically receive clinical access.
- Result self-verification is prohibited by SDD-04 P-04; when result APIs are specified in a later package, encoder/editor identity must be checked server-side.
- Patient merge requester cannot approve own request.
- Role changes, identity changes, merge actions, publication, cancellation, and other privileged operations must be audited.

## 11. File and document handling

The initial module contracts do not expose arbitrary filesystem paths or direct file URLs. Any future document endpoint must use opaque document identifiers, authorization checks on every download, storage outside the public web root, and integrity metadata/checksums. File upload staging and DB/file consistency handling require a separate contract before report/document modules are implemented.

## 12. API acceptance and test requirements

Before approval/implementation, create contract tests covering:

- Authentication success/failure, privileged verification, logout, idle timeout, absolute timeout, and antiforgery.
- Generic auth failures and login throttling without account enumeration.
- Permission and record-scope denial for every endpoint family, including direct-ID access.
- BIGINT string serialization and UTC timestamp formats.
- Validation errors and Problem Details consistency.
- Pagination maximums and allow-listed sort/filter behavior.
- ETag/If-Match conflict behavior.
- Idempotency-key duplicate, mismatch, and concurrent request behavior.
- Patient identity change reason/elevated permission; merge requester/approver separation and cycle prevention.
- Test version draft immutability after publication and effective-date selection.
- Concurrent examination numbering, atomic registration, and selected-version integrity.
- Audit event generation and sensitive-data redaction.

## 13. Open decisions and dependencies

1. Final DTO fields for staff, patient, test definitions/parameters, and examination registration.
2. Patient-number and staff-number formats.
3. Exact patient search fields, normalization, and permitted filters.
4. Patient/department/worklist record-scope rules.
5. Identity-change request approval workflow and merge executor/approver relationship.
6. Test-definition version numbering and effective-date/time semantics.
7. Exact permission required to validate drafts and approve/publish versions.
8. Examination requested date/time capture and registration metadata.
9. Final SDD-04 status labels/transitions and cancellation/reopen commands.
10. ETag representation, required `If-Match` policy, and exact 428 handling.
11. Idempotency-key length, storage, retention, and response replay policy.
12. Login challenge format, offline MFA method, lockout thresholds, cookie renewal, and antiforgery token delivery.
13. Exact error-code catalog, correlation ID format, and validation conventions.
14. Target .NET/EF Core/provider versions and OpenAPI schema conventions.

## 14. Approval checklist

- [ ] Authentication/session contracts approved.
- [ ] Common DTO/error/pagination conventions approved.
- [ ] Dashboard contracts approved.
- [ ] Staff and access contracts approved.
- [ ] Patient and identity-change/merge contracts approved.
- [ ] Test catalog/version contracts approved.
- [ ] Examination registration/worklist/command contracts approved.
- [ ] Authorization, audit, concurrency, and idempotency rules approved.
- [ ] Open decisions resolved or explicitly deferred with interim policy.
- [ ] Contract-test requirements approved.

## 15. Approval status

Draft for user review. Endpoint paths and DTO examples are proposed contracts, not implemented or runtime-validated behavior. Approval of this document would accept the API design baseline only; unresolved policy details and technical compatibility validation remain separate gates. It does not authorize coding, database execution, or production deployment. A separate explicit implementation-start approval remains required.

## 16. Approval addendum — 3 November 2026

**Status:** Approved with documented open details.

The user approved SDD-11 as the detailed API contract baseline for the initial modules on 3 November 2026. Approval accepts the documented endpoint families, request/response conventions, authorization requirements, concurrency/idempotency expectations, and acceptance criteria as the governing design baseline. It does not close the open decisions listed in Section 13 and does not represent runtime validation or implementation completion.

Implementation authorization was separately granted on 4 November 2026 for development work. Production database execution and deployment remain unauthorized.
