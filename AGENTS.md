# Agent context — E-Commerce Marketplace

Portable project brief for AI agents (Cursor, Copilot, Claude Code, etc.).

## Product

Multi-vendor e-commerce marketplace. Buyers, sellers, admin. Microservices on .NET 10, storefront on Next.js (App Router).

## Repository layout

```
apps/web/ Next.js (storefront + seller/admin route groups)
services/building-blocks/ Shared logging + health helpers
services/gateway/ Ocelot API gateway (planned)
services/identity/ Identity API (health + logging scaffold)
services/* Catalog, Cart-Order, Inventory, Payment, Notification
infra/nginx/ Edge reverse proxy config
infra/graylog/ Observability (local profile)
docs/ Architecture, sprints, CI/CD, Cursor guide
```

## Stack

- **API:** Ocelot gateway, Nginx edge
- **Data:** SQL Server 2022 (one DB per service), Redis cache
- **Messaging:** RabbitMQ (domain events, sagas)
- **Observability:** Graylog + Serilog GELF, correlation IDs (`docs/LOGGING.md`, `docs/HEALTH.md`)
- **Local:** root `docker-compose.yml` (infra + **identity-api** + **gateway-api** + **web** Next.js + Nginx); UI+API at `http://localhost:8080` — no host `dotnet run` / `pnpm dev`. `dotnet build` for API compile checks only.
- **EC2 (now):** `docker-compose.web.yml` — Nginx + static fallback (full Next on EC2 later)

## Workflow

- Sprints: 1 week. Backlog on [GitHub Project #2](https://github.com/users/irfansahs/projects/2).
- Implement via issues; link to Project **Sprint** field.
- Do not commit unless the user asks.

## Security (non-negotiable)

- Never commit: `.env`, `*.pem`, `*.key`, real API keys, SSH private keys.
- EC2 deploy uses GitHub Secrets: `EC2_HOST`, `EC2_USER`, `EC2_SSH_KEY`.
- Do not put live IPs or PEM paths in README or public docs.

## Deploy

- **CI:** `.github/workflows/deploy-ec2.yml` on `main` push → SSH → `docker compose -f docker-compose.web.yml up -d`
- **Local full stack:** `cp .env.example .env` then `docker compose up -d --build`
- **Local web only:** `docker compose -f docker-compose.web.yml up -d`

## Cursor project assets

- Rules: `.cursor/rules/*.mdc`
- Skills: `.cursor/skills/*/SKILL.md`
- Guide: [docs/CURSOR_GUIDE.md](docs/CURSOR_GUIDE.md)

## Docs

- [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md)
- [docs/STACK.md](docs/STACK.md)
- [docs/SPRINTS.md](docs/SPRINTS.md)
- [docs/CI_CD.md](docs/CI_CD.md)
- [docs/SECURITY.md](docs/SECURITY.md)
- [docs/LOGGING.md](docs/LOGGING.md)
- [docs/I18N.md](docs/I18N.md)
- [docs/HEALTH.md](docs/HEALTH.md)
- [docs/GATEWAY.md](docs/GATEWAY.md)
- [docs/JWT.md](docs/JWT.md)
- [docs/TESTING_SPRINT2.md](docs/TESTING_SPRINT2.md)
- [docs/TESTING_SPRINT3.md](docs/TESTING_SPRINT3.md)
- [docs/TESTING_SPRINT4.md](docs/TESTING_SPRINT4.md)
- [docs/WEB_AUTH.md](docs/WEB_AUTH.md)
- [docs/IDENTITY.md](docs/IDENTITY.md)
- [docs/MESSAGING.md](docs/MESSAGING.md)
