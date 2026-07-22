# Sync local .env from .env.example (SQL Server)
# Run from repo root: powershell -File scripts/sync-env-from-example.ps1
# Does NOT overwrite .env if it already has MSSQL_SA_PASSWORD (use -Force to replace).

param(
    [switch]$Force
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
$example = Join-Path $root ".env.example"
$envFile = Join-Path $root ".env"

if (-not (Test-Path $example)) {
    Write-Error ".env.example not found"
}

if ((Test-Path $envFile) -and -not $Force) {
    $content = Get-Content $envFile -Raw
    if ($content -match "MSSQL_SA_PASSWORD" -and $content -notmatch "POSTGRES_") {
        Write-Host ".env already looks like SQL Server config. Use -Force to overwrite from .env.example."
        exit 0
    }
    if ($content -match "POSTGRES_") {
        Write-Host "Legacy PostgreSQL .env detected — updating from .env.example ..."
    }
}

Copy-Item $example $envFile -Force
Write-Host "Wrote $envFile from .env.example"
Write-Host "Edit MSSQL_SA_PASSWORD and RabbitMQ secrets if needed."
Write-Host "If SQL login fails after password change: docker compose down -v  (wipes DB volume)"
