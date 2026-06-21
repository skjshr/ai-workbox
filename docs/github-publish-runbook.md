# GitHub Publish Runbook

This runbook publishes AI Workbox only after local checks pass.

Do not publish from a dirty working tree.

## Current Local Truth

Project root:

```text
<local-projects>\ai-workbox
```

Suggested GitHub repo:

```text
skjshr/ai-workbox
```

Suggested visibility:

```text
public
```

Reason:

- The core value is a public developer tool.
- Public source is the career asset.
- Paid value, if any, should be the Japanese Preview Pack, recipes, and packaged support materials.

## Preflight

Run:

```powershell
.\scripts\preflight-github-publish.ps1
```

Required:

- working tree clean
- no remote already configured, or the remote matches the intended repo
- `public-check` passes
- preview zip exists
- `gh auth status` works
- public docs contain no private local paths

## Create Repo

Only run this when intentionally publishing:

```powershell
gh repo create skjshr/ai-workbox --public --source . --remote origin --description "Windows process lifecycle workbox for AI coding and dev-server commands."
```

Then push:

```powershell
git branch -M main
git push -u origin main
```

## Add Topics

```powershell
gh repo edit skjshr/ai-workbox --add-topic windows --add-topic dotnet --add-topic developer-tools --add-topic ai-coding --add-topic nextjs --add-topic nodejs --add-topic process-management --add-topic job-object
```

## Create First Release

Rebuild the preview pack:

```powershell
.\scripts\package-preview.ps1 -Version "0.1.0-preview"
```

Create release:

```powershell
gh release create v0.1.0-preview `
  .\artifacts\preview\AI-Workbox-Preview-Pack-0.1.0-preview.zip `
  --title "AI Workbox v0.1.0 Preview" `
  --notes-file .\docs\release-notes-v0.1.0-preview.md
```

## After Publish

Do not post broad marketing immediately.

First ask 3 relevant testers:

- Windows main machine
- Node/npm or Next.js
- Codex/Claude/Cursor/AI coding workflow
- Has had stuck `node.exe`, occupied ports, or unclear dev server state

Use:

- `docs/tester-outreach-jp.md`
- `docs/feedback-form-jp.md`

## Stop Conditions

Do not publish if:

- `public-check` fails
- working tree is dirty
- README implies security sandboxing
- Preview zip includes source/private verification notes by accident
- You are publishing because you feel anxious, not because the tester loop is ready
