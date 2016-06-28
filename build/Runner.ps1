param ($target, $build = $null)

if (!$build) {
	$build = $Env:APPVEYOR_BUILD_NUMBER 
}

if (!$build) {
	Write-Host -f Red "No build number provided"
	exit
}

Write-Host "PSake target: $target" -F Gray
Write-Host "Build number: $build" -F Gray

$ErrorActionPreference = "Stop"

Import-Module "$PSScriptRoot\modules\psake\psake.psm1"

Invoke-Psake "$PSScriptRoot\Build.ps1" $target -Parameters @{ "buildNumber" = $build }

if ($psake.build_success -eq $false) {
	Write-Host "Build Failed"
	exit 1 
} else { 
	exit 0 
}