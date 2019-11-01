param (
    [switch]$runFrontendTests,
    [switch]$run,
    [switch]$Verbose
)

$backendPath = "..\Backend"
$frontendPath = "..\Frontend"

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
        .\gradlew :prepare --console=plain
        $code = $LastExitCode
    }
    Else {
        .\gradlew :prepare > $null --quiet --console=plain
        $code = $LastExitCode
    }
    If ($code -ne 0) { throw "Could not prepare. Gradlew exit code: $code." }
}
Finally {
    Pop-Location
}

Write-Host "Building T4 backend"
Push-Location -Path $backendPath
Try {
    If ($Verbose -eq $true) {
        dotnet msbuild -m ForTea.Backend.sln
        $code = $LastExitCode
    }
    Else {
        dotnet msbuild -m ForTea.Backend.sln > $null
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
        .\gradlew $gradleArgs
        $code = $LastExitCode
    }
    Else {
        .\gradlew $gradleArgs 2>&1> $null
        $code = $LastExitCode
    }
    If ($code -ne 0) { throw "Main gradle work failed. Gradlew exit code: $code." }
}
Finally {
    Pop-Location
}

If ($Verbose -eq $true) { Write-Host "`n---- Rider plugin build finished. Binaries are at ForTea\Frontend\build\distributions ----`n" }
