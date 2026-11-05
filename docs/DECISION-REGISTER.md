# LabTrack Decision Register

| ID | Decision | Current position | Status |
|---|---|---|---|
| D-001 | Release 1 schema table count | 48 tables | Accepted |
| D-002 | Initial database state | New database; no production migration | Accepted |
| D-003 | Database engine | MariaDB 10.11.x | Accepted |
| D-004 | Current server OS | Existing Windows 10 Pro | Confirmed |
| D-005 | Recommended production OS | Windows 11 Pro if hardware-compatible and supported | Recommended |
| D-006 | Deployment | LAN-only centralized server; browser clients | Accepted |
| D-007 | Concurrent users | 5–10 expected; verify by testing | Planning assumption |
| D-008 | API BIGINT JSON representation | Serialize as strings | Approved in SDD-05 |
| D-009 | API style | ASP.NET Core controllers | Approved in SDD-05 |
| D-010 | API errors | Problem Details style | Approved in SDD-05 |
| D-011 | Retry safety | Idempotency keys for retry-sensitive commands | Approved in SDD-05 |
| D-012 | Workflow API | Explicit command endpoints | Approved in SDD-05 |
| D-013 | Frontend navigation | Persistent sidebar | Approved in SDD-06 |
| D-014 | Dashboard | Role/permission-filtered | Approved in SDD-06 |
| D-015 | Result entry | Full-page workspace | Approved in SDD-06 |
| D-016 | Verification layout | Side-by-side review | Approved in SDD-06 |
| D-017 | Report preview | Similar to final printable PDF | Approved in SDD-06 |
| D-018 | List loading | Server-side pagination/filtering | Approved in SDD-06 |
| D-019 | Initial theme | Light theme first | Approved in SDD-06 |
| D-020 | Security design direction | SDD-07 | Approved |
| D-021 | Consolidated design/readiness baseline | SDD-08 v0.1 approved 2026-11-03; approval does not authorize implementation | Approved |
| D-022 | Proceed with SDD-03 SQL reconciliation | Documentation reconciliation authorized 2026-11-03; static reconciliation completed and SDD-03 v0.2 approved; MariaDB runtime validation remains open | Approved for documentation; runtime gate open |

| D-023 | SDD-03 static SQL inventory | 48 tables, 428 columns, 48 PKs, 33 unique keys, 58 secondary indexes, 98 FK declarations, 8 CHECK declarations in source SQL; counts are static, not runtime-verified | Approved as documentation baseline 2026-11-03; runtime validation remains open |

## Open decisions and gates

- Complete MariaDB 10.11 runtime validation before schema implementation.
- Final role/permission matrix and separation-of-duty rules.
- Finalize detailed examination and examination-test status dictionaries and exceptional transitions.
- Patient numbering and merge approval process.
- Final role/permission assignments for privileged MFA, audit exports, and emergency administration.
- Exact audit retention period under applicable laboratory policy.
- Backup tooling, destination, encryption, monitoring, restore-test cadence, and verification of RPO/RTO targets.
- TLS certificate issuance, trust installation, renewal, and monitoring procedure.
- Actual server hardware compatibility and supported OS servicing path.
- Detailed API contracts, database execution validation, wireframes, and test strategy.

See `docs/SDD/SDD-08_Consolidated-Design-and-Implementation-Readiness-Review.md`.

### Additional approved decisions

| ID | Decision | Current position | Status |
|---|---|---|---|
| D-024 | SDD-03 documentation approval | SDD-03 v0.2 approved 2026-11-03; MariaDB runtime validation remains a separate gate | Approved |
| D-025 | SDD-04 workflow decision review | P-01 through P-07 approved in simplified form on 2026-11-03; detailed status dictionaries and operational details remain to be finalized | Partially approved |
| D-026 | SDD-04 P-03 examination completion/reopen/cancel | Every test must be verified or properly cancelled for examination completion; reopen/cancel restricted, reasoned, and audited; detailed state labels remain under review | Approved (2026-11-03) |
| D-027 | SDD-04 P-04 result verification independence | Encoder/editor cannot verify the same result/revision; another authorized staff member must verify | Approved (2026-11-03) |
| D-028 | SDD-04 P-05 critical-result handling | Record attempts/acknowledgements; flag unresolved communication; follow lab escalation and report-release policy | Approved (2026-11-03) |
| D-029 | SDD-04 P-06 report signing/release | Authorized category-specific signers; signatory count and signer/releaser separation follow lab policy; amendments preserve prior report version | Approved (2026-11-03) |

| ID | Decision | Current position | Status |
|---|---|---|---|
| D-030 | SDD-04 P-01 patient information and merge approval | Identity-detail changes require elevated permission/reason; merge requester cannot approve own merge | Approved (2026-11-03) |
| D-031 | SDD-04 P-02 test catalog publication | Separate edit and publish authority; published definitions corrected through new versions | Approved (2026-11-03) |
| D-032 | SDD-04 P-07 cancellation authority | Authorized cancellation with reason; elevated authorization after verified/released work; preserve downstream history | Approved (2026-11-03) |

| ID | Decision | Current position | Status |
|---|---|---|---|
| D-033 | SDD-07 S-01 privileged-user MFA | Additional login verification/MFA for system administrators and authorized report signatories, using a reliable offline/LAN-capable method | Approved (2026-11-03) |
| D-034 | SDD-07 S-02 session limits | 15-minute inactivity timeout and 8-hour absolute session limit | Approved (2026-11-03) |
| D-035 | SDD-07 S-03 patient-data access | Restrict access to information needed for assigned duties | Approved (2026-11-03) |
| D-036 | SDD-07 S-04 separation of duties | No self-verification; merge requester cannot approve own merge; signer/releaser separation follows laboratory policy | Approved (2026-11-03) |
| D-037 | SDD-07 S-05 audit retention | Retain for applicable laboratory-policy period; no auto-deletion until period is defined; older-record review restricted | Approved (2026-11-03) |
| D-038 | SDD-07 S-06 backup/recovery | Daily backups, minimum 30-day retention, separate storage, regular restore tests; target RPO 24h/RTO 8h | Approved (2026-11-03) |
| D-039 | SDD-07 S-07 TLS certificate administration | Designated system administrator manages, installs, renews, and monitors certificate expiry; clients trust certificate | Approved (2026-11-03) |
| D-040 | SDD-07 S-08 audit exports | Only specifically authorized administrators/auditors may export; log actor, time, and reason; ordinary staff cannot export | Approved (2026-11-03) |
| D-041 | SDD-07 S-09 emergency recovery | Designated emergency administrator; separate secure credentials; identity verification; log and post-review every use/recovery | Approved (2026-11-03) |
| D-042 | SDD-07 S-10 supported OS path | Windows 11 Pro preferred if supported by hardware; Windows 10 Pro temporary/transition only with verified support; otherwise evaluate supported OS including Linux | Approved (2026-11-03) |


| D-045 | SDD-09 role-permission matrix | Seven role categories, permission baseline, default-deny, and separation-of-duty controls approved 2026-11-03; detailed record scopes remain open | Approved with remaining details open |
| D-046 | SDD-10 initial screen wireframes and acceptance criteria | Initial module screen structure and acceptance criteria approved 2026-11-03; open operational decisions remain documented | Approved |
| D-047 | SDD-11 detailed API contracts | Initial-module API contract baseline approved 2026-11-03; unresolved DTO fields, scopes, statuses, and policy details remain open | Approved with remaining details open |
| D-048 | SDD-12 initial test strategy | Test strategy and synthetic-data specification approved as a documentation baseline; no tests executed or passed by this document | Approved (documentation baseline only) |

| ID | Decision | Current position | Status |
|---|---|---|---|
| D-049 | Result value types | Support both numerical and qualitative result values. Numerical interpretation uses the selected published reference interval; qualitative interpretation uses catalog-configured options and interpretation metadata. No clinical ranges or reactive/positive classifications are invented by application code. | Confirmed by user (2026-11-04) |
| D-050 | Implementation start | Development implementation authorized 2026-11-04. This does not authorize production database execution, real patient-data loading, or deployment. | Authorized within stated boundary |
| D-051 | Initial implementation host | Existing Windows server PC remains the initial development/hosting target; Linux portability remains a future-proofing objective. Windows 11 Pro remains the recommended production upgrade path where supported. | Confirmed |
| D-052 | EF Core/MariaDB provider qualification | Do not pin an unverified provider. Qualify an EF Core provider against MariaDB 10.11.x, .NET 10, required migrations, type mappings, and concurrency behavior before persistent API startup. | Open technical gate |


| ID | Decision | Current position | Status |
|---|---|---|---|
| D-053 | Patient record scope | Patient read and routine demographic update require an active, effective examination assignment for the patient and active, effective membership in the same department. Patient registration remains a separate permission for authorized registration staff and does not itself grant patient read/edit scope. | Confirmed by user (2026-11-05) |
