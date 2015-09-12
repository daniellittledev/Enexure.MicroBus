#Requires -Version 2.0
function Invoke-MsBuild
{
<#
	.SYNOPSIS
	Builds the given Visual Studio solution or project file using MSBuild.
	
	.DESCRIPTION
	Executes the MSBuild.exe tool against the specified Visual Studio solution or project file.
	Returns true if the build succeeded, false if the build failed, and null if we could not determine the build result.
	If using the PathThru switch, the process running MSBuild is returned instead.
	
	.PARAMETER Path
	The path of the Visual Studio solution or project to build (e.g. a .sln or .csproj file).
	
	.PARAMETER MsBuildParameters
	Additional parameters to pass to the MsBuild command-line tool. This can be any valid MsBuild command-line parameters except for the path of 
	the solution/project to build.
	
	See http://msdn.microsoft.com/en-ca/library/vstudio/ms164311.aspx for valid MsBuild command-line parameters.
	
	.OUTPUTS
	When the -PassThru switch is not provided, a boolean value is returned; $true indicates that MsBuild completed successfully, $false indicates 
	that MsBuild failed with errors (or that something else went wrong), and $null indicates that we were unable to determine if the build succeeded or failed.
	
	When the -PassThru switch is provided, the process being used to run the build is returned.
	
	.EXAMPLE
	$buildSucceeded = Invoke-MsBuild -Path "C:\Project\MySolution.sln"
	
	if ($buildSucceeded)
		Write-Host "Build completed successfully."
	} else  {
		Write-Host "Build failed. Check the build log file for errors."
	}
	
	Perform the default MSBuild actions on the Visual Studio solution to build the projects in it, and returns whether the build succeeded or failed.
	The PowerShell script will halt execution until MsBuild completes.
	
	.LINK
	Project home: http://icemedia.com.au
	
	.NOTES
	Name:   Invoke-MsBuild
	Author: Daniel Little, Very loosely based on Daniel Schroeder's version
	Version: 1.0
#>
	[CmdletBinding(DefaultParameterSetName="Wait")]
	param
	(
		[parameter(Position=0,Mandatory=$true,ValueFromPipeline=$true,HelpMessage="The path to the file to build with MsBuild (e.g. a .sln or .csproj file).")]
		[ValidateScript({Test-Path $_})]
		[string] $Path,

		[parameter(Mandatory=$false)]
		[Alias("MSBP")]
		$MSBuildProperties,
		
		[parameter(Mandatory=$false)]
		[Alias("MSBTargets")]
		[Alias("MSBT")]
		$MSBuildTargets
		
	)

	BEGIN { }
	END { }
	PROCESS
	{
		# Turn on Strict Mode to help catch syntax-related errors.
		# 	This must come after a script's/function's param section.
		# 	Forces a function to be the first non-comment code to appear in a PowerShell Script/Module.
		Set-StrictMode -Version Latest

		# Local Variables.
		$solutionFileName = (Get-ItemProperty -Path $Path).Name
		$buildCrashed = $false;


		# Get the path to the MsBuild executable.
		$msBuildPath = Get-MsBuildPath "14.0"
	
		$PropertiesArgument = ""
		if ($MSBuildProperties -ne $null) {
			$Pairs = ($MSBuildProperties.Keys | % { "$_=" + $MSBuildProperties.Item($_) }) -Join ";"
			$PropertiesArgument = "/property:$Pairs"
		}
	
		$TargetsArgument = ""
		if ($MSBuildTargets -ne $null) {
			$Values = $MSBuildTargets -Join ";"
			$TargetsArgument = "/Target:$Values"
		}
	
		# Build the arguments to pass to MsBuild.
		$buildArguments = """$Path"" $PropertiesArgument $TargetsArgument /verbosity:m"

		# Perform the build.
		& $msBuildPath $buildArguments

	}
}

function Get-MsBuildPath($Version)
{
<#
	.SYNOPSIS
	Gets the path to the latest version of MsBuild.exe. Returns $null if a path is not found.
	
	.DESCRIPTION
	Gets the path to the latest version of MsBuild.exe. Returns $null if a path is not found.
	
	.PARAMETER Version
	Tries to get the path for the specified version. If $null the current executing version is used.
#>

	if ($Version -eq $null) {
		$RuntimeEnvironment = [System.Runtime.InteropServices.RuntimeEnvironment]
		$RuntimeDirectory = $RuntimeEnvironment::GetRuntimeDirectory()
		return Join-Path $RuntimeDirectory "MsBuild.exe"
	}

	# Try to find an instance of that particular version in the registry
	$regKey = "HKLM:\SOFTWARE\Microsoft\MSBuild\ToolsVersions\${Version}"
	$itemProperty = Get-ItemProperty $RegKey -ErrorAction SilentlyContinue

	# If registry entry exists, then get the msbuild path and return 
	if ($itemProperty -ne $null -and $itemProperty.MSBuildToolsPath -ne $null) {
	
		# Get the path from the registry entry, and return it if it exists.
		$msBuildPath = Join-Path $itemProperty.MSBuildToolsPath -ChildPath "MsBuild.exe"
		if (Test-Path $msBuildPath) {
			return $msBuildPath
		}
	}

	# Return that we were not able to find MsBuild.exe.
	return $null
}

Export-ModuleMember -Function Invoke-MsBuild