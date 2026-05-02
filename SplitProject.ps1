$ErrorActionPreference = "Stop"

$solutionDir = "C:\Users\abbas\Development\AP-clinical-System"
Set-Location $solutionDir

Write-Host "Creating Projects..."
dotnet new webapi -n AP-clinical-system.API --force
dotnet new mvc -n AP-clinical-system.MVC --force
dotnet new mvc -n AP-clinical-system.Reporting --force
dotnet new mvc -n AP-clinical-system.Admin --force

Write-Host "Adding Projects to Solution..."
dotnet sln AP-clinical-system.sln add "AP-clinical-system.API\AP-clinical-system.API.csproj"
dotnet sln AP-clinical-system.sln add "AP-clinical-system.MVC\AP-clinical-system.MVC.csproj"
dotnet sln AP-clinical-system.sln add "AP-clinical-system.Reporting\AP-clinical-system.Reporting.csproj"
dotnet sln AP-clinical-system.sln add "AP-clinical-system.Admin\AP-clinical-system.Admin.csproj"

Write-Host "Moving Data Layer (Models, Migrations, etc.) to API Project..."
Move-Item -Path ".\AP-clinical-system\Models" -Destination ".\AP-clinical-system.API\Models" -Force -ErrorAction SilentlyContinue
Move-Item -Path ".\AP-clinical-system\ViewModels" -Destination ".\AP-clinical-system.API\ViewModels" -Force -ErrorAction SilentlyContinue
Move-Item -Path ".\AP-clinical-system\Controllers\TestCalls.http" -Destination ".\AP-clinical-system.API\TestCalls.http" -Force -ErrorAction SilentlyContinue

Write-Host "Moving MVC Files to MVC Project..."
Copy-Item -Path ".\AP-clinical-system\Controllers\*" -Destination ".\AP-clinical-system.MVC\Controllers\" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item -Path ".\AP-clinical-system\Views\*" -Destination ".\AP-clinical-system.MVC\Views\" -Recurse -Force -ErrorAction SilentlyContinue
Copy-Item -Path ".\AP-clinical-system\wwwroot\*" -Destination ".\AP-clinical-system.MVC\wwwroot\" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Moving Admin Files to Admin Project..."
New-Item -ItemType Directory -Force -Path ".\AP-clinical-system.Admin\Views\Admin" -ErrorAction SilentlyContinue
Move-Item -Path ".\AP-clinical-system.MVC\Controllers\AdminController.cs" -Destination ".\AP-clinical-system.Admin\Controllers\" -Force -ErrorAction SilentlyContinue
Move-Item -Path ".\AP-clinical-system.MVC\Views\Admin\*" -Destination ".\AP-clinical-system.Admin\Views\Admin\" -Force -ErrorAction SilentlyContinue
Remove-Item -Path ".\AP-clinical-system.MVC\Views\Admin" -Recurse -Force -ErrorAction SilentlyContinue

Write-Host "Adding Project References..."
dotnet add ".\AP-clinical-system.MVC\AP-clinical-system.MVC.csproj" reference ".\AP-clinical-system.API\AP-clinical-system.API.csproj"
dotnet add ".\AP-clinical-system.Admin\AP-clinical-system.Admin.csproj" reference ".\AP-clinical-system.API\AP-clinical-system.API.csproj"

Write-Host "Done! Please verify everything copied correctly, then you can delete the old AP-clinical-system folder."
