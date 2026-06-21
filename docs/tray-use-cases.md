# Tray Use Cases

AI Workbox Tray is the low-friction surface for people who do not want to keep a terminal open just to remember what AI/dev-server commands are still alive.

It is still not a security sandbox.

## Primary User

Windows developer using at least two:

- Codex, Claude Code, Cursor, or another AI coding workflow
- Node/npm or Next.js
- Playwright or other test runners that start a dev server
- repeated local AI-assisted changes where commands can outlive attention

## Use Case 1: "What Is Still Running?"

The user starts a dev server through the CLI:

```powershell
workbox run --name web -- cmd /c "npm run dev"
```

The tray icon stays present while the user continues working.

Expected tray behavior:

- Tooltip shows whether any workboxes are running.
- Context menu includes running boxes.
- Status window lists name, state, PID, start time, and command.
- Double-clicking or pressing Inspect shows process tree and listening ports.

Value:

```text
The user does not need to remember which terminal started the server.
```

## Use Case 2: "Stop Only This Work"

The user sees a running `web` workbox and chooses `Stop web` from the tray menu.

Expected tray behavior:

- Confirmation appears before stopping.
- Stop targets the named workbox only.
- Status refreshes after stop.
- Other unrelated `node.exe` processes are not targeted by name.

Value:

```text
Avoids broad taskkill-style cleanup.
```

## Use Case 3: "Clean Dead Records"

The user has old state records from commands that already exited.

Expected tray behavior:

- `Prune Inactive Records` removes inactive state records.
- Project files are not deleted.
- Running workboxes are not pruned.

Value:

```text
The visible list stays clean without pretending to be a PC cleaner.
```

## Use Case 4: "Run The CLI First, Tray Second"

The tray app does not replace the CLI.

Expected workflow:

1. CLI starts work.
2. Tray monitors and stops work.
3. CLI remains the precise automation/debug interface.

Value:

```text
The product remains operational and scriptable while adding everyday visibility.
```

## Non-Goals

- No background auto-kill.
- No automatic file cleanup.
- No malware containment.
- No secret isolation.
- No full task manager replacement.
- No scanning and killing unrelated processes.

## Product Decision

The tray direction is stronger than CLI-only if testers say:

- "I would leave this running."
- "I want a visible reminder of AI-started dev servers."
- "I want one click to inspect/stop a named workbox."

The tray direction is weak if testers say:

- "I only need a command."
- "I expected it to kill any random Node process."
- "I expected a secure sandbox."
