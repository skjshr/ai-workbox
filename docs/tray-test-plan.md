# Tray Test Plan

This plan verifies that AI Workbox Tray improves usability without weakening the CLI safety boundary.

## Build

```powershell
dotnet build .\ai-workbox.sln
```

Pass:

- `AiWorkbox.Cli` builds.
- `AiWorkbox.Tray` builds.

## Package

```powershell
.\scripts\package-preview.ps1 -Version "tray-test"
```

Pass:

- `artifacts\preview\AI-Workbox-Preview-Pack-tray-test.zip` exists.
- Extracted `bin` contains `workbox.exe`.
- Extracted `bin` contains `workbox-tray.exe`.
- Extracted root contains `try-smoke.ps1`.

## Tray Launch Smoke

From extracted pack:

```powershell
Start-Process .\bin\workbox-tray.exe
```

Pass:

- Process starts without console output.
- Tray icon appears.
- Context menu opens.
- Exit removes the process.

## Empty State

Precondition:

```powershell
.\bin\workbox.exe prune
```

Action:

- Open tray status.

Pass:

- Status window shows no workboxes.
- Tooltip says no running boxes.
- No stop action is shown for non-existent boxes.

## Running Workbox

Action:

```powershell
.\bin\workbox.exe run --name tray_probe --timeout-seconds 30 -- powershell.exe -NoProfile -Command "Start-Sleep -Seconds 20"
```

While it runs:

- Open tray status.
- Refresh.
- Inspect `tray_probe`.

Pass:

- `tray_probe` appears as running.
- PID is shown.
- Inspect shows process tree.
- Stop action is available.

## Stop From Tray

Action:

- Choose `Stop tray_probe`.
- Confirm stop.

Pass:

- Workbox stops.
- Status refreshes.
- `workbox list` no longer reports it as running.
- Stop confirmation appears before termination.

## Prune From Tray

Action:

- Click `Prune`.

Pass:

- Inactive records are removed.
- Running records remain.
- No project files are deleted.

## Failure Case

Action:

- Launch `workbox-tray.exe` without `workbox.exe` next to it in package mode.

Pass:

- Status window reports CLI unavailable.
- The app does not crash.

## Regression Boundary

Tray must not:

- claim security sandboxing
- auto-kill processes without confirmation
- kill unrelated `node.exe` processes
- delete project files
- require admin rights
