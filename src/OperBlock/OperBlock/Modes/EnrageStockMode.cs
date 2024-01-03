using System;
using System.Device;
using Iot.Device.Button;

namespace OperBlock.Modes
{
    public class EnrageStockMode : OperModeBase
    {
        private bool _pressedFlag;
        
        private GpioButton _button;

        public EnrageStockMode(Lamp[] lamps, GpioButton button, TimeSpan delay)
            : base(lamps)
        {
            _button = button;
            Delay = delay;
        }

        public TimeSpan Delay { get; set; }

        public override void Tick()
        {
            if (_button.IsPressed)
            {
                _pressedFlag = true;
                
                foreach (var lamp in Lamps)
                {
                    lamp.SetBrightness(1f);
                }

                DelayHelper.Delay(Delay, true);

                foreach (var lamp in Lamps)
                {
                    lamp.SetBrightness(0f);
                }

                DelayHelper.Delay(Delay, true);
            }
            else if(_pressedFlag)
            {
                _pressedFlag = false;
                
                foreach (var lamp in Lamps)
                {
                    lamp.SetBrightness(0f);
                }
            }
        }
    }
}