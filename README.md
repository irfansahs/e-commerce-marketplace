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
| PostgreSQL | 5432 | Veritabanı |
| Redis | 6379 | Cache |
| RabbitMQ | 5672, 15672 | AMQP + Management UI |
| Nginx | 8080 | Static `index.html` (+ ileride Ocelot) |

RabbitMQ yönetim arayüzü: `http://localhost:15672` (kullanıcı/şifre `.env` içinde).

Durdurmak: `docker compose down` (veriler volume’larda kalır).

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
