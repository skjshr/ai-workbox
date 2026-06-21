# Release Checklist

Use this before publishing AI Workbox or packaging a zip for testers.

## Code

- `dotnet build .\ai-workbox.sln` passes.
- `workbox --help` includes all commands.
- `workbox run` natural exit works.
- `workbox run --timeout-seconds` returns `124` on timeout.
- `workbox stop <name>` stops a live box and returns the launcher with `130`.
- `workbox inspect <name>` shows npm/node process tree and ports.
- `workbox doctor nextjs --path <project>` detects existing Next.js dev server state without stopping it.
- `workbox prune` removes inactive state only.

## Safety

- README says v0 is not a security sandbox.
- SECURITY.md says v0 does not isolate files, network, or secrets.
- No command deletes project files.
- No command stops processes outside a named workbox.
- No telemetry, network upload, analytics, or external service calls.
- No company/private project names in public docs, except local verification notes kept out of public release if needed.

## Package

- `artifacts/ai-workbox-v0-win-x64.zip` is rebuilt from the latest Release output.
- Zip contains `workbox.exe`, `.dll`, `.deps.json`, `.runtimeconfig.json`.
- Zip is marked as local/test if not publicly released.
- `.\scripts\package-preview.ps1` creates a tester Preview Pack zip when needed.

## Public Repo Check

- `.\scripts\public-check.ps1` passes.
- Public docs do not include local usernames or private project paths.
- Local verification notes are kept outside the public repo when necessary.

## Public Copy

- Describe it as "Windows process lifecycle workbox for AI/dev-server commands".
- Do not use "secure sandbox" or "endpoint security".
- Mention Next.js/npm dev-server use cases.
- Mention limitations before paid/support claims.
