param ($target, $build = $null)

if (!$build) {
	$build = $Env:APPVEYOR_BUILD_NUMBER 
}

if (!$build) {
	Write-Host -f Red "No build number provided"
	exit
}

Write-Host "PSake target: $target"
Write-Host "Build number: $build"

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\modules\psake\psake.psm1"

Invoke-Psake "$PSScriptRoot\Build.ps1" -Parameters @{ "target" = $target; "buildNumber" = $build }  