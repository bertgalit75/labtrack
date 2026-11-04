# SDD-07 — Security, Privacy, Audit, and Access-Control Design

**Version:** 0.2  
**Status:** Approved design baseline — policy decisions S-01 through S-10 approved; implementation details and verification gates remain open  
**Release:** 1  
**Date:** 3 November 2026

## 1. Purpose

Define the security and privacy controls for LabTrack Release 1, including identity, authentication, authorization, patient-data protection, auditability, session security, file handling, and LAN deployment.

This is a design draft. It does not certify legal or regulatory compliance and does not authorize implementation.

## 2. Security objectives

1. Prevent unauthorized access to patient, examination, result, and report information.
2. Ensure users can perform only actions permitted by their assigned roles and effective permissions.
3. Preserve accountability for consequential actions.
4. Protect credentials, sessions, files, and backups.
5. Minimize exposure of sensitive information in logs and error responses.
6. Maintain secure operation on a LAN without requiring internet connectivity.

## 3. Identity and authentication

- Use ASP.NET Core Identity as the proposed identity foundation.
- Store password hashes only; never store plaintext passwords.
- Use secure password-hashing defaults supported by the selected framework version.
- Use generic login failure messages to reduce account enumeration.
- Apply configurable failed-login throttling/lockout.
- Require administrative controls for account activation, deactivation, and recovery.
- Do not permit shared staff accounts.
- Record authentication events appropriate for security monitoring.
- Require additional login verification/MFA for system administrators and authorized report signatories, using a method that works reliably offline/on the LAN. The exact mechanism and enrollment/recovery procedure must be specified and tested.

## 4. Session and browser security

- Prefer same-origin application/API hosting.
- Use Secure, HttpOnly, appropriately SameSite-configured authentication cookies.
- Require antiforgery protection for state-changing cookie-authenticated requests.
- Do not store credentials or bearer tokens in localStorage or sessionStorage.
- Enforce a 15-minute inactivity timeout and an 8-hour absolute session limit. Test expiry, renewal, logout, and server-side invalidation behavior.
- Clear sensitive client cache/state on logout and session expiry.
- Use HTTPS even on the LAN. A designated system administrator is responsible for managing, installing, renewing, and monitoring the internal TLS certificate and its expiry. Client machines must trust the certificate through a managed installation process.
- Apply suitable security headers and a restrictive Content Security Policy where compatible.

## 5. Authorization model

- Use role-based access with granular permissions.
- Support user-specific permission effects only under controlled administrative policy.
- Backend authorization is authoritative for every protected API operation.
- Frontend visibility is a usability feature, not a security boundary.
- Deny by default when a permission is absent or cannot be evaluated.
- Apply object-level checks so a user cannot access a record merely by changing an identifier.
- Separate high-impact duties where required, especially result verification, patient merge, authorization administration, and report signing/release.

### 5.1 Proposed permission domains

- Patient read/register/update/approve/merge.
- Test catalog read/create/edit/publish/deactivate.
- Examination read/register/update/cancel.
- Result read/encode/revise/submit.
- Result verify/return.
- Critical-result communicate/acknowledge/review.
- Report prepare/sign/release/amend/distribute.
- Staff and access administration.
- Audit read/export.
- System configuration and backup administration.

Exact permission codes, role mappings, and separation-of-duty rules remain open.

## 6. Patient and clinical data privacy

- Collect and display only information required for the user's task.
- Restrict patient and clinical data access to information needed for each user’s assigned duties, enforced by backend authorization and object-level checks.
- Avoid placing patient names, result values, or other sensitive details in URLs.
- Avoid sensitive data in browser console logs, telemetry, exception messages, and audit descriptions.
- Redact sensitive fields from audit before/after payloads unless explicitly justified and authorized.
- Use synthetic data in development and automated tests.
- Do not copy production patient data into developer workstations or Git repositories.
- Define retention, archival, correction, and disclosure procedures before production use.

## 7. Audit and workflow accountability

Record, as appropriate:

- Actor identity.
- Action/event type.
- Entity type and identifier.
- UTC timestamp.
- Reason or comments where required.
- Correlation ID.
- Relevant before/after values, with privacy redaction.
- Source context such as IP address when justified.

Audit records must be read-only through ordinary application functions. Protect them against alteration through application controls, database permissions, and operational safeguards. Retain audit records for the applicable laboratory-policy retention period. Do not automatically delete records until that period has been formally defined. Older records may be reviewed only by specifically authorized personnel.

Workflow events and audit logs serve related but distinct purposes: workflow events describe business-process transitions; audit logs describe accountable actions and changes.

## 8. File and report security

- Store reports and supporting files outside the public web root.
- Use opaque storage keys rather than user-controlled filesystem paths.
- Validate file type, size, and content as appropriate.
- Compute and verify SHA-256 checksums.
- Do not trust the uploaded filename or MIME type alone.
- Authorize every file download through the application.
- Prevent path traversal and arbitrary file access.
- Protect temporary/staging files and clean up abandoned uploads.
- Back up document storage together with the database and test restoration.

## 9. LAN and server security

- Restrict access to the application host to the trusted LAN.
- Use Windows Firewall rules for only required ports.
- Do not expose MariaDB directly to client workstations.
- Bind MariaDB to localhost or the minimum required server-side interface.
- Use a dedicated least-privilege database account for the application.
- Separate application runtime identity from administrative access.
- Keep OS, .NET runtime, MariaDB, and dependencies on supported security updates.
- Maintain secure configuration and certificate/key storage. Audit exports are restricted to specifically authorized administrators/auditors; record who exported, when, and why. Ordinary staff cannot export audit records. Exact permission assignments must be included in the role-permission matrix.
- Windows 11 Pro remains the recommended production OS upgrade path if compatible; Windows 10 Pro is the current existing OS and requires a verified supported servicing arrangement.

## 10. Backup, recovery, and continuity

Back up:

- MariaDB database.
- Report and document storage.
- Application configuration required for recovery.
- Certificate and key material under a separate secure procedure.

Perform daily backups and retain at least 30 days of backups. Store backups on separate physical or network storage, protected against unauthorized access and ransomware. Conduct regular restore tests. Planning targets are RPO 24 hours and RTO 8 hours; verify these targets through timed recovery exercises. A backup is not considered validated until a restore test has succeeded. The exact backup tooling, schedule execution time, storage location, encryption, monitoring, and restore-test cadence must be documented.

## 11. Account recovery and emergency administration

- Maintain a designated emergency administrator account for recovery when ordinary administrative access is unavailable.
- Store emergency recovery credentials securely and separately from normal staff credentials.
- Verify the identity and authority of anyone requesting emergency access or account recovery.
- Log every emergency access and recovery action, including actor, time, reason, and affected account/system.
- Review emergency access and recovery activity afterward.
- Define custody, access, rotation, and periodic verification procedures before production use.

## 12. Security monitoring and incident handling

- Record relevant authentication and authorization failures.
- Monitor repeated failed logins and unusual administrative actions.
- Establish a procedure for suspected account compromise.
- Support account disablement and session invalidation.
- Define incident documentation, containment, recovery, and notification responsibilities.
- Avoid logging passwords, session cookies, access tokens, or unnecessary clinical data.

## 13. Security acceptance criteria

- [ ] All protected API endpoints enforce backend authorization.
- [ ] Object-level access checks prevent identifier-based data exposure.
- [ ] Authentication cookies use Secure and HttpOnly settings.
- [ ] Antiforgery protection is applied to state-changing cookie-authenticated requests.
- [ ] No credentials or session tokens are stored in browser localStorage.
- [ ] Session timeout and invalidation behavior is tested.
- [ ] Privileged actions are audited.
- [ ] Audit payloads are reviewed for sensitive-data leakage.
- [ ] File downloads require authorization.
- [ ] File storage is outside the public web root.
- [ ] Database access is unavailable directly from client PCs.
- [ ] Backup and restore procedures are tested.
- [ ] Supported OS and dependency servicing is verified.
- [ ] Security tests cover role boundaries and separation of duties.

## 14. Approved decisions and remaining specification

1. Should MFA be required for administrators and signatories? Recommended: yes, if a workable LAN-compatible method is selected.
2. What are the idle and absolute session timeouts? Proposed starting point: 15-minute idle timeout and 8-hour absolute lifetime, subject to operational review.
3. Which roles may access patient records, and should access be restricted by work assignment or unit?
4. Which actions require separation of duties? Recommended: verifier should not verify their own submission; merge requester should not be sole approver; report signer/releaser rules need explicit policy.
5. What audit retention period is required?
6. What backup schedule, retention, RPO, and RTO are required?
7. Who administers internal TLS certificates and trusted client installation?
8. Are audit exports allowed, and which roles may perform them?
9. What is the required process for account recovery and administrator emergency access?
10. What is the final supported Windows servicing/upgrade path?

### Approved policy decisions (3 November 2026)

- **S-01 — Privileged-user additional verification:** MFA/additional login verification is required for system administrators and authorized report signatories, provided the selected method works reliably offline/on the LAN.
- **S-02 — Session limits:** 15-minute inactivity timeout and 8-hour absolute session limit.
- **S-03 — Patient-data access:** Limit access to information needed for assigned duties.
- **S-04 — Separation of duties:** Enforce separation where practical, including no self-verification and no self-approval of patient merges; signer/releaser policy follows laboratory policy.
- **S-05 — Audit retention:** Retain audit records for the applicable laboratory-policy period; do not auto-delete until the period is defined; older-record review is restricted to authorized personnel.
- **S-06 — Backup and recovery:** Daily backups, at least 30 days retention, separate physical/network storage, regular restore tests, target RPO 24 hours and RTO 8 hours.
- **S-07 — TLS administration:** A designated system administrator manages, installs, renews, and monitors certificate expiry; client machines trust the certificate.
- **S-08 — Audit exports:** Export is limited to specifically authorized administrators/auditors; log who, when, and why; ordinary staff cannot export.
- **S-09 — Emergency recovery:** Designated emergency administrator account; securely and separately stored recovery credentials; identity verification; log every emergency access/recovery; review afterward.
- **S-10 — Supported operating system:** Windows 11 Pro is preferred for production if hardware supports it. Windows 10 Pro may be used only as a temporary development/transition environment subject to verified support status. If supported Windows cannot run on the hardware, evaluate another supported OS, including Linux.

### Remaining specification and verification items

Approval establishes the policy direction; it does not mean every operational detail is complete. Before production, specify the exact audit retention period under applicable laboratory policy; backup software, schedule, destination, encryption, monitoring, and restore-test cadence; verify RPO/RTO through recovery exercises; document certificate issuance/trust/renewal procedures; assign audit-export permissions in the role-permission matrix; define emergency credential custody and rotation; and verify actual hardware/OS compatibility and supported servicing status. The 15-minute/8-hour session controls and offline-capable MFA mechanism must be implemented and tested.

## 15. Approval status

SDD-07 v0.2 is approved as the Release 1 security and privacy design baseline, with S-01 through S-10 approved on 3 November 2026. Detailed role mappings, operational procedures, technical configuration, security testing, and production verification remain open. This approval does not authorize implementation or coding; implementation remains subject to the SDD-08 readiness gates and separate explicit implementation-start approval.
