properties {
	$pwd = Split-Path $psake.build_script_file	
	$build_directory  = "$pwd\output\condep-cli"
	$configuration = "Release"
	$preString = "-beta"
	$releaseNotes = ""
	$nuget = "$pwd\..\tools\nuget.exe"
}
 
include .\..\tools\psake_ext.ps1

function GetNugetAssemblyVersion($assemblyPath) {
	$versionInfo = Get-Item $assemblyPath | % versioninfo

	return "$($versionInfo.FileMajorPart).$($versionInfo.FileMinorPart).$($versionInfo.FileBuildPart)$preString"
}

task default -depends Build-All, Pack-All
task ci -depends Build-All, Pack-All

task Pack-All -depends Pack-ConDep-Console
task Build-All -depends Clean, ResotreNugetPackages, Build, Create-BuildSpec-ConDep-Console

task ResotreNugetPackages {
	Exec { & $nuget restore "$pwd\..\src\condep-cli.sln" }
}

task Build {
	Exec { msbuild "$pwd\..\src\condep-cli.sln" /t:Build /p:Configuration=$configuration /p:OutDir=$build_directory /p:GenerateProjectSpecificOutputFolder=true}
}

task Clean {
	Write-Host "Cleaning Build output"  -ForegroundColor Green
	Remove-Item $build_directory -Force -Recurse -ErrorAction SilentlyContinue
}

task Create-BuildSpec-ConDep-Console {
	Generate-Nuspec-File `
		-file "$build_directory\condep.console.nuspec" `
		-version $(GetNugetAssemblyVersion $build_directory\ConDep.Console\ConDep.exe) `
		-id "ConDep" `
		-title "ConDep" `
		-licenseUrl "http://www.condep.io/license/" `
		-projectUrl "http://www.condep.io/" `
		-description "ConDep is a highly extendable Domain Specific Language for Continuous Deployment, Continuous Delivery and Infrastructure as Code on Windows." `
		-iconUrl "https://raw.github.com/condep/ConDep/master/images/ConDepNugetLogo.png" `
		-releaseNotes "$releaseNotes" `
		-tags "Continuous Deployment Delivery Infrastructure WebDeploy Deploy msdeploy IIS automation powershell remote aws azure" `
		-dependencies @(
			@{ Name="ConDep.Dsl"; Version="[5.0.0-beta8,6)"},
			@{ Name="ConDep.Execution"; Version="[5.0.0-beta996,6)"},
			@{ Name="ConDep.Dsl.Operations"; Version="[5.0.0-beta2,6)"},
			@{ Name="ConDep.Dsl.Remote.Helpers"; Version="[3.1.0,4)"},
			@{ Name="ConDep.Node"; Version="[4.0.0,5)"},
			@{ Name="ConDep.WebQ.Client"; Version="[2.0.0,3)"},
			@{ Name="NDesk.Options"; Version="[0.2.1]"},
			@{ Name="SlowCheetah.Tasks.Unofficial"; Version="[1.0.0]"}
		) `
		-files @(
			@{ Path="ConDep.Console\ConDep.exe"; Target="lib/net40"}
		)
}

task Pack-ConDep-Console {
	Exec { & $nuget pack "$build_directory\condep.console.nuspec" -OutputDirectory "$build_directory" }
}