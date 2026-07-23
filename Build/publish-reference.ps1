param(
    [string]$Configuration = "Release",
    [string]$Runtime = "win-x64"
)

$ErrorActionPreference = "Stop"

$root = Split-Path -Parent $PSScriptRoot
$project = Join-Path $root "LicenseGenerator_Wpf\LicenseGenerator_Wpf.csproj"
$output = Join-Path $root "artifacts\release-reference"

Write-Host "========================================"
Write-Host " PB BZH License Generator WPF"
Write-Host " Publication de reference non obfusquee"
Write-Host "========================================"
Write-Host ""

Write-Host "Projet      : $project"
Write-Host "Configuration : $Configuration"
Write-Host "Runtime     : $Runtime"
Write-Host "Sortie      : $output"
Write-Host ""

if (Test-Path $output) {
    Remove-Item $output -Recurse -Force
}

dotnet clean $project -c $Configuration

dotnet publish $project `
    -c $Configuration `
    -r $Runtime `
    --self-contained true `
    -p:PublishSingleFile=true `
    -p:PublishReadyToRun=false `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -o $output

Write-Host ""
Write-Host "Publication terminee."
Write-Host ""

Get-ChildItem $output | Select-Object Name, Length, LastWriteTime