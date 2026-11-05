# Backend implementation scaffold

The backend is split into Domain, Application, Infrastructure, and WebApi projects. All projects target .NET 10.

The API host contains an initial ASP.NET Core Identity scaffold, a liveness endpoint, and the first Patient Management API slice: patient registration, search/list, detail, and routine demographic updates. Staff/access, catalog, examination, result, and report use cases are not yet complete.

`DevelopmentInMemory` is the only enabled persistence mode. It is intentionally ephemeral and is not suitable for clinical records or production. Any non-development startup fails until a MariaDB 10.11 EF Core provider has been qualified and explicitly configured.

The EF mappings in `LabTrackDbContext` are an initial partial reconciliation against the 48-table SQL baseline. They are not migration-ready and have not been runtime-tested.


Patient API implementation notes and explicit interim security/numbering limitations are documented in `docs/implementation/PATIENT-MANAGEMENT-API-IMPLEMENTATION.md`. The new patient service tests are authored but have not yet been run.