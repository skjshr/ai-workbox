$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $MyInvocation.MyCommand.Path
$workbox = Join-Path $root "bin\workbox.exe"

if (-not (Test-Path -Path $workbox)) {
    throw "bin\workbox.exe was not found. Run this script from the extracted AI Workbox Preview Pack folder."
}

& $workbox --help
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

& $workbox run --name smoke --timeout-seconds 5 -- powershell.exe -NoProfile -Command "Start-Sleep -Seconds 2; 'done'"
if ($LASTEXITCODE -ne 0) {
    exit $LASTEXITCODE
}

& $workbox list
& $workbox prune
