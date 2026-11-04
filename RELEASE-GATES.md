# LabTrack release gates (must all be satisfied before production)

- [ ] MariaDB 10.11 EF Core provider selected, compatibility qualified, and package/version pinned.
- [ ] Entity mappings, indexes, constraints, collation, decimal precision, date/time behavior, and migrations reviewed and tested against disposable MariaDB.
- [ ] Production startup guard remains in place; no bypass or DevelopmentInMemory production mode.
- [ ] SDD-11 API contract baseline checked against implementation.
- [ ] SDD-09 authorization matrix finalized/approved and all role/permission enforcement tested server-side.
- [ ] Identity, password policy, session/cookie/token security, lockout, audit events, and audit immutability verified.
- [ ] SDD-12 synthetic test strategy executed; tests/results recorded; UAT formally accepted.
- [ ] Patient/result printout, abnormal flags, reference intervals, units, decimal handling, report footer/signatory rules verified.
- [ ] Windows OS lifecycle/support and endpoint protection plan approved.
- [ ] TLS certificate chain, SAN, workstation trust distribution, and renewal process tested.
- [ ] LAN subnet and firewall scope approved; DB port 3306 inaccessible from workstations.
- [ ] Least-privilege service identity, directory ACLs, log retention, and secret rotation approved.
- [ ] Encrypted backups and a successful restore drill completed.
- [ ] Monitoring, health checks, incident response, patching, and rollback plan documented.
- [ ] Separate written authorization for production DB creation/migration/seeding and deployment obtained.

Approvals of design documents alone do not satisfy test execution, UAT, production DB authorization, or deployment authorization.
