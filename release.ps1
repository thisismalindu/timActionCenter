[CmdletBinding()]
param(
    [ValidateSet('Patch', 'Minor', 'Major')]
    [string]$Bump = 'Patch',

    [ValidateSet('win-x64', 'win-arm64')]
    [string]$Runtime = 'win-x64',

    [string]$Remote = 'origin'
)

$ErrorActionPreference = 'Stop'

function Invoke-Git
{
    & git @args

    if ($LASTEXITCODE -ne 0)
    {
        throw "git $($args -join ' ') failed with exit code $LASTEXITCODE."
    }
}

function Get-NextVersion
{
    param(
        [string[]]$RemoteTags,
        [string]$Increment
    )

    $versions = foreach ($remoteTag in $RemoteTags)
    {
        if ($remoteTag -match 'refs/tags/v(?<major>\d+)\.(?<minor>\d+)\.(?<patch>\d+)$')
        {
            [Version]::new(
                [int]$Matches.major,
                [int]$Matches.minor,
                [int]$Matches.patch)
        }
    }

    $latestVersion = $versions | Sort-Object -Descending | Select-Object -First 1

    if ($null -eq $latestVersion)
    {
        return [Version]::new(0, 1, 0)
    }

    switch ($Increment)
    {
        'Major' { return [Version]::new($latestVersion.Major + 1, 0, 0) }
        'Minor' { return [Version]::new($latestVersion.Major, $latestVersion.Minor + 1, 0) }
        default { return [Version]::new($latestVersion.Major, $latestVersion.Minor, $latestVersion.Build + 1) }
    }
}

$repositoryRoot = $PSScriptRoot
$packageScript = Join-Path $repositoryRoot 'scripts\package-release.ps1'
$workflowPath = Join-Path $repositoryRoot '.github\workflows\release.yml'

Push-Location $repositoryRoot

try
{
    if (-not (Get-Command git -ErrorAction SilentlyContinue))
    {
        throw 'Git is not installed or is not available on PATH.'
    }

    if (-not (Test-Path -LiteralPath $workflowPath))
    {
        throw 'The GitHub release workflow is missing.'
    }

    $currentBranch = [string](Invoke-Git branch --show-current | Select-Object -First 1)
    $currentBranch = $currentBranch.Trim()

    if ([string]::IsNullOrWhiteSpace($currentBranch))
    {
        throw 'A release cannot be created from a detached HEAD.'
    }

    $workingTreeChanges = @(Invoke-Git status --porcelain)

    if ($workingTreeChanges.Count -gt 0)
    {
        throw 'Commit or stash all changes before creating a release.'
    }

    $remoteTags = @(Invoke-Git ls-remote --tags --refs $Remote 'refs/tags/v*')
    $nextVersion = Get-NextVersion -RemoteTags $remoteTags -Increment $Bump
    $version = $nextVersion.ToString(3)
    $tagName = "v$version"

    Write-Output "Creating $tagName from branch '$currentBranch'."

    & $packageScript -Version $version -Runtime $Runtime

    if ($LASTEXITCODE -ne 0)
    {
        throw "Packaging failed with exit code $LASTEXITCODE."
    }

    Invoke-Git push $Remote "HEAD:refs/heads/$currentBranch"

    $localTag = @(Invoke-Git tag --list $tagName)

    if ($localTag.Count -gt 0)
    {
        $existingTagCommit = [string](Invoke-Git rev-list -n 1 $tagName | Select-Object -First 1)
        $headCommit = (Invoke-Git rev-parse HEAD | Select-Object -First 1).Trim()

        if ($existingTagCommit.Trim() -ne $headCommit)
        {
            throw "Local tag $tagName already points to a different commit."
        }
    }
    else
    {
        Invoke-Git tag -a $tagName -m "timActionCenter $version"
    }

    Invoke-Git push $Remote "refs/tags/$tagName"

    Write-Output ''
    Write-Output "Released $tagName."
    Write-Output "The package is in artifacts\ and GitHub Actions is publishing the GitHub Release."
}
finally
{
    Pop-Location
}
