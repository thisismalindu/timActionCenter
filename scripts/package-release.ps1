param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+([-.][0-9A-Za-z.-]+)?$')]
    [string]$Version,

    [ValidateSet('win-x64', 'win-arm64')]
    [string]$Runtime = 'win-x64'
)

$ErrorActionPreference = 'Stop'

$repositoryRoot = Split-Path -Parent $PSScriptRoot
$projectPath = Join-Path $repositoryRoot 'timActionCenter\timActionCenter.csproj'
$artifactsDirectory = Join-Path $repositoryRoot 'artifacts'
$packageName = "timActionCenter-$Version-$Runtime"
$publishDirectory = Join-Path $artifactsDirectory $packageName
$archivePath = Join-Path $artifactsDirectory "$packageName.zip"
$checksumPath = "$archivePath.sha256"

New-Item -ItemType Directory -Force -Path $artifactsDirectory | Out-Null

if (Test-Path -LiteralPath $publishDirectory)
{
    Remove-Item -LiteralPath $publishDirectory -Recurse -Force
}

if (Test-Path -LiteralPath $archivePath)
{
    Remove-Item -LiteralPath $archivePath -Force
}

if (Test-Path -LiteralPath $checksumPath)
{
    Remove-Item -LiteralPath $checksumPath -Force
}

dotnet publish $projectPath `
    --configuration Release `
    --runtime $Runtime `
    --self-contained true `
    --output $publishDirectory `
    -p:PublishSingleFile=true `
    -p:IncludeNativeLibrariesForSelfExtract=true `
    -p:DebugType=None `
    -p:DebugSymbols=false `
    -p:Version=$Version

if ($LASTEXITCODE -ne 0)
{
    throw "dotnet publish failed with exit code $LASTEXITCODE."
}

Compress-Archive -Path (Join-Path $publishDirectory '*') -DestinationPath $archivePath

$hash = (Get-FileHash -Algorithm SHA256 -LiteralPath $archivePath).Hash.ToLowerInvariant()
"$hash  $([System.IO.Path]::GetFileName($archivePath))" | Set-Content -LiteralPath $checksumPath

Write-Output "Created $archivePath"
Write-Output "Created $checksumPath"
