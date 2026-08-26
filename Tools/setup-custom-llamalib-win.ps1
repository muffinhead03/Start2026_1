<#
.SYNOPSIS
    Swaps in a Windows-fixed build of LlamaLib for the Gemma 4 hint system.

.DESCRIPTION
    LLMUnity auto-downloads the OFFICIAL LlamaLib v2.0.5 release from
    github.com/undreamai/LlamaLib on first project open. That official
    Windows build has known crash bugs with Gemma 4 (see
    Tools/WINDOWS_GEMMA4_SETUP.md for details). This script overwrites
    just the Windows native DLLs with a custom build that fixes them.

    It does NOT touch macOS/Linux/Android/iOS binaries, and it does NOT
    change LLMUnity or LlamaLib as packages - it only swaps 4 files
    inside Assets/StreamingAssets/LlamaLib-v2.0.5/win-x64/native/.

.NOTES
    Safe to re-run any time (e.g. after LLMUnity re-downloads the
    official binaries following a Library wipe or package update).
#>

$ErrorActionPreference = "Stop"

$repoRoot   = Split-Path -Parent $PSScriptRoot
$sourceDir  = Join-Path $PSScriptRoot "LlamaLib-win-custom-build\win-x64"
$targetDir  = Join-Path $repoRoot "Assets\StreamingAssets\LlamaLib-v2.0.5\win-x64\native"

$files = @(
    "llamalib_win-x64_avx2.dll",
    "llamalib_win-x64_avx.dll",
    "llamalib_win-x64_noavx.dll",
    "llamalib_win-x64_runtime.dll"
)

Write-Host "=== LlamaLib Windows custom-build setup ===" -ForegroundColor Cyan

if (-not (Test-Path $targetDir)) {
    Write-Host ""
    Write-Host "ERROR: $targetDir does not exist yet." -ForegroundColor Red
    Write-Host "This folder is created by LLMUnity the first time it resolves" -ForegroundColor Red
    Write-Host "packages / downloads models. Open the project in Unity once," -ForegroundColor Red
    Write-Host "let it finish downloading, THEN run this script again." -ForegroundColor Red
    exit 1
}

$missing = $files | Where-Object { -not (Test-Path (Join-Path $sourceDir $_)) }
if ($missing) {
    Write-Host "ERROR: missing source file(s) in ${sourceDir}:" -ForegroundColor Red
    $missing | ForEach-Object { Write-Host "  - $_" -ForegroundColor Red }
    Write-Host "Did you pull the full branch (git lfs / large files included)?" -ForegroundColor Red
    exit 1
}

foreach ($f in $files) {
    $src = Join-Path $sourceDir $f
    $dst = Join-Path $targetDir $f
    Copy-Item -Path $src -Destination $dst -Force
    Write-Host "  Replaced: $f" -ForegroundColor Green
}

Write-Host ""
Write-Host "Done. The Windows-fixed LlamaLib build is now active." -ForegroundColor Cyan
Write-Host "If Unity is already open, close and reopen the project" -ForegroundColor Cyan
Write-Host "(or at least close Play mode and let scripts recompile) before testing." -ForegroundColor Cyan
