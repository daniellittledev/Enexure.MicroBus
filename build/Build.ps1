Import-Module "$PSScriptRoot\modules\msbuild\Invoke-MsBuild.psm1"

properties {
	$solutionName = "Enexure.MicroBus"
	$configuration = "Release"

	$solutionDir = Resolve-Path "$PSScriptRoot\.."
	$solutionFile = "$solutionDir\$solutionName.sln"
	$nuget = "$PSScriptRoot\nuget.exe"
}

task default -depends Fail 

task Fail {
	throw "Try calling Test or Package"
}

task Compile { 

	Write-Host "Compiling"
	Write-Host "|-----------------------------------------"

	$projects = ls "$solutionDir\src"

	foreach($project in $projects) {

		Write-Host "Building $($project.FullName)" -F Cyan
		exec { dotnet build $project.FullName --configuration $configuration }
	}
}

task Test { 

	Write-Host "Testing"
	Write-Host "|-----------------------------------------"

	$projects = ls "$solutionDir\src" | ? { $_.Name -like "*Tests" }

	foreach($project in $projects) {

		Write-Host "Running tests for $project" -F Cyan
		exec { dotnet test "$($project.FullName)" }

	}
}

task Package -depends Compile { 

	Write-Host "Packaging"

	$projects = ls "$solutionDir\src" | ? { -not ($_.Name -like "*Tests") }

	foreach($project in $projects) {

	Write-Host "Packing $project" -F Cyan
		exec { dotnet pack $project.FullName --configuration $configuration }
	}
}

task Clean { 
	
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}