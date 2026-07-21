#!/usr/bin/env bash
# Block git commands that stage or reference secret files (.env, *.pem).
set -euo pipefail

input=$(cat)
command=""
if command -v jq >/dev/null 2>&1; then
  command=$(echo "$input" | jq -r '.command // empty')
else
  command=$(echo "$input" | sed -n 's/.*"command"[[:space:]]*:[[:space:]]*"\([^"]*\)".*/\1/p' | head -1)
fi

if [[ -z "$command" ]]; then
  echo '{"permission":"allow"}'
  exit 0
fi

lower=$(echo "$command" | tr '[:upper:]' '[:lower:]')

if echo "$lower" | grep -qE '\.(pem|env)([^a-z]|$)|bot-anahtar|\.env\.local|secrets/'; then
  if echo "$lower" | grep -qE 'git add|git commit|git push|git restore --staged'; then
    cat <<'EOF'
{"permission":"deny","user_message":"Blocked: do not git add/commit .env or *.pem. They are gitignored; use GitHub Secrets for deploy keys.","agent_message":"block-secrets hook denied git command referencing secret paths."}
EOF
    exit 0
  fi
fi

echo '{"permission":"allow"}'
exit 0
