# LabTrack

LabTrack is a modular laboratory information system under controlled development.

## Selected deployment architecture

The selected Windows hosting architecture is ASP.NET Core Kestrel hosted as a Windows Service, serving the React/Vite frontend from the same application origin. The initial development/test host is Windows 10 Pro at LAN address 192.168.0.116. Future production use requires a supported, serviced operating system and completion of all release gates.

See deployment/kestrel/README.md for deployment layout, build and validation scripts, HTTPS, LAN restrictions, service preparation and Linux migration strategy.

The old IIS materials under deployment/windows/ are retained as historical references only.

## Development safety boundary

The API currently permits startup only in Development with Persistence:Mode=DevelopmentInMemory. MariaDB 10.11 provider and mappings are not yet qualified. Do not bypass this restriction. No production database, SQL, migrations, seeding, service installation/start, firewall modification or production deployment is authorized by this repository state.

Approved SDD baselines and authorization/release gates remain binding. Design approval is not equivalent to test execution, UAT or production release approval.

## Build

Run from repository root in PowerShell:

    .\deployment\kestrel\scripts\Build-LabTrack.ps1

The script requires .NET 10 SDK, Node.js/npm and the committed frontend lockfile. It builds and stages artifacts only; it does not deploy or access a database.
