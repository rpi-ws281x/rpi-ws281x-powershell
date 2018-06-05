using System;
using System.Drawing;
using System.Management.Automation;  // PowerShell namespace.
using System.Threading;

namespace WS281x.CmdLets
{
	
	[Cmdlet(VerbsCommon.Set, "BreathingLed")]
	public class BreathingLed : Cmdlet
	{
		// [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		// public Color Color {get; set;}

		// [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		// public int LedId {get; set;}

		[Parameter(Mandatory = false)]
		public SwitchParameter Invert {get;set;}

		[Parameter(Mandatory = false)]
		public int GpioPin {get; set;}
		//private WS281x _Controller;

		private int _brightness {get;set;}

		public BreathingLed()
		{
			Invert = false;
			GpioPin = 18;
			
		}

		protected override void BeginProcessing()
		{
			// Settings settings = Settings.CreateDefaultSettings();
            // settings.Channels[0] = new Channel(30, GpioPin, _brightness, Invert, StripType.WS2812_STRIP);
			//WS281x _Controller = new WS281x(settings);
		}

		protected override void	ProcessRecord()
		{
			Settings settings = Settings.CreateDefaultSettings();
            //settings.Channels[0] = new Channel(30, GpioPin, _brightness, Invert, StripType.WS2812_STRIP);
			bool breathingAsc = true;
			int i = 0;
			while(true)
			{
				settings.Channel = new Channel(30, GpioPin, (byte)_brightness, Invert, StripType.WS2812_STRIP);
				using(WS281x controller = new WS281x(settings))
				{
					//controller.SetLEDColor(this.LedId, this.Color);
					controller.SetLEDColor(0,Color.Green);
					controller.SetLEDColor(1,Color.Yellow);
					controller.SetLEDColor(2,Color.Red);
					controller.Render();
				}
				//Console.WriteLine($"Hello - {i}");
				//Console.WriteLine($"Current brightness - {_brightness}");

				if(_brightness >= 255)
				{
					_brightness = 255;
					breathingAsc = false;
				}
				else if (_brightness <= 0) {
					breathingAsc = true;
					_brightness = 0;
				}

				if(breathingAsc)
				{
					_brightness +=1;
				}
				else
				{
					_brightness -=1;	
				}
				i+=1;
				Console.WriteLine($"index = {i} / Brightness = {_brightness} / BreathingAsc = {breathingAsc}");
				//Thread.Sleep(50);
			}
		}

		// public void Dispose()
		// {
		// 	_Controller.Dispose();
		// }
	}
}