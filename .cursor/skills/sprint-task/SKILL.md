---
name: sprint-task
description: Create or refine GitHub Issues and Project board tasks with Sprint field. Use for backlog, sprint planning, or issue templates.
---

# Sprint task (GitHub)

## Project

- Owner: `irfansahs`
- Project: **E-Commerce Marketplace** (#2)
- Sprint field: single-select (Sprint 0 … Sprint 10, Faz C - AWS CI/CD)

## Create issue

```bash
gh issue create --repo irfansahs/e-commerce-marketplace \
  --title "[Sprint N] Short title" \
  --body "Acceptance criteria..." \
  --label infra,backend   # pick relevant labels
```

Labels: `epic`, `infra`, `backend`, `frontend`, `gateway`, `messaging`, `observability`, `docs`, `aws`.

## Add to Project + Sprint

1. `gh project item-add 2 --owner irfansahs --url <issue-url> --format json`
2. Set Sprint via `gh project item-edit` with field id and single-select option id (or set in GitHub UI).

Bulk bootstrap script (reference): [scripts/create-backlog-issues.ps1](scripts/create-backlog-issues.ps1).

## Issue quality

- One clear outcome per issue
- Acceptance criteria as checklist
- No secrets in body

See [docs/SPRINTS.md](docs/SPRINTS.md).
