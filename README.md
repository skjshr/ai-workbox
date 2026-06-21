# AI Workbox

AI Workbox is a small Windows CLI experiment for running AI-coding-related commands inside a named local workbox.

The v0 goal is narrow: start a command, group it with a Windows Job Object, list the box state, and stop the box as one unit.

It is not a security sandbox.

## Why

AI coding tools make it easy to leave behind dev servers, Node processes, child processes, and unclear runtime state. AI Workbox gives those tasks a local lifecycle:

```powershell
workbox run --name web -- npm run dev
workbox list
workbox inspect web
workbox stop web
```

## Safety Boundary

v0 does:

- group a root process and child processes with a Windows Job Object
- inspect the root process, child process tree, and listening TCP ports
- stop the workbox as one unit
- support timeout-based termination
- write small local state files under `%LOCALAPPDATA%\AiWorkbox\boxes`

v0 does not:

- isolate files
- isolate network access
- protect secrets or credentials
- prevent a process from reading files available to the user
- provide a security guarantee
- delete files
- modify the registry
- require admin rights

## Build

```powershell
dotnet build .\ai-workbox.sln
```

## Publish A Local Test Zip

```powershell
dotnet publish .\src\AiWorkbox.Cli\AiWorkbox.Cli.csproj -c Release -r win-x64 --self-contained false -o .\artifacts\win-x64
Compress-Archive -Path .\artifacts\win-x64\* -DestinationPath .\artifacts\ai-workbox-v0-win-x64.zip -Force
```

## Run From Source

```powershell
dotnet run --project .\src\AiWorkbox.Cli -- list
dotnet run --project .\src\AiWorkbox.Cli -- run --name smoke --timeout-seconds 5 -- pwsh -NoProfile -Command "Start-Sleep -Seconds 2; 'done'"
```

## Commands

```powershell
workbox run --name <name> [--timeout-seconds <seconds>] -- <command> [args...]
workbox list
workbox inspect <name>
workbox doctor nextjs [--path <project-dir>]
workbox stop <name>
workbox prune
```

Box names may contain letters, numbers, `-`, and `_`.

`prune` removes inactive Workbox state records only. It does not delete project files or stop processes.

Example `inspect` output from an npm dev server:

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

Example `doctor nextjs` output from an existing Next.js dev server:

```text
doctor: nextjs
project: C:\dev\example-next-app
lock: present (locked by another process)
log: present (...\.next\dev\logs\next-development.log)
node_processes:
- pid=12345 ports=3000,52263,52264,52273,52275
  exe=C:\Program Files\nodejs\node.exe
  command="C:\Program Files\nodejs\node.exe" ...\next\dist\server\lib\start-server.js
```

## Product Boundary

This is not a template pack or portfolio-evidence project. The useful object is the local workbox behavior. Public README, releases, and docs are secondary proof that the tool exists.

## Current Verification

See [docs/verification.md](docs/verification.md).

## Recipes

- [Next.js](docs/nextjs-recipe.md)
- [Preview Guide JP](docs/preview-guide-jp.md)
- [Feedback Form JP](docs/feedback-form-jp.md)

## Project Docs

- [Model](docs/model.md)
- [Safety boundaries](docs/safety-boundaries.md)
- [Security policy](SECURITY.md)
- [Verification](docs/verification.md)
- [Release checklist](docs/release-checklist.md)
- [Public launch plan](docs/public-launch-plan.md)
- [GitHub publish runbook](docs/github-publish-runbook.md)
- [v0.1.0 preview release notes](docs/release-notes-v0.1.0-preview.md)
- [Commercial validation](docs/commercial-validation.md)
- [Market test](docs/market-test.md)
- [Paid preview checklist JP](docs/paid-preview-checklist-jp.md)
- [Tester outreach JP](docs/tester-outreach-jp.md)
- [Career story](docs/career-story.md)
