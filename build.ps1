Import-Module '.\tools\win-tools\psake\psake.psm1'
Invoke-psake .\build\default.ps1
if ($psake.build_success -eq $false) { exit 1 } else { exit 0 }
