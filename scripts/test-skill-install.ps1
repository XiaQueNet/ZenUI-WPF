# Behavioral tests run in a unique temporary directory, without network or real profile changes.
$ErrorActionPreference = 'Stop'
$installer = Join-Path $PSScriptRoot 'install-from-git.ps1'
$source = Join-Path (Split-Path $PSScriptRoot -Parent) 'skills/zenui-wpf'
$testRoot = Join-Path ([IO.Path]::GetTempPath()) ('zenui-install-test-' + [Guid]::NewGuid().ToString('N'))
$testHome = Join-Path $testRoot 'profile with spaces'
$target = Join-Path $testHome 'skills/zenui-wpf'
New-Item -ItemType Directory -Path $testRoot | Out-Null

function Assert-True($Value, $Message) {
    if (-not $Value) { throw $Message }
}
function Assert-Failure([scriptblock]$Action) {
    $failed = $false
    try { & $Action } catch { $failed = $true }
    Assert-True $failed 'Expected installation to fail.'
}

try {
    & $installer -SourcePath $source -CodexHome $testHome
    Assert-True (Test-Path -LiteralPath (Join-Path $target 'assets/FormView.xaml')) 'Asset missing.'
    Set-Content -LiteralPath (Join-Path $target 'old-marker.txt') -Value 'preserve me'
    & $installer -SourcePath $source -CodexHome $testHome
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $target 'old-marker.txt'))) 'Stale file was retained.'
    $backups = @(Get-ChildItem -LiteralPath (Join-Path $testHome 'skill-backups') -Directory)
    Assert-True ($backups.Count -eq 1) 'Expected one backup outside skills directory.'
    Assert-True (Test-Path -LiteralPath (Join-Path $backups[0].FullName 'old-marker.txt')) 'Backup lost old file.'

    $incomplete = Join-Path $testRoot 'incomplete'
    New-Item -ItemType Directory -Path $incomplete | Out-Null
    Copy-Item -LiteralPath (Join-Path $source 'SKILL.md') -Destination $incomplete
    Assert-Failure { & $installer -SourcePath $incomplete -CodexHome $testHome }
    Assert-True (Test-Path -LiteralPath (Join-Path $target 'SKILL.md')) 'Invalid source damaged installation.'

    # Inject a failure between backup and promotion; real Move-Item handles rollback.
    Set-Content -LiteralPath (Join-Path $target 'rollback-marker.txt') -Value 'restore me'
    function Move-Item {
        param($LiteralPath, $Destination)
        if ($LiteralPath -like '*skill-install-staging*') { throw 'Simulated promotion failure' }
        Microsoft.PowerShell.Management\Move-Item -LiteralPath $LiteralPath -Destination $Destination
    }
    Assert-Failure { & $installer -SourcePath $source -CodexHome $testHome }
    Remove-Item Function:\Move-Item
    Assert-True (Test-Path -LiteralPath (Join-Path $target 'rollback-marker.txt')) 'Rollback did not restore prior version.'

    # Build a GitHub-shaped archive; emulate iwr | iex without requiring a script path.
    $archiveRoot = Join-Path $testRoot 'ZenUI-WPF-main'
    $archiveSkills = Join-Path $archiveRoot 'skills'
    New-Item -ItemType Directory -Path $archiveSkills -Force | Out-Null
    Copy-Item -LiteralPath $source -Destination $archiveSkills -Recurse
    Set-Content -LiteralPath (Join-Path $archiveRoot 'unrelated.txt') -Value 'not installed'
    $script:fixtureZip = Join-Path $testRoot 'fixture.zip'
    Add-Type -AssemblyName System.IO.Compression
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $fixture = [IO.Compression.ZipFile]::Open($script:fixtureZip, [IO.Compression.ZipArchiveMode]::Create)
    try {
        foreach ($file in Get-ChildItem -LiteralPath $archiveRoot -File -Recurse) {
            $relative = $file.FullName.Substring($archiveRoot.Length).TrimStart('\','/').Replace('\','/')
            $null = [IO.Compression.ZipFileExtensions]::CreateEntryFromFile($fixture, $file.FullName, ('ZenUI-WPF-main/' + $relative))
        }
    }
    finally { $fixture.Dispose() }
    function Invoke-WebRequest {
        param($Uri, $OutFile, $TimeoutSec, [switch]$UseBasicParsing)
        Assert-True ($Uri -eq 'https://codeload.github.com/XiaQueNet/ZenUI-WPF/zip/main') 'Unexpected download URL.'
        Copy-Item -LiteralPath $script:fixtureZip -Destination $OutFile
    }
    $bootstrap = [scriptblock]::Create((Get-Content -LiteralPath $installer -Raw))
    & $bootstrap -CodexHome $testHome
    Assert-True (-not (Test-Path -LiteralPath (Join-Path $target 'unrelated.txt'))) 'Unrelated repository file installed.'
    foreach ($file in Get-ChildItem -LiteralPath $source -Recurse -File) {
        $relative = $file.FullName.Substring((Get-Item -LiteralPath $source).FullName.Length).TrimStart('\','/')
        Assert-True ((Get-FileHash -LiteralPath $file.FullName).Hash -eq
            (Get-FileHash -LiteralPath (Join-Path $target $relative)).Hash) "File mismatch: $relative"
    }

    # A malicious ZIP path must fail before touching the installed version.
    Add-Type -AssemblyName System.IO.Compression.FileSystem
    $zip = [IO.Compression.ZipFile]::Open($script:fixtureZip, [IO.Compression.ZipArchiveMode]::Update)
    try { $null = $zip.CreateEntry('ZenUI-WPF-main/skills/zenui-wpf/../../escape.txt') }
    finally { $zip.Dispose() }
    Assert-Failure { & $bootstrap -CodexHome $testHome }
    Assert-True (Test-Path -LiteralPath (Join-Path $target 'SKILL.md')) 'Malformed ZIP damaged installation.'
    Remove-Item Function:\Invoke-WebRequest
    function Invoke-WebRequest { throw 'Simulated network failure' }
    Assert-Failure { & $bootstrap -CodexHome $testHome }
    Assert-True (Test-Path -LiteralPath (Join-Path $target 'SKILL.md')) 'Network failure damaged installation.'
    Remove-Item Function:\Invoke-WebRequest
    Assert-True (@(Get-ChildItem -LiteralPath (Join-Path $testHome 'skill-install-staging') -Force).Count -eq 0) 'Staging was not cleaned.'
    Write-Host 'PASS: first install, update, backup, invalid source, rollback, downloaded archive, path traversal, network failure, and cleanup.'
}
finally {
    $resolvedTestRoot = [IO.Path]::GetFullPath($testRoot)
    $tempPrefix = [IO.Path]::GetFullPath([IO.Path]::GetTempPath()).TrimEnd('\','/') + [IO.Path]::DirectorySeparatorChar
    if (-not $resolvedTestRoot.StartsWith($tempPrefix, [StringComparison]::OrdinalIgnoreCase) -or
        (Split-Path $resolvedTestRoot -Leaf) -notlike 'zenui-install-test-*') { throw 'Unsafe test cleanup path.' }
    Remove-Item -LiteralPath $resolvedTestRoot -Recurse -Force
}
