using System;
using System.Device.Gpio;
using System.Diagnostics;
using Iot.Device.Button;

namespace OperBlock.Modes
{
    public class StockOperMode : OperModeBase
    {
        private GpioButton _button;

        public StockOperMode(Lamp[] lamps, GpioButton button)
            : base(lamps)
        {
            _button = button;
        }

        public override void Tick()
        {
            foreach (var lamp in Lamps)
            {
                lamp.SetBrightness(_button.IsPressed ? 1f : 0f);
            }
        }
    }
}