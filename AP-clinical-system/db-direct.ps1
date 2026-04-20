<#
.SYNOPSIS
  One-stop DB management for AP Clinical System.

.DESCRIPTION
  Pipeline:
    1. Sync entities to DbContext
    2. Detect pending model changes and auto-create migration
    3. Safety guard: block DropColumn / DropTable with live data
    4. Apply migration(s)

.PARAMETER Name
  Custom migration name (default: auto-generated timestamp).

.PARAMETER Force
  Skip the destructive-op safety check.

.PARAMETER SyncOnly
  Only sync entities to context, then exit.

.PARAMETER StatusOnly
  Only sync + show pending migration status, then exit.
#>

param(
    [string]$Name,
    [switch]$Force,
    [switch]$SyncOnly,
    [switch]$StatusOnly
)

$ErrorActionPreference = "Stop"
$scriptDir     = Split-Path -Parent $MyInvocation.MyCommand.Path
$entitiesDir   = Join-Path $scriptDir "Models\Entities"
$contextFile   = Join-Path $scriptDir "Models\sql_Context\AP_Context.cs"
$migrationsDir = Join-Path $scriptDir "Models\sql_Context\Migrations"
$migOutputDir  = "Models/sql_Context/Migrations"
$nl            = [Environment]::NewLine

# ── Helpers ──────────────────────────────────────────────────────

function Write-Step  { param($msg) Write-Host ("" + $nl + ">>> " + $msg) -ForegroundColor Cyan }
function Write-Ok    { param($msg) Write-Host ("  [OK] " + $msg) -ForegroundColor Green }
function Write-Skip  { param($msg) Write-Host ("  [--] " + $msg) -ForegroundColor DarkGray }
function Write-Warn  { param($msg) Write-Host ("  [!!] " + $msg) -ForegroundColor Yellow }
function Write-Err   { param($msg) Write-Host ("  [XX] " + $msg) -ForegroundColor Red }

# =================================================================
#  STEP 1:  Sync entity classes to DbContext
# =================================================================
Write-Step "Syncing entities to DbContext..."

# Discover all public class names from entity files
$entityNames = @()
if (Test-Path $entitiesDir) {
    $entityNames = Get-ChildItem "$entitiesDir\*.cs" -ErrorAction SilentlyContinue | ForEach-Object {
        $content = Get-Content $_.FullName -Raw
        if ($content -match 'public\s+class\s+(\w+)') { $Matches[1] }
    }
}

if (-not $entityNames) {
    Write-Skip "No entity classes found in Models/Entities/."
} else {
    # Read current context and find existing DbSets
    $contextContent = Get-Content $contextFile -Raw
    $existingDbSets = [regex]::Matches($contextContent, 'DbSet<(\w+)>') | ForEach-Object { $_.Groups[1].Value }
    $missing = $entityNames | Where-Object { $_ -notin $existingDbSets }

    if (-not $missing) {
        Write-Ok ("All " + $entityNames.Count + " entities already registered.")
    } else {
        # Build DbSet lines (ClassName -> ClassNames)
        $lines = $missing | ForEach-Object { "        public DbSet<$_> ${_}s { get; set; }" }

        # Ensure the using directive exists
        $usingLine = "using AP_clinical_system.Models.Entities;"
        if ($contextContent -notmatch [regex]::Escape($usingLine)) {
            $usingReplacement = '$1' + $nl + $usingLine
            $contextContent = $contextContent -replace '(using Microsoft\.EntityFrameworkCore;)', $usingReplacement
        }

        # Insert new DbSet lines before the closing braces of the class
        $insertBlock = ($lines -join $nl) + $nl
        $closingReplacement = $nl + $insertBlock + '$1}' + $nl + '}' + $nl
        $contextContent = $contextContent -replace '(\s*)\}\s*\}\s*$', $closingReplacement

        Set-Content -Path $contextFile -Value $contextContent -NoNewline
        Write-Ok ("Added " + $missing.Count + " DbSet(s): " + ($missing -join ", "))
    }
}

if ($SyncOnly) {
    Write-Host ""
    Write-Host "Done (sync only)." -ForegroundColor Green
    exit 0
}

# =================================================================
#  STEP 2:  Detect pending model changes and auto-create migration
# =================================================================
Write-Step "Checking for pending model changes..."

# Build the project first so EF can inspect the compiled model
$buildOutput = & dotnet build "$scriptDir" --nologo -v q 2>&1
if ($LASTEXITCODE -ne 0) {
    Write-Err "Build failed. Fix compilation errors first:"
    $buildOutput | ForEach-Object { Write-Host ("    " + $_) -ForegroundColor Red }
    exit 1
}
Write-Ok "Build succeeded."

# Use dotnet ef migrations has-pending-model-changes (.NET 8+)
# Falls back to attempting a migration if command unavailable
$pendingCheck = & dotnet ef migrations has-pending-model-changes --project "$scriptDir" --output-dir $migOutputDir 2>&1
$hasPending = $false

if ($LASTEXITCODE -eq 0) {
    $pendingText = ($pendingCheck | Out-String)
    if ($pendingText -match "changes") {
        $hasPending = $true
    }
} else {
    # Command might not exist on older EF tools - assume pending
    $hasPending = $true
}

if ($hasPending) {
    # Generate migration name: user-supplied or timestamp-based
    if (-not $Name) {
        $Name = "Auto_" + (Get-Date -Format "yyyyMMdd_HHmmss")
    }

    Write-Host ("  Creating migration '" + $Name + "'...") -ForegroundColor White
    $migOutput = & dotnet ef migrations add $Name --project "$scriptDir" --output-dir $migOutputDir 2>&1
    $migText   = ($migOutput | Out-String)

    if ($LASTEXITCODE -ne 0) {
        if ($migText -match "No changes") {
            Write-Ok "Model is up-to-date. No migration needed."
        } else {
            Write-Err "Migration creation failed:"
            $migOutput | ForEach-Object { Write-Host ("    " + $_) -ForegroundColor Red }
            exit 1
        }
    } else {
        # Check for the empty-migration case (EF creates a file but it has no ops)
        $latestMigration = Get-ChildItem "$migrationsDir\*.cs" -ErrorAction SilentlyContinue |
            Where-Object { $_.Name -notlike "*ModelSnapshot*" -and $_.Name -notlike "*Designer*" } |
            Sort-Object LastWriteTime -Descending |
            Select-Object -First 1

        if ($latestMigration) {
            $migContent = Get-Content $latestMigration.FullName -Raw
            # If Up() body is empty (only whitespace/braces), remove the empty migration
            if ($migContent -match '(?s)void Up\(.*?\)\s*\{(\s*)\}') {
                Write-Skip "No model changes detected. Removing empty migration..."
                & dotnet ef migrations remove --project "$scriptDir" --output-dir $migOutputDir --force 2>&1 | Out-Null
            } else {
                Write-Ok ("Migration '" + $Name + "' created.")
            }
        } else {
            Write-Ok ("Migration '" + $Name + "' created.")
        }
    }
} else {
    Write-Ok "Model is up-to-date. No migration needed."
}

# =================================================================
#  STEP 3:  Show pending migration status
# =================================================================
Write-Step "Checking pending migrations..."

$pendingMigrations = & dotnet ef migrations list --project "$scriptDir" --output-dir $migOutputDir --no-connect 2>&1 |
    Where-Object { $_ -match "\(Pending\)" }

if ($pendingMigrations) {
    Write-Warn ($pendingMigrations.Count.ToString() + " pending migration(s):")
    $pendingMigrations | ForEach-Object { Write-Host ("    " + $_) -ForegroundColor Yellow }
} else {
    Write-Ok "No pending migrations."
}

if ($StatusOnly) {
    Write-Host ""
    Write-Host "Done (status only)." -ForegroundColor Green
    exit 0
}

# =================================================================
#  STEP 4:  Safety guard - block destructive ops with live data
# =================================================================
if (-not $Force) {
    Write-Step "Running safety check for destructive operations..."

    # Parse connection string
    $config   = Get-Content (Join-Path $scriptDir "appsettings.json") -Raw | ConvertFrom-Json
    $cs       = $config.ConnectionStrings.DefaultConnection
    $parts    = @{}
    $cs -split ";" | ForEach-Object {
        $kv = $_ -split "=", 2
        if ($kv.Count -eq 2) { $parts[$kv[0].Trim().ToLower()] = $kv[1].Trim() }
    }
    $dbServer = if ($parts["server"])   { $parts["server"] }   else { "localhost" }
    $dbName   = if ($parts["database"]) { $parts["database"] } else { $parts["initial catalog"] }

    if (-not (Test-Path $migrationsDir)) {
        Write-Skip "No migrations folder yet. Nothing to check."
    } else {
        # Grab migration files (exclude snapshots and designer files)
        $files = Get-ChildItem "$migrationsDir\*.cs" |
            Where-Object { $_.Name -notlike "*ModelSnapshot*" -and $_.Name -notlike "*Designer*" }

        # Parse Up() methods for DropColumn / DropTable calls
        $dropColPattern  = '(?i)DropColumn\s*\([^)]*name\s*:\s*"([^"]+)"[^)]*table\s*:\s*"([^"]+)"'
        $dropTblPattern  = '(?i)DropTable\s*\(\s*name\s*:\s*"([^"]+)"'

        $ops = foreach ($f in $files) {
            $fileContent = Get-Content $f.FullName -Raw
            $up = $fileContent
            if ($fileContent -match '(?s)void Up\(.*?\)(.*?)void Down\(') {
                $up = $Matches[1]
            }

            [regex]::Matches($up, $dropColPattern) | ForEach-Object {
                [pscustomobject]@{ Type="Col"; Table=$_.Groups[2].Value; Column=$_.Groups[1].Value }
            }

            [regex]::Matches($up, $dropTblPattern) | ForEach-Object {
                [pscustomobject]@{ Type="Tbl"; Table=$_.Groups[1].Value; Column=$null }
            }
        }

        if (-not $ops) {
            Write-Ok "No destructive operations found."
        } else {
            $blocked = @()

            foreach ($op in $ops) {
                if ($op.Type -eq "Col") {
                    $q = "SELECT COUNT(*) FROM [" + $op.Table + "] WHERE [" + $op.Column + "] IS NOT NULL"
                } else {
                    $q = "SELECT COUNT(*) FROM [" + $op.Table + "]"
                }

                $n = & sqlcmd -S $dbServer -d $dbName -E -h -1 -W -Q $q 2>&1
                $trimmed = ($n | Where-Object { $_ -match '^\d+$' } | Select-Object -First 1)

                if ($trimmed -and [int]$trimmed -gt 0) {
                    if ($op.Type -eq "Col") {
                        $blocked += "[" + $op.Table + "].[" + $op.Column + "] has " + $trimmed + " row(s) - would lose data"
                    } else {
                        $blocked += "[" + $op.Table + "] has " + $trimmed + " row(s) - would lose data"
                    }
                }
            }

            if ($blocked) {
                Write-Err "Destructive operations BLOCKED:"
                $blocked | ForEach-Object { Write-Host ("    " + $_) -ForegroundColor Red }
                Write-Host ""
                Write-Warn "Use -Force to override, or migrate/backup the data first."
                exit 1
            } else {
                Write-Ok "Destructive ops found but affected tables/columns are empty."
            }
        }
    }
} else {
    Write-Warn "Safety check SKIPPED (-Force)."
}

# =================================================================
#  STEP 5:  Apply migrations
# =================================================================
Write-Step "Applying migrations..."

& dotnet ef database update --project "$scriptDir"
if ($LASTEXITCODE -ne 0) {
    Write-Err "Database update failed."
    exit $LASTEXITCODE
}

Write-Host ""
Write-Host "================================================" -ForegroundColor Green
Write-Host "  All done! Database is up-to-date.             " -ForegroundColor Green
Write-Host "================================================" -ForegroundColor Green
Write-Host ""
