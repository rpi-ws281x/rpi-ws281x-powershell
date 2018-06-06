Import-Module ./../bin/Debug/netcoreapp2.0/linux-arm/publish/ws281xPowerShell.dll -Force 
$LedStripSettings = [PSCustomObject]@{
	NumberOfLeds = 21
	Brightness = 50
	Color = [System.Drawing.Color]::Green
	Speed = "Fast"
}

while($true)
{
	while((Get-GpioPin -Id 26 -PullMode PullUp).Value -eq 'High')
	{
		
	}
	$held = 0
	while((Get-GpioPin -Id 26 -PullMode PullUp).Value -eq 'Low' -AND $held -lt 10) #lt 10 because each time $held is incremented, 100ms have passed, after 10 iterations, 1 second passed
	{
		Start-Sleep -Milliseconds 100
		++$held
	}
	
	if($held -lt 10) #realeased before 1s
	{
		$LedStripSettings | Set-LedAccrossStrip
	}
	else # it was pressed at least during 1 second ( might still be being pressed)
	{
		Write-Host "Button pressed for at least 1 second. "
		Set-RainbowCycle -NumberOfLeds $LedStripSettings.NumberOfLeds -Brightness $LedStripSettings.Brightness -NumberOfCycles 1
		Start-Sleep -Milliseconds 200
	}
}