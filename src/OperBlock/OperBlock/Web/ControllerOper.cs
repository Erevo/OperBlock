using System;
using System.Device.Gpio;
using System.Net;
using nanoFramework.WebServer;

namespace OperBlock.Web
{
    class ControllerOper
    {
        public static void ToggleLamp(string lampIndex)
        {
            if (!int.TryParse(lampIndex, out var index))
            {
                return;
            }

            var lamps = Program.Lamps;
            if (index > lamps.Length)
            {
                return;
            }

            var lamp = Program.Lamps[index];
            if (lamp.Enabled || lamp.Brightness > 0f)
            {
                lamp.ToggleOff();
            }
            else
            {
                lamp.ToggleOn();
            }
        }
        
        public static void SetMaxBrightness(string lampIndex, string lampMaxBrightness)
        {
            if (!int.TryParse(lampIndex, out var index))
            {
                return;
            }

            if (!float.TryParse(lampMaxBrightness, out var maxBrightness))
            {
                return;
            }

            maxBrightness /= 100f;

            var lamps = Program.Lamps;
            if (index > lamps.Length)
            {
                return;
            }

            var lamp = Program.Lamps[index];
            lamp.MaxBrightness = maxBrightness;
            lamp.SetBrightness(maxBrightness);
        }
    }
}