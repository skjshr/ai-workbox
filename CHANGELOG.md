# Changelog

## 0.1.0-local

Date: 2026-06-21

Initial local proof.

- Added `run` to start a named workbox with Windows Job Object grouping.
- Added `list` to show local workbox records.
- Added `inspect` to show process tree and listening TCP ports.
- Added `doctor nextjs` to detect existing Next.js dev-server state.
- Added `stop` to terminate a named workbox.
- Added `prune` to remove inactive Workbox state records.
- Verified timeout exit code `124`.
- Verified manual stop exit code `130`.
- Verified npm dev-server fixture process-tree cleanup.
- Verified real Next.js process detection without stopping it.

This is not a public release yet.
