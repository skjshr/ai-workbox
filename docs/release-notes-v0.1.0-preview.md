# AI Workbox v0.1.0 Preview

Initial Windows preview for local AI/dev-server process lifecycle control.

## What It Does

- `run`: start a command in a named workbox
- `list`: show local workbox records
- `inspect`: show the root process, child process tree, and listening TCP ports
- `doctor nextjs`: detect existing Next.js dev-server state without stopping it
- `stop`: terminate a named workbox
- `prune`: remove inactive Workbox state records

## Verified Locally

- Natural command exit
- Timeout termination with exit code `124`
- Stop from another process with launcher exit code `130`
- npm dev-server fixture with child Node process cleanup
- Next.js doctor detection of lock/log/process/port state

## Security Boundary

AI Workbox v0 is not a security sandbox.

It does not:

- isolate files
- isolate network access
- protect secrets
- contain malicious code
- replace VMs, containers, or endpoint security tools

It is a Windows process lifecycle tool.

## Preview Pack

The attached zip includes:

- `bin/workbox.exe`
- Japanese preview guide
- feedback form
- Next.js recipe
- safety boundary notes

## Tester Request

If you try this, please answer:

- When would you run it?
- Was `inspect` useful?
- Did `doctor nextjs` explain an existing dev server?
- Did anything imply security guarantees that v0 does not provide?
- Would a Japanese setup/troubleshooting pack be worth paying for?
