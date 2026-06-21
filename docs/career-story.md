# Career Story

Use this to explain AI Workbox in interviews or portfolio pages.

## One-Liner

AI Workbox is a Windows CLI that gives AI coding and dev-server processes a named lifecycle: run, inspect, diagnose, stop.

## Problem

AI coding tools make it easier to launch long-running commands, dev servers, and child processes. On Windows, this can leave developers with unclear process trees, occupied ports, and no obvious boundary around what the AI started.

## Design Choice

I deliberately did not claim this was a security sandbox.

The first useful boundary is process lifecycle:

- Windows Job Object for grouping
- process snapshot for tree inspection
- TCP listener mapping for port ownership
- Next.js project doctor for existing dev-server state
- explicit stop/prune commands

## Technical Points

- C# / .NET CLI
- Windows Job Object P/Invoke
- Toolhelp process snapshot
- TCP listener inspection through `netstat`
- PowerShell/CIM process command-line lookup for Next.js doctor
- docs that distinguish lifecycle control from security isolation

## What It Shows

- Windows internals awareness
- AI development workflow awareness
- DevOps/SRE-style lifecycle thinking
- Security boundary discipline
- Practical tooling instead of generic app building

## Honest Limits

- Not a VM/container replacement
- No file/network/secret isolation
- No malicious-code containment
- Needs more real-world testing before paid release

## Next Technical Step

Add optional profiles:

```text
workbox run --profile nextjs --name web
```

Profiles should remain local, explicit, and inspectable.
