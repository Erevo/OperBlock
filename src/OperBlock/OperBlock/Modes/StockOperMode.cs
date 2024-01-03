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
            foreach (var lamp in Lamps)
            {
                //lamp.SetBrightness(_button.IsPressed ? 1f : 0f);
                lamp.SetBrightness(true ? 1f : 0f);
            }
        }
    }
}