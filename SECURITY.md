# Security Policy

AI Workbox v0 is not a security sandbox.

## Supported Boundary

The current supported boundary is process lifecycle control:

- start a command
- place the root process in a Windows Job Object
- observe local process tree and listening ports
- stop the named workbox

## Unsupported Boundary

Do not rely on AI Workbox v0 for:

- secret isolation
- file-system isolation
- network isolation
- malicious-code containment
- enterprise endpoint protection
- company-managed device policy enforcement

## Reporting Issues

For now, report issues through the project repository once it is published.

Do not include secrets, credentials, private source code, `.env` files, browser profile data, or company-owned information in bug reports.

## Safe Testing

Use throwaway commands or local fixtures first:

```powershell
workbox run --name smoke --timeout-seconds 5 -- pwsh -NoProfile -Command "Start-Sleep -Seconds 2; 'done'"
```

Before running third-party tools inside Workbox, assume they can still read files and use the network as the current user.
