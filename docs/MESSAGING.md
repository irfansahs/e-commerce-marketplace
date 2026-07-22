# Messaging (RabbitMQ)

Local broker: root [docker-compose.yml](../docker-compose.yml) (`rabbitmq` service, UI port **15672**).

## Identity — UserRegistered (Sprint 3)

| Item | Value |
|------|--------|
| Exchange | `marketplace.events` (topic, durable) |
| Routing key | `identity.user.registered` |
| Publisher | Identity API on successful register |
| Payload | `{ userId, email, role, preferredCulture, occurredAtUtc }` (locale-neutral) |

Consumers (Catalog, Notification) will be added in later sprints.

## Pattern

- MVP: **direct publish** via `IIntegrationEventPublisher` in Identity.
- Later: outbox + MassTransit (Sprint 5+).
