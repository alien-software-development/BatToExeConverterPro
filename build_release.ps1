# ========================================================================
#   ALIEN BAT TO EXE CONVERTER PRO STUDIO v3.5 - MASTER BUILD SCRIPT
# ========================================================================
$ErrorActionPreference = "Stop"

$RootDir = $PSScriptRoot
if (-not $RootDir) { $RootDir = (Get-Location).Path }

Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host "   ALIEN BAT TO EXE CONVERTER PRO STUDIO v3.5 - COMPILATION PIPELINE" -ForegroundColor Green
Write-Host "========================================================================" -ForegroundColor Cyan
Write-Host "Workspace: $RootDir" -ForegroundColor Gray

$ReleaseDir = Join-Path $RootDir "Release_Build"
$StandaloneAppDir = Join-Path $ReleaseDir "BatToExePro_Standalone"
$StandaloneKeygenDir = Join-Path $ReleaseDir "AlienKeygen_Standalone"
$InstallerOutDir = Join-Path $ReleaseDir "Installer"

# Ensure directories exist
New-Item -ItemType Directory -Force -Path $StandaloneAppDir | Out-Null
New-Item -ItemType Directory -Force -Path $StandaloneKeygenDir | Out-Null
New-Item -ItemType Directory -Force -Path $InstallerOutDir | Out-Null

# 1. Publish BatToExeConverter Standalone
Write-Host ""
Write-Host "[1/3] Building & Publishing BatToExeConverter Pro Studio v3.5..." -ForegroundColor Yellow
$batProj = Join-Path $RootDir "BatToExeConverter\BatToExeConverter.csproj"
dotnet publish $batProj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true -o $StandaloneAppDir
if ($LASTEXITCODE -ne 0) { throw "Failed to publish BatToExeConverter!" }

# 2. Publish AlienKeygen Standalone
Write-Host ""
Write-Host "[2/3] Building & Publishing AlienKeygen Master Keygen v3.5..." -ForegroundColor Yellow
$keygenProj = Join-Path $RootDir "AlienKeygen\AlienKeygen.csproj"
dotnet publish $keygenProj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true -o $StandaloneKeygenDir
if ($LASTEXITCODE -ne 0) { throw "Failed to publish AlienKeygen!" }

# 3. Publish BatToExeSetup Installer
Write-Host ""
Write-Host "[3/3] Building & Publishing Offline Setup Wizard v3.5 with UAC Elevation..." -ForegroundColor Yellow
$setupProj = Join-Path $RootDir "BatToExeSetup\BatToExeSetup.csproj"
dotnet publish $setupProj -c Release -r win-x64 --self-contained false /p:PublishSingleFile=true -o $InstallerOutDir
if ($LASTEXITCODE -ne 0) { throw "Failed to publish BatToExeSetup!" }

Write-Host ""
Write-Host "========================================================================" -ForegroundColor Green
Write-Host "   ALL BUILDS COMPLETED SUCCESSFULLY!" -ForegroundColor Green
Write-Host "========================================================================" -ForegroundColor Green
Write-Host "Generated Release Artifacts:" -ForegroundColor Cyan
Write-Host "  - Standalone Studio: $StandaloneAppDir\BatToExeConverter.exe" -ForegroundColor White
Write-Host "  - Standalone Keygen: $StandaloneKeygenDir\AlienKeygen.exe" -ForegroundColor White
Write-Host "  - Offline Setup:     $InstallerOutDir\BatToExePro_Setup.exe" -ForegroundColor White
