# Creates backlog issues and adds them to GitHub Project 2 with Sprint field.
# Run from repo root: pwsh -File scripts/create-backlog-issues.ps1

$ErrorActionPreference = "Stop"
$Owner = "irfansahs"
$Repo = "e-commerce-marketplace"
$ProjectNumber = 2
$ProjectId = "PVT_kwHOBVoo-s4BeBCs"
$SprintFieldId = "PVTSSF_lAHOBVoo-s4BeBCszhYeV3Y"

$Sprints = @{
  "S0" = "fb89d1e9"
  "S1" = "552df1b8"
  "S2" = "87b6ee03"
  "S3" = "09327db0"
  "S4" = "d20c7fd5"
  "S5" = "a1d1c17d"
  "S6" = "54764d84"
  "S7" = "1f44951c"
  "S8" = "4074e28b"
  "S9" = "ff927d4f"
  "S10" = "4c84c7e6"
  "AWS" = "c07e7258"
}

$issues = @(
  @{ Title = "[Sprint 0] Epic: Monorepo and Docker platform"; Labels = "epic,infra"; Sprint = "S0"; Body = "Foundation sprint: monorepo layout, Compose, base infra services." },
  @{ Title = "[Sprint 0] Per-service SQL Server database naming"; Labels = "infra,backend"; Sprint = "S0"; Body = "Create identity_db, catalog_db, etc. via infra/mssql/init T-SQL and EF migrations." },
  @{ Title = "[Sprint 0] Document local runbook in README"; Labels = "docs"; Sprint = "S0"; Body = "Verify README matches compose ports and env vars." },

  @{ Title = "[Sprint 1] Graylog stack (Compose profile observability)"; Labels = "infra,observability"; Sprint = "S1"; Body = "Graylog + dependencies in docker-compose.observability.yml" },
  @{ Title = "[Sprint 1] Serilog + GELF correlation-id standard"; Labels = "backend,observability"; Sprint = "S1"; Body = "BuildingBlocks logging package spike." },
  @{ Title = "[Sprint 1] Healthcheck convention /health"; Labels = "backend,docs"; Sprint = "S1"; Body = "Document and enforce health endpoints on all HTTP services." },

  @{ Title = "[Sprint 2] Ocelot gateway service scaffold"; Labels = "gateway,backend"; Sprint = "S2"; Body = "Empty host, ocelot.json route template /api/{service}/..." },
  @{ Title = "[Sprint 2] JWT validation at gateway"; Labels = "gateway,backend"; Sprint = "S2"; Body = "Align claims with Identity service contract." },
  @{ Title = "[Sprint 2] Nginx upstream to Ocelot"; Labels = "gateway,infra"; Sprint = "S2"; Body = "Uncomment proxy_pass in infra/nginx/nginx.conf" },

  @{ Title = "[Sprint 3] Identity service - register/login/JWT"; Labels = "backend"; Sprint = "S3"; Body = "Roles: Buyer, Seller, Admin." },
  @{ Title = "[Sprint 3] UserRegistered event to RabbitMQ"; Labels = "backend,messaging"; Sprint = "S3"; Body = "Outbox or direct publish pattern." },

  @{ Title = "[Sprint 4] Catalog - product/category/seller listing"; Labels = "backend"; Sprint = "S4"; Body = "Multi-vendor ownership rules." },
  @{ Title = "[Sprint 4] Catalog domain events"; Labels = "backend,messaging"; Sprint = "S4"; Body = "ProductCreated/Updated publishers." },

  @{ Title = "[Sprint 5] Inventory stock reservation model"; Labels = "backend"; Sprint = "S5"; Body = "Reserve/release on order flow." },
  @{ Title = "[Sprint 5] RabbitMQ topology exchanges queues DLQ"; Labels = "messaging,infra"; Sprint = "S5"; Body = "Document idempotency and retry policy." },

  @{ Title = "[Sprint 6] Cart API"; Labels = "backend"; Sprint = "S6"; Body = "Session/user cart." },
  @{ Title = "[Sprint 6] Order checkout state machine"; Labels = "backend,messaging"; Sprint = "S6"; Body = "Saga/choreography outline with Inventory and Payment." },

  @{ Title = "[Sprint 7] Payment adapter stub"; Labels = "backend"; Sprint = "S7"; Body = "PaymentSucceeded/Failed events." },
  @{ Title = "[Sprint 7] Notification service event consumer"; Labels = "backend,messaging"; Sprint = "S7"; Body = "Email/in-app stub." },

  @{ Title = "[Sprint 8] Next.js App Router scaffold"; Labels = "frontend"; Sprint = "S8"; Body = "apps/web with auth and catalog browse." },
  @{ Title = "[Sprint 8] Product detail SEO pages"; Labels = "frontend"; Sprint = "S8"; Body = "SSR/SSG for storefront." },

  @{ Title = "[Sprint 9] Seller panel route group and authz"; Labels = "frontend,backend"; Sprint = "S9"; Body = "Product and inventory management UI." },

  @{ Title = "[Sprint 10] CI build test docker"; Labels = "infra,backend,frontend"; Sprint = "S10"; Body = "GitHub Actions on PR." },
  @{ Title = "[Sprint 10] Security checklist"; Labels = "docs,backend"; Sprint = "S10"; Body = "CORS, secrets, gateway auth review." },

  @{ Title = "[Faz C] AWS EC2 deploy via GitHub Actions"; Labels = "epic,aws,infra"; Sprint = "AWS"; Body = "See docs/CI_CD.md. Secrets: EC2_HOST, EC2_USER, EC2_SSH_KEY." },
  @{ Title = "[Faz C] Verify Docker on EC2 and clone repo"; Labels = "aws,infra"; Sprint = "AWS"; Body = "Pre-deploy smoke on EC2." },
  @{ Title = "[Faz C] deploy-ec2.yml workflow"; Labels = "aws"; Sprint = "AWS"; Body = "SSH deploy docker compose up -d. No PEM in repo." }
)

foreach ($issue in $issues) {
  $labelArg = $issue.Labels -split "," | ForEach-Object { "--label"; $_.Trim() }
  $issueUrl = gh issue create --repo "$Owner/$Repo" --title $issue.Title --body $issue.Body @labelArg
  Write-Host "Created: $issueUrl"
  $itemJson = gh project item-add $ProjectNumber --owner $Owner --url $issueUrl --format json
  $itemId = ($itemJson | ConvertFrom-Json).id
  $sprintOpt = $Sprints[$issue.Sprint]
  gh project item-edit --id $itemId --project-id $ProjectId --field-id $SprintFieldId --single-select-option-id $sprintOpt | Out-Null
}

Write-Host "Done. $($issues.Count) issues created."
