# Verification

Date: 2026-06-21

Environment:

- Windows 11
- .NET SDK 10.0.100

## Build

Command:

```powershell
dotnet build .\ai-workbox.sln
```

Result:

```text
Build succeeded.
0 warnings
0 errors
```

## Natural Exit

Command:

```powershell
dotnet run --project .\src\AiWorkbox.Cli -- run --name smoke --timeout-seconds 5 -- pwsh -NoProfile -Command "Start-Sleep -Seconds 1; 'done'"
```

Result:

```text
started box 'smoke'
done
```

Exit code: `0`

## Timeout

Command:

```powershell
dotnet run --project .\src\AiWorkbox.Cli -- run --name timeout_exit --timeout-seconds 1 -- pwsh -NoProfile -Command "Start-Sleep -Seconds 20"
```

Result:

```text
timeout reached; stopping box 'timeout_exit'
```

Exit code: `124`

## Stop From Another Process

Setup:

```powershell
Start-Process dotnet ... workbox run --name manual_stop -- pwsh -NoProfile -Command "Start-Sleep -Seconds 60"
```

Stop:

```powershell
dotnet run --project .\src\AiWorkbox.Cli -- stop manual_stop
```

Result:

```text
manual_stop running ...
stopped box 'manual_stop'
launcher_exited=True exit=130
```

## npm Dev Server Fixture

Fixture:

```text
fixtures/npm-dev-server
```

Command shape:

```powershell
workbox run --name npm_fixture_probe -- cmd /c "npm run dev"
```

Result:

```text
npm_fixture_probe running ...
status=200 content=workbox fixture pid=107548 child=104128 url=/workbox
server_alive_before=True child_alive_before=True
stopped box 'npm_fixture_probe'
launcher_exited=True exit=130
server_alive_after=False child_alive_after=False
port_clear
```

This proves v0 can run an npm dev command, keep a child Node process inside the workbox, and stop the server plus child process as one unit.

## Inspect

Command:

```powershell
workbox inspect inspect_probe
```

Result:

```text
box: inspect_probe
state: running
root_pid: 108288
command: cmd /c npm run dev
processes:
- pid=108288 ppid=106676 name=cmd.exe
  - pid=108316 ppid=108288 name=node.exe
    - pid=64740 ppid=108316 name=cmd.exe
      - pid=106144 ppid=64740 name=node.exe ports=43124
        - pid=103612 ppid=106144 name=node.exe
```

After `workbox stop inspect_probe`:

```text
processes: none alive
```

This proves v0 can show a real npm process tree and associate a listening TCP port with the owning process.

## Real Next.js Project Probe

Target:

```text
C:\dev\example-next-app
```

Command shape:

```powershell
workbox run --name nextjs_probe --timeout-seconds 20 -- cmd /c "npm run dev -- -p 43123"
```

Result:

```text
Next.js started, then exited because another next dev server was already running.
Existing PID: 12345
Existing URL: http://localhost:3000
Exit code: 1
```

The existing process was not stopped because it may belong to active user work. This is a product signal: Workbox should eventually add a Next.js recipe/preflight for "project already has a dev server".

## Next.js Doctor

Command:

```powershell
workbox doctor nextjs --path C:\dev\example-next-app
```

Result:

```text
doctor: nextjs
project: C:\dev\example-next-app
dev_dir: C:\dev\example-next-app\.next\dev
lock: present (locked by another process)
log: present (C:\dev\example-next-app\.next\dev\logs\next-development.log)
node_processes:
- pid=12000
  command="node" "...next\dist\bin\next" dev --hostname 0.0.0.0
- pid=12345 ports=3000,52263,52264,52273,52275
  command="C:\Program Files\nodejs\node.exe" ...\next\dist\server\lib\start-server.js
- pid=12346
  command="node" ...\.next\dev\build\cf208e1b0e4c4caf.js 52273
- pid=12347
  command="node" ...\.next\dev\build\cf208e1b0e4c4caf.js 52275
- pid=12348
  command="node" ...\.next\dev\build\56416d4ae4ce586f.js 52264
```

This proves v0 can detect a real existing Next.js dev server without stopping it.

## Prune

Command:

```powershell
workbox prune
workbox list
```

Result:

```text
pruned 7 inactive box record(s)
no boxes
```

`prune` removes inactive Workbox state records only.

## Artifact

Created:

```text
artifacts/ai-workbox-v0-win-x64.zip
```

This is a framework-dependent Windows x64 build for local testing, not a public release.
