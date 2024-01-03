using System;
using System.Device;
using nanoFramework.EncButton.Core;

namespace OperBlock.Modes
{
    public class EnrageStockMode : OperModeBase
    {
        private bool _pressedFlag;
        
        private Button _button;

        public EnrageStockMode(Lamp[] lamps, Button button, TimeSpan delay)
            : base(lamps)
        {
            _button = button;
            Delay = delay;
        }

        public TimeSpan Delay { get; set; }

        public override void Tick()
        {
            if (true)
            //if (_button.IsPressed)
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