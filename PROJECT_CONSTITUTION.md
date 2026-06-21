# AI Workbox Constitution

## Thesis

AI Workbox is a Windows-first local workbox for AI coding and dev-server processes.

The value is not a template, README, or portfolio artifact. The value is the practical ability to start work inside a named box, observe it, and stop it as one unit when it runs away.

## Core Mechanism

The core mechanism is a lifecycle boundary around local developer processes:

```text
run command -> assign to Windows Job Object -> record local state -> list state -> stop job
```

If this mechanism is weak, the product is weak. Extra docs, UI, or sales pages do not compensate.

The second mechanism is observability before termination:

```text
inspect box -> show process tree -> annotate listening ports -> decide whether to stop
```

## Non-Goals

- Do not claim to be a secure sandbox.
- Do not promise secret protection, network isolation, or file isolation.
- Do not add SaaS, accounts, telemetry, or cloud sync in v0.
- Do not add automatic file cleanup until dry-run behavior is proven.
- Do not become a generic PC cleaner.
- Do not make evidence collection the product.

## Design Constraints

- Windows-first.
- No admin rights required for v0.
- Destructive actions must be explicit and scoped to a named workbox.
- Local state should be inspectable.
- Failure modes must be documented instead of hidden.
- The CLI should remain usable before any GUI exists.

## Product Grammar

Use plain operational words:

- workbox
- run
- list
- inspect
- stop
- timeout
- process tree
- port
- boundary
- state

Avoid inflated security language:

- secure sandbox
- isolation guarantee
- enterprise protection
- endpoint security

## Verification Gates

v0 is credible only when these pass on Windows:

- `dotnet build`
- `workbox run` starts a command
- timeout stops a long-running command and returns exit code 124
- another process can call `workbox stop <name>` and stop the running box
- `cmd /c npm run dev` can be run and stopped with a child Node process
- `workbox inspect <name>` shows npm/node process tree and listening port
- `workbox doctor nextjs --path <project>` detects an existing Next.js dev server without stopping it
- `workbox prune` removes inactive state records only
- docs clearly state that v0 is not a security sandbox

## Commercial Direction

Free:

- CLI source
- basic docs
- core workbox behavior

Paid later:

- packaged zip
- recipes for Codex/Claude/Next.js
- safer defaults
- richer profiles
- logs and troubleshooting guide

Do not sell until the free version is useful on the user's own machine.
