#Setting version number in assemblies when building on appveyor
$appVeyorBuildVersion = $env:APPVEYOR_BUILD_VERSION
$version = $appVeyorBuildVersion | % {$_.replace("-beta",".")}
Set-AppveyorBuildVariable -Name "assembly_version" -Value "$version"
Set-AppveyorBuildVariable -Name "assembly_file_version" -Value "$version"
Set-AppveyorBuildVariable -Name "assembly_informational_version" -Value "$version"