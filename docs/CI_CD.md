# AWS CI/CD

## Akış

`main` push veya manuel **Actions → Deploy to EC2** → SSH → `git pull` → `docker compose -f docker-compose.web.yml up -d`

Şimdilik EC2’de yalnızca Nginx + `apps/web/public/index.html` (port 80).

## GitHub Secrets

| Secret | Açıklama |
|--------|----------|
| `EC2_HOST` | EC2 public IP veya hostname |
| `EC2_USER` | `ec2-user` |
| `EC2_SSH_KEY` | Private key içeriği (PEM) |

Secrets’ı CLI ile set etmek:

```bash
gh secret set EC2_HOST --body "YOUR_IP"
gh secret set EC2_USER --body "ec2-user"
gh secret set EC2_SSH_KEY < bot-anahtar.pem
```

**PEM asla commit edilmez** (`.gitignore`: `*.pem`).

## EC2 önkoşullar

- Docker Engine + Compose plugin
- Git
- Security group: **22** (SSH), **80** (HTTP)
- Repo path: `~/e-commerce-marketplace`

## Lokal web smoke

```bash
docker compose -f docker-compose.web.yml up -d
# http://localhost:80  (Windows’ta admin gerekebilir; alternatif: NGINX_PORT=8080)
```

Tam altyapı (SQL Server/Redis/RabbitMQ) için kök `docker-compose.yml` kullanılır.
