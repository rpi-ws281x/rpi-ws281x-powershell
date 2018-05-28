using System;
using System.Drawing;

namespace WS281x
{
    class Program
    {
        static void Main(string[] args)
        {
            int ledCount = 22;
            int gpioPin = 18;
            byte brightness = 255;
            bool invert = false;
            Settings settings = Settings.CreateDefaultSettings();
            settings.Channels[0] = new Channel(ledCount, gpioPin, brightness, invert, StripType.WS2812_STRIP);
            using (WS281x controller = new WS281x(settings))
			{
                controller.SetLEDColor(0, Color.Brown);
                controller.SetLEDColor(1,Color.DarkSeaGreen);
                controller.Render();
			}
        }
    }
}
