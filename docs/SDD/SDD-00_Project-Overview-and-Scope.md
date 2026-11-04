# SDD-00 — Project Overview and Scope

**Version:** 0.1 (consolidated baseline)  
**Status:** Approved with notes  
**Release:** 1

## 1. Purpose

LabTrack is a desktop-first, browser-based laboratory management system for centralized use by laboratory staff over a local area network.

## 2. Technology baseline

- Frontend: React + TypeScript
- Backend: ASP.NET Core Web API
- Database: MariaDB 10.11.x
- Deployment: centralized LAN server with browser-based client workstations

## 3. Release 1 scope

1. Authentication and Staff Access Management
2. Patient Management
3. Test Catalog Management
4. Examination Management
5. Result Encoding and Revision Management
6. Result Submission and Verification
7. Critical Result Communication Records
8. Report Generation and Release
9. Report Amendment and Distribution Records
10. Audit Trail and Workflow Event Management

## 4. Out of scope for Release 1

Independent specimen management, billing/payment, inventory/reagents, equipment management, QC/EQA, appointments, patient portal, external laboratory integrations, automated external notifications, public internet hosting, multi-site synchronization, and high-availability multi-server deployment.

## 5. Design principles

- Backend authorization and business-rule enforcement.
- Traceable workflows and preserved history.
- Result revisions rather than destructive overwrites.
- Preserve the test-definition version selected at examination registration.
- Controlled patient merge and report amendment.
- Transaction-safe numbering and workflow updates.
- No direct database access from client workstations.
- Normal operations must work without internet access.

## 6. User categories (proposed)

System Administrator, Laboratory Administrator, Reception/Registration Staff, Laboratory Staff, Verifying Staff, Authorized Signatory, and Auditor/Supervisor. Exact roles and permissions remain subject to the access-control design.

## 7. Approval note

The user approved this document with the note that Windows 10 Pro is the current existing server OS, while Windows 11 remains the recommended production upgrade path.

## 8. Open decisions

Official laboratory identity; final staff roles and permissions; patient numbering; examination numbering policy; final status transitions; test catalog publication rules; verification independence; critical-result escalation; report signatory requirements; backup retention and recovery objectives; final supported Windows path; and actual server hardware/storage requirements.
