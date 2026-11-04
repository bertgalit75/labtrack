# Architecture Overview

LabTrack uses a centralized modular-monolith architecture:

`Browser (React) -> ASP.NET Core Web API -> Application/Domain Services -> Infrastructure -> MariaDB`

The application host serves the frontend and API on one origin where practical. Report/document files are stored server-side outside the public web root.

Client workstations do not connect directly to MariaDB. Business rules and authorization are enforced by the backend.
