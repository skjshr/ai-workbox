# Tester Validation Log

Use anonymized tester IDs only.

Do not put real names, handles, email addresses, private project names, workplace details, screenshots, secrets, or full local paths in this file.

## Goal

Decide whether AI Workbox should continue as a money/career kernel.

The next gate is:

```text
3 relevant testers OR 1 real user run OR 1 paid-pack interest signal.
```

## Tester Qualification

A relevant tester matches at least two:

- Windows main machine
- Node/npm or Next.js
- Codex/Claude/Cursor/AI coding workflow
- has had stuck `node.exe`, occupied ports, or unclear dev server state
- does not want Docker/VM overhead for every small project

## Tester Table

| ID | Date sent | Qualified? | Material sent | Understood use case? | Ran tool? | Paid-pack signal? | Main quote/paraphrase | Decision |
|---|---|---:|---|---:|---:|---:|---|---|
| T1 |  |  | README / Preview Pack / Both |  |  |  |  |  |
| T2 |  |  | README / Preview Pack / Both |  |  |  |  |  |
| T3 |  |  | README / Preview Pack / Both |  |  |  |  |  |

## Signal Rules

Good:

- Can describe when they would run it.
- Has experienced stuck Node/dev-server/port confusion.
- Asks for a packaged zip or setup guide.
- Runs `workbox --help`, `inspect`, or `doctor nextjs`.
- Says a Japanese troubleshooting guide would be useful.

Weak:

- Says it is "cool" but cannot name a use case.
- Only asks for a GUI.
- Compares it to Docker without understanding the lighter lifecycle boundary.

Bad:

- Expects malware/security containment.
- Expects company endpoint policy bypass.
- Only sees a generic task manager.
- Cannot imagine running it.

## Decision After 3 Testers

Continue commercial validation if one is true:

- 2/3 understand the use case.
- 1 tester runs it.
- 1 tester gives a concrete paid-pack signal.

Otherwise:

- Keep AI Workbox as career OSS/tooling.
- Do not build a paid package yet.

## Private Notes Policy

If private tester details are needed, keep them outside the public repo.

Suggested local-only path:

```text
work/ai-workbox/private-tester-notes.md
```
