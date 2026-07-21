# Health endpoint convention (Sprint 1)

Every HTTP microservice **must** expose:

```http
GET /health
```

## Response

- Status: `200 OK`
- Body (JSON):

```json
{ "status": "Healthy", "service": "identity" }
```

## Shared helper

```csharp
app.MapMarketplaceHealth("identity");
```

From `Marketplace.BuildingBlocks.Health`. Prefer calling after `AddMarketplaceLogging` so `service` can be resolved from DI when the name argument is omitted.

## Probes

- Docker / Compose: `curl -f http://localhost:<port>/health` or HTTP healthcheck.
- Kubernetes (later): liveness/readiness → same path.
- Do not require auth on `/health`.
