# Next.js Recipe

AI Workbox can run `npm run dev`, but Next.js has its own project-level dev-server guard.

If a Next.js dev server is already running for the same project, a second `next dev` may exit even if you choose a different port.

Observed example:

```text
Another next dev server is already running.
Local: http://localhost:3000
PID: 12345
Dir: C:\dev\example-next-app
```

## Recommended Use

Start the dev server through Workbox from the beginning:

```powershell
workbox run --name web -- cmd /c "npm run dev"
```

Then stop it through Workbox:

```powershell
workbox stop web
```

## If Next.js Is Already Running

Do not blindly kill the PID. It may belong to active work.

First inspect the project:

```powershell
workbox doctor nextjs --path C:\dev\example-next-app
```

Example:

```text
doctor: nextjs
project: C:\dev\example-next-app
lock: present (locked by another process)
log: present (C:\dev\example-next-app\.next\dev\logs\next-development.log)
node_processes:
- pid=12345 ports=3000,52263,52264,52273,52275
  command="C:\Program Files\nodejs\node.exe" ...\next\dist\server\lib\start-server.js
```

Check:

```powershell
Get-Process -Id <pid>
Get-NetTCPConnection -OwningProcess <pid> -ErrorAction SilentlyContinue
```

Then decide whether to stop it manually or keep using the existing server.

## Future Workbox Preflight

The current `doctor nextjs` command detects common Next.js dev-server lock/log files and matching `node.exe` processes.

A future version should run this as an optional preflight before launch:

```text
This project already appears to have a Next.js dev server.
Use the existing server, stop it manually, or rerun from a clean state.
```
