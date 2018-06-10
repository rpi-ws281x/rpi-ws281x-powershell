# rpi-ws281x-powershell

This project is based on the [rpi-ws281x-csharp](https://github.com/rpi-ws281x/rpi-ws281x-csharp) and allows the usage of PowerShell Core to interact with this type of LEDs!

Since this wraps another library (a C library - ws2811.so), we need to set this .so file where the `P/Invoke` search. In order to do so:

1 - clone this repo

2 - `import-module ./rpi-ws281x-powershell/bin/Debug/netcoreapp2.0/linux-arm/publish/ws281x.psd1`


# New to PowerShell? 

[Check here how to install it](https://github.com/PowerShell/PowerShell/tree/master/docs/installation/linux.md#raspbian)

# Examples

This will properly documented in a near future.

In the folder that you've just cloned, enter on PowerShell with `sudo pwsh`

## Explosion
```
Import-Module ./bin/Debug/netcoreapp2.0/linux-arm/publish/ws281xPowerShell.dll
$explosionSettings = [PSCustomObject]@{
	NumberOfLeds = 21
	Brightness = 5
	LeftSideColor = [System.Drawing.Color]::Blue
	RightSideColor = [System.Drawing.Color]::Green
	Speed = "Slow"
}
$explosionSettings | Set-Explosion
```