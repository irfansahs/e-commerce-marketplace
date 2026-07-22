# API Gateway (Ocelot)

Host: `services/gateway/Marketplace.Gateway.Api`

## Routes

| Upstream | Downstream (local dev) | JWT when secure mode |
|----------|-------------------------|----------------------|
| `/api/identity/health` | `http://localhost:5211/health` | Public |
| `/api/identity/{everything}` | `http://localhost:5211/{everything}` | Bearer required |

Pattern for new services: `/api/{service}/…` → downstream service host/port.

## Run locally

```bash
dotnet run --project services/identity/Marketplace.Identity.Api
dotnet run --project services/gateway/Marketplace.Gateway.Api
curl http://localhost:5280/api/identity/health
```

Step-by-step test checklist: [TESTING_SPRINT2.md](TESTING_SPRINT2.md).

## JWT

- Contract with Identity: [JWT.md](JWT.md).
- `Jwt:Enabled=false` (default dev): `ocelot.json` only, no tokens.
- `Jwt:Enabled=true`: loads `ocelot.secure.json` + JWT Bearer validation (`AuthenticationProviderKey`: `Bearer`).

## Nginx

`infra/nginx/nginx.conf` proxies `/api/` to gateway (`host.docker.internal:5280` when gateway runs on the host). Static UI on `/`.

When gateway runs in Docker later, switch `proxy_pass` to `http://gateway:8080`.
