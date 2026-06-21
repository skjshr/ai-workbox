param(
    [string]$Version = "0.1.0-local",
    [switch]$FrameworkDependent
)

$ErrorActionPreference = "Stop"
$root = Resolve-Path (Join-Path $PSScriptRoot "..")
$packageRoot = Join-Path $root "artifacts\preview\AI-Workbox-Preview-Pack-$Version"
$zipPath = Join-Path $root "artifacts\preview\AI-Workbox-Preview-Pack-$Version.zip"

Push-Location $root
try {
    $selfContained = -not $FrameworkDependent
    dotnet publish ".\src\AiWorkbox.Cli\AiWorkbox.Cli.csproj" -c Release -r win-x64 --self-contained:$selfContained -o ".\artifacts\win-x64"
    if ($LASTEXITCODE -ne 0) {
        exit $LASTEXITCODE
    }

    if (Test-Path -Path $packageRoot) {
        Remove-Item -Path $packageRoot -Recurse -Force
    }

    New-Item -ItemType Directory -Force -Path $packageRoot | Out-Null
    New-Item -ItemType Directory -Force -Path (Join-Path $packageRoot "bin") | Out-Null
    New-Item -ItemType Directory -Force -Path (Join-Path $packageRoot "docs") | Out-Null

    Copy-Item -Path ".\artifacts\win-x64\*" -Destination (Join-Path $packageRoot "bin") -Recurse -Force
    Copy-Item -Path ".\README.md" -Destination $packageRoot -Force
    Copy-Item -Path ".\LICENSE" -Destination $packageRoot -Force
    Copy-Item -Path ".\SECURITY.md" -Destination $packageRoot -Force
    Copy-Item -Path ".\scripts\preview-smoke.ps1" -Destination (Join-Path $packageRoot "try-smoke.ps1") -Force
    Copy-Item -Path ".\docs\preview-guide-jp.md" -Destination (Join-Path $packageRoot "docs") -Force
    Copy-Item -Path ".\docs\feedback-form-jp.md" -Destination (Join-Path $packageRoot "docs") -Force
    Copy-Item -Path ".\docs\nextjs-recipe.md" -Destination (Join-Path $packageRoot "docs") -Force
    Copy-Item -Path ".\docs\safety-boundaries.md" -Destination (Join-Path $packageRoot "docs") -Force

    if (Test-Path -Path $zipPath) {
        Remove-Item -Path $zipPath -Force
    }

    Compress-Archive -Path "$packageRoot\*" -DestinationPath $zipPath -Force
    Write-Host "preview-pack: $zipPath"
}
finally {
    Pop-Location
}
