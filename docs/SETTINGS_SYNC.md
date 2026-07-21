# Cursor Settings Sync (chat değil)

Settings Sync yalnızca **editör tercihlerini** (settings, keybindings, extensions) hesabınla taşır. **Sohbet geçmişi senkronlanmaz.**

## Açmak (Windows)

1. Cursor’da sol alttan veya **File → Preferences → Settings Sync** (veya Command Palette: `Settings Sync: Turn On`).
2. GitHub / Cursor hesabınla oturum aç.
3. Senkronlanacakları seç:
   - **Settings** (önerilir)
   - **Keybindings** (önerilir)
   - **Extensions** (isteğe bağlı)
4. İkinci bilgisayarda aynı hesap + **Turn On Sync**.

## Bu makinede önerilen ayarlar

`%APPDATA%\Cursor\User\settings.json` içinde (zaten var olanlar korunur):

- `files.autoSave`: `afterDelay`
- `editor.formatOnSave`: `true`
- `files.watcherExclude` / `files.exclude`: `**/node_modules`, `**/bin`, `**/obj`

## Chat yedeği

| Yöntem | Açıklama |
|--------|----------|
| Export Transcript | Chat → export → `docs/decisions/` |
| SpecStory | `.specstory/` klasörü (secret kontrolü şart) |
| GitHub | Sadece rules/skills/AGENTS.md — konuşma değil |

## Chat dosyaları (bilgi)

```
%APPDATA%\Cursor\User\workspaceStorage\   # sidebar / workspace metadata
%USERPROFILE%\.cursor\projects\          # agent transcripts
```

Proje yolu değişirse (farklı klasör adı) eski chat o workspace’te kalır.
