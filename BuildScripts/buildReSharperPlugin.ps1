param (
    [switch]$Verbose
)

If ($Verbose -eq $true) { Write-Host "Building R# plugin..." }
Push-Location -Path $backedPath
Write-Host "pushed location"
Try {
    .\build.ps1 pack
    $code = $LastExitCode
    If ($code -ne 0) { throw "Could not build ReSharper plugin: nuke exit code: $code." }
}
Finally {
    Pop-Location
}

If ($Verbose -eq $true) { Write-Host "`n---- R# plugin build finished. Binaries are at ForTea\Backend\output\Debug ----`n" }
