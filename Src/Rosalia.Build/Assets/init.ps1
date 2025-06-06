<#
.SYNOPSIS
    Adds Rosalia launch profile to project's launchSettings.json
.NOTES
    This script runs automatically when the NuGet package is installed
#>

param(
    [string]$installPath,
    [string]$toolsPath,
    [string]$package,
    [string]$project
)

Write-Host "installPath: $installPath"
Write-Host "toolsPath: $toolsPath"
Write-Host "package: $package"
Write-Host "project: $project"
Write-Host "Get-Location: $(Get-Location)"

# Load the EnvDTE project model
try {
    $dteProject = Get-Interface $project.Object ([EnvDTE.Project])
    $projectPath = Split-Path $dteProject.FullName -Parent
}
catch {
    Write-Host "Warning: Could not access project properties. Trying to infer project path."
    # Try to infer from $project if it's a string path
    if ($project -is [string] -and (Test-Path $project)) {
        $projectPath = Split-Path $project -Parent
    }
    else {
        # Try using the current directory
        $cwd = Get-Location
        $projectPath = $cwd
    }
}

# Get the Rosalia version from the package
$rosaliaVersion = $package.Split('.')[-1] -replace '[^\d\.]',''
if (-not $rosaliaVersion) {
    $rosaliaVersion = "2.5.83"  # fallback version
}

# Path to launchSettings.json
$launchSettingsPath = Join-Path $projectPath "Properties\launchSettings.json"
$propertiesPath = Join-Path $projectPath "Properties"

# Create Properties directory if needed
if (-not (Test-Path $propertiesPath)) {
    New-Item -ItemType Directory -Path $propertiesPath | Out-Null
}

# Load existing settings or create new
$settings = @{profiles = @{}}
if (Test-Path $launchSettingsPath) {
    try {
        $settings = Get-Content $launchSettingsPath -Raw | ConvertFrom-Json -AsHashtable
    }
    catch {
        Write-Host "Warning: Existing launchSettings.json is invalid. Creating new one."
    }
}

# Add/update Rosalia profile
$settings.profiles["Run with Rosalia"] = @{
    commandName = "Executable"
    executablePath = "`$(NuGetPackageRoot)rosalia\$rosaliaVersion\tools\Rosalia.exe"
    commandLineArgs = "-c echo `$(TargetPath)"
    workingDirectory = "`$(ProjectDir)"
}

# Save with pretty formatting
$json = $settings | ConvertTo-Json -Depth 5
[System.IO.File]::WriteAllText($launchSettingsPath, $json)

Write-Host "Successfully added Rosalia launch profile to $launchSettingsPath"