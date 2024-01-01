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
    }
}