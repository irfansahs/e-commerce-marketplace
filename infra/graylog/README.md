# Graylog observability (Sprint 1)

Local-only stack. **Do not** add to [docker-compose.web.yml](../../docker-compose.web.yml) or EC2 deploy.

## Start

Base infra first, then observability profile:

```bash
docker compose up -d
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile observability up -d
```

## UI

- URL: http://localhost:9000
- User: `admin`
- Password: value used for `GRAYLOG_ROOT_PASSWORD_SHA2` in `.env` (default dev: `admin` — change in `.env`)

## GELF (for .NET Serilog)

- UDP **12201** on host → Graylog container
- In Graylog UI: **System → Inputs → GELF UDP** (create if not auto-configured; bind 0.0.0.0:12201)
- .NET wiring: [docs/LOGGING.md](../../docs/LOGGING.md) (`AddMarketplaceLogging` + Identity smoke host)

## Resources

OpenSearch + Mongo + Graylog need ~2GB+ RAM. Use Docker Desktop memory limit accordingly.

## Env secrets

See [.env.example](../../.env.example). Generate root password hash:

```bash
# Linux/macOS/Git Bash
echo -n "YourPassword" | sha256sum
# Windows PowerShell
powershell -NoProfile -Command "[BitConverter]::ToString([System.Security.Cryptography.SHA256]::Create().ComputeHash([Text.Encoding]::UTF8.GetBytes('YourPassword'))).Replace('-','').ToLower()"
```

Set `GRAYLOG_PASSWORD_SECRET` (≥16 chars) and `GRAYLOG_ROOT_PASSWORD_SHA2` in `.env`.
