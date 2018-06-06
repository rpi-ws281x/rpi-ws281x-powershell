using System;
using System.Collections.Generic;
using System.Drawing;
using System.Management.Automation;  // PowerShell namespace.
using System.Threading;

namespace WS281x.CmdLets
{
	
	[Cmdlet(VerbsCommon.Set, "RainbowCycle")]
	public class RainbowCycle : Cmdlet
	{
		// [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		// public Color Color {get; set;}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public int NumberOfLeds {get; set;}
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		public byte Brightness { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Invert { get; set; }
		
		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 2)]
		public int NumberOfCycles { get; set ;}

		public int GpioPin {get; set;}

		private bool shouldAbort;
		
		//private WS281x _controller;

		public RainbowCycle()
		{
			Invert = false;
			GpioPin = 18;
			
		}

		protected override void BeginProcessing()
		{

		}

		protected override void	ProcessRecord()
		{
			if(NumberOfCycles == 0)
			{
				NumberOfCycles = 1;
			}

			Settings settings = Settings.CreateDefaultSettings();
            settings.Channel = new Channel(NumberOfLeds, GpioPin, Brightness, Invert, StripType.WS2812_STRIP);
			WS281x controller = new WS281x(settings);
			List<Color> colors = GetColors();

			for(int iterations = 0 ; iterations < NumberOfCycles ; ++iterations)
			{
				for(int colorCycle = 0; colorCycle < colors.Count; ++colorCycle)
				{
					Color currentColor = colors[colorCycle];
					for(int i = 0 ; i < NumberOfLeds ; ++i)
					{
						//Iterate over all LEDs and display the current color
						controller.SetLEDColor(i,currentColor);
						controller.Render();
						Thread.Sleep(25);	
					}
				}
			}
		}

		private List<Color> GetColors() => new List<Color>()
		{
			Color.DarkRed,
			Color.Red,
			Color.Orange,
			Color.Yellow,
			Color.Green,
			Color.Lime,
			Color.Cyan,
			Color.Blue
		};
	}
}