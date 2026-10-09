# Requires PowerShell 5.1 or later. Self-contained so it also works through iwr | iex.
[CmdletBinding()]
param(
    [ValidatePattern('^[A-Za-z0-9_.-]+/[A-Za-z0-9_.-]+$')]
    [string]$Repository = 'XiaQueNet/ZenUI-WPF',
    [ValidateNotNullOrEmpty()]
    [string]$Ref = 'main',
    [string]$CodexHome = $env:CODEX_HOME,
    # Optional local source for offline installation from a downloaded checkout.
    [string]$SourcePath
)

# Keep preference and helper functions out of the caller's scope when using iex.
& {
    param($Repository, $Ref, $CodexHome, $SourcePath)
    $ErrorActionPreference = 'Stop'

    function Assert-ChildPath {
        param([string]$Parent, [string]$Child)
        $prefix = [IO.Path]::GetFullPath($Parent).TrimEnd('\', '/') + [IO.Path]::DirectorySeparatorChar
        $full = [IO.Path]::GetFullPath($Child)
        if (-not $full.StartsWith($prefix, [StringComparison]::OrdinalIgnoreCase)) {
            throw "Path is outside the intended directory: $full"
        }
    }

    function Assert-NoReparsePoint {
        param([string]$Path)
        $current = [IO.Path]::GetFullPath($Path)
        while ($current) {
            if (Test-Path -LiteralPath $current) {
                $item = Get-Item -LiteralPath $current -Force
                if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) {
                    throw "Install paths must not contain symbolic links or junctions: $current"
                }
            }
            $current = Split-Path -Path $current -Parent
        }
    }

    function Assert-Skill {
        param([string]$Path)
        $required = @(
            'SKILL.md', 'agents/openai.yaml', 'assets/FormView.xaml',
            'references/getting-started.md', 'references/controls.md',
            'references/recipes.md', 'references/theming.md',
            'references/converters.md', 'references/troubleshooting.md'
        )
        foreach ($relative in $required) {
            $file = Join-Path $Path $relative
            if (-not (Test-Path -LiteralPath $file -PathType Leaf) -or
                (Get-Item -LiteralPath $file).Length -eq 0) {
                throw "Incomplete ZenUI skill: $relative is missing or empty."
            }
        }
        $content = Get-Content -LiteralPath (Join-Path $Path 'SKILL.md') -Raw -Encoding UTF8
        if ($content -notmatch '(?m)^name:\s*zenui-wpf\s*$') {
            throw 'SKILL.md does not identify the zenui-wpf skill.'
        }
    }

    if ([string]::IsNullOrWhiteSpace($CodexHome)) {
        $CodexHome = Join-Path ([Environment]::GetFolderPath('UserProfile')) '.codex'
    }
    $installHome = $ExecutionContext.SessionState.Path.GetUnresolvedProviderPathFromPSPath($CodexHome)
    $skillsRoot = Join-Path $installHome 'skills'
    $target = Join-Path $skillsRoot 'zenui-wpf'
    $backupRoot = Join-Path $installHome 'skill-backups'
    $workRoot = Join-Path $installHome 'skill-install-staging'
    $work = Join-Path $workRoot ([Guid]::NewGuid().ToString('N'))
    $stage = Join-Path $work 'zenui-wpf'
    foreach ($path in @($installHome, $target, $backupRoot, $work)) {
        Assert-NoReparsePoint $path
    }
    Assert-ChildPath $skillsRoot $target
    Assert-ChildPath $workRoot $work
    New-Item -ItemType Directory -Path $installHome -Force | Out-Null
    $lock = $null
    $backup = $null
    $installed = $false
    try {
        # Keep the lock file after disposing: deleting it can race with another installer.
        $lock = [IO.File]::Open((Join-Path $installHome '.zenui-wpf-install.lock'),
            [IO.FileMode]::OpenOrCreate, [IO.FileAccess]::ReadWrite, [IO.FileShare]::None)
        New-Item -ItemType Directory -Path $stage -Force | Out-Null
        if (-not [string]::IsNullOrWhiteSpace($SourcePath)) {
            $source = (Resolve-Path -LiteralPath $SourcePath).ProviderPath
            Assert-NoReparsePoint $source
            Assert-Skill $source
            # Reject links before recursive copy, including links nested in the source.
            $queue = New-Object 'System.Collections.Generic.Queue[string]'
            $queue.Enqueue($source)
            while ($queue.Count -gt 0) {
                foreach ($item in Get-ChildItem -LiteralPath $queue.Dequeue() -Force) {
                    if ($item.Attributes -band [IO.FileAttributes]::ReparsePoint) {
                        throw "Source contains a symbolic link or junction: $($item.FullName)"
                    }
                    if ($item.PSIsContainer) { $queue.Enqueue($item.FullName) }
                }
            }
            Get-ChildItem -LiteralPath $source -Force | Copy-Item -Destination $stage -Recurse -Force
        }
        else {
            $zipPath = Join-Path $work 'source.zip'
            $encodedRef = [Uri]::EscapeDataString($Ref)
            $uri = "https://codeload.github.com/$Repository/zip/$encodedRef"
            Write-Host "Downloading $Repository ($Ref)..."
            $previousTls = [Net.ServicePointManager]::SecurityProtocol
            try {
                [Net.ServicePointManager]::SecurityProtocol = $previousTls -bor [Net.SecurityProtocolType]::Tls12
                Invoke-WebRequest -UseBasicParsing -Uri $uri -OutFile $zipPath -TimeoutSec 180
            }
            finally {
                [Net.ServicePointManager]::SecurityProtocol = $previousTls
            }
            Add-Type -AssemblyName System.IO.Compression.FileSystem
            $archive = [IO.Compression.ZipFile]::OpenRead($zipPath)
            try {
                $archiveRoot = $null
                foreach ($entry in $archive.Entries) {
                    if ($entry.FullName -notmatch '^([^/]+)/skills/zenui-wpf/(.+)$') { continue }
                    $root = $Matches[1]
                    $relative = $Matches[2]
                    if ($archiveRoot -and $archiveRoot -ne $root) { throw 'Ambiguous archive roots.' }
                    $archiveRoot = $root
                    if ($relative -match '(^|[/\\])\.\.([/\\]|$)|[:\\]') {
                        throw "Unsafe archive entry: $($entry.FullName)"
                    }
                    $output = Join-Path $stage $relative
                    Assert-ChildPath $stage $output
                    if ($entry.FullName.EndsWith('/')) {
                        New-Item -ItemType Directory -Path $output -Force | Out-Null
                    }
                    else {
                        New-Item -ItemType Directory -Path (Split-Path $output -Parent) -Force | Out-Null
                        [IO.Compression.ZipFileExtensions]::ExtractToFile($entry, $output, $false)
                    }
                }
            }
            finally { $archive.Dispose() }
        }

        Assert-Skill $stage
        New-Item -ItemType Directory -Path $skillsRoot -Force | Out-Null
        if (Test-Path -LiteralPath $target) {
            if (-not (Test-Path -LiteralPath $target -PathType Container)) { throw "Not a directory: $target" }
            Assert-NoReparsePoint $target
            New-Item -ItemType Directory -Path $backupRoot -Force | Out-Null
            $backup = Join-Path $backupRoot ('zenui-wpf-' + (Get-Date -Format 'yyyyMMdd-HHmmss') + '-' + [Guid]::NewGuid().ToString('N'))
            Assert-ChildPath $backupRoot $backup
            Assert-ChildPath $skillsRoot $target
            Move-Item -LiteralPath $target -Destination $backup
        }
        try {
            Assert-ChildPath $work $stage
            Move-Item -LiteralPath $stage -Destination $target
            $installed = $true
            Assert-Skill $target
        }
        catch {
            if ($installed -and (Test-Path -LiteralPath $target)) {
                Assert-ChildPath $skillsRoot $target
                Remove-Item -LiteralPath $target -Recurse -Force
            }
            if ($backup -and (Test-Path -LiteralPath $backup)) {
                Assert-ChildPath $backupRoot $backup
                Move-Item -LiteralPath $backup -Destination $target
                $backup = $null
            }
            throw
        }
        Write-Host "Installed zenui-wpf: $target"
        if ($backup) { Write-Host "Previous version backed up: $backup" }
        Write-Host 'Try in Codex: Use $zenui-wpf to build a WPF page.'
        Write-Host 'If the skill is not listed, restart Codex. Run this installer again to update.'
    }
    finally {
        try {
            if ($lock -and (Test-Path -LiteralPath $work)) {
                Assert-ChildPath $workRoot $work
                Remove-Item -LiteralPath $work -Recurse -Force
            }
        }
        finally { if ($lock) { $lock.Dispose() } }
    }
} $Repository $Ref $CodexHome $SourcePath
