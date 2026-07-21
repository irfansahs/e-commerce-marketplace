---
name: deploy-ec2
description: Deploy static web to AWS EC2 via GitHub Actions or manual SSH. Use when user mentions EC2, deploy, production site, or deploy-ec2 workflow.
---

# Deploy to EC2

## Preconditions

- GitHub Secrets set: `EC2_HOST`, `EC2_USER`, `EC2_SSH_KEY` (never commit PEM).
- Workflow: [.github/workflows/deploy-ec2.yml](.github/workflows/deploy-ec2.yml)
- Stack file: [docker-compose.web.yml](docker-compose.web.yml) (Nginx + `apps/web/public`)

## Automated (preferred)

1. Push to `main` or run **Actions → Deploy to EC2 → Run workflow**.
2. Workflow SSHs to EC2, clones/updates `~/e-commerce-marketplace`, runs `docker compose -f docker-compose.web.yml up -d`.
3. Verify: `curl -sI http://$EC2_HOST/` → HTTP 200, HTML body.

## Manual smoke (if Actions fails)

On EC2 (via SSH, key local only):

```bash
cd ~/e-commerce-marketplace
git pull origin main
export NGINX_PORT=80
sudo docker compose -f docker-compose.web.yml pull
sudo docker compose -f docker-compose.web.yml up -d
sudo docker compose -f docker-compose.web.yml ps
curl -sI http://127.0.0.1/
```

## EC2 security group

- Inbound: 22 (SSH, restricted), 80 (HTTP).

## Do not

- Put PEM or host secrets in repo, issues, or `docs/decisions/`.
- Deploy full `docker-compose.yml` on small EC2 without user approval (SQL Server needs ~2GB+ RAM; small EC2 cannot run it).

See [docs/CI_CD.md](docs/CI_CD.md).
