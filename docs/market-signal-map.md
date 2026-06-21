# Market Signal Map

AI Workbox is not commercially validated yet.

This file is only a map for finding relevant testers. It is not proof that the product should be sold.

## Observed Pain Shape

The promising pain is not "people want a sandbox."

The promising pain is:

```text
local AI/dev commands start processes -> Node/Next.js/dev servers keep running -> ports/process trees become unclear -> broad kill commands are risky
```

AI Workbox should test that narrow pain.

## External Anchors

These are public signals that the problem space exists:

- Next.js source contains Windows-specific port lookup using `netstat -ano` for listening processes: https://github.com/vercel/next.js/blob/canary/packages/next/src/server/lib/start-server.ts
- Next.js users discuss dev servers getting stuck and killing Node runtime processes to reset port state: https://github.com/vercel/next.js/discussions/60143
- Claude Code issue reports broad Node process termination killing Claude Code and VS Code together: https://github.com/anthropics/claude-code/issues/3068
- Claude Code support documents Windows/WSL/Node runtime friction and recommends bundled/native runtime paths in some cases: https://support.claude.com/en/articles/14552646-troubleshoot-claude-code-installation-and-authentication
- Windows users search for hidden Node/port conflicts around local dev servers: https://stackoverflow.com/questions/39322089/node-js-port-3000-already-in-use-but-it-actually-isnt

Use these only to frame tester questions.

Do not treat them as demand proof.

## Tester Search Criteria

Prioritize people or public threads where at least two are visible:

- Windows development
- Node/npm or Next.js
- AI coding tool such as Codex, Claude Code, Cursor, or similar
- local dev server/port/process confusion
- avoidance of heavier Docker/VM workflows for small commands

Do not target:

- Linux-only or Mac-only workflows
- security sandbox buyers
- enterprise endpoint/security users
- people asking only for deployment hosting help
- people whose issue is clearly app code, package install, or browser cache only

## Outreach Routes

### Route A: Known Technical Contacts

Best first route.

Send the short DM from `docs/validation-next-actions-jp.md`.

Record only anonymized results in `docs/tester-validation-log.md`.

### Route B: Public GitHub Issue

Use:

```text
https://github.com/skjshr/ai-workbox/issues/1
```

This is the canonical public place for feedback.

### Route C: Relevant Public Threads

Only reply when the thread is active and the reply would be useful.

Do not spam old threads. Do not pretend AI Workbox is the fix for someone's specific bug.

Acceptable reply shape:

```text
I am testing a small Windows CLI for this narrow problem: running npm/Next.js commands in a named process group, then inspecting child processes and ports before stopping the group.

It is not a security sandbox and it will not fix app bugs, but it may help with orphaned dev servers or unclear node.exe/port state.

If this matches your pain, I am looking for feedback here:
https://github.com/skjshr/ai-workbox/issues/1
```

## What Counts As Signal

Good signal:

- "I had this exact stuck node/port problem."
- "I can name when I would run it."
- "I downloaded the Preview Pack."
- "I ran `try-smoke.ps1`."
- "I ran `doctor nextjs`."
- "I would pay for a Japanese troubleshooting/setup pack."

Weak signal:

- "Interesting."
- "Looks cool."
- "Make a GUI."
- "This is like Docker."

Bad signal:

- Expects malware containment.
- Expects secrets/file/network isolation.
- Expects company endpoint-policy bypass.
- Cannot name a concrete use case.

## Stop Rule

Stop outreach after:

```text
3 relevant tester responses
```

Then update `docs/tester-validation-log.md` and decide:

- Continue commercial validation
- Keep as career OSS only
- Pivot away from AI Workbox as money core
