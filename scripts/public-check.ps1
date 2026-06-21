param(
    [switch]$SkipBuild
)

$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "..")
Push-Location $root
try {
    $publicFiles = @(
        "README.md",
        "CHANGELOG.md",
        "PROJECT_CONSTITUTION.md",
        "SECURITY.md"
    ) + (Get-ChildItem -Path "docs" -Filter "*.md" | ForEach-Object { $_.FullName })

    $forbidden = @(
        "C:\\Users\\phrx4",
        "phrx4",
        "ExamServer",
        "examserver",
        "main-production"
    )

    foreach ($pattern in $forbidden) {
        $hits = Select-String -Path $publicFiles -Pattern $pattern -SimpleMatch -ErrorAction SilentlyContinue
        if ($hits) {
            $hits | ForEach-Object {
                Write-Error "Forbidden public text '$pattern' in $($_.Path):$($_.LineNumber)"
            }
        }
    }

    $links = Select-String -Path "README.md" -Pattern "\]\(([^)]+)\)" -AllMatches |
        ForEach-Object { $_.Matches } |
        ForEach-Object { $_.Groups[1].Value } |
        Where-Object { $_ -notmatch "^(https?:|mailto:)" }

    foreach ($link in $links) {
        $path = Join-Path $root $link
        if (-not (Test-Path -Path $path)) {
            Write-Error "Missing README link target: $link"
        }
    }

    if (-not $SkipBuild) {
        dotnet build ".\ai-workbox.sln"
        if ($LASTEXITCODE -ne 0) {
            exit $LASTEXITCODE
        }
    }

    Write-Host "public-check: ok"
}
finally {
    Pop-Location
}
