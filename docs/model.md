# Model

AI Workbox models a command as a local workbox.

```text
workbox
  name
  Windows Job Object name
  root process id
  command line
  started at
  finished at
  state
```

The CLI stores state in:

```text
%LOCALAPPDATA%\AiWorkbox\boxes\<name>.json
```

## v0 Lifecycle

1. `run` creates a named Windows Job Object.
2. `run` starts the requested root process.
3. `run` assigns the root process to the Job Object.
4. Child processes normally stay in the same Job Object.
5. `stop` opens the named Job Object from the state file.
6. `stop` terminates the Job Object.
7. `prune` removes inactive local state records.

## Inspection Model

`inspect` combines:

- Workbox state JSON
- Windows process snapshot from Toolhelp APIs
- TCP listener data from `netstat -ano -p tcp`

It starts from the recorded root process id, walks child processes by parent pid, and annotates processes that own listening TCP ports.

## Doctor Model

`doctor nextjs` is a read-only project preflight.

It checks:

- `.next/dev/lock`
- `.next/dev/logs/next-development.log`
- `node.exe` processes whose command line contains the project path
- listening TCP ports owned by those processes

The command does not stop or modify any process.

## Known Limits

- If a child process deliberately breaks away from the Job Object, v0 may not stop it.
- If the `run` CLI is no longer alive, the Job Object may already be closed.
- `list` is based on local state files and root process liveness, not full kernel enumeration.
- `prune` only deletes Workbox state JSON files.
- `inspect` uses parent pid traversal, so it may miss processes that re-parent or deliberately detach.
- `doctor nextjs` depends on Windows process command-line access through PowerShell/CIM.
