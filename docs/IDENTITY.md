# Identity service

Project: [services/identity/Marketplace.Identity.Api](../services/identity/Marketplace.Identity.Api)

## Database

- SQL Server: `identity_db`
- Table: `Users` (EF migrations under `Data/Migrations`)

## Roles

`Buyer` (public register), `Seller`, `Admin` (dev seed only).

## API

| Method | Path | Auth |
|--------|------|------|
| POST | `/auth/register` | Public — creates **Buyer** |
| POST | `/auth/login` | Public |
| GET | `/auth/me` | Bearer JWT |

Via gateway: prefix `/api/identity` (e.g. `/api/identity/auth/login`).

## JWT

Same contract as [JWT.md](JWT.md). Signing settings in `appsettings` (`Jwt:SigningKey` must match gateway when `Jwt:Enabled=true`).

## i18n

Errors use `errorCode` + localized `title` ([I18N.md](I18N.md)). `PreferredCulture` stored on user (`tr-TR` default).

## Dev admin seed

- Email: `admin@marketplace.local`
- Password: `Admin123!` (dev only — see [TESTING_SPRINT3.md](TESTING_SPRINT3.md))

## Events

[MESSAGING.md](MESSAGING.md) — `UserRegistered` on register.
