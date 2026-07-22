---
name: local-infra-up
description: Start local Docker infrastructure for marketplace development. Use when user asks docker compose, local sql server, rabbitmq, or dev environment.
---

# Local infra up

## Full stack (SQL Server, Redis, RabbitMQ, Identity, Gateway, Next.js, Nginx)

```bash
cp .env.example .env   # if .env missing
```

If `.env` still contains `POSTGRES_*`, replace it from `.env.example` (project uses **SQL Server 2022** only).

```bash
docker compose up -d --build
docker compose ps
```

Entry: **http://localhost:8080** — Next UI (`/tr`, `/en`), BFF `/api/auth/*`, Identity via `/api/identity/*`. Host’ta `dotnet run` / `pnpm dev` yok.

## Observability (Graylog — optional)

```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile observability up -d
```

| Service | Port |
|---------|------|
| SQL Server | 14330 (host) |
| Redis | 6379 |
| RabbitMQ | 5672, UI 15672 |
| Nginx + Next | 8080 |
| Graylog UI | 9000 (observability profile) |
| GELF UDP | 12201 (observability profile) |

See [infra/graylog/README.md](infra/graylog/README.md) for Graylog.

Smoke:

- `http://localhost:8080/tr/login` → Next login
- `http://localhost:8080/api/identity/health` → Healthy
- `http://localhost:15672/` → RabbitMQ management (credentials from `.env`)

## Web only (minimal EC2-style static)

```bash
docker compose -f docker-compose.web.yml up -d
```

## Stop

```bash
docker compose down
```

Volumes retain data. Requires Docker Desktop running on Windows.

Follow [.cursor/rules/docker-infra.mdc](.cursor/rules/docker-infra.mdc).
