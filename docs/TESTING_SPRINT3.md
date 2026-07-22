# Sprint 3 test rehberi (i18n + Identity + RabbitMQ)

## 1) Altyapı

```powershell
cd C:\Users\irfan\OneDrive\Masaüstü\e-commerce
docker compose up -d mssql rabbitmq
dotnet build Marketplace.slnx
```

SQL Server: `localhost,14330` (Docker; Windows’ta yerel SQL 1433 ile çakışmaması için). Şifre `.env` → `MSSQL_SA_PASSWORD`. Identity `launchSettings` veya shell:

```powershell
$env:MSSQL_SA_PASSWORD = "Marketplace_Local1!"
$env:MSSQL_HOST_PORT = "14330"
```

Tek komut: `powershell -File scripts/dev-local.ps1 -StartApis -Smoke`

Migration uygulaması Identity ilk çalıştırmada (Development) otomatik yapılır.

## 2) Servisleri başlat

**Terminal A**

```powershell
dotnet run --project services\identity\Marketplace.Identity.Api
```

**Terminal B**

```powershell
dotnet run --project services\gateway\Marketplace.Gateway.Api
```

## 3) Register (Türkçe hata metni)

```powershell
curl.exe -s -X POST http://localhost:5280/api/identity/auth/register `
  -H "Content-Type: application/json" `
  -H "Accept-Language: tr" `
  -d "{\"email\":\"buyer@test.com\",\"password\":\"Test1234!\"}"
```

201 + `accessToken` beklenir.

Aynı e-posta tekrar (409, `errorCode`: `IDENTITY_EMAIL_TAKEN`, Türkçe `title`):

```powershell
curl.exe -s -X POST http://localhost:5280/api/identity/auth/register `
  -H "Content-Type: application/json" `
  -H "Accept-Language: tr" `
  -d "{\"email\":\"buyer@test.com\",\"password\":\"Test1234!\"}"
```

İngilizce `title` için `Accept-Language: en` kullan; `errorCode` aynı kalmalı.

## 4) Login + /me

```powershell
curl.exe -s -X POST http://localhost:5280/api/identity/auth/login `
  -H "Content-Type: application/json" `
  -d "{\"email\":\"buyer@test.com\",\"password\":\"Test1234!\"}"
```

Token’ı kopyala:

```powershell
curl.exe -s http://localhost:5280/api/identity/auth/me `
  -H "Authorization: Bearer YOUR_TOKEN_HERE"
```

## 5) Admin seed (dev)

```powershell
curl.exe -s -X POST http://localhost:5280/api/identity/auth/login `
  -H "Content-Type: application/json" `
  -d "{\"email\":\"admin@marketplace.local\",\"password\":\"Admin123!\"}"
```

## 6) RabbitMQ

- UI: http://localhost:15672 (user/pass `.env` — default `marketplace` / `change_me_local_only`)
- Exchange `marketplace.events` register sonrası oluşur; publish log Identity konsolunda görünür.

## 7) Nginx (opsiyonel)

Gateway + Identity açıkken:

```powershell
docker compose up -d nginx
curl.exe -s http://localhost:8080/api/identity/health
```

Bkz. [TESTING_SPRINT2.md](TESTING_SPRINT2.md).
