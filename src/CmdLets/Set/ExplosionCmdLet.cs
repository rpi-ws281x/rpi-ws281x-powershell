using System;
using System.Collections.Generic;
using System.Drawing;
using System.Management.Automation;  // PowerShell namespace.
using System.Threading;

namespace WS281x.CmdLets
{

	[Cmdlet(VerbsCommon.Set, "Explosion")]
	public class Explosion : Cmdlet
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public int NumberOfLeds {get; set;}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		public byte Brightness { get; set; }

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 2)]
		public Color LeftSideColor { get; set; }

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 3)]
		public Color RightSideColor { get; set; }

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 4)]
		public Color ExplosionColor { get; set; }

		[Parameter(Mandatory = false, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 5)]
		[ValidateSet("Slow", "Medium","Fast")]
		public string Speed { get; set; }

		[Parameter(Mandatory = false)]
		public SwitchParameter Invert {get;set;}

		[Parameter(Mandatory = false)]
		public int GpioPin {get; set;}

		//* SPEED!!!!
		private Dictionary<string,int> _speedTranslation;

		public Explosion()
		{
			Invert = false;
			GpioPin = 18;

		}

		protected override void BeginProcessing()
		{
			_speedTranslation = new Dictionary<string,int>()
			{
				{"Slow", 500},
				{"Medium", 250},
				{"Fast", 50},
			};
		}

		protected override void	ProcessRecord()
		{
			if(ExplosionColor == Color.Empty)
			{
				ExplosionColor = Color.OrangeRed;
			}
			if(string.IsNullOrEmpty(Speed))
				Speed = "Medium";

			Settings settings = Settings.CreateDefaultSettings();
            settings.Channel = new Channel(NumberOfLeds, GpioPin, Brightness, Invert, StripType.WS2811_STRIP_GRB);
			WS281x controller = new WS281x(settings);

			int leftSideIterations = NumberOfLeds / 2;

			// If it's even, both sides will have the same amount of iterations, otherwise make right side have one more
			int rightSideIterations = (NumberOfLeds % 2 == 0) ? leftSideIterations : leftSideIterations+1;
			int totalIterations = 0, leftSide = 0, rightSide = NumberOfLeds-1;
			for(; totalIterations < rightSideIterations ; ++totalIterations)
			{

				controller.SetLEDColor(leftSide++,LeftSideColor);
				controller.SetLEDColor(rightSide--,RightSideColor);
				controller.Render();
				Thread.Sleep(_speedTranslation[Speed]);
			}

			for(; totalIterations >= 0 ; --totalIterations)
			{
				controller.SetLEDColor(leftSide--,ExplosionColor);
				controller.SetLEDColor(rightSide++,ExplosionColor);
				controller.Render();
				Thread.Sleep(10);
			}

			//Thread.Sleep(_speedTranslation[Speed]);

			//controller.SetColorOnAllLEDs(ExplosionColor);
			//controller.Render();
			//for some reason it has to be explicitly disposed
			controller.Dispose();
		}
	}
}
