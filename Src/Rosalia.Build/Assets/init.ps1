param($installPath, $toolsPath, $package, $project)

$csprojPath = $project.FileName
$projectRootDirectory = Split-Path $csprojPath -parent

Set-Location $projectRootDirectory

$toolsRelativePath = Resolve-Path $toolsPath -relative
Set-Location $toolsPath
$projectRelativePath = Resolve-Path $projectRootDirectory -relative

# Load the csproj as XML
[xml]$xml = Get-Content $csprojPath

# Find a PropertyGroup without a Condition, or create one if none exists
$propertyGroup = $xml.Project.PropertyGroup | Where-Object { -not $_.Condition }
if (-not $propertyGroup) {
    $propertyGroup = $xml.CreateElement("PropertyGroup")
    $xml.Project.AppendChild($propertyGroup) | Out-Null
}

function Set-Or-UpdateProperty($group, $name, $value) {
    $property = $group.SelectSingleNode($name)
    if ($property) {
        $property.InnerText = $value
    } else {
        $newProperty = $xml.CreateElement($name)
        $newProperty.InnerText = $value
        $group.AppendChild($newProperty) | Out-Null
    }
}

Set-Or-UpdateProperty $propertyGroup "StartAction" "Program"
Set-Or-UpdateProperty $propertyGroup "StartProgram" ("$(MSBuildProjectDirectory)\" + $toolsRelativePath + "\Rosalia.exe")
Set-Or-UpdateProperty $propertyGroup "StartWorkingDirectory" ("$(MSBuildProjectDirectory)\" + $toolsRelativePath)
Set-Or-UpdateProperty $propertyGroup "StartArguments" ('/hold ' + $projectRelativePath + '\bin\$(Configuration)\' + $project.Name + '.dll')

# Save the modified XML back to the csproj file
$xml.Save($csprojPath)