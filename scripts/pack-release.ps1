[CmdletBinding()]
param(
    [Parameter(Mandatory = $true)]
    [ValidatePattern('^\d+\.\d+\.\d+(?:-[0-9A-Za-z]+(?:\.[0-9A-Za-z]+)*)?$')]
    [string]$Version,

    [Parameter(Mandatory = $true)]
    [ValidateSet('ZenUI.Wpf', 'ZenUI.Wpf.Converters')]
    [string[]]$Package,

    [switch]$Force
)

$ErrorActionPreference = 'Stop'

function Invoke-NativeCommand {
    param(
        [Parameter(Mandatory = $true)]
        [string]$FilePath,

        [Parameter(Mandatory = $true)]
        [string[]]$ArgumentList,

        [Parameter(Mandatory = $true)]
        [string]$WorkingDirectory
    )

    Push-Location $WorkingDirectory
    try {
        & $FilePath @ArgumentList
        if ($LASTEXITCODE -ne 0) {
            throw "$FilePath exited with code $LASTEXITCODE."
        }
    }
    finally {
        Pop-Location
    }
}

function Get-GitValue {
    param(
        [Parameter(Mandatory = $true)]
        [string[]]$ArgumentList,

        [Parameter(Mandatory = $true)]
        [string]$WorkingDirectory
    )

    $output = & git -C $WorkingDirectory @ArgumentList 2>&1
    if ($LASTEXITCODE -ne 0) {
        throw "git $($ArgumentList -join ' ') failed: $($output -join [Environment]::NewLine)"
    }

    return ($output -join [Environment]::NewLine).Trim()
}

function Get-ProjectPackageVersion {
    param(
        [Parameter(Mandatory = $true)]
        [string]$ProjectPath
    )

    [xml]$project = Get-Content -LiteralPath $ProjectPath -Raw
    $versionGroup = $project.Project.PropertyGroup |
        Where-Object { $_.VersionPrefix } |
        Select-Object -First 1

    if ($null -eq $versionGroup) {
        throw "VersionPrefix is missing from $ProjectPath."
    }

    $packageVersion = [string]$versionGroup.VersionPrefix
    $versionSuffix = [string]$versionGroup.VersionSuffix
    if (-not [string]::IsNullOrWhiteSpace($versionSuffix)) {
        $packageVersion = "$packageVersion-$versionSuffix"
    }

    return $packageVersion
}

function Test-NuGetPackage {
    param(
        [Parameter(Mandatory = $true)]
        [System.IO.FileInfo]$Package,

        [Parameter(Mandatory = $true)]
        [string]$ExpectedId,

        [Parameter(Mandatory = $true)]
        [string]$ExpectedVersion,

        [Parameter(Mandatory = $true)]
        [string]$ExpectedCommit
    )

    $archive = [System.IO.Compression.ZipFile]::OpenRead($Package.FullName)
    try {
        $nuspecEntry = $archive.Entries |
            Where-Object { $_.FullName -like '*.nuspec' } |
            Select-Object -First 1
        if ($null -eq $nuspecEntry) {
            throw "NuSpec is missing from $($Package.Name)."
        }

        $reader = [System.IO.StreamReader]::new($nuspecEntry.Open())
        try {
            [xml]$nuspec = $reader.ReadToEnd()
        }
        finally {
            $reader.Dispose()
        }

        $metadata = $nuspec.package.metadata
        if ([string]$metadata.id -ne $ExpectedId) {
            throw "Expected package ID $ExpectedId, found $($metadata.id)."
        }
        if ([string]$metadata.version -ne $ExpectedVersion) {
            throw "Expected package version $ExpectedVersion, found $($metadata.version)."
        }
        if ([string]$metadata.license.'#text' -ne 'MIT') {
            throw "MIT license metadata is missing from $($Package.Name)."
        }
        if ([string]$metadata.repository.url -ne 'https://github.com/XiaQueNet/ZenUI-WPF.git') {
            throw "Repository URL is incorrect in $($Package.Name)."
        }
        if ([string]$metadata.repository.commit -ne $ExpectedCommit) {
            throw "Repository commit is incorrect in $($Package.Name)."
        }

        foreach ($requiredEntry in @('README.md', 'CHANGELOG.md', 'zenui-icon.png')) {
            if ($archive.Entries.FullName -notcontains $requiredEntry) {
                throw "$requiredEntry is missing from $($Package.Name)."
            }
        }

        $frameworks = $archive.Entries.FullName |
            Where-Object { $_ -like 'lib/*/*.dll' } |
            ForEach-Object { ($_ -split '/')[1] } |
            Sort-Object -Unique
        if (($frameworks -join ',') -ne 'net462,net471,net472,net5.0-windows7.0,net8.0-windows7.0') {
            throw "Target frameworks are incorrect in $($Package.Name): $($frameworks -join ', ')."
        }
        if (-not ($archive.Entries.FullName | Where-Object { $_ -like 'lib/*/*.xml' })) {
            throw "XML documentation is missing from $($Package.Name)."
        }
    }
    finally {
        $archive.Dispose()
    }
}

$repositoryRoot = [System.IO.Path]::GetFullPath((Join-Path $PSScriptRoot '..'))
$releaseRoot = [System.IO.Path]::GetFullPath((Join-Path $repositoryRoot 'artifacts\releases'))
$outputDirectory = [System.IO.Path]::GetFullPath((Join-Path $releaseRoot $Version))
$releaseRootPrefix = $releaseRoot.TrimEnd(
    [System.IO.Path]::DirectorySeparatorChar,
    [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar

if (-not $outputDirectory.StartsWith($releaseRootPrefix, [System.StringComparison]::OrdinalIgnoreCase)) {
    throw "Release output must remain under $releaseRoot."
}

$tagName = "v$Version"
$tagCommit = Get-GitValue -WorkingDirectory $repositoryRoot -ArgumentList @(
    'rev-parse',
    '--verify',
    "$tagName^{}"
)

if (Test-Path -LiteralPath $outputDirectory) {
    if (-not $Force) {
        throw "Release output already exists: $outputDirectory. Use -Force to regenerate it before publication."
    }

    Remove-Item -LiteralPath $outputDirectory -Recurse -Force
}

New-Item -ItemType Directory -Path $releaseRoot -Force | Out-Null

$worktreePath = Join-Path ([System.IO.Path]::GetTempPath()) (
    'ZenUI-WPF-release-' + [System.Guid]::NewGuid().ToString('N'))
$worktreeAdded = $false

try {
    Invoke-NativeCommand -FilePath 'git' -WorkingDirectory $repositoryRoot -ArgumentList @(
        'worktree',
        'add',
        '--detach',
        $worktreePath,
        $tagName
    )
    $worktreeAdded = $true

    $releasePackages = foreach ($packageId in $Package) {
        $projectPath = switch ($packageId) {
            'ZenUI.Wpf' { 'src\ZenUI.Wpf\ZenUI.Wpf.csproj' }
            'ZenUI.Wpf.Converters' { 'src\ZenUI.Wpf.Converters\ZenUI.Wpf.Converters.csproj' }
        }
        $fullProjectPath = Join-Path $worktreePath $projectPath
        $projectVersion = Get-ProjectPackageVersion -ProjectPath $fullProjectPath
        if ($projectVersion -ne $Version) {
            throw "Expected $packageId version ${Version}, found $projectVersion."
        }

        [pscustomobject]@{
            Id = $packageId
            ProjectPath = $projectPath
        }
    }

    Invoke-NativeCommand -FilePath 'dotnet' -WorkingDirectory $worktreePath -ArgumentList @(
        'restore',
        'ZenUI.Wpf.slnx'
    )
    Invoke-NativeCommand -FilePath 'dotnet' -WorkingDirectory $worktreePath -ArgumentList @(
        'build',
        'ZenUI.Wpf.slnx',
        '-c',
        'Release',
        '--no-restore'
    )
    Invoke-NativeCommand -FilePath 'dotnet' -WorkingDirectory $worktreePath -ArgumentList @(
        'test',
        'ZenUI.Wpf.slnx',
        '-c',
        'Release',
        '--no-build',
        '--no-restore'
    )
    Push-Location $worktreePath
    try {
        $vulnerabilityJson = & dotnet list ZenUI.Wpf.slnx package `
            --vulnerable `
            --include-transitive `
            --format json `
            --no-restore
        if ($LASTEXITCODE -ne 0) {
            throw "dotnet list package exited with code $LASTEXITCODE."
        }
    }
    finally {
        Pop-Location
    }

    $vulnerabilityReport = ($vulnerabilityJson -join [Environment]::NewLine) |
        ConvertFrom-Json
    $vulnerablePackages = foreach ($project in $vulnerabilityReport.projects) {
        foreach ($framework in $project.frameworks) {
            foreach ($package in @($framework.topLevelPackages) + @($framework.transitivePackages)) {
                if (@($package.vulnerabilities).Count -gt 0) {
                    "$($package.id) $($package.resolvedVersion)"
                }
            }
        }
    }
    if (@($vulnerablePackages).Count -gt 0) {
        throw "Vulnerable packages found: $($vulnerablePackages -join ', ')."
    }

    New-Item -ItemType Directory -Path $outputDirectory | Out-Null
    foreach ($releasePackage in $releasePackages) {
        Invoke-NativeCommand -FilePath 'dotnet' -WorkingDirectory $worktreePath -ArgumentList @(
            'pack',
            $releasePackage.ProjectPath,
            '-c',
            'Release',
            '--no-build',
            '-o',
            $outputDirectory
        )
    }

    Add-Type -AssemblyName System.IO.Compression.FileSystem

    $expectedPackages = @{}
    foreach ($releasePackage in $releasePackages) {
        $expectedPackages["$($releasePackage.Id).$Version.nupkg"] = $releasePackage.Id
    }
    $packages = Get-ChildItem -LiteralPath $outputDirectory -Filter '*.nupkg' -File
    $symbolPackages = Get-ChildItem -LiteralPath $outputDirectory -Filter '*.snupkg' -File
    if ($packages.Count -ne $releasePackages.Count -or
        $symbolPackages.Count -ne $releasePackages.Count) {
        throw "Expected $($releasePackages.Count) .nupkg and .snupkg file(s) in $outputDirectory."
    }

    foreach ($package in $packages) {
        if (-not $expectedPackages.ContainsKey($package.Name)) {
            throw "Unexpected package in release directory: $($package.Name)."
        }

        Test-NuGetPackage `
            -Package $package `
            -ExpectedId $expectedPackages[$package.Name] `
            -ExpectedVersion $Version `
            -ExpectedCommit $tagCommit
    }

    foreach ($symbolPackage in $symbolPackages) {
        $archive = [System.IO.Compression.ZipFile]::OpenRead($symbolPackage.FullName)
        try {
            if (-not ($archive.Entries.FullName | Where-Object { $_ -like 'lib/*/*.pdb' })) {
                throw "Portable PDB is missing from $($symbolPackage.Name)."
            }
        }
        finally {
            $archive.Dispose()
        }
    }

    $hashLines = Get-ChildItem -LiteralPath $outputDirectory -File |
        Where-Object { $_.Extension -in @('.nupkg', '.snupkg') } |
        Sort-Object Name |
        ForEach-Object {
            $hash = Get-FileHash -LiteralPath $_.FullName -Algorithm SHA256
            "$($hash.Hash)  $($_.Name)"
        }
    [System.IO.File]::WriteAllLines(
        (Join-Path $outputDirectory 'SHA256SUMS.txt'),
        $hashLines,
        [System.Text.UTF8Encoding]::new($false))

    Write-Host ''
    Write-Host "Release packages are ready: $outputDirectory"
    Get-ChildItem -LiteralPath $outputDirectory -File |
        Sort-Object Name |
        Select-Object Name, Length |
        Format-Table -AutoSize
}
catch {
    if (Test-Path -LiteralPath $outputDirectory) {
        Remove-Item -LiteralPath $outputDirectory -Recurse -Force
    }
    throw
}
finally {
    if ($worktreeAdded) {
        & git -C $repositoryRoot worktree remove --force $worktreePath
        if ($LASTEXITCODE -ne 0) {
            Write-Warning "Could not remove temporary worktree: $worktreePath"
        }
        & git -C $repositoryRoot worktree prune
    }
}
