# Script to run that will copy the needed dependency into /usr/bin
if (-not (Test-Path "/usr/bin/ws2811.so")) {
    Write-Host "copying 'ws2811.so' to '/usr/bin' for initial setup."
    Copy-Item "$PSScriptRoot/ws2811.so" /usr/bin
}