# Sprint 4 test rehberi (Next.js auth UI)

Stack tamamen Docker. Tek tarayıcı girişi: **http://localhost:8080**

## 1) Up

```powershell
cd C:\Users\irfan\OneDrive\Masaüstü\e-commerce
docker compose up -d --build
```

Bekle: `marketplace-web` + `marketplace-nginx` healthy.

```powershell
docker compose ps
curl.exe -s -o NUL -w "%{http_code}" http://localhost:8080/tr/login
```

## 2) Tarayıcı

1. Aç: http://localhost:8080/tr/login  
2. Tema ikonu ile dark/light değişimini doğrula  
3. Dil: header’da `EN` → `/en/login`  
4. Register: yeni e-posta + şifre (≥8) → `/tr/account` (email + rol)  
5. Logout → login  
6. Login: `admin@marketplace.local` / `Admin123!` → account  
7. Yanlış şifre → toast (tr/en)

## 3) BFF smoke (cookie)

```powershell
# Login sets httpOnly cookie (use -c/-b jar)
curl.exe -s -c cookies.txt -X POST http://localhost:8080/api/auth/login `
  -H "Content-Type: application/json" `
  -d "{\"email\":\"admin@marketplace.local\",\"password\":\"Admin123!\",\"locale\":\"tr\"}"

curl.exe -s -b cookies.txt http://localhost:8080/api/auth/me
```

## 4) Gateway identity (doğrudan, cookie’siz)

```powershell
curl.exe -s http://localhost:8080/api/identity/health
```

## 5) Loglar

```powershell
docker compose logs -f web gateway-api identity-api nginx
```

Detay: [WEB_AUTH.md](WEB_AUTH.md).
