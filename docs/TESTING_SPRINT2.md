# Sprint 2 test rehberi (Gateway + Nginx)

Önkoşul: .NET 10 SDK, Docker Desktop (Nginx testi için).

## 1) Build

```powershell
cd C:\Users\irfan\OneDrive\Masaüstü\e-commerce
dotnet build Marketplace.slnx
```

## 2) Identity + Gateway (doğrudan)

İki ayrı terminal:

**Terminal A — Identity**

```powershell
dotnet run --project services\identity\Marketplace.Identity.Api
```

Beklenen: `http://localhost:5211` dinliyor.

**Terminal B — Gateway**

```powershell
dotnet run --project services\gateway\Marketplace.Gateway.Api
```

Beklenen: `http://localhost:5280` dinliyor.

### Kontroller

```powershell
# Identity doğrudan
curl.exe -s http://localhost:5211/health

# Gateway kendi health
curl.exe -s http://localhost:5280/health

# Gateway üzerinden Identity (Ocelot)
curl.exe -s http://localhost:5280/api/identity/health
```

Hepsi `200` ve JSON içinde `"status":"Healthy"`. Identity üzerinden geçen çağrıda `"service":"identity"` görürsün.

Correlation id (opsiyonel):

```powershell
curl.exe -s -D - http://localhost:5280/health -o NUL
```

Response header’da `X-Correlation-Id` olmalı.

## 3) Nginx edge (Docker + host’ta gateway)

Gateway ve Identity **host’ta** çalışırken Nginx container’ı `/api/` isteklerini `host.docker.internal:5280`’e yollar.

```powershell
docker compose up -d nginx
```

(Nginx zaten ayaktaysa config değiştiyse: `docker compose up -d --force-recreate nginx`)

```powershell
# Static landing
curl.exe -sI http://localhost:8080/

# API edge → gateway → identity
curl.exe -s http://localhost:8080/api/identity/health
```

Port farklıysa `.env` içindeki `NGINX_PORT` değerini kullan.

## 4) JWT modu (Sprint 2 #8 — Identity olmadan smoke)

Varsayılan: `Jwt:Enabled=false` — token gerekmez.

JWT’yi denemek için geçici olarak `services/gateway/Marketplace.Gateway.Api/appsettings.Development.json`:

```json
"Jwt": {
  "Enabled": true,
  "Issuer": "marketplace-identity",
  "Audience": "marketplace-api",
  "SigningKey": "dev-only-signing-key-min-32-chars!!"
}
```

Gateway’i yeniden başlat. Token olmadan korumalı route 401 döner; health açık kalır:

```powershell
curl.exe -s -o NUL -w "%{http_code}" http://localhost:5280/api/identity/health
# 200

# Örnek: ileride eklenecek /api/identity/me gibi route — şimdilik sadece health var
```

Gerçek login/token Sprint 3’te Identity ile gelir. Sözleşme: [JWT.md](JWT.md).

## 5) Sorun giderme

| Belirti | Olası neden |
|---------|-------------|
| Connection refused 5211 | Identity çalışmıyor |
| Connection refused 5280 | Gateway çalışmıyor |
| 502 from Nginx `/api/` | Gateway kapalı veya Docker `host.docker.internal` erişemiyor (Linux’ta compose’a `extra_hosts` gerekebilir) |
| Ocelot hata startup | `ocelot.json` syntax; `dotnet build` loglarına bak |

Detay: [GATEWAY.md](GATEWAY.md).
