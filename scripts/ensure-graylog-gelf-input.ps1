# Ensures Graylog has a GELF UDP input on 12201 (required for Serilog sinks).
# Run: pwsh -File scripts/ensure-graylog-gelf-input.ps1
# Default creds: admin / admin (dev hash in .env.example)

param(
    [string]$BaseUrl = "http://localhost:9000",
    [string]$User = "admin",
    [string]$Password = "admin"
)

$ErrorActionPreference = "Stop"
$pair = "${User}:${Password}"
$cred = [Convert]::ToBase64String([Text.Encoding]::ASCII.GetBytes($pair))
$headers = @{
    Authorization   = "Basic $cred"
    Accept          = "application/json"
    "X-Requested-By" = "marketplace-cli"
}

$inputs = Invoke-RestMethod -Uri "$BaseUrl/api/system/inputs" -Headers $headers
$existing = $inputs.inputs | Where-Object {
    $_.type -eq "org.graylog2.inputs.gelf.udp.GELFUDPInput" -and $_.attributes.port -eq 12201
}
if ($existing) {
    Write-Host "GELF UDP input already present: $($existing.title) ($($existing.id))"
    exit 0
}

$body = @{
    title         = "GELF UDP"
    type          = "org.graylog2.inputs.gelf.udp.GELFUDPInput"
    global        = $true
    configuration = @{
        bind_address     = "0.0.0.0"
        port             = 12201
        recv_buffer_size = 262144
    }
} | ConvertTo-Json -Depth 5

$created = Invoke-RestMethod -Uri "$BaseUrl/api/system/inputs" -Method Post -Headers ($headers + @{ "Content-Type" = "application/json" }) -Body $body
Write-Host "Created GELF UDP input id=$($created.id)"
