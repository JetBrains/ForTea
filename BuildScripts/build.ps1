param (
    [switch]$runFrontendTests,
    [switch]$run
)
if (Test-Path Env:DOTNET_SDK_DIRECTORY) 
{
    Set-Item -Path Env:PATH -Value "$($Env:DOTNET_SDK_DIRECTORY);$($Env:PATH)"
}
$baseDir = "${PSScriptRoot}\..\"
$backendPath = "${baseDir}\Backend"
$frontendPath = "${baseDir}\Frontend"

$gradleArgs = @()
If ($runFrontendTests -eq $true) {
    $gradleArgs += ":test"
}
If ($run -eq $true) {
    $mainWorkName = "Running IDE"
    $gradleArgs += ":runIde"
}
Else {
    $mainWorkName = "Building T4 Frontend"
    $gradleArgs += ":buildPlugin"
}
$gradleArgs += "--console=plain"

Write-Host "Preparing to build T4 plugin"
Push-Location -Path $frontendPath
Try {
    & "${baseDir}Frontend\gradlew.bat" :prepare --console=plain
    $code = $LastExitCode
    If ($code -ne 0) { throw "Could not prepare. Gradlew exit code: $code." }
}
Finally {
    Pop-Location
}

Write-Host "Restoring T4 backend packages"
Push-Location -Path $backendPath
Try {
    dotnet restore ForTea.Backend.sln --no-cache --force
    $code = $LastExitCode
    If ($code -ne 0) { throw "Could not restore packages in backend. Dotnet exit code: $code." }
}
Finally {
    Pop-Location
}

Write-Host "Building T4 backend"
Push-Location -Path $backendPath
Try {
    dotnet build ForTea.Backend.sln -c Release --no-restore --nologo --no-incremental
    $code = $LastExitCode
    If ($code -ne 0) { throw "Could not compile backend. MsBuild exit code: $code." }
}
Finally {
    Pop-Location
}

Write-Host $mainWorkName
Push-Location -Path $frontendPath
Try {
    & "${baseDir}Frontend\gradlew.bat" $gradleArgs
    $code = $LastExitCode
    If ($code -ne 0) { throw "Main gradle work failed. Gradlew exit code: $code." }
}
Finally {
    Pop-Location
}
