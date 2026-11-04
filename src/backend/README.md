# Backend implementation scaffold

The backend is split into Domain, Application, Infrastructure, and WebApi projects. All projects target .NET 10.

The API host currently contains only an initial ASP.NET Core Identity authentication scaffold and a liveness endpoint. Patient, staff/access, catalog, examination, result, report, and audit use cases are not yet complete.

`DevelopmentInMemory` is the only enabled persistence mode. It is intentionally ephemeral and is not suitable for clinical records or production. Any non-development startup fails until a MariaDB 10.11 EF Core provider has been qualified and explicitly configured.

The EF mappings in `LabTrackDbContext` are an initial partial reconciliation against the 48-table SQL baseline. They are not migration-ready and have not been runtime-tested.
