# SDD-04 — Application Business Rules and Workflow State Transitions

**Version:** 0.4 (decision review update)  
**Status:** Draft — P-01 through P-07 approved; detailed status dictionaries and operational policy details remain to be finalized  
**Release:** 1  
**Prepared:** 2026-11-03  
**Supersedes:** v0.2 decision review (retained in Git history)

## 1. Purpose and decision status

This document defines the business rules and state-transition decisions required before implementation contracts and workflows can be frozen. It carries forward the accepted source-defined baseline and separates it from proposed policies that require owner approval.

**Important:** Items labelled *Proposed for approval* are recommendations, not approved laboratory policy. Do not implement them as final policy until explicitly approved. The document does not authorize coding or database execution.

## 2. Global rules — accepted design baseline

1. Backend rules are authoritative; frontend controls are not security boundaries.
2. State transitions are validated server-side against the current persisted state and the actor's permissions.
3. Consequential state changes, history records, and workflow events are committed in the same database transaction where applicable.
4. Use optimistic concurrency or equivalent safeguards for conflicting updates.
5. Preserve result revisions, released report history, and audit evidence; ordinary users cannot rewrite historical records.
6. Retry-sensitive commands must be idempotent as defined in SDD-05.
7. Store event timestamps in UTC and display them in the configured laboratory timezone.

## 3. Patient management

### 3.1 Accepted baseline
- Examination registration requires an Active canonical patient.
- Potential duplicate records enter a controlled review process; they are not silently combined.
- Patient merges must be authorized, traceable, and cycle-safe.

### 3.2 Proposed policy for approval
- Routine demographic corrections may be made by a permitted user with before/after audit evidence and a reason for designated sensitive fields.
- Changes to identity-defining fields (for example, legal name or date of birth) should require a reason and elevated permission; whether a second approver is required is an owner decision.
- A merge requester should not be the sole approver of that merge. A merge should be blocked when either record has unresolved workflow conditions defined by policy; historical examinations and reports remain attributable and discoverable.
- Merge execution should be a dedicated command, not a generic patient update.

**Decision P-01 — Approved (2026-11-03):** Authorized staff may correct ordinary patient information. Changes to important identity details (such as legal name or date of birth) require elevated permission and a recorded reason. Patient merges require a second authorized approver; the requester cannot approve their own merge. The detailed field-level permission matrix remains to be specified.

## 4. Test catalog

### 4.1 Accepted baseline
- An examination test preserves the exact `TestDefinitionVersionId` selected at registration.
- Published versions are immutable historical references; corrections are made by creating a new version.
- Publication requires authorization and validation.

### 4.2 Proposed policy for approval
- Only a published version within its effective period may be selected for a new examination test.
- Retiring or superseding a catalog version must not alter existing examination tests.
- A test version with incomplete required parameters, invalid reference ranges, or unresolved required metadata must not be published.

**Decision P-02 — Approved (2026-11-03):** Authorized staff may create and edit test definitions. A separately authorized person must publish a definition before it is used for new examinations. Published definitions are not silently edited; corrections create a new version.

## 5. Examination and examination-test lifecycle

### 5.1 Accepted source-defined baseline
- Only Active canonical patients may be selected.
- Monthly examination number format: `EX-YYYYMM-000001`, allocated through transaction-safe `NumberingCounters`; gaps are acceptable, duplicates are not.
- Add currently applicable published test catalog versions and preserve each selected `TestDefinitionVersionId`.
- Repeated test instances are distinguished with `TestInstanceNumber`.
- Record status changes with actor, timestamp, and reason.
- Cancellation is controlled and must preserve downstream result/report history.
- Independent specimen management and specimen numbering are excluded from Release 1.
- Validate status transitions in the application and record `ExaminationStatusHistory` and `WorkflowEvents` in the same transaction.

### 5.2 Proposed examination statuses

| Status | Meaning | Proposed permitted next states |
|---|---|---|
| Draft | Registration started but not finalized | Registered, Cancelled |
| Registered | Examination number assigned and registration committed | InProgress, Cancelled |
| InProgress | At least one examination test is being worked | PartiallyCompleted, Completed, Cancelled |
| PartiallyCompleted | Some tests complete while others remain active | InProgress, Completed, Cancelled |
| Completed | All required examination tests have reached an allowed terminal outcome | Reopened (restricted), Amended only through the relevant result/report workflows |
| Cancelled | Examination cancelled through an authorized command | No ordinary transition; administrative correction only by controlled reversal/event |

These are proposed labels and semantics. Whether `Draft` is persisted or exists only in the UI/API is also to be decided. `Amended` is not recommended as an examination status; amendments belong to result/report history.

### 5.3 Proposed examination-test statuses

| Status | Meaning | Proposed permitted next states |
|---|---|---|
| Pending | Added to examination, work not started | InProgress, Cancelled |
| InProgress | Result work has started | ForVerification, Cancelled |
| ForVerification | A result revision has been submitted for verification | InProgress (returned), Verified |
| Verified | Required verification completed | InProgress (controlled correction/revision), Cancelled only under restricted policy |
| Cancelled | Test instance cancelled with reason | No ordinary transition |

**Decision P-03 — APPROVED (2026-11-03, simplified policy):** An examination is complete only when every test instance is either verified or properly cancelled. Reopening or cancellation is restricted to authorized users, requires a reason, and must be recorded in the audit/workflow history. The detailed status labels and permitted transition table remain subject to final consistency review; this approval does not settle every exceptional transition.

## 6. Result encoding, revision, and verification

### 6.1 Accepted baseline
- Result values must conform to the parameter's configured value type and applicable validation rules.
- Corrections create a new revision; previously persisted revisions are retained.
- Each revision records actor, timestamp, status, and reason where required.
- A submission captures the exact result revisions submitted for review.
- Verification decisions and return reasons are recorded as traceable events.

### 6.2 Proposed separation-of-duties policy
- A user who encoded or materially edited a result revision must not verify that same revision.
- A returned result may be corrected by an authorized encoder, creating a new revision; the new revision must be submitted and verified.
- A verifier must review the values, units, reference intervals/flags where applicable, patient/examination identity, and relevant history required by the laboratory's procedure.
- Administrative override, if ever permitted, must be exceptional, permission-restricted, reasoned, and audited; it must not silently bypass verification.

**Decision P-04 — APPROVED (2026-11-03):** A user who entered or edited a result cannot verify that same result. Another authorized staff member must verify it. The restriction is applied to the same result/revision, not automatically to every test in the examination.

## 7. Critical-result communication

### 7.1 Accepted baseline
- Critical-result communication attempts and acknowledgements are recorded as traceable events.

### 7.2 Proposed policy
- Critical-result identification follows configured test-specific criteria; the system must not invent clinical thresholds.
- Record the critical value/flag, communication recipient and role, method, attempt time, outcome, acknowledgement time, and user recording the event, subject to privacy controls.
- Failed contact attempts must be recorded with outcome and next escalation action.
- The laboratory must define an escalation ladder and time targets outside the software's assumptions.

**Decision P-05 — APPROVED (2026-11-03, simplified policy):** The system records critical-result communication attempts and acknowledgements and clearly flags unresolved communication. Staff follow the laboratory's escalation procedure. Report-release handling follows the laboratory's approved policy; the software will not invent a universal hard stop or clinical escalation rule.

## 8. Report signing, release, amendment, and distribution

### 8.1 Accepted baseline
- Report content is generated from eligible verified submissions.
- Signing records preserve signatory identity and credential/authorization snapshots.
- Release is a controlled action.
- Amendments create traceable replacement releases; prior releases remain preserved.

### 8.2 Proposed policy
- Report release requires all included results to meet verification requirements and any applicable critical-result disposition policy.
- Signatory eligibility is evaluated at signing time using the staff member's active role/credential/authorization and effective dates.
- A report amendment must identify the prior release, reason, initiating actor, approval/signature evidence, and replacement release. The prior report remains retrievable and clearly marked superseded, not deleted.
- Distribution events record recipient/channel, actor, time, and outcome where required by policy.

**Decision P-06 — APPROVED (2026-11-03, simplified policy):** Only staff authorized for the relevant test/report category may sign. The required number of signatories, and whether the signer may also release the report, are determined by laboratory policy. An amendment creates a new report version and preserves the previous release.

## 9. Cancellation and downstream preservation

**Decision P-07 — Approved (2026-11-03):** Only authorized staff may cancel an examination or test. A reason is required and recorded. Cancellation after verified results or a released report requires elevated authorization. Existing results, verification records, and released reports are preserved; cancellation never physically deletes them. Implementation controls:
- Require a reason and record actor/time.
- Prevent ordinary cancellation after verified results or released reports exist.
- If cancellation after downstream work is clinically/administratively necessary, use a dedicated elevated command that preserves all result revisions, verification events, report releases, and distribution history, and records the effect on each downstream item.
- Never physically delete the examination or its historical records as a cancellation mechanism.

## 10. Proposed permission and separation-of-duty matrix

This is a policy proposal for review, not a final role matrix. Exact named roles and permissions are to be finalized in the access-control matrix under SDD-07.

| Action | Proposed control |
|---|---|
| Register examination | Registration permission; active canonical patient required |
| Edit patient identity fields | Elevated patient-edit permission; reason and audit |
| Merge patients | Dedicated permission; requester/approver separation recommended |
| Draft/edit test catalog | Catalog-maintenance permission |
| Publish test version | Separate publish permission recommended |
| Encode result | Result-encode permission |
| Verify result | Result-verify permission; cannot verify own revision recommended |
| Record critical communication | Critical-result communication permission |
| Sign report | Active signatory authorization for applicable report category |
| Release report | Separate release permission or signatory permission as approved |
| Amend report | Dedicated amend permission; reason and approval policy |
| Override/reopen/cancel | Elevated permission, reason, audit, and restricted scope |
| View/export audit | Explicit audit-read/export permissions |

## 11. State transition implementation requirements

For each command, the API/application service must:
1. Load the target and relevant related records under appropriate concurrency protection.
2. Authenticate the actor and authorize the specific action and object.
3. Validate current state, requested transition, required fields, and separation-of-duty rules.
4. Validate cross-record invariants (same patient/examination/test/revision/signatory context as applicable).
5. Persist the state change, history row, and workflow event atomically.
6. Return a conflict response for stale/concurrent state rather than silently overwriting.
7. Avoid exposing sensitive patient/result details in logs or error responses.

## 12. Decision register for this review

| ID | Decision requiring owner response | Recommended starting position | Status |
|---|---|---|---|
| P-01 | Patient information changes and merge approval | Elevated permission and reason for identity details; separate merge requester and approver | Approved (2026-11-03) |
| P-02 | Test catalog publication authority | Separate edit and publish permissions; published definitions corrected through new versions | Approved (2026-11-03) |
| P-03 | Examination completion, reopening, and cancellation policy | Every test verified or properly cancelled; restricted reopen/cancel with reason and audit; detailed state table review remains | Approved (simplified policy, 2026-11-03) |
| P-04 | Encoder may verify own work | No; another authorized staff member verifies the same result/revision | Approved (2026-11-03) |
| P-05 | Critical-result communication and report-release handling | Record attempts/acknowledgements; flag unresolved cases; follow laboratory escalation and release policy | Approved (2026-11-03) |
| P-06 | Report signing, signatory count, and release | Authorized category-specific signers; count and signer/releaser separation follow laboratory policy; preserve prior report on amendment | Approved (2026-11-03) |
| P-07 | Cancellation authority and downstream restrictions | Authorized cancellation; reason; elevated approval after verified/released work; preserve history | Approved (2026-11-03) |

## 13. Acceptance criteria for SDD-04

SDD-04 may be approved when:
- The examination and examination-test status dictionaries and every allowed transition are accepted.
- Completion, reopening, and cancellation rules are unambiguous.
- Encoder/verifier separation is decided.
- Critical-result communication and release dependency is decided.
- Report signatory and release responsibilities are decided.
- Patient demographic approval and merge separation are decided.
- Rules are consistent with SDD-03 data constraints, SDD-05 command API design, SDD-06 screen behavior, and SDD-07 access controls.
- Every consequential transition has actor, UTC timestamp, reason where required, and immutable/append-only history treatment defined.

## 14. Explicit exclusions and limitations

- No independent specimen-management workflow or specimen numbering in Release 1.
- No clinical threshold or critical-result escalation timing is invented by this document.
- No final staff role/permission matrix is established here.
- No SQL execution, production database operation, or application implementation is authorized by this draft.

---

**Review status:** P-01 through P-07 have been approved in simplified form on 2026-11-03. Detailed status dictionaries, exact permission assignments, and laboratory-specific operational procedures remain to be specified. This approval does not authorize coding or database execution.
