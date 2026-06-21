# Kernel Decision

Date: 2026-06-21

Question:

```text
Can AI Workbox be the user's current money/career kernel?
```

Short answer:

```text
Adopt as the current kernel candidate. Do not mark commercial validation complete yet.
```

## Original Requirements

The user wanted one core that:

- can become money
- can become career capital
- does not depend on the current job content
- has low legal burden
- is executable
- requires low ongoing effort
- has very low initial investment
- may use EC but is not constrained to EC
- is not trivially replaceable by generic AI output
- is not mainly "collecting evidence"

## Requirement Audit

| Requirement | Current Evidence | Status |
|---|---|---|
| One clear core | AI Workbox has a concrete thesis: Windows process lifecycle workbox for AI coding/dev-server commands. | Met |
| Can become money | Preview Pack, paid checklist, BOOTH/Ko-fi path, 1,480 JPY test, market test docs exist. | Plausible, unvalidated |
| Can become career capital | C#/.NET CLI, Windows Job Object, process tree inspection, Next.js doctor, security boundary docs, career story. | Met |
| Not tied to current job | Built from personal AI/dev workflow, not finance Excel/testing work. | Met |
| Low legal burden | Software/digital guide, MIT source, no regulated domain; sales checklist includes 特商法/tax checks. | Mostly met |
| Executable | `run/list/inspect/doctor nextjs/stop/prune` implemented and locally verified. | Met |
| Low ongoing effort | CLI + docs + zip; no SaaS, no accounts, no server, no telemetry. Support burden still untested. | Mostly met |
| Very low initial investment | Uses local Windows/.NET/GitHub/BOOTH or Ko-fi path; no cloud/API spend required. | Met |
| EC optional | Paid path is digital zip/guide, not physical EC. | Met |
| Not generic AI template | Contains Windows Job Object, Toolhelp process snapshot, TCP listener inspection, real Next.js doctor. | Met |
| Not evidence collection | The product is a usable local tool; GitHub/release docs are byproduct. | Met |

## Technical Evidence

Implemented:

- `workbox run`
- `workbox list`
- `workbox inspect`
- `workbox doctor nextjs`
- `workbox stop`
- `workbox prune`

Verified:

- `dotnet build`
- timeout exit code `124`
- stop from another process exit code `130`
- npm dev-server fixture HTTP response
- child Node process cleanup
- process tree display
- listening port display
- real Next.js dev-server detection without stopping it
- preview zip creation
- preview zip extraction and `workbox.exe --help`
- `public-check`
- `github-publish-preflight`

Repo state:

```text
<local-projects>\ai-workbox
```

Commits:

```text
9e16900 Initial AI Workbox proof
7bc122d Add paid preview validation docs
40f83a3 Add preview pack materials
432474b Add GitHub launch scaffolding
2ae0f90 Add GitHub publish runbook
```

Preview pack:

```text
artifacts/preview/AI-Workbox-Preview-Pack-0.1.0-preview.zip
```

## Commercial Evidence

Prepared:

- `docs/commercial-validation.md`
- `docs/market-test.md`
- `docs/paid-preview-checklist-jp.md`
- `docs/preview-guide-jp.md`
- `docs/feedback-form-jp.md`
- `docs/tester-outreach-jp.md`
- `docs/github-publish-runbook.md`
- `docs/release-notes-v0.1.0-preview.md`

Not yet proven:

- Any external user wants it
- Any external user can run it
- Any user would pay for the Japanese Preview Pack
- Any hiring/interview conversation responds strongly to it

## Legal And Support Boundary

Safe current framing:

```text
Windows process lifecycle workbox for AI coding and dev-server commands.
```

Unsafe framing:

```text
secure sandbox
malware containment
endpoint security
company PC safety
secret isolation
```

Selling must wait until:

- 特商法 display items are checked for the chosen platform
- tax tracking sheet exists
- refund/cancel wording is written
- support scope is explicitly limited

## Decision

AI Workbox should be treated as the current money/career kernel candidate.

It is strong enough to replace generic "cash experiment" wandering because:

- it is technically real
- it fits the user's actual pain
- it connects to DevOps/SRE/security/Windows internals
- it can be public OSS
- it can produce a paid digital Preview Pack without inventory
- it avoids SaaS support and cloud cost
- it has a clear tester loop

It is not yet proven as a money source.

## Next Gate

The next gate is not more implementation.

The next gate is external signal:

```text
3 relevant testers OR 1 real user run OR 1 paid-pack interest signal.
```

Relevant tester means at least two:

- Windows main machine
- Node/npm or Next.js
- Codex/Claude/Cursor/AI coding workflow
- has had stuck `node.exe`, occupied ports, or unclear dev server state

## Stop Or Pivot Conditions

Pivot away from commercialization if:

- people understand it only as a task manager
- people expect security sandboxing
- nobody can name when they would run it
- support questions become environment-specific and heavy
- the user does not actually use it weekly

If this happens, keep AI Workbox as career OSS/tooling, not as the cash core.

## Goal Completion Status

The goal is not complete yet.

Completed:

- one strong candidate core exists
- implementation exists
- career story exists
- low-cost digital commercial path exists
- legal/safety boundaries are documented
- preview pack exists

Not completed:

- market demand is not verified
- no tester has validated use case
- no paid interest or sale exists
- no public GitHub/release exists yet

Completion should require at least:

```text
AI Workbox published or privately tested, and external signal proves whether it should continue as money/career core.
```
