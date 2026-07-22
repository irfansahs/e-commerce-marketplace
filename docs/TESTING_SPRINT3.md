# Sprint 3 test rehberi (i18n + Identity + RabbitMQ)

Tüm servisler **Docker Compose** içinde çalışır; host’ta `dotnet run` yok.

## 1) Stack

```powershell
cd C:\Users\irfan\OneDrive\Masaüstü\e-commerce
powershell -File scripts/dev-local.ps1 -Smoke
```

veya:

```powershell
docker compose up -d --build
```

Migration + admin seed: `identity-api` container’ında `Database__ApplyMigrationsOnStartup=true` ile otomatik.

## 2) API tabanı

| Yol | Açıklama |
|-----|----------|
| `http://localhost:8080/api/identity/...` | Nginx → Gateway → Identity (tercih edilen) |

## 3) Register (Türkçe hata metni)

```powershell
Invoke-RestMethod -Uri "http://localhost:8080/api/identity/auth/register" -Method Post `
  -ContentType "application/json" -Headers @{ "Accept-Language" = "tr" } `
  -Body '{"email":"buyer@test.com","password":"Test1234!"}'
```

201 + `accessToken` beklenir.

## 4) Login + /me

Login sonrası token ile:

```powershell
Invoke-RestMethod -Uri "http://localhost:8080/api/identity/auth/me" `
  -Headers @{ Authorization = "Bearer YOUR_TOKEN" }
```

## 5) Admin seed (dev)

`admin@marketplace.local` / `Admin123!` — login endpoint aynı.

## 6) RabbitMQ

- UI: http://localhost:15672 (`.env` kullanıcı/şifre)
- Exchange `marketplace.events` register sonrası oluşur.

## 7) Loglar

```powershell
docker compose logs -f identity-api gateway-api
```
