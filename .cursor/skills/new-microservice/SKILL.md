---
name: new-microservice
description: Scaffold a new .NET microservice in the monorepo. Use when adding a service, bounded context, or new folder under services/.
---

# New microservice

## Steps

1. **Name** — kebab folder under `services/` (e.g. `pricing` → `services/pricing/`).
2. **Skeleton** — .NET 8 Web API project with:
   - `GET /health`
   - `appsettings.json` + connection string placeholder for dedicated SQL Server DB (`Server=localhost,1433;Database={service}_db;User Id=sa;...`)
   - Dockerfile (multi-stage) when user asks for container deploy
3. **Data** — DB name pattern: `{service}_db` on shared SQL Server instance; add the database to [infra/mssql/init/01-create-databases.sql](infra/mssql/init/01-create-databases.sql) (do not share DB with other services).
4. **Gateway** — add Ocelot route stub comment in `services/gateway` when gateway exists: `/api/{service}/{everything}`.
5. **Compose** — optional service entry in root `docker-compose.yml` only after image/build exists.
6. **Events** — if async: document RabbitMQ exchange/queue in service README.
7. **Issue** — create GitHub issue with labels `backend`, link to Project Sprint field.

## Conventions

Follow [.cursor/rules/dotnet-services.mdc](.cursor/rules/dotnet-services.mdc) and [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Do not

- Put secrets in appsettings committed to git; use env vars / user secrets locally.
