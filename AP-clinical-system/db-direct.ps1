# Run this instead of `dotnet ef database update`.
# Checks for DropColumn/DropTable ops with live data first, then migrates if safe.

$scriptDir     = Split-Path -Parent $MyInvocation.MyCommand.Path
$migrationsDir = Join-Path $scriptDir "Migrations"

# Parse connection string from appsettings.json
$config  = Get-Content (Join-Path $scriptDir "appsettings.json") -Raw | ConvertFrom-Json
$cs      = $config.ConnectionStrings.DefaultConnection
$parts   = @{}; $cs -split ";" | ForEach-Object { $kv = $_ -split "=", 2; if ($kv.Count -eq 2) { $parts[$kv[0].Trim().ToLower()] = $kv[1].Trim() } }
$dbServer = if ($parts["server"])   { $parts["server"] }   else { "localhost" }
$dbName   = if ($parts["database"]) { $parts["database"] } else { $parts["initial catalog"] }

# Nothing to check if no migrations folder exists yet
if (-not (Test-Path $migrationsDir)) { & dotnet ef database update; exit $LASTEXITCODE }

# Grab migration files, excluding EF-generated snapshots and designer files
$files = Get-ChildItem "$migrationsDir\*.cs" |
    Where-Object { $_.Name -notlike "*ModelSnapshot*" -and $_.Name -notlike "*Designer*" }

# Parse each migration file for DropColumn / DropTable calls inside Up() only
# (scoping to Up() avoids false positives from the Down() rollback method)
$ops = foreach ($f in $files) {
    $up = if (($c = Get-Content $f.FullName -Raw) -match "(?s)void Up\(.*?\)(.*?)void Down\(") { $Matches[1] } else { $c }

    # Match: DropColumn(name: "Col", table: "Tbl")
    [regex]::Matches($up, '(?i)DropColumn\s*\([^)]*name\s*:\s*"([^"]+)"[^)]*table\s*:\s*"([^"]+)"') |
        ForEach-Object { [pscustomobject]@{ Type="Col"; Table=$_.Groups[2].Value; Column=$_.Groups[1].Value } }

    # Match: DropTable(name: "Tbl")
    [regex]::Matches($up, '(?i)DropTable\s*\(\s*name\s*:\s*"([^"]+)"') |
        ForEach-Object { [pscustomobject]@{ Type="Tbl"; Table=$_.Groups[1].Value; Column=$null } }
}

# No destructive ops found — safe to proceed directly to migration
if (-not $ops) { & dotnet ef database update; exit $LASTEXITCODE }

$blocked = @()

foreach ($op in $ops) {
    # For columns: count non-null rows. For tables: count all rows.
    $q = if ($op.Type -eq "Col") {
        "SELECT COUNT(*) FROM [$($op.Table)] WHERE [$($op.Column)] IS NOT NULL"
    } else {
        "SELECT COUNT(*) FROM [$($op.Table)]"
    }

    # Use sqlcmd with trusted connection (-E), -h -1 strips headers, -W trims whitespace
    $n = & sqlcmd -S $dbServer -d $dbName -E -h -1 -W -Q $q 2>&1

    # If the query returned a number > 0, this drop would cause data loss
    $trimmed = ($n | Where-Object { $_ -match '^\d+$' } | Select-Object -First 1)
    if ($trimmed -and [int]$trimmed -gt 0) {
        $blocked += if ($op.Type -eq "Col") { "BLOCKED: [$($op.Table)].[$($op.Column)] has $trimmed row(s)" }
                    else                     { "BLOCKED: [$($op.Table)] has $trimmed row(s)" }
    }
}

# If any drops are unsafe, print errors and exit — otherwise run the migration
if ($blocked) { $blocked | ForEach-Object { Write-Error $_ }; exit 1 }
else          { & dotnet ef database update; exit $LASTEXITCODE }
