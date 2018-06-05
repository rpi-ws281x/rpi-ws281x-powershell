using System;
using System.Runtime.InteropServices;

namespace WS281x.Native
{
	[StructLayout(LayoutKind.Sequential)]
	internal struct ws2811_t
	{
		public long render_wait_time;
		public IntPtr device;
		public IntPtr rpi_hw;
		public uint freq;
		public int dmanum;
		// [MarshalAs(UnmanagedType.ByValArray, ArraySubType = UnmanagedType.CustomMarshaler, SizeConst = PInvoke.RPI_PWM_CHANNELS)]
		// public ws2811_channel_t[] channel;
		public ws2811_channel_t channel1;
		public ws2811_channel_t channel2;
	}
}
