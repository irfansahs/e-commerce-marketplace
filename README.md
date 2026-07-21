# E-Commerce Marketplace

Çok satıcılı (multi-vendor) e-ticaret pazaryeri. Mikroservis mimarisi, Docker Compose ile lokal geliştirme, ileride AWS EC2 üzerinde CI/CD.

## Stack (özet)

| Katman | Teknoloji |
|--------|-----------|
| Frontend | Next.js (App Router) |
| Backend | .NET 8 mikroservisler |
| API Gateway | Ocelot |
| Edge | Nginx |
| Messaging | RabbitMQ |
| Cache | Redis |
| Database | PostgreSQL (servis başına DB) |
| Logs (ileri) | Graylog + Serilog |
| Lokal | Docker Compose |

Detay: [docs/STACK.md](docs/STACK.md), [docs/ARCHITECTURE.md](docs/ARCHITECTURE.md).

## Repo yapısı

```
apps/web/              Next.js (planlanıyor)
services/              Mikroservisler + Ocelot gateway
infra/                 Nginx, Graylog vb.
docs/                  Mimari ve sprint dokümanları
```

## Lokal altyapı (Docker)

Önkoşul: [Docker Desktop](https://www.docker.com/products/docker-desktop/) veya Docker Engine + Compose v2.

```bash
cp .env.example .env
docker compose up -d
```

| Servis | Port | Açıklama |
|--------|------|----------|
| PostgreSQL | 5432 | `marketplace` + servis DB’leri (aşağıda) |
| Redis | 6379 | Cache |
| RabbitMQ | 5672, 15672 | AMQP + Management UI |
| Nginx | 8080 | Static `index.html` (+ ileride Ocelot) |

RabbitMQ yönetim arayüzü: `http://localhost:15672` (kullanıcı/şifre `.env` içinde).

**PostgreSQL servis veritabanları** (ilk `docker compose up` ile, boş volume):

`identity_db`, `catalog_db`, `cart_order_db`, `inventory_db`, `payment_db`, `notification_db`

Init script: [infra/postgres/init/01-create-databases.sql](infra/postgres/init/01-create-databases.sql). Mevcut volume’da init tekrar çalışmaz; DB’leri sıfırlamak için:

```bash
docker compose down -v
docker compose up -d
```

Durdurmak: `docker compose down` (veriler volume’larda kalır; `-v` volume’ları siler).

## Geliştirme sırası

Sprint ve backlog GitHub Projects üzerinde yönetilir. Önce altyapı (Docker, gateway, observability), sonra domain servisleri ve UI.

Bkz. [docs/SPRINTS.md](docs/SPRINTS.md).

## Cursor (AI)

- Proje bağlamı: [AGENTS.md](AGENTS.md)
- Rehber: [docs/CURSOR_GUIDE.md](docs/CURSOR_GUIDE.md)
- Rules: `.cursor/rules/` · Skills: `.cursor/skills/` (ör. `/deploy-ec2`)

## Güvenlik

Asla commit etmeyin: `.env`, `*.pem`, gerçek API anahtarları. Bkz. [docs/SECURITY.md](docs/SECURITY.md).

## Lisans

MIT (veya repoda belirtildiği gibi).
