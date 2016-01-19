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

	nuget sources add -Name "api.nuget.org" -Source "https://api.nuget.org/v3/index.json"
	iex ((new-object net.webclient).DownloadString('https://raw.githubusercontent.com/aspnet/Home/dev/dnvminstall.ps1'))
	dnvm update-self
	dnvm upgrade
	dnvm install 1.0.0-rc1-final
	dnvm list
	dnvm use 1.0.0-rc1-final

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