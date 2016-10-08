$appVeyorBuildVersion = $env:APPVEYOR_BUILD_VERSION

$version = $appVeyorBuildVersion.Split('-') | Select-Object -First 1
$buildNumber = $appVeyorBuildVersion.Split('-') | Select-Object -Last 1 | % {$_.replace("beta","")}

Set-AppveyorBuildVariable -Name "assembly_version" -Value "$version.$buildNumber"
Set-AppveyorBuildVariable -Name "assembly_file_version" -Value "$version.$buildNumber"
Set-AppveyorBuildVariable -Name "assembly_informational_version" -Value "$version.$buildNumber"