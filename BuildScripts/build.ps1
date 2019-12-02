﻿ param (
    [switch]$runFrontendTests,
    [switch]$run,
    [switch]$Verbose
)
if (Test-Path Env:DOTNET_SDK_DIRECTORY) 
{
    Set-Item -Path Env:PATH -Value "$($Env:DOTNET_SDK_DIRECTORY);$($Env:PATH)"
}
$baseDir = "${PSScriptRoot}\..\"
$backendPath = "${baseDir}\Backend"
$frontendPath = "${baseDir}\Frontend"

$gradleArgs = @()
If ($Verbose -eq $true) { Write-Host "Will build rider plugin" }
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
If ($Verbose -eq $true) {
    $gradleArgs += "--info"
    $gradleArgs += "--stacktrace"
    Write-Host "gradlew args = $gradleArgs"
}
Else {
    $gradleArgs += "--quiet"
    $gradleArgs += "--console=plain"
}

Write-Host "Preparing to build T4 plugin"
Push-Location -Path $frontendPath
Try {
    If ($Verbose -eq $true) {
        & "${baseDir}Frontend\gradlew.bat" :prepare --console=plain
        $code = $LastExitCode
    }
    Else {
        & "${baseDir}Frontend\gradlew.bat" :prepare > $null --quiet --console=plain
        $code = $LastExitCode
    }
    If ($code -ne 0) { throw "Could not prepare. Gradlew exit code: $code." }
}
Finally {
    Pop-Location
}

Write-Host "Restoring T4 backend packages"
Push-Location -Path $backendPath
Try {
    If ($Verbose -eq $true) {
        dotnet restore ForTea.Backend.sln --no-cache --force        
        $code = $LastExitCode
    }
    Else {
        dotnet restore ForTea.Backend.sln --no-cache --force        
        $code = $LastExitCode
    }
    If ($code -ne 0) { throw "Could not compile backend. MsBuild exit code: $code." }    
}
Finally {
    Pop-Location
}

Write-Host "Building T4 backend"
Push-Location -Path $backendPath
Try {
    If ($Verbose -eq $true) {        
        dotnet build ForTea.Backend.sln -c Release --no-restore --nologo --no-incremental        
        $code = $LastExitCode
    }
    Else {        
        dotnet build ForTea.Backend.sln -c Release --no-restore --nologo --no-incremental        
        $code = $LastExitCode
    }
    If ($code -ne 0) { throw "Could not compile backend. MsBuild exit code: $code." }    
}
Finally {
    Pop-Location
}


Write-Host $mainWorkName
Push-Location -Path $frontendPath
Try {
    If ($Verbose -eq $true) {
        & "${baseDir}Frontend\gradlew.bat" "$gradleArgs"
        $code = $LastExitCode
    }
    Else {
        & "${baseDir}Frontend\gradlew.bat" $gradleArgs 2>&1> $null
        $code = $LastExitCode
    }
    If ($code -ne 0) { throw "Main gradle work failed. Gradlew exit code: $code." }
}
Finally {
    Pop-Location
}

If ($Verbose -eq $true) { Write-Host "`n---- Rider plugin build finished. Binaries are at ForTea\Frontend\build\distributions ----`n" }
 
