# Windows server deployment preparation

Initial target: existing dedicated Windows server PC. Production upgrade to a supported Windows 11 Pro configuration remains recommended where hardware-compatible. Linux remains a future portability target.

No deployment is authorized or performed by this repository update.

Before a production deployment can be designed or approved:

1. Confirm Windows edition/build and vendor security servicing status.
2. Confirm server hardware, RAM, storage, UPS, and network topology.
3. Install a supported .NET 10 hosting/runtime stack after application/provider qualification.
4. Qualify the EF Core provider and MariaDB 10.11.x mappings on a disposable instance.
5. Define LAN DNS/hostname, firewall rules, HTTPS certificate trust distribution, and browser support.
6. Define database service account, least-privilege grants, backup encryption/key custody, and restore drills.
7. Complete SDD-12 tests and formal UAT/release gates.
8. Obtain separate production database and deployment authorization.

Do not expose MariaDB to workstation subnets. Workstations connect only to the application host over the approved LAN HTTPS endpoint.

See [DEPLOYMENT-PREPARED.md](DEPLOYMENT-PREPARED.md) for the selected IIS/LAN deployment profile, release gates, network/HTTPS plan, and scripts. These assets are preparation-only and intentionally do not activate MariaDB persistence or deploy to production.
