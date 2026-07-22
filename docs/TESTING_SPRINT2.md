# Sprint 2 test rehberi (Gateway + Nginx)

Önkoşul: Docker Desktop. Identity + Gateway yalnızca **container** içinde çalışır.

## 1) Stack

```powershell
cd C:\Users\irfan\OneDrive\Masaüstü\e-commerce
docker compose up -d --build
```

(Nginx eski config ile kaldıysa: `docker compose restart nginx`)

## 2) Kontroller (edge üzerinden)

```powershell
curl.exe -s http://localhost:8080/api/identity/health
curl.exe -s http://localhost:8080/health
```

Gateway health doğrudan host’tan yok; Nginx `/api/` → `gateway-api:8080` → Ocelot → `identity-api:8080`.

Correlation id:

```powershell
curl.exe -s -D - http://localhost:8080/api/identity/health -o NUL
```

## 3) Build (CI / IDE)

```powershell
dotnet build Marketplace.slnx
```

Host’ta `dotnet run` kullanılmaz.

## 4) JWT modu

Varsayılan compose: `Jwt__Enabled=false` gateway container’ında.

Detay: [JWT.md](JWT.md), [GATEWAY.md](GATEWAY.md).

## 5) Sorun giderme

| Belirti | Olası neden |
|---------|-------------|
| 502 `/api/` | Gateway henüz healthy değil veya Nginx eski config — `docker compose ps`, `docker compose restart nginx` |
| Identity unhealthy | MSSQL/RabbitMQ — `docker compose logs identity-api` |
