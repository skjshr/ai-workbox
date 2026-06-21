# Public Launch Plan

AI Workbox should be published as a small technical tool, not as a startup landing page.

## Before GitHub Public

- `.\scripts\public-check.ps1` passes.
- `.\scripts\preflight-github-publish.ps1` passes.
- Working tree is clean.
- README says v0 is not a security sandbox.
- `SECURITY.md` is present.
- `LICENSE` is present.
- `docs/market-test.md` and `docs/feedback-form-jp.md` exist.
- No local usernames, private project paths, or company data in public docs.

## Repository Settings

Suggested repo:

```text
skjshr/ai-workbox
```

Suggested description:

```text
Windows process lifecycle workbox for AI coding and dev-server commands.
```

Suggested topics:

```text
windows
dotnet
developer-tools
ai-coding
nextjs
nodejs
process-management
job-object
```

## First Public README Positioning

Use:

```text
AI Workbox groups AI-coding and dev-server commands into named local workboxes so you can inspect process trees, see listening ports, and stop the whole workbox.
```

Avoid:

```text
secure sandbox
enterprise endpoint security
malware containment
AI safety product
```

## First Release

Do not publish a paid release first.

First GitHub release:

```text
v0.1.0-preview
```

Release title:

```text
AI Workbox v0.1.0 Preview
```

Release notes:

```text
Initial Windows preview.

- run/list/inspect/stop/prune
- doctor nextjs
- Windows Job Object process grouping
- npm dev-server fixture verification
- explicit non-sandbox security boundary
```

Attach only a preview zip after checking it does not include local verification notes.

## Tester Gate

After public or private preview, use:

- `docs/tester-outreach-jp.md`
- `docs/feedback-form-jp.md`
- GitHub issue template `Tester feedback`

Continue only if testers can say when they would run it.
