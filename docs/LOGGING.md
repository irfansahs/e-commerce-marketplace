# Logging standard (Sprint 1)

Shared package: `Marketplace.BuildingBlocks` (`services/building-blocks/`).

## Usage in a service

```csharp
builder.AddMarketplaceLogging("identity");
// ...
app.UseMarketplaceCorrelationId();
app.UseSerilogRequestLogging();
```

## Correlation ID

- Request header: `X-Correlation-Id` (generated if missing).
- Echoed on the response.
- Pushed to Serilog `LogContext` as `CorrelationId`.

## GELF → Graylog

| Setting | Default | Notes |
|---------|---------|--------|
| `Gelf:Enabled` | `true` | Set `false` to skip Graylog sink |
| `Gelf:Host` | `localhost` | Graylog host from the app host |
| `Gelf:Port` | `12201` | GELF UDP |

Console sink is always on. Graylog down does not block startup (UDP).

Start observability:

```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile observability up -d
```

In Graylog UI: **System → Inputs → GELF UDP** on `0.0.0.0:12201` if not already created. See [infra/graylog/README.md](../infra/graylog/README.md).
