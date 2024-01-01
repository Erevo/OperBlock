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

        public override void Start()
        {
            _button.ButtonDown += ButtonOnButtonDown;
            _button.ButtonUp += ButtonOnButtonUp;

            foreach (var lamp in Lamps)
            {
                lamp.ToggleOn(0f);
            }
        }

        public override void Stop()
        {
            _button.ButtonDown -= ButtonOnButtonDown;
            _button.ButtonUp -= ButtonOnButtonUp;

            base.Stop();
        }

        private void ButtonOnButtonDown(object sender, EventArgs e)
        {
            foreach (var lamp in Lamps)
            {
                lamp.SetBrightness(1f);
            }
        }

        private void ButtonOnButtonUp(object sender, EventArgs e)
        {
            foreach (var lamp in Lamps)
            {
                lamp.SetBrightness(0f);
            }
        }
    }
}