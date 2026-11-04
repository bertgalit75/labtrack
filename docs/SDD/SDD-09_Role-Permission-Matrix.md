# SDD-09 — Role-Permission Matrix

**Version:** 0.1  
**Status:** Draft for user review — proposed role grants, not approved  
**Release:** 1  
**Date:** 3 November 2026  
**Depends on:** SDD-00, SDD-04, SDD-05, SDD-07

## 1. Purpose and status

This document translates the approved security and workflow policies into a proposed role-based permission model for LabTrack Release 1. It is a review draft. The role names originate from the project scope; the individual grants below are proposed defaults and require user approval before they become the authorization baseline.

This document does not authorize coding, database execution, or production access. Backend authorization remains authoritative. The system must deny access when a required permission is absent or cannot be evaluated.

## 2. Permission notation

| Mark | Meaning |
|---|---|
| R | Read/view within the user's assigned duties and permitted record scope |
| C | Create/register/encode a new record or work item |
| U | Update permitted non-historical fields or draft work |
| X | Execute a controlled workflow command |
| A | Approve/review an action or exception |
| S | Sign a report for an authorized report/test category |
| L | Release a report, only when separately authorized by laboratory policy |
| E | Export audit information; requires a dedicated permission and reason |
| — | No grant proposed by this role |

A mark is not a blanket grant to every record or field in that domain. Object-level, assignment, status, effective-date, and category checks still apply. Historical results, revisions, released reports, and audit records are not freely editable.

## 3. Proposed Release 1 roles

| Role | Intended responsibility | Privilege boundary |
|---|---|---|
| System Administrator (SA) | Technical operation: application configuration, account lifecycle, role assignment, TLS, backup/recovery, and system health | No routine patient/clinical-record access by default. Technical administration does not imply clinical authority. MFA required. |
| Laboratory Administrator (LA) | Laboratory operational configuration, test catalog stewardship, controlled operational oversight | No automatic report-signing authority. High-impact commands require specific permissions and reasons. |
| Reception/Registration Staff (RS) | Patient registration/correction within permitted fields and examination registration | No result encoding, verification, signing, release, or audit export. |
| Laboratory Staff (LS) | Assigned examination work, result entry/revision, submission, and critical-result communication recording | Cannot verify a result/revision they entered or edited. No report signing/release by default. |
| Verifying Staff (VS) | Independent review, verification, and return of results | Cannot verify the same result/revision they entered or edited. No report signing/release by default. |
| Authorized Signatory (AS) | Sign reports for explicitly authorized test/report categories | MFA required. Signatory eligibility and count are checked at signing time. Release is not implied by signing. |
| Auditor/Supervisor (AU) | Read-only oversight, audit review, and specifically authorized supervisory approvals | Auditor and supervisor capabilities should be assigned as separate permission packages; audit export is not automatic. |

A person may hold more than one role only where the combination does not defeat separation of duties. Role assignment must be approved and audited. The emergency administrator account is a controlled recovery mechanism, not an ordinary staff role.

## 4. Proposed role-permission matrix

The following grants are conservative starting proposals. “A” and “X” are limited to the named command and its policy conditions. Blank authority must not be inferred from a role title.

| Permission / action | SA | LA | RS | LS | VS | AS | AU |
|---|:---:|:---:|:---:|:---:|:---:|:---:|:---:|
| **Identity and access** ||||||||
| View/manage staff accounts and activation | X | R | — | — | — | — | R* |
| Assign/revoke roles and permissions | X | — | — | — | — | — | R* |
| Configure authentication, sessions, MFA policy | X | — | — | — | — | — | — |
| Use emergency recovery account | X† | — | — | — | — | — | — |
| **Patients** ||||||||
| Search/view patient demographics | — | R | R | R* | R* | R* | R* |
| Register patient | — | — | C | — | — | — | — |
| Correct routine demographics | — | A* | U | — | — | — | — |
| Change identity-defining fields with reason | — | A* | — | — | — | — | — |
| Request patient merge | — | X* | — | — | — | — | — |
| Approve patient merge (not requester) | — | A* | — | — | — | — | A* |
| Execute approved patient merge | — | X* | — | — | — | — | — |
| **Test catalog** ||||||||
| View test catalog and published versions | — | R | R | R | R | R | R* |
| Create/edit unpublished test definitions | — | U | — | — | — | — | — |
| Publish a test-definition version | — | A/X* | — | — | — | — | — |
| Deactivate future use of a test version | — | A/X* | — | — | — | — | — |
| **Examinations and results** ||||||||
| Register examination | — | — | C | — | — | — | — |
| View assigned examination/worklist | — | R* | R* | R* | R* | R* | R* |
| Update permitted registration details | — | U* | U* | — | — | — | — |
| Encode/edit result revision | — | — | — | C/U | — | — | — |
| Submit results for verification | — | — | — | X | — | — | — |
| Verify or return result revision | — | — | — | — | X | — | — |
| Reopen examination / controlled exception | — | A/X* | — | — | — | — | A* |
| Cancel examination/test before verified/released work | — | A/X* | — | — | — | — | — |
| Cancel after verified results or released report | — | A* | — | — | — | — | A* |
| **Critical-result communication** ||||||||
| Record communication attempt/acknowledgement | — | R* | — | C/X | R* | R* | R* |
| Review unresolved critical communication | — | A* | — | R* | R* | R* | A* |
| **Reports** ||||||||
| Prepare report draft / preview | — | R* | — | — | R* | R* | R* |
| Sign applicable report category | — | — | — | — | — | S | — |
| Release report | — | — | — | — | — | L* | — |
| Initiate report amendment | — | A/X* | — | — | — | — | A* |
| Distribute/reissue released report | — | X* | — | — | — | L* | — |
| **Audit and oversight** ||||||||
| View audit trail | — | R* | — | — | — | — | R* |
| Export audit records with reason | — | — | — | — | — | — | E* |
| View workflow event history | — | R* | R* | R* | R* | R* | R* |
| **Technical operations** ||||||||
| Manage TLS certificate and trust procedure | X | — | — | — | — | — | — |
| Run/monitor backup and restore operations | X | — | — | — | — | — | — |
| Manage application/system configuration | X | — | — | — | — | — | — |

**Legend notes:** `*` requires a separately assigned granular permission and/or a defined scope; `†` only through the controlled emergency-access procedure. `A/X` means the policy must decide whether approval and execution are held by the same role or separated. The matrix intentionally does not give the System Administrator routine clinical access.

## 5. Mandatory separation-of-duty and contextual controls

1. **Result verification:** A user who entered or edited a result cannot verify that same result/revision. Another authorized verifier must perform verification. This is a mandatory rule from SDD-04 P-04, not merely a role-matrix preference.
2. **Patient merge:** The requester cannot approve their own merge. A second authorized approver is required. The executor and approver may be the same person only if the final laboratory policy explicitly permits it and the requester/approver separation remains intact; this needs confirmation.
3. **Report signing/release:** Only active signatories authorized for the applicable category may sign. Required signatory count and whether the signer may also release are determined by laboratory policy. The `L` grant is therefore not automatic with `S`.
4. **Cancellation:** Cancellation requires an authorized command and reason. After verified results or released reports exist, elevated authorization is required and downstream history must be preserved.
5. **Test catalog:** Editing and publishing are separate permissions. Published versions are immutable; corrections create a new version.
6. **Audit export:** Export requires a dedicated permission, recorded actor/time/reason, and must not be granted to ordinary staff by default.
7. **Privileged authentication:** System Administrators and Authorized Signatories require the approved LAN/offline-capable additional login verification/MFA mechanism.
8. **Patient privacy:** Every patient/clinical read is limited to assigned duties and relevant record scope; a role grant alone does not authorize unrestricted browsing.
9. **Emergency access:** Emergency administrator credentials are separate from normal accounts. Every use is logged and reviewed afterward; routine access must not use this account.
10. **Default deny:** Unlisted actions and unresolved grants are denied until explicitly assigned and tested.

## 6. Permission-code proposal

Use stable permission identifiers independent of UI labels. Proposed initial codes:

- `staff.accounts.read`, `staff.accounts.manage`, `access.roles.assign`, `access.policy.manage`
- `patients.read`, `patients.register`, `patients.demographics.update`, `patients.identity.update`, `patients.merge.request`, `patients.merge.approve`, `patients.merge.execute`
- `catalog.tests.read`, `catalog.tests.edit`, `catalog.tests.publish`, `catalog.tests.deactivate`
- `examinations.read`, `examinations.register`, `examinations.update`, `examinations.reopen`, `examinations.cancel`, `examinations.cancel.elevated`
- `results.read`, `results.encode`, `results.revise`, `results.submit`, `results.verify`, `results.return`
- `critical-results.record`, `critical-results.review`
- `reports.prepare`, `reports.sign`, `reports.release`, `reports.amend`, `reports.distribute`
- `audit.read`, `audit.export`, `workflow-events.read`
- `system.configuration.manage`, `system.tls.manage`, `system.backup.manage`, `system.recovery.emergency`

The final permission catalog may split these into finer-grained permissions after API and screen contracts are detailed. Permission identifiers are proposals, not a database seed list.

## 7. Decisions required from the laboratory before approval

1. Confirm the seven role names and whether Auditor and Supervisor should be separate roles rather than one role with separate permission packages.
2. Confirm whether Laboratory Administrator may request, approve, and execute patient merges, and whether approval and execution must be separate people.
3. Identify which roles may access patient demographics, clinical results, and reports, and whether access is limited by department, assigned worklist, or examination assignment.
4. Confirm who may correct routine demographics and which identity fields require elevated approval.
5. Confirm whether Laboratory Administrator alone may publish test versions, or whether a second approver is required.
6. Identify who may reopen/cancel examinations and tests, and who may approve cancellation after verified/released work.
7. Confirm which roles may record critical-result communication and which role reviews unresolved cases.
8. Define report signatory categories, required signature count, and whether signers may release reports.
9. Identify the exact audit reviewer and audit-export roles; confirm whether export needs a second approval.
10. Confirm whether System Administrator may ever receive clinical-data access, and under what audited break-glass conditions.
11. Confirm whether users may hold multiple roles and define prohibited role combinations.
12. Confirm which staff may administer user accounts versus assign roles/permissions; these should be separate capabilities where practical.

## 8. Acceptance criteria for this matrix

- [ ] User approves final role names and role definitions.
- [ ] Every permission code has an owner and a documented business purpose.
- [ ] Every role grant is explicitly approved; all other permissions default to deny.
- [ ] Patient/clinical access scope is defined and testable.
- [ ] Result self-verification is blocked at the service/API layer.
- [ ] Patient merge requester/approver separation is enforced.
- [ ] Signatory category, credential validity, signature count, and release authority are enforced.
- [ ] Cancellation and reopening require appropriate authority, reason, and audit evidence.
- [ ] Audit export is separately authorized and logged.
- [ ] MFA applies to System Administrators and Authorized Signatories using a tested LAN/offline method.
- [ ] Automated authorization tests cover allowed, denied, and cross-record access attempts.

## 9. Approval status

**Not approved.** This v0.1 matrix is submitted for review. Approval should be recorded only after the open decisions in Section 7 are resolved or explicitly deferred with a documented interim policy. Approval of SDD-07 S-01 through S-10 establishes security policy direction but does not approve these proposed role grants. Implementation remains gated by SDD-08 and requires separate explicit implementation-start approval.
