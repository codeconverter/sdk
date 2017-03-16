$version = "1.0.9-alpha"
$releaseNotes = "Adding throw"

$projects = @('CodeConverter.Common', 'CodeConverter.CSharp', 'CodeConverter.PowerShell', 'CodeConverter')

foreach($project in $projects)
{
	$fileName = "$project\$project.csproj"
	[xml]$proj = Get-Content ".\$fileName"
	$proj.Project.PropertyGroup.Version = $version
	$proj.Project.PropertyGroup.PackageReleaseNotes = $releaseNotes

	$proj.Save((Join-Path $PSScriptRoot $fileName))
}

dotnet build -c Release

.\nuget.exe push CodeConverter.Common\bin\Release\codeconverter.common.$version.nupkg -Source https://www.nuget.org/api/v2/package
.\nuget.exe push CodeConverter.CSharp\bin\Release\codeconverter.csharp.$version.nupkg -Source https://www.nuget.org/api/v2/package
.\nuget.exe push CodeConverter.PowerShell\bin\Release\codeconverter.powershell.$version.nupkg -Source https://www.nuget.org/api/v2/package
.\nuget.exe push CodeConverter\bin\Release\codeconverter.$version.nupkg -Source https://www.nuget.org/api/v2/package
