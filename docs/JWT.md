# JWT contract (Gateway ↔ Identity)

Sprint 2 gateway validation; Sprint 3 Identity will issue tokens with this shape.

| Setting | Value |
|---------|--------|
| Issuer (`iss`) | `marketplace-identity` |
| Audience (`aud`) | `marketplace-api` |
| Algorithm | HS256 (symmetric, dev/stub) |
| Header | `Authorization: Bearer {token}` |

## Claims (MVP)

| Claim | Purpose |
|-------|---------|
| `sub` | User id (GUID string) |
| `email` | Login email |
| `role` | `Buyer`, `Seller`, or `Admin` |

Gateway reads the same `Jwt:*` settings as Identity will use for signing (see gateway `appsettings.json`).

## Routes and auth

| Path | JWT required when `Jwt:Enabled=true` |
|------|--------------------------------------|
| `GET /api/identity/health` | No (public probe) |
| Other `/api/identity/*` | Yes |
| `GET /health` on gateway (`:5280`) | No (Ocelot öncesi middleware; liveness) |

When `Jwt:Enabled=true`, gateway loads `ocelot.secure.json` (protected identity routes use provider key `Bearer`).
