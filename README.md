# E-Commerce Marketplace

Çok satıcılı (multi-vendor) e-ticaret pazaryeri. Mikroservis mimarisi, Docker Compose ile lokal geliştirme, ileride AWS EC2 üzerinde CI/CD.

## Stack (özet)

| Katman | Teknoloji |
|--------|-----------|
| Frontend | Next.js (App Router) |
| Backend | .NET 10 mikroservisler |
| API Gateway | Ocelot |
| Edge | Nginx |
| Messaging | RabbitMQ |
| Cache | Redis |
| Database | SQL Server 2022 (servis başına DB) |
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

Cursor/VS Code'da backend, UI ve altyapıyı tek E-Commerce çalışma alanında açmak için:

```bash
code E-Commerce.code-workspace
```

.NET projeleri güncel XML solution formatındaki `Marketplace.slnx` içindedir. SDK sürümü `global.json`, ortak derleme kuralları `Directory.Build.props` ile yönetilir.

## Lokal altyapı (Docker)

Önkoşul: [Docker Desktop](https://www.docker.com/products/docker-desktop/) veya Docker Engine + Compose v2.

```bash
cp .env.example .env
docker compose up -d
```

| Servis | Port | Açıklama |
|--------|------|----------|
| SQL Server | 1433 | Servis DB’leri (aşağıda); user `sa` |
| Redis | 6379 | Cache |
| RabbitMQ | 5672, 15672 | AMQP + Management UI |
| Nginx | 8080 | Static `index.html` (+ ileride Ocelot) |

RabbitMQ yönetim arayüzü: `http://localhost:15672` (kullanıcı/şifre `.env` içinde).

**SQL Server servis veritabanları** (`mssql-init` container’ı ile oluşturulur):

`identity_db`, `catalog_db`, `cart_order_db`, `inventory_db`, `payment_db`, `notification_db`

Init script: [infra/mssql/init/01-create-databases.sql](infra/mssql/init/01-create-databases.sql) (idempotent T-SQL; her `up` ile eksik DB’ler eklenir). Bağlantı: `localhost,1433`, user `sa`, şifre `.env` (`MSSQL_SA_PASSWORD`). Volume’u sıfırlamak için:

```bash
docker compose down -v
docker compose up -d
```

Durdurmak: `docker compose down` (veriler volume’larda kalır; `-v` volume’ları siler).

### Observability (Graylog, opsiyonel — lokal)

RAM ~2GB+ ister; EC2 web deploy’a dahil değil.

```bash
docker compose -f docker-compose.yml -f docker-compose.observability.yml --profile observability up -d
```

| Servis | Port | Açıklama |
|--------|------|----------|
| Graylog UI | 9000 | `admin` + `.env` şifre hash’i (dev: `admin`) |
| GELF UDP | 12201 | Serilog / container logları (ileri) |

Bkz. [infra/graylog/README.md](infra/graylog/README.md).

## Geliştirme sırası

Sprint ve backlog GitHub Projects üzerinde yönetilir. Önce altyapı (Docker, gateway, observability), sonra domain servisleri ve UI.

Bkz. [docs/SPRINTS.md](docs/SPRINTS.md).

Gateway + Nginx test adımları (Sprint 2): [docs/TESTING_SPRINT2.md](docs/TESTING_SPRINT2.md).

Identity + i18n (Sprint 3): [docs/TESTING_SPRINT3.md](docs/TESTING_SPRINT3.md).

## Cursor (AI)

- Proje bağlamı: [AGENTS.md](AGENTS.md)
- Rehber: [docs/CURSOR_GUIDE.md](docs/CURSOR_GUIDE.md)
- Rules: `.cursor/rules/` · Skills: `.cursor/skills/` (ör. `/deploy-ec2`)

## Güvenlik

Asla commit etmeyin: `.env`, `*.pem`, gerçek API anahtarları. Bkz. [docs/SECURITY.md](docs/SECURITY.md).

## Lisans

MIT (veya repoda belirtildiği gibi).
