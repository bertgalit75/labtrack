# SDD-08 — Consolidated Design and Implementation Readiness Review

**Version:** 0.1  
**Status:** Approved — Design Baseline Accepted  
**Release:** 1  
**Date:** 3 November 2026

## 1. Purpose

This document consolidates the current LabTrack design baseline and identifies inconsistencies, unresolved decisions, validation work, and approval gates that must be addressed before implementation begins.

It is a readiness review, not authorization to start coding.

## 2. Current SDD approval register

| ID | Document | Current status | Notes |
|---|---|---|---|
| SDD-00 | Project Overview and Scope | Approved with notes | Windows 10 Pro is current; Windows 11 Pro remains recommended production upgrade |
| SDD-01 | System Architecture and Deployment | Approved with notes | Same OS note; hardware and deployment details remain open |
| SDD-02 | Database Design and Data Architecture | Approved | 48-table baseline accepted |
| SDD-03 | Data Dictionary and Database Constraints | Approval to reconfirm | Detailed source-defined constraints must be distinguished from proposed/application-enforced rules |
| SDD-04 | Application Business Rules and Workflow State Transitions | Accepted to proceed | Workflow decisions remain open |
| SDD-05 | API Contracts and Application Service Design | Approved | Proposed endpoint inventory still needs detailed contract definitions |
| SDD-06 | Frontend Architecture, Navigation, Screens, and UI Behavior | Approved | Screen behavior depends on finalized workflows |
| SDD-07 | Security, Privacy, Audit, and Access-Control Design | Approved | Security policy decisions remain open |
| SDD-08 | Consolidated Design and Implementation Readiness Review | Draft | This document |

An approved document does not automatically close its open decisions.

## 3. Consolidated system baseline

### 3.1 Technology

- Frontend: React + TypeScript.
- Backend: ASP.NET Core Web API.
- Database: MariaDB 10.11.x.
- Browser clients use a centralized LAN-hosted application.
- Desktop-first user interface.
- Expected concurrent users: 5–10, subject to performance testing.

### 3.2 Deployment

- Existing dedicated PC is the initial server.
- Windows 10 Pro is the current existing OS.
- Windows 11 Pro is the recommended production upgrade path if compatible and properly supported.
- LAN-only operation; normal application use must not require internet access.
- Future Linux migration is a portability objective, not part of Release 1 deployment.
- No direct MariaDB access from client workstations.

### 3.3 Architecture

- Centralized three-tier logical architecture.
- Modular-monolith backend.
- Same-origin frontend and API preferred.
- API base path `/api/v1`.
- Backend authorization and business rules are authoritative.
- Reports/documents stored outside the public web root.

### 3.4 Database

- MariaDB 10.11.x.
- 48-table accepted review baseline.
- New database; no production data migration currently required.
- SQL remains a review artifact until disposable-instance execution and validation are completed.
- Cross-table invariants and workflow constraints require explicit implementation and testing.

### 3.5 Workflow baseline

- Patient must be Active and canonical to be selected for examination registration.
- Examination number format: `EX-YYYYMM-000001`.
- Numbering must be transaction-safe; gaps acceptable, duplicates not.
- Examination tests preserve the selected test-definition version.
- Result changes create revisions rather than overwrite history.
- Submission captures exact result revisions.
- Verification, critical communication, signing, report release, amendment, and distribution are controlled and auditable.
- Independent specimen management is excluded from Release 1.

## 4. Cross-document consistency checks

The following are mandatory review checks:

| Area | Consistency requirement | Current finding |
|---|---|---|
| Identity | SDD-01, 05, 06, 07 use the same cookie-based authentication model | Direction is consistent; detailed cookie/session policy remains open |
| Authorization | Backend enforcement is authoritative in API and UI documents | Consistent |
| IDs | BIGINT identifiers are strings in JSON and handled safely by TypeScript | Approved in SDD-05; implementation mapping still needs tests |
| Workflows | API commands and UI actions map to approved server-side transitions | Not complete; SDD-04 decisions remain open |
| Result history | Result revisions and submission snapshots remain immutable | Consistent as a design rule; enforcement needs implementation tests |
| Report release | UI preview/sign/release distinction maps to backend release commands | Consistent direction; signatory and critical-result policy remain open |
| Database | SDD-02 accepts 48 tables; SDD-03 dictionary reflects exact SQL | 48 accepted; SDD-03 constraint reconciliation remains |
| Security | File access, audit, session, and backup rules align across architecture and security documents | Direction is consistent; operational values remain open |
| Deployment | LAN-only, no direct DB access, internet-independent normal operation | Consistent |

## 5. Release-blocking decision register

The following decisions should be resolved before implementation of the affected modules. Recommendations are not approvals.

### 5.1 Roles and permissions

- Confirm final staff role names.
- Define permission codes and role mappings.
- Define object-level access restrictions.
- Define separation of duties for verification, patient merge, access administration, and report signing/release.

### 5.2 Patient management

- Confirm patient-number format and allocation policy.
- Define which demographic fields require approval.
- Define merge requester, approver, and executor separation.
- Define merge reversal policy and downstream record behavior.

### 5.3 Examination and result workflows

- Approve examination and examination-test status dictionaries.
- Approve permitted state transitions and cancellation rules.
- Decide whether a result encoder may verify their own submission.
- Define result revision reasons and correction controls.
- Confirm exact submission and return behavior.

### 5.4 Critical results and reports

- Decide whether critical-result acknowledgement is mandatory before report release or whether documented escalation is sufficient.
- Define required signatory roles and number of signatures.
- Define whether report signer and releaser must be different people.
- Define amendment request and approval requirements.
- Approve report numbering and final report layout requirements.

### 5.5 Security and operations

- Decide MFA scope.
- Set idle and absolute session timeout.
- Set audit retention.
- Set backup schedule, retention, RPO, and RTO.
- Define TLS certificate administration and client trust deployment.
- Confirm Windows servicing/upgrade path.
- Inspect server hardware and define storage capacity.

## 6. Required technical validation before implementation

### 6.1 Database validation

- Execute the review SQL on a disposable MariaDB 10.11 instance.
- Verify table creation, indexes, foreign keys, defaults, and data types.
- Inspect `SHOW CREATE TABLE` output.
- Reconcile the data dictionary against the exact SQL.
- Verify EF Core and MariaDB provider compatibility.
- Test numbering concurrency and cross-table integrity.
- Test merge/amendment cycle prevention.
- Test backup and restore including stored documents.

### 6.2 API validation

- Produce detailed endpoint and DTO contracts.
- Define exact Problem Details error codes and validation conventions.
- Define authentication, authorization, antiforgery, and session behavior.
- Define idempotency-key scope and retention.
- Define concurrency conflict behavior.
- Verify BIGINT string serialization.

### 6.3 Frontend validation

- Produce screen-level wireframes for major workflows.
- Confirm role-specific navigation and dashboard behavior.
- Define the shared status dictionary and UI presentation rules.
- Define loading, empty, error, retry, and concurrency-conflict states.
- Confirm browser compatibility for the target workstation environment.

### 6.4 Security validation

- Create a role-permission matrix.
- Create a threat model.
- Define audit event catalog and redaction policy.
- Define secure file upload/download requirements.
- Define session and account recovery policies.
- Define backup protection and recovery procedure.

## 7. Implementation readiness gates

Implementation should begin only when all applicable gates are satisfied:

- [ ] SDD-03 is explicitly approved after source SQL constraint reconciliation.
- [ ] SDD-04 workflow decisions required for the first implementation modules are resolved.
- [ ] SDD-07 security policy decisions required for the first implementation modules are resolved.
- [ ] Role-permission matrix is approved.
- [ ] Database schema has been executed and validated on disposable MariaDB 10.11.
- [ ] Detailed API contracts are approved for the first implementation modules.
- [ ] Major screen wireframes and acceptance criteria are approved.
- [ ] Actual server hardware and OS support path are assessed.
- [ ] Backup and restore approach is defined.
- [ ] Initial test strategy and synthetic test data approach are approved.

## 8. Proposed implementation sequence after readiness approval

1. Repository and development environment setup.
2. Database schema validation and migration baseline.
3. Backend foundation, health checks, configuration, and logging.
4. Authentication, staff profiles, roles, and permissions.
5. Patient management.
6. Test catalog and version publication.
7. Examination registration and worklists.
8. Result encoding, revision, and submission.
9. Verification and critical-result communication.
10. Report preparation, signing, release, amendment, and distribution.
11. Audit review, security hardening, performance testing, backup/restore validation, and deployment.

The sequence may be refined after workflow and dependency review.

## 9. Approval record

**Approved by user:** 3 November 2026  
**Approval scope:** Consolidated design baseline, cross-document consistency findings, open-decision register, technical validation requirements, and implementation-readiness gates.

Approval authorizes continuation of specification, reconciliation, and validation work. It does not close the listed open decisions, certify the SQL as executable, or authorize application implementation. A separate explicit implementation-start approval is required after the release-blocking decisions and validation gates are satisfied.


## 10. Post-approval status update (3 November 2026)

This addendum records decisions made after the original SDD-08 v0.1 approval; it does not change the original approval scope or close remaining readiness gates.

- SDD-03 v0.2 is approved as a documentation baseline; MariaDB runtime validation remains outstanding.
- SDD-04 decisions P-01 through P-07 are approved in simplified form; detailed state dictionaries and laboratory-specific operational details remain open.
- SDD-07 decisions S-01 through S-10 are approved; detailed operational procedures and verification remain open.
- SDD-09 v0.1 Role-Permission Matrix has been prepared for review. Its proposed role grants are not approved.
- Implementation remains blocked until the readiness gates are satisfied and separate explicit implementation-start approval is given.


## Subsequent design-package status (2026-11-03)

- SDD-09 Role-Permission Matrix: approved as a design baseline; contextual access scopes remain open.
- SDD-10 Initial Module Screen Wireframes and Acceptance Criteria: approved.
- SDD-11 Detailed API Contracts for Initial Modules: approved with documented open details.
- SDD-12 Initial Test Strategy and Synthetic Test-Data Specification: draft for review.
- Runtime database validation, actual hardware/OS assessment, backup/restore verification, and final detailed workflow/policy decisions remain gates. Implementation still requires separate explicit user authorization.


## Implementation authorization addendum (2026-11-04)

The user separately authorized development implementation on 4 November 2026. SDD-11 remains approved with documented open details. SDD-12 is approved as a test-strategy and synthetic-data documentation baseline only; no tests have been executed or passed. This implementation authorization permits development code and controlled synthetic-data work, but does not authorize production database execution, real patient-data loading, or deployment. Existing technical and operational gates remain open and are tracked in `docs/implementation/IMPLEMENTATION-STATUS-2026-11-04.md`.
