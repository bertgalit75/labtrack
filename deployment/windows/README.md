# Windows deployment

Selected hosting architecture: Kestrel hosted as a Windows Service. IIS is not the selected deployment architecture.

See ../kestrel/README.md for the active Windows deployment preparation, build scripts, HTTPS, LAN restrictions, service identity, validation and Linux migration strategy.

The target development/test host is Windows 10 Pro at 192.168.0.116. Ordinary Windows 10 reached end of support on 2025-10-14; do not treat it as a production PHI host without a qualifying ESU/LTSC lifecycle and approved security plan. Prefer supported Windows 11 Pro or Windows Server for production.

Files in this directory are retained as historical IIS preparation material only. They must not be used for the active deployment workflow. No service installation, firewall changes, database operations, migration, or production deployment is authorized by this documentation update.
