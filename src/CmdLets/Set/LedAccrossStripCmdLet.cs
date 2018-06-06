using System;
using System.Collections.Generic;
using System.Drawing;
using System.Management.Automation;  // PowerShell namespace.
using System.Threading;

namespace WS281x.CmdLets
{
	
	[Cmdlet(VerbsCommon.Set, "LedAccrossStrip")]
	public class LedAccrossStrip : Cmdlet
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public int NumberOfLeds {get; set;}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		public byte Brightness { get; set; }

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 2)]
		public Color Color { get; set; }

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 5)]
		[ValidateSet("Slow", "Medium","Fast")]
		public string Speed { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Invert {get;set;}

		public int GpioPin {get; set;}

		//* SPEED!!!!
		private Dictionary<string,int> _speedTranslation;

		public LedAccrossStrip()
		{
			Invert = false;
			GpioPin = 18;
			
		}

		protected override void BeginProcessing()
		{
			_speedTranslation = new Dictionary<string,int>()
			{
				{"Slow", 300},
				{"Medium", 125},
				{"Fast", 50},
			};
		}

		protected override void	ProcessRecord()
		{
			Settings settings = Settings.CreateDefaultSettings();
            settings.Channel = new Channel(NumberOfLeds, GpioPin, Brightness, Invert, StripType.WS2812_STRIP);
			WS281x controller = new WS281x(settings);

			
			for(int i = 0 ; i < NumberOfLeds ; ++i)
			{
				for(int j = 0 ; j < i ; ++j)
				{
					controller.SetLEDColor(j, Color.Empty);
				}
				controller.SetLEDColor(i,Color);	
				controller.Render();
				Thread.Sleep(_speedTranslation[Speed]);
			}
			//"hack". turn the last led of 
			controller.SetLEDColor(NumberOfLeds-1,Color.Empty);
			controller.Render();
			//for some reason it has to be explicitly disposed
			controller.Dispose();
		}
	}
}