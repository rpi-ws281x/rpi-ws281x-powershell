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
		public SwitchParameter Invert {get;set;}

		[Parameter(Mandatory = false)]
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
			Settings settings = Settings.CreateDefaultSettings();
            settings.Channel = new Channel(NumberOfLeds, GpioPin, Brightness, Invert, StripType.WS2812_STRIP);
			WS281x controller = new WS281x(settings);
			List<Color> colors = GetColors();
			int currentColorIndex = 0;
			while(!shouldAbort)
			{
				Color currentColor = colors[currentColorIndex++];
				for(int i = 0 ; i < NumberOfLeds ; ++i)
				{
					//Iterate over all LEDs and display the current color
					controller.SetLEDColor(i,currentColor);
					controller.Render();
					Thread.Sleep(25);
					
				}
				if(currentColorIndex >= colors.Count)
				{
					currentColorIndex = 0;
				}				
			}
		}

		private List<Color> GetColors() => new List<Color>()
		{
			Color.FromArgb(0x201000),
			Color.FromArgb(0x202000),
			Color.Green,
			Color.FromArgb(0x002020),
			Color.Blue,
			Color.FromArgb(0x100010),
			Color.FromArgb(0x200010)
		};
	}
}