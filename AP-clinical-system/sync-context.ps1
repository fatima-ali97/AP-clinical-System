# Scans Models/Entities/*.cs for class names, adds missing DbSet<> entries to AP_Context.cs.
# Run this after creating a new entity file.

$scriptDir   = Split-Path -Parent $MyInvocation.MyCommand.Path
$entitiesDir = Join-Path $scriptDir "Models\Entities"
$contextFile = Join-Path $scriptDir "Models\sql_Context\AP_Context.cs"

# Read all public class names from entity files
$entityNames = Get-ChildItem "$entitiesDir\*.cs" -ErrorAction SilentlyContinue | ForEach-Object {
    $content = Get-Content $_.FullName -Raw
    # Match "public class ClassName" (stops at whitespace, colon, or brace)
    if ($content -match 'public\s+class\s+(\w+)') { $Matches[1] }
}

if (-not $entityNames) { Write-Host "No entity classes found in Models/Entities/."; exit 0 }

# Read current context file and find which DbSets already exist
$contextContent = Get-Content $contextFile -Raw
$existing = [regex]::Matches($contextContent, 'DbSet<(\w+)>') | ForEach-Object { $_.Groups[1].Value }

# Filter to only the missing ones
$missing = $entityNames | Where-Object { $_ -notin $existing }

if (-not $missing) { Write-Host "All entities already in context. Nothing to add."; exit 0 }

# Build the DbSet lines to inject, using plural name convention (ClassName + "s")
$lines = $missing | ForEach-Object { "        public DbSet<$_> ${_}s { get; set; }" }

# Also build the using directive needed for entities
$usingLine = "using AP_clinical_system.Models.Entities;"

# Add the using if not already present
if ($contextContent -notmatch [regex]::Escape($usingLine)) {
    $contextContent = $contextContent -replace '(using Microsoft\.EntityFrameworkCore;)', "`$1`r`n$usingLine"
}

# Insert new DbSet lines right before the closing brace of the class
# Find the last "}" that closes the class (second-to-last "}" in the file)
$insertBlock = ($lines -join "`r`n") + "`r`n"
$contextContent = $contextContent -replace '(\s*)\}\s*\}\s*$', "`r`n$insertBlock`$1}`r`n}`r`n"

Set-Content -Path $contextFile -Value $contextContent -NoNewline

Write-Host "Added $($missing.Count) DbSet(s): $($missing -join ', ')"
