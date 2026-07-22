# Logging standard (Sprint 1)

Shared package: `Marketplace.BuildingBlocks` (`services/building-blocks/`).

## Usage in a service

```csharp
builder.AddMarketplaceLogging("identity");
// ...
app.UseMarketplaceCorrelationId();
app.UseSerilogRequestLogging();
```

## Where to log what (best practice)

| Concern | Service | Level / event |
|---------|---------|----------------|
| Failed / successful login, register | **Identity** | Warning / Information — `EventType` e.g. `identity.auth.login_failed` |
| HTTP edge latency, 5xx proxy | **Gateway** | Request logging (Serilog) |
| Business domain (orders, stock) | Owning microservice | Information / Warning |
| UI / BFF cookie issues | Next.js (later) | Optional; auth outcomes still belong in Identity |

Do **not** put auth success/failure audit only on Gateway: Gateway is a pass-through and does not know credential validity. Never log passwords or JWTs.

Identity helpers: `AuthAudit` (`identity.auth.login_failed`, `login_succeeded`, `register_*`).

Graylog search examples:

```
EventType:identity.auth.login_failed
service:identity
```

## Correlation ID

- Request header: `X-Correlation-Id` (generated if missing).
- Echoed on the response.
- Pushed to Serilog `LogContext` as `CorrelationId`.

## GELF → Graylog

| Setting | Default | Notes |
|---------|---------|--------|
| `Gelf:Enabled` | `true` | Set `false` to skip Graylog sink |
| `Gelf:Host` | `localhost` | From Docker services use **`graylog`** (compose DNS) |
| `Gelf:Port` | `12201` | GELF UDP |

Console sink is always on. Graylog down does not block startup (UDP).

Local compose sets `Gelf__Host=graylog` on `identity-api` / `gateway-api`. Start observability profile so the `graylog` hostname resolves on `marketplace-network`:

```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile observability up -d
```

In Graylog UI: **System → Inputs → GELF UDP** on `0.0.0.0:12201` if not already created. See [infra/graylog/README.md](../infra/graylog/README.md).
