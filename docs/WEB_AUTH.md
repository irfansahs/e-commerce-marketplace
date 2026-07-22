# Web auth (Next.js BFF)

Storefront auth lives in `apps/web`. Identity JWT is never exposed to JavaScript; the browser only talks to same-origin Next Route Handlers behind Nginx.

## Flow

1. Browser → `POST /api/auth/login` or `/api/auth/register` (Nginx → `web:3000`)
2. Next handler → `INTERNAL_API_URL` + `/api/identity/auth/...` (Docker DNS → `gateway-api:8080`)
3. On success, JWT is stored in httpOnly cookie `marketplace_access_token` (`SameSite=Lax`, `Secure` when `COOKIE_SECURE=true`)
4. `GET /api/auth/me` and Account page read the cookie and call Identity `/auth/me` with `Authorization: Bearer`
5. `POST /api/auth/logout` deletes the cookie
6. Middleware redirects unauthenticated users away from `/[locale]/account` to login

## Pages

| Path | Purpose |
|------|---------|
| `/tr` or `/en` | Home CTA |
| `/[locale]/login` | Login form |
| `/[locale]/register` | Buyer register |
| `/[locale]/account` | Session (`/me`) + logout |

## Env (container)

| Variable | Meaning |
|----------|---------|
| `INTERNAL_API_URL` | Gateway base, e.g. `http://gateway-api:8080` |
| `COOKIE_SECURE` | `true` only behind HTTPS |

## Errors

API `errorCode` (e.g. `IDENTITY_EMAIL_TAKEN`) maps to `messages/{locale}.json` → `errors.*`; fallback is ProblemDetails `title`.

## Stack notes

- Tailwind + shadcn-style UI, `next-themes` (dark/light), `next-intl` (`tr` default)
- No NextAuth; no host `pnpm dev` / `dotnet run` for local stack — `docker compose up -d --build`

See [TESTING_SPRINT4.md](TESTING_SPRINT4.md).
