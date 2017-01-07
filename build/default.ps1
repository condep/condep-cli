properties {
	$pwd = Split-Path $psake.build_script_file	
	$build_directory  = "$pwd\output\condep-cli"
	$configuration = "Release"
	$releaseNotes = ""
	$nuget = "$pwd\..\tools\nuget.exe"
}
 
include .\..\tools\psake_ext.ps1

function GetNugetAssemblyVersion($assemblyPath) {
    
    if(Test-Path Env:\APPVEYOR_BUILD_VERSION)
    {
        $appVeyorBuildVersion = $env:APPVEYOR_BUILD_VERSION
     
		# Getting the version number. Without the beta part, if its a beta package   
        $version = $appVeyorBuildVersion.Split('.')
        $major = $version[0] 
        $minor = $version[1] 
        $patch = $version[2].Split('-') | Select-Object -First 1

        # Setting beta postfix, if beta build. The beta number must be 5 digits, therefor this operation.
        $betaString = ""
        if($appVeyorBuildVersion.Contains("beta"))
        {
        	$buildNumber = $appVeyorBuildVersion.Split('-') | Select-Object -Last 1 | % {$_.replace("beta","")}
        	switch ($buildNumber.length) 
        	{	 
            	1 {$buildNumber = $buildNumber.Insert(0, '0').Insert(0, '0').Insert(0, '0').Insert(0, '0')} 
            	2 {$buildNumber = $buildNumber.Insert(0, '0').Insert(0, '0').Insert(0, '0')} 
            	3 {$buildNumber = $buildNumber.Insert(0, '0').Insert(0, '0')}
            	4 {$buildNumber = $buildNumber.Insert(0, '0')}                
            	default {$buildNumber = $buildNumber}
        	}
        	$betaString = "-beta$buildNumber" 
        }	
        return "$major.$minor.$patch$betaString"
    }
    else
    {
		#When building on local machine, set versionnumber from assembly info.
        $versionInfo = Get-Item $assemblyPath | % versioninfo
        return "$($versionInfo.FileVersion)"
    }
}

task default -depends Build-All, Pack-All
task ci -depends Build-All, Pack-All

task Build-All -depends Clean, ResotreNugetPackages, Build, Check-VersionExists, Create-BuildSpec-ConDep-Console
task Pack-All -depends Pack-ConDep-Console

task Check-VersionExists {
	$version = $(GetNugetAssemblyVersion $build_directory\ConDep.Console\ConDep.exe) 
	Exec { 
		$packages = & $nuget list "ConDep" -source "https://www.myget.org/F/condep/api/v3/index.json" -prerelease -allversions
		ForEach($package in $packages){
			$packageName = $package.Split(' ') | Select-Object -First 1
			if($packageName -eq "ConDep"){
				$packageVersionNumber = $package.Split(' ') | Select-Object -Last 1
				if($packageVersionNumber -eq $version){
					throw "ConDep $packageVersionNumber already exists on myget. Have you forgot to update version in appveyor.yml?"
				}
			}
		}
	}
}

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
			@{ Name="ConDep.Dsl"; Version="[5.0.1-beta00001,6)"},
			@{ Name="ConDep.Execution"; Version="[5.0.1-beta00001,6)"},
			@{ Name="ConDep.Dsl.Operations"; Version="[5.0.1-beta00001,6)"},
			@{ Name="ConDep.Dsl.Remote.Helpers"; Version="[5.0.1-beta00001,6)"},
			@{ Name="ConDep.Node"; Version="[5.0.1-beta00001,6)"},
			@{ Name="ConDep.WebQ.Client"; Version="[2.0.0]"},
			@{ Name="NDesk.Options"; Version="[0.2.1]"},
			@{ Name="SlowCheetah.Tasks.Unofficial"; Version="[1.0.0]"}
		) `
		-files @(
			@{ Path="ConDep.Console\ConDep.exe"; Target="lib/net45"}
		)
}

task Pack-ConDep-Console {
	Exec { & $nuget pack "$build_directory\condep.console.nuspec" -OutputDirectory "$build_directory" }
}