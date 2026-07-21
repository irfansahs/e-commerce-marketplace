# Teknoloji stack

## Seçimler ve gerekçe

- **Next.js (App Router)**: Storefront SEO, SSR/SSG, satıcı/admin route group’ları tek uygulamada.
- **.NET 8**: Mikroservisler, performans, ekosistem (EF Core, MassTransit vb.).
- **Ocelot**: .NET ile uyumlu API gateway, route/QoS/JWT entegrasyonu.
- **Nginx**: Edge reverse proxy, TLS termination (prod), static/SSR yönlendirme.
- **RabbitMQ**: Olgun mesajlaşma, management UI, DLQ desenleri.
- **Redis**: Dağıtık cache, rate limit yardımcıları.
- **PostgreSQL**: Servis başına DB, ACID.
- **Graylog**: Merkezi log, GELF ile container/servis entegrasyonu.
- **Docker Compose**: Lokal ve EC2’de tutarlı ortam.

## Henüz eklenmeyen (backlog)

Ocelot host, .NET servis image’ları, Graylog compose profile, Next.js app — sprint issue’ları ile eklenecek.
