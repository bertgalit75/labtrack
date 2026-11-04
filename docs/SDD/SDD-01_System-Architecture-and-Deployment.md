# SDD-01 — System Architecture and Deployment

**Version:** 0.2 (consolidated baseline)  
**Status:** Approved with notes  
**Release:** 1

## 1. Architecture

Use a centralized three-tier logical architecture:

1. **Presentation:** React + TypeScript frontend, served by the application host.
2. **Application:** ASP.NET Core Web API containing authentication, authorization, business logic, workflows, transactions, reporting, and audit behavior.
3. **Data:** MariaDB 10.11.x plus protected server-side report/document storage.

Preferred same-origin deployment example: `https://labtrack.local` with API routes under `/api/v1`.

## 2. Deployment baseline

- Dedicated existing PC as the initial server.
- Windows 10 Pro is the current installed/intended initial OS.
- LAN-only operations.
- Browser clients; no client-side database installation.
- Expected 5–10 simultaneous users.
- Future Linux migration is desired.
- Kestrel hosted as a Windows Service; MariaDB hosted as a Windows service.
- Static IP or DHCP reservation recommended.
- HTTPS with an internally trusted certificate.
- Windows Firewall permits only required application traffic.
- MariaDB should listen only on localhost/server-side interfaces required by the application; client PCs must not connect directly.

## 3. Security and operations

- Backend authorization is authoritative.
- Secure HttpOnly cookies for same-origin authentication; do not store credentials in localStorage.
- Store reports/documents outside the public web root.
- Store file metadata and references in the database.
- Use UTC timestamps in storage and local timezone formatting in the UI.
- Back up database and document storage to separate physical storage.
- Perform periodic restore tests.

## 4. Portability

Keep business logic and filesystem configuration portable. Avoid Windows-only business logic. Future Linux hosting may use systemd or containers after a separate deployment review.

## 5. Release 1 deployment exclusions

No public internet hosting, cloud database, public remote access, multi-server HA, or failover.

## 6. Capacity caveat

Actual server hardware has not been supplied or verified. A planning target discussed was a modern 4-core CPU, 16 GB RAM, SSD 500 GB or larger, Gigabit Ethernet, separate backup storage, and UPS. These are not verified minimums; test actual hardware with representative data and 5–10 concurrent users.

## 7. Windows servicing note

Windows 10 standard support ended on 2025-10-14. Do not assume standard security updates are available. Windows 11 Pro is the recommended production upgrade if hardware supports it; otherwise confirm a supported servicing/ESU arrangement. LAN isolation does not replace security updates.

## 8. Approval note

Approved with the explicit note that Windows 10 Pro is the current existing server OS and Windows 11 remains the recommended production upgrade path.

## 9. Open decisions

Exact Windows servicing path, hardware specifications, LAN DNS/hostname, certificate arrangement, backup destination/retention, UPS, and compatible .NET/EF Core/MariaDB provider versions.
