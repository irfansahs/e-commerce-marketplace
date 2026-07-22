# API Gateway (Ocelot)

Host: `services/gateway/Marketplace.Gateway.Api` · Docker service: **`gateway-api`** (port **8080** internal)

## Routes

| Upstream | Downstream (Docker) | JWT when secure mode |
|----------|---------------------|----------------------|
| `/api/identity/health` | `http://identity-api:8080/health` | Public |
| `/api/identity/{everything}` | `http://identity-api:8080/{everything}` | Bearer required |

Pattern for new services: `/api/{service}/…` → downstream Docker DNS name.

## Run locally

```bash
docker compose up -d --build
curl http://localhost:8080/api/identity/health
```

Step-by-step: [TESTING_SPRINT2.md](TESTING_SPRINT2.md).

## JWT

- Contract with Identity: [JWT.md](JWT.md).
- `Jwt:Enabled=false` (default compose): `ocelot.json` only, no tokens.
- `Jwt:Enabled=true`: loads `ocelot.secure.json` + JWT Bearer validation (`AuthenticationProviderKey`: `Bearer`).

## Nginx

`infra/nginx/nginx.conf` proxies `/api/` to **`http://gateway-api:8080`**. Static UI on `/`. Public entry: **`http://localhost:8080`**.
