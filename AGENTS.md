# Agent context — E-Commerce Marketplace

Portable project brief for AI agents (Cursor, Copilot, Claude Code, etc.).

## Product

Multi-vendor e-commerce marketplace. Buyers, sellers, admin. Microservices on .NET 8, storefront on Next.js (App Router).

## Repository layout

```
apps/web/           Next.js (storefront + seller/admin route groups)
services/gateway/   Ocelot API gateway (planned)
services/*          Identity, Catalog, Cart-Order, Inventory, Payment, Notification
infra/nginx/        Edge reverse proxy config
infra/graylog/      Observability (planned)
docs/               Architecture, sprints, CI/CD, Cursor guide
```

## Stack

- **API:** Ocelot gateway, Nginx edge
- **Data:** SQL Server 2022 (one DB per service), Redis cache
- **Messaging:** RabbitMQ (domain events, sagas)
- **Observability (later):** Graylog + Serilog, correlation IDs
- **Local:** root `docker-compose.yml` (full infra)
- **EC2 (now):** `docker-compose.web.yml` — Nginx + static `apps/web/public/index.html` on port 80

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
- **Local full stack:** `cp .env.example .env` then `docker compose up -d`
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
