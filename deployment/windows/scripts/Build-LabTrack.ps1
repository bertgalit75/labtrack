[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$Runtime = 'win-x64'
)
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
$backend = Join-Path $repoRoot 'src\backend\LabTrack.WebApi\LabTrack.WebApi.csproj'
$frontend = Join-Path $repoRoot 'src\frontend\labtrack-web'
$out = Join-Path $repoRoot 'artifacts\windows'

Write-Host 'Building LabTrack artifacts only. No database or deployment actions will be run.'
dotnet restore (Join-Path $repoRoot 'LabTrack.sln')
dotnet build (Join-Path $repoRoot 'LabTrack.sln') -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed.' }
Push-Location $frontend
try {
    npm install
    if ($LASTEXITCODE -ne 0) { throw 'npm install failed.' }
    npm run build
    if ($LASTEXITCODE -ne 0) { throw 'npm run build failed.' }
} finally { Pop-Location }
New-Item -ItemType Directory -Force -Path (Join-Path $out 'api'), (Join-Path $out 'wwwroot') | Out-Null
dotnet publish $backend -c $Configuration -r $Runtime --self-contained false -o (Join-Path $out 'api')
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish failed.' }
Copy-Item -Path (Join-Path $frontend 'dist\*') -Destination (Join-Path $out 'wwwroot') -Recurse -Force
Write-Host "Build artifacts staged at $out"
Write-Warning 'The current API intentionally refuses non-DevelopmentInMemory startup until MariaDB provider/mappings are qualified. Build success does not imply deployability.'
