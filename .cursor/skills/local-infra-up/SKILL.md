---
name: local-infra-up
description: Start local Docker infrastructure for marketplace development. Use when user asks docker compose, local postgres, rabbitmq, or dev environment.
---

# Local infra up

## Full stack (Postgres, Redis, RabbitMQ, Nginx)

```bash
cp .env.example .env   # if .env missing
docker compose up -d
docker compose ps
```

| Service | Port |
|---------|------|
| Postgres | 5432 |
| Redis | 6379 |
| RabbitMQ | 5672, UI 15672 |
| Nginx (static) | 8080 |

Smoke:

- `http://localhost:8080/` → HTML landing
- `http://localhost:15672/` → RabbitMQ management (credentials from `.env`)

## Web only (minimal)

```bash
docker compose -f docker-compose.web.yml up -d
```

## Stop

```bash
docker compose down
```

Volumes retain data. Requires Docker Desktop running on Windows.

Follow [.cursor/rules/docker-infra.mdc](.cursor/rules/docker-infra.mdc).
