# AWS CI/CD (Faz C — plan)

Bu faz **repo + lokal Docker** tamamlandıktan sonra uygulanır.

## Hedef

`main` branch push veya manuel workflow → GitHub Actions → SSH ile EC2 → `git pull` + `docker compose up -d`.

## GitHub Secrets

| Secret | Açıklama |
|--------|----------|
| `EC2_HOST` | EC2 public hostname veya IP |
| `EC2_USER` | Örn. `ec2-user` |
| `EC2_SSH_KEY` | Private key içeriği (PEM) |

## EC2 önkoşullar

- Docker + Docker Compose plugin
- Git
- Repo clone path (örn. `/home/ec2-user/e-commerce-marketplace`)
- Security group: SSH kaynağı kısıtlı

## Workflow (eklenecek)

Dosya: `.github/workflows/deploy-ec2.yml` (henüz aktif değil; issue ile takip edilir)

Adımlar:

1. Checkout
2. SSH action ile EC2’ye bağlan
3. `cd` repo → `git pull origin main`
4. `docker compose pull` (image’lar hazır olduğunda)
5. `docker compose up -d`

## Not

`screen -r` gibi oturum yönetimi opsiyonel; production için `systemd` unit veya compose restart policy tercih edilir.
