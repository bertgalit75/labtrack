# SDD-06 — Frontend Architecture, Navigation, Screens, and UI Behavior

**Version:** 0.1 (consolidated baseline)  
**Status:** Approved  
**Release:** 1

## 1. Technology baseline

- React + TypeScript.
- Vite.
- Tailwind CSS and shadcn/ui.
- React Router.
- TanStack Query.
- React Hook Form and Zod.
- Vitest and React Testing Library.

Exact package versions are deferred until implementation planning.

## 2. Design principles

Desktop-first, workflow-oriented, consistent, clear, safe, traceable, permission-aware, efficient, accessible, and independent of external internet resources during normal LAN use.

## 3. Main layout and navigation

Persistent left sidebar, top header, breadcrumbs, page-level actions, and main workspace.

Primary modules:

- Dashboard
- Patients
- Examinations
- Test Catalog
- Results
- Verification
- Critical Results
- Reports
- Staff & Access
- Audit Trail

Navigation visibility is permission-aware; backend authorization remains authoritative.

## 4. Screen inventory

- Dashboard: role/permission-filtered work overview.
- Patients: list/search, registration, details, change requests, duplicate review, merge.
- Test Catalog: definition list, details, version editor, parameter/reference configuration.
- Examinations: list, registration, details, test additions/cancellation, history.
- Results: worklist, result-entry workspace, revision history.
- Verification: submission worklist and exact-revision review.
- Critical Results: communication and acknowledgement history.
- Reports: worklist, preview, signing, release, amendment, distribution.
- Staff & Access: staff profiles, role/permission assignment, credentials, authorizations.
- Audit Trail: read-only filtered audit/workflow history.

## 5. Shared components

AppLayout, SidebarNavigation, PageHeader, DataTable, StatusBadge, PatientSearch, TestSelector, ConfirmationDialog, ReasonDialog, FormField, LoadingState, EmptyState, ErrorState, HistoryTimeline, RevisionHistory, and ReportPreview.

## 6. State and API integration

TanStack Query manages server state and cache invalidation. React Hook Form and Zod provide client-side form feedback. The backend remains authoritative for validation and permissions.

Authentication uses `/auth/me` and secure cookies. Do not store credentials or session tokens in localStorage. Clear sensitive cached data on logout/session expiration.

## 7. Usability and security

Keyboard-accessible navigation and forms; visible labels; accessible validation; textual status labels in addition to color; explicit confirmation for consequential actions; no untrusted HTML rendering; no secrets in frontend build configuration.

## 8. Performance targets (proposed)

- Ordinary navigation: under 2 seconds.
- Common list retrieval: under 2 seconds.
- Patient search: under 2 seconds.
- Standard form submission: under 3 seconds.
- Dashboard initial load: under 3 seconds.
- Typical report preview: under 5 seconds.

Targets require measurement on actual hardware and representative data with 5–10 concurrent users.

## 9. Approval

Approved. Workflow status labels and transition behavior remain dependent on the approved workflow definitions and unresolved SDD-04 decisions.
