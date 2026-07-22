# Local dev: Docker infra + build + optional API smoke
# Run from repo root: pwsh -File scripts/dev-local.ps1
#   -StartApis     also run Identity + Gateway (background jobs in this session)
#   -Smoke          curl register/login after -StartApis

param(
    [switch]$StartApis,
    [switch]$Smoke
)

$ErrorActionPreference = "Stop"
$root = Split-Path -Parent $PSScriptRoot
Set-Location $root

function Import-DotEnv {
    param([string]$Path)
    if (-not (Test-Path $Path)) { return }
    Get-Content $Path | ForEach-Object {
        $line = $_.Trim()
        if ($line -eq "" -or $line.StartsWith("#")) { return }
        $i = $line.IndexOf("=")
        if ($i -lt 1) { return }
        $name = $line.Substring(0, $i).Trim()
        $value = $line.Substring($i + 1).Trim()
        Set-Item -Path "Env:$name" -Value $value
    }
}

Import-DotEnv (Join-Path $root ".env")

if (-not $env:MSSQL_SA_PASSWORD) { $env:MSSQL_SA_PASSWORD = "Marketplace_Local1!" }
if (-not $env:MSSQL_HOST_PORT) { $env:MSSQL_HOST_PORT = "14330" }

Write-Host ">>> Docker compose (MSSQL host port $($env:MSSQL_HOST_PORT))..."
docker compose up -d mssql redis rabbitmq nginx

Write-Host ">>> Waiting for SQL Server..."
$deadline = (Get-Date).AddMinutes(3)
do {
    $ok = docker exec marketplace-mssql /opt/mssql-tools18/bin/sqlcmd -S localhost -U sa -P $env:MSSQL_SA_PASSWORD -C -Q "SELECT 1" 2>$null
    if ($LASTEXITCODE -eq 0) { break }
    Start-Sleep -Seconds 3
} while ((Get-Date) -lt $deadline)
if ($LASTEXITCODE -ne 0) { throw "MSSQL did not become ready in time." }

docker compose run --rm mssql-init | Out-Null

Write-Host ">>> dotnet build..."
dotnet build Marketplace.slnx -v q
if ($LASTEXITCODE -ne 0) { throw "Build failed." }

if ($StartApis) {
    Write-Host ">>> Starting Identity (5211) and Gateway (5280) as background jobs..."
    $identityJob = Start-Job -ScriptBlock {
        param($root, $pwd, $port)
        Set-Location $root
        $env:MSSQL_SA_PASSWORD = $pwd
        $env:MSSQL_HOST_PORT = $port
        $env:ASPNETCORE_ENVIRONMENT = "Development"
        dotnet run --project "services\identity\Marketplace.Identity.Api" --no-build
    } -ArgumentList $root, $env:MSSQL_SA_PASSWORD, $env:MSSQL_HOST_PORT

    $gatewayJob = Start-Job -ScriptBlock {
        param($root)
        Set-Location $root
        $env:ASPNETCORE_ENVIRONMENT = "Development"
        dotnet run --project "services\gateway\Marketplace.Gateway.Api" --no-build
    } -ArgumentList $root

    Write-Host ">>> Waiting for /health..."
    $deadline = (Get-Date).AddMinutes(2)
    do {
        try {
            $h1 = Invoke-WebRequest -Uri "http://localhost:5211/health" -UseBasicParsing -TimeoutSec 3
            $h2 = Invoke-WebRequest -Uri "http://localhost:5280/health" -UseBasicParsing -TimeoutSec 3
            if ($h1.StatusCode -eq 200 -and $h2.StatusCode -eq 200) { break }
        } catch { }
        Start-Sleep -Seconds 2
    } while ((Get-Date) -lt $deadline)

    if ($Smoke) {
        Write-Host ">>> Smoke: register..."
        $body = '{"email":"smoke@test.com","password":"Test1234!"}'
        curl.exe -s -X POST "http://localhost:5280/api/identity/auth/register" `
            -H "Content-Type: application/json" -H "Accept-Language: tr" -d $body
        Write-Host ""
    }

    Write-Host "Identity job id: $($identityJob.Id)  Gateway job id: $($gatewayJob.Id)"
    Write-Host "Stop APIs: Stop-Job $($identityJob.Id),$($gatewayJob.Id); Remove-Job $($identityJob.Id),$($gatewayJob.Id) -Force"
}

Write-Host "Done."
Write-Host "  SQL:    localhost,$($env:MSSQL_HOST_PORT)  sa / (see .env MSSQL_SA_PASSWORD)"
Write-Host "  Rabbit: http://localhost:15672"
Write-Host "  Nginx:  http://localhost:8080"
Write-Host "  Gateway: http://localhost:5280  (run with -StartApis)"
