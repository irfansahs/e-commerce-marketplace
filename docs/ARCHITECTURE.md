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

- PostgreSQL: tek instance; servis başına database:
  - `marketplace` (varsayılan)
  - `identity_db`, `catalog_db`, `cart_order_db`, `inventory_db`, `payment_db`, `notification_db`
- Lokal oluşturma: [infra/postgres/init/01-create-databases.sql](infra/postgres/init/01-create-databases.sql) (`docker-entrypoint-initdb.d`, yalnızca boş volume).
- Redis: oturum/cache, gateway QoS yardımcıları.
- RabbitMQ: exchange/queue tasarımı servis başına dokümante edilecek (DLQ, idempotency).

## Observability (Sprint 1 — lokal)

- Compose: [docker-compose.observability.yml](../docker-compose.observability.yml) profile `observability` (Graylog 6 + MongoDB + OpenSearch).
- UI: http://localhost:9000 · GELF UDP: 12201.
- .NET (Sprint 1 #5): Serilog sink → GELF; correlation id middleware (BuildingBlocks).
- Health: `GET /health` tüm HTTP servislerde (Sprint 1 #6).
- EC2: yalnızca `docker-compose.web.yml`; Graylog lokal geliştirmede.

## Monorepo

```
apps/web/
services/gateway/      # Ocelot
services/identity/
services/catalog/
services/cart-order/
services/inventory/
services/payment/
services/notification/
infra/nginx/
infra/graylog/
```

## Deploy hedefi (Faz C)

GitHub Actions → AWS EC2 üzerinde `docker compose`. SSH anahtarı yalnızca GitHub Secrets’ta.
