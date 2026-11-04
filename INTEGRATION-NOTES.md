# Source integration checklist

The current repository source was not mounted in this session; therefore this package does not patch `Program.cs` or the `.csproj` automatically.

Before service hosting, inspect the actual target framework and host setup, then add the compatible `Microsoft.Extensions.Hosting.WindowsServices` NuGet package and call `UseWindowsService()` on the host builder. Verify the Windows Service lifetime and graceful shutdown behavior in a non-production test environment.

For frontend hosting, ensure the Vite output is copied into the API app's `wwwroot`. Configure ASP.NET Core static files and SPA fallback with API exclusion. Confirm the React app calls the API using a relative same-origin path. Preserve authentication/authorization/audit middleware and the existing production startup guard. Do not introduce permissive CORS when frontend and API share the same origin.

The service install helper assumes the published apphost is named `LabTrack.WebApi.exe`; verify `AssemblyName` and publish output. It creates the service using the explicitly supplied service credential and intentionally does not start it. Set and verify directory ACLs and certificate private-key access for that identity through the approved host-hardening procedure before starting.
