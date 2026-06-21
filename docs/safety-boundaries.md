# Safety Boundaries

AI Workbox v0 is a process lifecycle tool, not a sandbox.

## Safe Claims

- It can launch a command.
- It can place the launched process in a Windows Job Object.
- It can stop the Job Object.
- It can stop child processes that remain in that Job Object.
- It can apply a wall-clock timeout.

## Unsafe Claims

Do not claim v0:

- securely isolates AI agents
- protects secrets
- blocks network access
- blocks file access
- prevents data exfiltration
- replaces VM/container sandboxing
- is safe for company-managed endpoints
- is an enterprise security control

## Destructive Operations

v0 intentionally avoids file cleanup, registry edits, service control, and automatic process killing outside a named workbox.

Future cleanup features must start as dry-run only.
