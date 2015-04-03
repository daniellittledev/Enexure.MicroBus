Import-Module "$PSScriptRoot\modules\msbuild\Invoke-MsBuild.psm1"
. "$PSScriptRoot\Versioning.ps1"

properties {
	$solutionName = "Enexure.MicroBus"
	$configuration = "Release"
	
	$solutionDir = Resolve-Path "$PSScriptRoot\.."
	$solutionFile = "$solutionDir\$solutionName.sln"
	$versions = Get-Versions $solutionDir $buildNumber
	$artifactsDir = "$solutionDir\artifacts"
	$nuget = "$PSScriptRoot\nuget.exe"
	$nunit = "$PSScriptRoot\nunit\nunit-console.exe"
}

task default -depends Package

task Package -depends Test { 
	
	if (!(Test-Path $artifactsDir)) {
		New-Item -Type Directory $artifactsDir
	}
	
	foreach($version in $versions) {
		
		$project = (ls -Path $version.Project -Filter "*.csproj")[0]
		& $nuget pack $project.FullName -OutputDirectory "$solutionDir\artifacts" -Properties "Configuration=$configuration"
		& $nuget pack $project.FullName -OutputDirectory "$solutionDir\artifacts" -Properties "Configuration=$configuration" -Symbols
	}
}

task Test -depends Compile { 
		
	& $nunit "$solutionDir\src\Enexure.MicroBus.Tests\bin\$configuration\Enexure.MicroBus.Tests.dll"	
}

task Compile -depends Version { 

	Invoke-MsBuild $solutionFile -MSBuildProperties @{ Configuration = "$configuration" }
}

task Version -depends Clean {

	Set-VersionProperties $versions
}

task Clean { 
	
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}