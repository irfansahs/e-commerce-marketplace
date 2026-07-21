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

- PostgreSQL: her servis kendi veritabanı (`identity_db`, `catalog_db`, …).
- Redis: oturum/cache, gateway QoS yardımcıları.
- RabbitMQ: exchange/queue tasarımı servis başına dokümante edilecek (DLQ, idempotency).

## Observability (ileri sprint)

- Serilog + correlation id
- Graylog (GELF)
- Health: `/health` tüm HTTP servislerde

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
