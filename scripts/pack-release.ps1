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

function Test-PackageConsumer {
    param(
        [Parameter(Mandatory = $true)]
        [string]$PackageId,

        [Parameter(Mandatory = $true)]
        [string]$ExpectedVersion,

        [Parameter(Mandatory = $true)]
        [string]$PackageDirectory
    )

    $temporaryRoot = [System.IO.Path]::GetFullPath([System.IO.Path]::GetTempPath())
    $consumerRoot = [System.IO.Path]::GetFullPath((Join-Path $temporaryRoot (
        'ZenUI-WPF-consumer-' + [System.Guid]::NewGuid().ToString('N'))))
    $temporaryRootPrefix = $temporaryRoot.TrimEnd(
        [System.IO.Path]::DirectorySeparatorChar,
        [System.IO.Path]::AltDirectorySeparatorChar) + [System.IO.Path]::DirectorySeparatorChar
    if (-not $consumerRoot.StartsWith(
            $temporaryRootPrefix,
            [System.StringComparison]::OrdinalIgnoreCase)) {
        throw "Consumer verification directory must remain under $temporaryRoot."
    }
    $frameworkAssets = [ordered]@{
        'net462' = 'net462'
        'net47' = 'net462'
        'net471' = 'net471'
        'net472' = 'net472'
        'net48' = 'net472'
        'net481' = 'net472'
        'net5.0-windows' = 'net5.0-windows7.0'
        'net6.0-windows' = 'net5.0-windows7.0'
        'net7.0-windows' = 'net5.0-windows7.0'
        'net8.0-windows' = 'net8.0-windows7.0'
        'net9.0-windows' = 'net8.0-windows7.0'
        'net10.0-windows' = 'net8.0-windows7.0'
    }

    try {
        New-Item -ItemType Directory -Path $consumerRoot | Out-Null
        $escapedPackageDirectory = [System.Security.SecurityElement]::Escape(
            [System.IO.Path]::GetFullPath($PackageDirectory))
        $nugetConfig = @"
<?xml version="1.0" encoding="utf-8"?>
<configuration>
  <config>
    <add key="globalPackagesFolder" value="packages" />
  </config>
  <packageSources>
    <clear />
    <add key="release" value="$escapedPackageDirectory" />
    <add key="nuget.org" value="https://api.nuget.org/v3/index.json" />
  </packageSources>
  <packageSourceMapping>
    <packageSource key="release">
      <package pattern="ZenUI.*" />
    </packageSource>
    <packageSource key="nuget.org">
      <package pattern="Microsoft.*" />
      <package pattern="NETStandard.Library" />
    </packageSource>
  </packageSourceMapping>
</configuration>
"@
        $nugetConfigPath = Join-Path $consumerRoot 'NuGet.Config'
        [System.IO.File]::WriteAllText(
            $nugetConfigPath,
            $nugetConfig,
            [System.Text.UTF8Encoding]::new($false))

        Write-Host "Verifying $PackageId $ExpectedVersion from local package output..."
        foreach ($entry in $frameworkAssets.GetEnumerator()) {
            $targetFramework = $entry.Key
            $expectedAssetFramework = $entry.Value
            $projectDirectory = Join-Path $consumerRoot $targetFramework
            New-Item -ItemType Directory -Path $projectDirectory | Out-Null

            $projectContent = @"
<Project Sdk="Microsoft.NET.Sdk">
  <PropertyGroup>
    <TargetFramework>$targetFramework</TargetFramework>
    <UseWPF>true</UseWPF>
    <LangVersion>8.0</LangVersion>
    <CheckEolTargetFramework>false</CheckEolTargetFramework>
  </PropertyGroup>
  <ItemGroup>
    <PackageReference Include="$PackageId" Version="$ExpectedVersion" />
  </ItemGroup>
</Project>
"@
            $projectPath = Join-Path $projectDirectory 'Consumer.csproj'
            [System.IO.File]::WriteAllText(
                $projectPath,
                $projectContent,
                [System.Text.UTF8Encoding]::new($false))

            if ($PackageId -eq 'ZenUI.Wpf') {
                $codeContent = @'
using System;
using System.Windows;
using ZenUI.Wpf.Controls;

namespace ReleaseConsumer
{
    internal static class Contract
    {
        public static object Create()
        {
            var resources = new ResourceDictionary
            {
                Source = new Uri(
                    "/ZenUI.Wpf;component/Themes/Generic.xaml",
                    UriKind.Relative)
            };
            return new ZenButton { Content = resources };
        }
    }
}
'@
                $xamlContent = @'
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:zen="https://zenui.mnorg.cn/xaml/wpf">
    <Style x:Key="ConsumerButtonStyle" TargetType="{x:Type zen:ZenButton}" />
</ResourceDictionary>
'@
            }
            else {
                $codeContent = @'
using ZenUI.Wpf.Converters;

namespace ReleaseConsumer
{
    internal static class Contract
    {
        public static object Create()
        {
            return new BoolToVisibilityConverter();
        }
    }
}
'@
                $xamlContent = @'
<ResourceDictionary
    xmlns="http://schemas.microsoft.com/winfx/2006/xaml/presentation"
    xmlns:x="http://schemas.microsoft.com/winfx/2006/xaml"
    xmlns:zenConverters="https://zenui.mnorg.cn/xaml/wpf/converters">
    <zenConverters:BoolToVisibilityConverter x:Key="BoolToVisibility" />
</ResourceDictionary>
'@
            }

            [System.IO.File]::WriteAllText(
                (Join-Path $projectDirectory 'Contract.cs'),
                $codeContent,
                [System.Text.UTF8Encoding]::new($false))
            [System.IO.File]::WriteAllText(
                (Join-Path $projectDirectory 'Contract.xaml'),
                $xamlContent,
                [System.Text.UTF8Encoding]::new($false))

            Invoke-NativeCommand -FilePath 'dotnet' -WorkingDirectory $projectDirectory -ArgumentList @(
                'restore',
                'Consumer.csproj',
                '--configfile',
                $nugetConfigPath
            )
            Invoke-NativeCommand -FilePath 'dotnet' -WorkingDirectory $projectDirectory -ArgumentList @(
                'build',
                'Consumer.csproj',
                '-c',
                'Release',
                '--no-restore'
            )

            $assetsPath = Join-Path $projectDirectory 'obj\project.assets.json'
            $assets = Get-Content -LiteralPath $assetsPath -Raw | ConvertFrom-Json
            $packageKey = "$PackageId/$ExpectedVersion"
            $target = $assets.targets.PSObject.Properties | Select-Object -First 1
            $packageTarget = $target.Value.PSObject.Properties |
                Where-Object { $_.Name -eq $packageKey } |
                Select-Object -First 1
            if ($null -eq $packageTarget) {
                throw "$PackageId $ExpectedVersion was not restored for $targetFramework."
            }

            $compileAssets = @($packageTarget.Value.compile.PSObject.Properties.Name)
            $expectedAssetPrefix = "lib/$expectedAssetFramework/"
            if (-not ($compileAssets | Where-Object {
                    $_.StartsWith($expectedAssetPrefix, [System.StringComparison]::OrdinalIgnoreCase)
                })) {
                throw "$PackageId selected incorrect assets for ${targetFramework}: $($compileAssets -join ', ')."
            }
        }
        Write-Host "$PackageId consumer verification passed for $($frameworkAssets.Count) target frameworks."
    }
    finally {
        if (Test-Path -LiteralPath $consumerRoot) {
            Remove-Item -LiteralPath $consumerRoot -Recurse -Force
        }
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
        '--max-parallel-test-modules',
        '1',
        '--no-build',
        '--no-restore'
    )
    foreach ($framework in @('net5.0-windows', 'net6.0-windows', 'net7.0-windows')) {
        Invoke-NativeCommand -FilePath 'dotnet' -WorkingDirectory $worktreePath -ArgumentList @(
            'run',
            '--project',
            'tests\ZenUI.Wpf.ModernCompatibilityTests\ZenUI.Wpf.ModernCompatibilityTests.csproj',
            '-c',
            'Release',
            '-f',
            $framework,
            '--no-build',
            '--no-restore'
        )
    }
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

    foreach ($packageFile in $packages) {
        if (-not $expectedPackages.ContainsKey($packageFile.Name)) {
            throw "Unexpected package in release directory: $($packageFile.Name)."
        }

        Test-NuGetPackage `
            -Package $packageFile `
            -ExpectedId $expectedPackages[$packageFile.Name] `
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

    foreach ($releasePackage in $releasePackages) {
        Test-PackageConsumer `
            -PackageId $releasePackage.Id `
            -ExpectedVersion $Version `
            -PackageDirectory $outputDirectory
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
