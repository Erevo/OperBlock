using System;
using System.Device.Gpio;
using System.Diagnostics;
using nanoFramework.EncButton.Core;

namespace OperBlock.Modes
{
    public class StockOperMode : OperModeBase
    {
        private Button _button;

        public StockOperMode(Lamp[] lamps, Button button)
            : base(lamps)
        {
            _button = button;
        }

        public override void Tick()
        {
            var btnState = _button.ReadRaw();

            foreach (var lamp in Lamps)
            {
                lamp.SetBrightness(btnState ? 1f : 0f);
            }
        }
    }
}