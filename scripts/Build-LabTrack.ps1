[CmdletBinding()]
param(
    [string]$Configuration = 'Release',
    [string]$Runtime = 'win-x64'
)
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
$solution = Join-Path $repoRoot 'LabTrack.sln'
$apiProject = Join-Path $repoRoot 'src\backend\LabTrack.WebApi\LabTrack.WebApi.csproj'
$frontend = Join-Path $repoRoot 'src\frontend\labtrack-web'
$out = Join-Path $repoRoot 'artifacts\kestrel\publish'
foreach ($p in @($solution,$apiProject,(Join-Path $frontend 'package.json'))) { if (-not (Test-Path $p)) { throw "Expected repository file not found: $p" } }
Write-Host 'Build/stage only. No database, migration, service, firewall, or deployment actions.'
dotnet restore $solution
if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed' }
dotnet build $solution -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed' }
Push-Location $frontend
try {
    if (Test-Path (Join-Path $frontend 'package-lock.json')) { npm ci } else { Write-Warning 'No package-lock.json; using npm install. Commit lockfile for reproducible releases.'; npm install }
    if ($LASTEXITCODE -ne 0) { throw 'npm dependency install failed' }
    npm run build
    if ($LASTEXITCODE -ne 0) { throw 'frontend build failed' }
} finally { Pop-Location }
New-Item -ItemType Directory -Force -Path $out | Out-Null
dotnet publish $apiProject -c $Configuration -r $Runtime --self-contained false -o $out
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish failed' }
$www = Join-Path $out 'wwwroot'
New-Item -ItemType Directory -Force -Path $www | Out-Null
Copy-Item (Join-Path $frontend 'dist\*') $www -Recurse -Force
if (-not (Test-Path (Join-Path $www 'index.html'))) { throw 'React build index.html was not staged into publish wwwroot' }
Write-Host "Staged Kestrel publish output: $out"
Write-Warning 'The API production startup guard remains authoritative. Publish success does not mean MariaDB or production startup is qualified.'
