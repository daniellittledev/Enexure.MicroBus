Import-Module "$PSScriptRoot\modules\msbuild\Invoke-MsBuild.psm1"

properties {
	$solutionName = "Enexure.MicroBus"
	$configuration = "Release"

	$solutionDir = Resolve-Path "$PSScriptRoot\.."
	$solutionFile = "$solutionDir\$solutionName.sln"
	$nuget = "$PSScriptRoot\nuget.exe"
	$nunit = "$PSScriptRoot\nunit\nunit-console.exe"
}

task default -depends Package

task Compile { 

	dnvm use 1.0.0-rc1-update1 -r clr

	Write-Host "Compiling"
	Write-Host "|-----------------------------------------"

	Write-Host "Running dnu restore" -F Cyan
	dnu restore $solutionDir

	$projects = ls "$solutionDir\src"

	foreach($project in $projects) {

		Write-Host "Building $($project.FullName)" -F Cyan
		exec { dnu build $project.FullName --configuration $configuration }
	}
}

task Test -depends Compile { 

	dnvm use 1.0.0-rc1-final -r coreclr

	Write-Host "Testing"
	Write-Host "|-----------------------------------------"

	$projects = ls "$solutionDir\src" | ? { $_.Name -like "*Tests" }

	foreach($project in $projects) {

		Write-Host "Running tests for $project" -F Cyan
		dnx -p "$($project.FullName)" test

	}
}

task Package -depends Test { 

	dnvm use 1.0.0-rc1-update1 -r clr

	Write-Host "Packaging"

	$projects = ls "$solutionDir\src" | ? { -not ($_.Name -like "*Tests") }

	foreach($project in $projects) {

	Write-Host "Packing $project" -F Cyan
		dnu pack $project.FullName --configuration $configuration
	}
}

task Clean { 
	
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}