# Cursor ile çalışmak (bu proje)

Kısa rehber: Agent, Rules, Skills, Hooks ve geçmişin nerede durduğu.

## Mağara adamı sözlüğü

| Parça | Ne? |
|-------|-----|
| **Agent** | İşçi: kod yazar, terminal, GitHub |
| **Plan / Ask / Debug** | Agent modları (tasarım / soru / hata avı) |
| **Rules** | Ev kuralları — `.cursor/rules/*.mdc` |
| **Skills** | Görev el kitabı — `.cursor/skills/*/SKILL.md` |
| **Hooks** | Kapı görevlisi — `.cursor/hooks.json` |
| **AGENTS.md** | Proje kimlik kartı (repo kökü) |

**Agent eski kalmadı.** Skills onun yerine geçmez; Agent’a “şu işi böyle yap” der.

## Ne nerede?

| Ne | Yer | Lokal silince |
|----|-----|----------------|
| Rules, Skills, AGENTS.md | GitHub repo | `git clone` ile gelir |
| Chat geçmişi | `%APPDATA%\Cursor\User\workspaceStorage` | **Cloud’a gitmez** |
| Transcript | `%USERPROFILE%\.cursor\projects\...\agent-transcripts\` | Lokal |

## Kalıcılık (best practice)

1. **AI ayarları repoda** — `.cursor/`, `AGENTS.md` commit edilir.
2. **Settings Sync** — Cursor hesabınla editör ayarlarını senkronla (chat değil):
   - `Ctrl+Shift+P` → **Settings Sync: Turn On**
   - İşaretle: Settings, Keybindings, Extensions (istediğin kadar)
3. **Önemli kararlar** — Chat → sağ tık **Export Transcript** → `docs/decisions/YYYY-MM-DD-konu.md` (secret/IP yok).
4. **SpecStory (opsiyonel)** — Otomatik chat yedeği; `.specstory/` commit etmeden önce PEM/IP kontrol et.

## Bu repodaki Skills

| Skill | Ne zaman |
|-------|----------|
| `deploy-ec2` | EC2 / GitHub Actions deploy |
| `new-microservice` | Yeni .NET servis iskeleti |
| `sprint-task` | Issue + Project Sprint |
| `local-infra-up` | Lokal Docker Compose |

Chat’te `/deploy-ec2` veya “EC2’ye deploy et” yazabilirsin.

## Öğrenme alıştırması

1. Yeni chat: “Stack ne?” → AGENTS.md / rules cevabı.
2. “EC2 deploy” → `deploy-ec2` skill.
3. `git add *.pem` dene → hook engellemeli.
4. Export transcript → `docs/decisions/`.
5. Repoyu silip clone et: rules/skills gelir; chat gelmez (normal).

## Settings Sync checklist (sen yap)

- [ ] Cursor’a giriş yap (`irfansahs`)
- [ ] Settings Sync açık
- [ ] İkinci PC’de aynı hesap + Sync açık
- [ ] Chat için Export veya SpecStory kullan

Detay: [docs/SETTINGS_SYNC.md](SETTINGS_SYNC.md)
