﻿using System;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using WS281x.Native;

namespace WS281x
{
	/// <summary>
	/// Wrapper class to controll WS281x LEDs
	/// </summary>
	public class WS281x : IDisposable
	{
		private ws2811_t _ws2811;
		private GCHandle _ws2811Handle;
		private bool _isDisposingAllowed;

		/// <summary>
		/// Initialize the wrapper
		/// </summary>
		/// <param name="settings">Settings used for initialization</param>
		public WS281x(Settings settings)
		{
			_ws2811 = new ws2811_t();
			_ws2811Handle = GCHandle.Alloc(_ws2811, GCHandleType.Pinned);

			_ws2811.dmanum	= settings.DMAChannel;
			_ws2811.freq	= settings.Frequency;
			_ws2811.channel_1 = default;
			InitChannel(ref _ws2811.channel_1, settings.Channel);

			Settings = settings;

			var initResult = PInvoke.ws2811_init(_ws2811Handle.AddrOfPinnedObject());
			if (initResult != ws2811_return_t.WS2811_SUCCESS)
			{
				var returnMessage = GetMessageForStatusCode(initResult);
				throw new Exception(String.Format("Error while initializing.{0}Error code: {1}{0}Message: {2}", Environment.NewLine, initResult.ToString(), returnMessage));
			}

			//Disposing is only allowed if the init was successfull.
			//Otherwise the native cleanUp function throws an error.
			_isDisposingAllowed = true;
		}

		/// <summary>
		/// Renders the content of the channels
		/// </summary>
		public void Render()
		{
			var ledColor = Settings.Channel.Leds.Select(x => x.RGBValue).ToArray();
			Marshal.Copy(ledColor, 0, _ws2811.channel_1.leds, ledColor.Count());
			var result = PInvoke.ws2811_render(_ws2811Handle.AddrOfPinnedObject());
			if (result != ws2811_return_t.WS2811_SUCCESS)
			{
				var returnMessage = GetMessageForStatusCode(result);
				throw new Exception(String.Format("Error while rendering.{0}Error code: {1}{0}Message: {2}", Environment.NewLine, result.ToString(), returnMessage));
			}
		}

		/// <summary>
		/// Sets the color of a given LED
		/// </summary>
		/// <param name="channelIndex">Channel which controls the LED</param>
		/// <param name="ledID">ID/Index of the LED</param>
		/// <param name="color">New color</param>
		public void SetLEDColor(int ledID, Color color)
		{
			Settings.Channel.Leds[ledID].Color = color;
		}

		public void SetColorOnAllLEDs(Color color)
		{
			for(int i = 0 ; i < Settings.Channel.Leds.Count ; ++i)
			{
				Settings.Channel.Leds[i].Color = color;
			}
		}

		/// <summary>
		/// Returns the settings which are used to initialize the component
		/// </summary>
		public Settings Settings { get; private set; }

		/// <summary>
		/// Initialize the channel properties.
		/// </summary>
		/// <param name="channel">Channel to initialize.</param>
		/// <param name="channelSettings">Settings for the channel.</param>
		private void InitChannel(ref ws2811_channel_t channel, Channel channelSettings)
		{
			channel.count = channelSettings.LEDCount;
			channel.gpionum = channelSettings.GPIOPin;
			channel.brightness = channelSettings.Brightness;
			channel.invert = Convert.ToInt32(channelSettings.Invert);

			if (channelSettings.StripType != StripType.Unknown)
			{
				channel.strip_type = (int)channelSettings.StripType;
			}
		}

		/// <summary>
		/// Returns the error message for the given status code
		/// </summary>
		/// <param name="statusCode">Status code to resolve</param>
		/// <returns></returns>
		private string GetMessageForStatusCode(ws2811_return_t statusCode)
		{
			var strPointer = PInvoke.ws2811_get_return_t_str((int)statusCode);
			return Marshal.PtrToStringAuto(strPointer);
		}

		private bool disposedValue = false; // To detect redundant calls

		protected virtual void Dispose(bool disposing)
		{
			if (!disposedValue)
			{
				if (disposing)
				{
					// TODO: dispose managed state (managed objects).
				}

				// TODO: free unmanaged resources (unmanaged objects) and override a finalizer below.
				// TODO: set large fields to null.

				if(_isDisposingAllowed)
				{
					PInvoke.ws2811_fini(_ws2811Handle.AddrOfPinnedObject());
					if(_ws2811Handle.IsAllocated)
					{
						_ws2811Handle.Free();
					}

					_isDisposingAllowed = false;
				}

				disposedValue = true;
			}
		}

		~WS281x()
		{
		// Do not change this code. Put cleanup code in Dispose(bool disposing) above.
			Dispose(false);
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}
	}
}
