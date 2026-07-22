# Local dev: full stack in Docker only (no host dotnet/pnpm run).
# Run from repo root: pwsh -File scripts/dev-local.ps1
#   -Smoke   BFF login + identity health after stack is up

param(
    [switch]$Smoke
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

Write-Host ">>> dotnet build (CI-style check)..."
dotnet build Marketplace.slnx -v q
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

Write-Host ">>> Docker compose up --build..."
docker compose up -d --build
if ($LASTEXITCODE -ne 0) { throw "Compose failed." }

Write-Host ">>> Waiting for UI + API..."
$deadline = (Get-Date).AddMinutes(8)
do {
    try {
        $ui = Invoke-WebRequest -Uri "http://localhost:8080/tr/login" -UseBasicParsing -TimeoutSec 5
        $api = Invoke-WebRequest -Uri "http://localhost:8080/api/identity/health" -UseBasicParsing -TimeoutSec 5
        if ($ui.StatusCode -eq 200 -and $api.StatusCode -eq 200) { break }
    } catch { }
    Start-Sleep -Seconds 5
} while ((Get-Date) -lt $deadline)

if ($Smoke) {
    Write-Host ">>> Smoke: BFF login..."
    $session = New-Object Microsoft.PowerShell.Commands.WebRequestSession
    $null = Invoke-RestMethod -Uri "http://localhost:8080/api/auth/login" -Method Post `
        -ContentType "application/json" -WebSession $session `
        -Body '{"email":"admin@marketplace.local","password":"Admin123!","locale":"tr"}'
    $me = Invoke-RestMethod -Uri "http://localhost:8080/api/auth/me" -WebSession $session
    Write-Host ($me | ConvertTo-Json -Compress)
}

Write-Host "Done."
Write-Host "  Storefront: http://localhost:8080/tr"
Write-Host "  Login:      http://localhost:8080/tr/login"
Write-Host "  API health: http://localhost:8080/api/identity/health"
Write-Host "  RabbitMQ:   http://localhost:15672"
