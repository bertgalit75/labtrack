# Patient authorization and data-access security gate

**Status:** Interim fail-closed implementation; not a production approval  
**Applies to:** Patient Management API on `feature/patient-management-api`  
**Related design:** SDD-07, SDD-09, SDD-11

## 1. Security findings

The patient API initially accepted a `patient_scope=all` claim for list, detail, and demographic-update operations. That claim did not prove that the caller had an assigned duty for any particular patient. SDD-07 requires object-level access and limits data to assigned duties. SDD-09 explicitly leaves the choice among department, assigned-worklist, and examination-assignment scope open. SDD-11 requires record-scope, assignment, status, and field-level checks.

The current domain and EF model do not contain a patient-to-user/department/worklist assignment entity or queryable relation. A broad claim cannot safely substitute for that missing data model.

## 2. Interim enforcement

Until an approved scope model exists:

| Operation | Interim behavior |
|---|---|
| `POST /api/v1/patients` | Remains separately gated by `patients.register` and antiforgery validation. Returns only patient ID, generated patient number, and ETag. |
| `GET /api/v1/patients` | Denied, including when a caller has `patients.read` and a broad `patient_scope=all` claim. |
| `GET /api/v1/patients/{id}` | Denied; direct-ID access cannot bypass the list restriction. |
| `PATCH /api/v1/patients/{id}` | Denied until field-level permission and record scope are enforced together. |
| Identity correction and merge | No generic PATCH/DELETE path; dedicated request/approval/execute workflows remain required and are not enabled by this API. |

The API read summary contract is intentionally limited to patient ID, patient number, name, status, and ETag. It excludes date of birth, sex, contact details, address, canonical linkage, and audit metadata. The registration receipt is narrower still.

No role grants are seeded. A permission code is not granted merely because it appears in a policy or test. SDD-09 remains the source for explicit role-grant approval and the open operational scope decisions.

## 3. Required production scope design

Before patient reads or edits can be enabled, select and approve a concrete scope model:

1. **Assignment source:** department/unit, assigned worklist, examination assignment, explicit patient assignment, or a documented combination.
2. **Authoritative relation:** persisted server-side records linking the authenticated staff identity to the permitted scope. Client-supplied patient IDs, query filters, and untrusted claims must never define the scope.
3. **Query enforcement:** apply scope predicates inside the database query before count, sort, pagination, and projection. Do not fetch all rows and filter in memory.
4. **Direct-ID enforcement:** apply the identical scope predicate to detail and update queries. Return a non-disclosing not-found response for records outside scope.
5. **Scope lifecycle:** define who can assign/revoke scope, effective dates, review, audit, and immediate revocation behavior.
6. **Field minimization:** approve field sets separately for list, detail, registration, correction, and each role/duty. Sensitive fields must not be included by default.
7. **Status rules:** define whether inactive, merged/non-canonical, archived, or otherwise restricted patients may be searched, read, selected, or edited for each workflow.
8. **Identity correction:** define protected identity fields, reason format, approver, before/after audit treatment, and optimistic concurrency.
9. **Merge:** define requester/approver/executor separation, cycle prevention, canonical-patient behavior, and preservation of historical examination/result references.

## 4. Required authorization tests

The acceptance suite must prove:

- Anonymous callers receive 401.
- Authenticated callers without the required permission receive 403.
- A broad `patient_scope=all` claim alone cannot grant access.
- A caller assigned to patient A cannot list, read, update, or infer patient B by changing an ID or search term.
- Revoking an assignment removes access without waiting for the user's next login.
- Pagination totals and result counts include only in-scope records.
- Every role/permission combination is tested for allowed and denied fields.
- Routine demographic edits cannot alter identity-defining fields.
- Identity changes and merges enforce approval separation and preserve audit/history.
- Registration responses and error messages do not disclose unneeded patient fields.
- Audit records contain actor, action, target, timestamp, and reason where required, but no passwords, tokens, or unnecessary patient data.

## 5. Other security gates before production

The following SDD-07 controls are not certified by the patient API integration tests and remain separate production gates:

- Verify active-account status and immediate session invalidation on deactivation.
- Implement and test the 15-minute inactivity and 8-hour absolute session limits.
- Implement the LAN/offline-capable additional verification required for System Administrators and Authorized Signatories.
- Finalize role grants, prohibited role combinations, privileged permissions, and emergency access.
- Define audit retention, audit export permission/reason, and tamper-resistance controls.
- Complete MariaDB 10.11 provider/mapping/concurrency qualification and production database authorization.
- Complete security review, restore testing, TLS operations, supported OS/dependency servicing, and deployment hardening.

## 6. Release rule

Patient list/detail/update access must remain denied until the assignment model, role grants, field-level matrix, and cross-record authorization tests are approved and implemented. This document records an interim security hold; it does not approve the unresolved role matrix or authorize production data, database execution, or deployment.
