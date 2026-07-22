# Mimari

## Genel bakış

İstek akışı: **Client (Next.js)** → **Nginx** → **Ocelot Gateway** → **Mikroservisler**.

Asenkron iletişim: **RabbitMQ** (domain events, saga adımları).

## Bounded context’ler (MVP)

| Servis | Sorumluluk |
|--------|------------|
| Identity | Kayıt, giriş, JWT, roller (Buyer, Seller, Admin) |
| Catalog | Ürün, kategori, satıcı listeleme |
| Inventory | Stok, rezervasyon |
| Cart/Order | Sepet, sipariş, checkout state |
| Payment | Ödeme adaptörü (stub → gerçek PSP) |
| Notification | E-posta / in-app bildirim (event consumer) |
| Gateway | Ocelot: routing, auth, rate limit |

## Veri

- SQL Server 2022: tek instance; servis başına database:
  - `identity_db`, `catalog_db`, `cart_order_db`, `inventory_db`, `payment_db`, `notification_db`
- Lokal oluşturma: [infra/mssql/init/01-create-databases.sql](infra/mssql/init/01-create-databases.sql) (`mssql-init` container’ı, idempotent T-SQL). Host bağlantısı `localhost,14330` (`.env` `MSSQL_HOST_PORT`), user `sa`.
- Redis: oturum/cache, gateway QoS yardımcıları.
- RabbitMQ: exchange/queue tasarımı servis başına dokümante edilecek (DLQ, idempotency).

## Observability (Sprint 1 — lokal)

- Compose: [docker-compose.observability.yml](../docker-compose.observability.yml) profile `observability` (Graylog 6 + MongoDB + OpenSearch).
- UI: http://localhost:9000 · GELF UDP: 12201.
- .NET (Sprint 1): Serilog → GELF via BuildingBlocks; see [LOGGING.md](LOGGING.md).
- Health: `GET /health` on all HTTP services; see [HEALTH.md](HEALTH.md).
- EC2: yalnızca `docker-compose.web.yml`; Graylog lokal geliştirmede.

## Monorepo

```
apps/web/
services/building-blocks/   # Serilog GELF + /health helpers
services/gateway/           # Ocelot API gateway
services/identity/          # Identity API (scaffold)
services/catalog/
services/cart-order/
services/inventory/
services/payment/
services/notification/
infra/nginx/
infra/graylog/
infra/mssql/
```

## Deploy hedefi (Faz C)

GitHub Actions → AWS EC2 üzerinde `docker compose`. SSH anahtarı yalnızca GitHub Secrets’ta.
