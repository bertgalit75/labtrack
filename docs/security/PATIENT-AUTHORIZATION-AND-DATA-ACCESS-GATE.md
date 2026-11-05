# Patient authorization and data-access security gate

**Status:** Examination/department scope implemented in application code; schema rollout and access administration remain gated  
**Applies to:** Patient Management API on `feature/patient-management-api`  
**Related design:** SDD-07, SDD-09, SDD-11

## 1. Confirmed access model

The approved model is **examination-based access intersected with department membership**. Separately authorized registration staff may register a patient using the `patients.register` permission; registration does not grant subsequent read or edit access.

A caller may read or update a patient only when all of the following are true at request time:

1. The authenticated caller has the operation permission (`patients.read` or `patients.demographics.update`).
2. A persisted, active `PatientExaminationAssignment` exists for that caller and patient.
3. The assignment is within its effective date interval.
4. The assignment references an actual examination whose `PatientId` matches the patient.
5. The caller has an active `UserDepartmentMembership` for the same department, also within its effective date interval.

A `patient_scope=all` claim, caller-supplied patient ID, query filter, or other client-controlled scope value cannot create or widen access. Search predicates are applied in the database before filtering, counting, sorting, pagination, and projection. Detail and update use the same predicate and return a non-disclosing 404 when the patient is outside scope. Revoking either assignment or department membership removes access on the next request.

## 2. Implementation

- Added persisted domain records for examination assignments, department membership, and departments.
- Added EF Core mappings, lookup indexes, restrictive relationships, and a composite examination/patient relationship to prevent an assignment from associating an examination with the wrong patient.
- Added the review-only MariaDB schema extension at `database/security/PatientExaminationDepartmentAccess.sql`. It is intentionally separate from the original consolidated install baseline until schema review and provider qualification are complete.
- Patient list, detail, and demographic-update service operations now require the authenticated user ID and apply the same persisted scope predicate.
- Patient list projection omits date of birth and sex; detail/update responses remain limited to patient ID, patient number, name, status, and ETag.
- Registration remains independently permission-gated and antiforgery-protected, and returns only patient ID, patient number, and ETag.
- No broad patient-scope claim is used by the implementation.

## 3. Operational provisioning boundary

The scope engine is implemented, but this branch does **not** yet provide an administrator-facing workflow for creating, approving, reviewing, or revoking department memberships and examination assignments. Those rows must not be populated by ad hoc client claims or self-service APIs. Before enabling real users, an authorized, audited access-administration workflow and its separation-of-duty rules must be implemented and tested. The review-only SQL extension must also be reconciled into the approved migration/install process and qualified on disposable MariaDB 10.11.

No default departments, department memberships, examination assignments, role grants, or permission grants are seeded. This avoids silently granting access. The user and department identifiers must come from trusted, provisioned records.

## 4. Required authorization tests

The acceptance suite must prove:

- Anonymous callers receive 401.
- Authenticated callers without the required operation permission receive 403.
- A broad `patient_scope=all` claim cannot grant access.
- A caller with no persisted assignment sees no patients.
- A caller assigned to patient A in department X can access A only while both the examination assignment and department membership are active and effective.
- A caller assigned to patient A cannot list, read, update, or infer patient B by changing an ID or search term.
- An assignment to a different department does not grant access unless the caller is a member of that same department.
- A mismatched examination/patient pair cannot grant access.
- Revoking or expiring either relation removes access on the next request.
- Pagination totals and result counts include only in-scope records.
- Every role/permission combination is tested for allowed and denied fields.
- Routine demographic edits cannot alter identity-defining fields.
- Identity changes and merges enforce approval separation and preserve audit/history.
- Registration responses and error messages do not disclose unneeded patient fields.
- Audit records contain actor, action, target, timestamp, and reason where required, but no passwords, tokens, or unnecessary patient data.

## 5. Other security gates before production

The following SDD-07 controls are not certified by the patient API tests and remain separate production gates:

- Implement and test an authorized access-administration workflow, including approval, review, revocation, and audit of department and examination assignments.
- Verify active-account status and immediate session invalidation on deactivation.
- Implement and test the 15-minute inactivity and 8-hour absolute session limits.
- Implement the LAN/offline-capable additional verification required for System Administrators and Authorized Signatories.
- Finalize role grants, prohibited role combinations, privileged permissions, and emergency access.
- Define audit retention, audit export permission/reason, and tamper-resistance controls.
- Complete MariaDB 10.11 provider/mapping/concurrency qualification and production database authorization.
- Complete security review, restore testing, TLS operations, supported OS/dependency servicing, and deployment hardening.

## 6. Release rule

The examination/department authorization predicate is implemented, but production access remains gated until the schema extension is migrated through the approved process, the assignment-administration workflow is available, role grants and field-level rules are approved, and cross-record authorization tests pass against MariaDB. This document does not authorize production data, database execution, or deployment.
