using System;
using System.Drawing;
using System.Management.Automation;  // PowerShell namespace.
namespace WS281x.CmdLets
{
	
	[Cmdlet(VerbsCommon.Set, "SingleLedColor")]
	public class SetSingleLedColor : Cmdlet , IDisposable
	{
		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 0)]
		public Color Color {get; set;}

		[Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position = 1)]
		public int LedId {get; set;}

		[Parameter(Mandatory = false)]
		public byte Brightness {get; set;}

		[Parameter(Mandatory = false)]
		public SwitchParameter Invert {get;set;}

		[Parameter(Mandatory = false)]
		public int GpioPin {get; set;}
		private WS281x _Controller;

		public SetSingleLedColor()
		{
			Invert = false;
			GpioPin = 18;
			
		}

		protected override void BeginProcessing()
		{
			Settings settings = Settings.CreateDefaultSettings();
            settings.Channel = new Channel(30, GpioPin, Brightness, Invert, StripType.WS2812_STRIP);
			_Controller = new WS281x(settings);
		}

		protected override void	ProcessRecord()
		{
			_Controller.SetLEDColor(this.LedId, this.Color);
			_Controller.Render();
		}

		public void Dispose()
		{
			_Controller.Dispose();
		}
	}
}