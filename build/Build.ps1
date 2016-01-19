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

task Package -depends Test { 

	$projects = ls "$solutionDir\src" | ? { -not ($_.Name -like "*Tests") }

	foreach($project in $projects) {

		dnu pack $project.FullName --configuration $configuration
	}
}

task Test -depends Compile { 

	$projects = ls "$solutionDir\src" | ? { $_.Name -like "*Tests" }

	foreach($project in $projects) {

		ls "$($project.FullName)\bin\$configuration\net45" | ? { $_.Name -like "*Tests.dll" } | % { & $nunit $_.FullName }
	}
}

task Compile { 

	dnu restore

	$projects = ls "$solutionDir\src"

	foreach($project in $projects) {

		dnu build $project.FullName --configuration $configuration
	}
}

task Clean { 
	
}

task ? -Description "Helper to display task info" {
	Write-Documentation
}