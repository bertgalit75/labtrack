# Historical IIS deployment preparation — not the selected architecture

This document is retained for historical context only. LabTrack's selected Windows hosting architecture is Kestrel hosted as a Windows Service, not IIS.

Do not execute the IIS initialization, IIS build, or staged IIS deployment scripts referenced by older revisions of this document.

For the current architecture, read ../kestrel/README.md.

The existing production startup guard remains intact. MariaDB provider qualification, authorization enforcement, test execution, UAT, supported operating-system review, TLS, backup/restore, monitoring, rollback and separate written production authorization remain mandatory. This historical document does not authorize deployment, database creation, SQL, migrations, seeding, service installation/start, or firewall changes.
