Import-Module "$PSScriptRoot\modules\msbuild\Invoke-MsBuild.psm1"
. "$PSScriptRoot\Versioning.ps1"

properties {
	$solutionName = "Enexure.MicroBus"

	$solutionDir = Resolve-Path "$PSScriptRoot\.."
	$solutionFile = "$solutionDir\$solutionName.sln"
	$versions = Get-Versions $solutionDir $buildNumber
	$artifactsDir = "$solutionDir\artifacts"
	$nuget = "$PSScriptRoot\nuget.exe"
}

task default -depends Package

task Package -depends Compile { 
	
	if (!(Test-Path $artifactsDir)) {
		New-Item -Type Directory $artifactsDir
	}
	
	foreach($version in $versions) {
		
		$project = (ls -Path $version.Project -Filter "*.csproj")[0]
		& $nuget pack $project.FullName -OutputDirectory "$solutionDir\artifacts" -Properties Configuration=Release
		& $nuget pack $project.FullName -OutputDirectory "$solutionDir\artifacts" -Properties Configuration=Release -Symbols
	}
}

task Compile -depends Version { 
	Invoke-MsBuild $solutionFile -MSBuildProperties @{ Configuration = "Release" }
}

task Version -depends Clean {

	Set-VersionProperties $versions

}

task Clean { 
	
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}