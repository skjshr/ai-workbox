param(
    [string]$ExpectedRepo = "skjshr/ai-workbox"
)

$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "..")
Push-Location $root
try {
    $status = git status --short
    if ($status) {
        Write-Error "Working tree is not clean. Commit or discard local changes before publishing."
    }

    $remote = git remote get-url origin 2>$null
    if ($LASTEXITCODE -eq 0 -and $remote) {
        if ($remote -notmatch [regex]::Escape($ExpectedRepo)) {
            Write-Error "origin remote does not look like $ExpectedRepo`: $remote"
        }
    }

    .\scripts\public-check.ps1

    $previewZip = ".\artifacts\preview\AI-Workbox-Preview-Pack-0.1.0-preview.zip"
    if (-not (Test-Path -Path $previewZip)) {
        Write-Error "Missing preview zip: $previewZip. Run .\scripts\package-preview.ps1 -Version '0.1.0-preview'."
    }

    gh auth status 1>$null
    if ($LASTEXITCODE -ne 0) {
        Write-Error "GitHub CLI is not authenticated."
    }

    if (-not (Test-Path -Path ".\docs\release-notes-v0.1.0-preview.md")) {
        Write-Error "Missing release notes."
    }

    if (-not (Test-Path -Path ".\.github\workflows\ci.yml")) {
        Write-Error "Missing GitHub Actions workflow."
    }

    Write-Host "github-publish-preflight: ok"
}
finally {
    Pop-Location
}
