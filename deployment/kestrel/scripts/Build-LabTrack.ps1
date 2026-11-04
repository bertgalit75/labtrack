[CmdletBinding()]
param(
    [ValidateSet('Debug','Release')][string]$Configuration = 'Release',
    [ValidateSet('win-x64','win-arm64')][string]$Runtime = 'win-x64'
)
$ErrorActionPreference = 'Stop'
$repoRoot = (Resolve-Path (Join-Path $PSScriptRoot '..\..\..')).Path
$solution = Join-Path $repoRoot 'LabTrack.sln'
$apiProject = Join-Path $repoRoot 'src\backend\LabTrack.WebApi\LabTrack.WebApi.csproj'
$frontend = Join-Path $repoRoot 'src\frontend\labtrack-web'
$lockFile = Join-Path $frontend 'package-lock.json'
$out = Join-Path $repoRoot 'artifacts\kestrel\publish'
foreach ($required in @($solution,$apiProject,(Join-Path $frontend 'package.json'),$lockFile)) {
    if (-not (Test-Path $required)) { throw "Required build input not found: $required" }
}
Write-Host 'Build/stage only. No service, firewall, database, SQL, migration, or deployment actions.'
dotnet restore $solution
if ($LASTEXITCODE -ne 0) { throw 'dotnet restore failed.' }
dotnet build $solution -c $Configuration --no-restore
if ($LASTEXITCODE -ne 0) { throw 'dotnet build failed.' }
Push-Location $frontend
try {
    npm ci
    if ($LASTEXITCODE -ne 0) { throw 'npm ci failed.' }
    npm run build
    if ($LASTEXITCODE -ne 0) { throw 'Frontend build failed.' }
} finally { Pop-Location }
if (Test-Path $out) { Remove-Item $out -Recurse -Force }
dotnet publish $apiProject -c $Configuration -r $Runtime --self-contained false -o $out
if ($LASTEXITCODE -ne 0) { throw 'dotnet publish failed.' }
$wwwroot = Join-Path $out 'wwwroot'
New-Item -ItemType Directory -Force -Path $wwwroot | Out-Null
Copy-Item (Join-Path $frontend 'dist\*') $wwwroot -Recurse -Force
if (-not (Test-Path (Join-Path $wwwroot 'index.html'))) { throw 'React index.html was not staged.' }
Write-Host "Kestrel publish output staged at: $out"
Write-Warning 'Publish success is not runtime qualification. Production startup guard remains in force.'
