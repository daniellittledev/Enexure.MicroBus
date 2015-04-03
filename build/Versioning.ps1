
function Get-VersionString($version) {
	return "$($version.Major).$($version.Minor).$($version.Patch).$($version.Revision)"
}

function Get-Versions($solutionRoot, $buildNumber) {

	$projectVersionFiles = Get-ChildItem -Path $solutionRoot -Filter "version.json" -Recurse
	
	$projectVersionFilesContents = $projectVersionFiles | % { @{ 
		Project = (Split-Path $_.FullName); 
		Version = [string](Get-Content $_.FullName) } }
		
	$versionDetail = $projectVersionFilesContents | % { @{ 
		Project = $_.Project; 
		Version = ConvertFrom-Json $_.Version } }
	
	foreach ($version in $versionDetail) {
		$ver = $version.Version
		if ($ver.Revision -eq "*") {
			$ver.Revision = $buildNumber
		}
	}
	
	return $versionDetail
}

function Set-VersionProperties($versions) {

	foreach ($version in $versions) {

		$projectDir = "$($version.Project)\Properties"
		$versionFile = "$projectDir\AssemblyVersion.cs"
		
		$versionString = Get-VersionString($version.Version)
		
		$versionFileContents = 
		"using System.Reflection;" + "`r`n" +
		"[assembly: AssemblyVersion(`"$versionString`")]" + "`r`n" +
		"[assembly: AssemblyFileVersion(`"$versionString`")]"

		Set-Content $versionFile $versionFileContents
	}
}
