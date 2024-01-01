using System.Device;
using Iot.Device.Button;

namespace OperBlock.Modes
{
    public class EnrageStockMode : OperModeBase
    {
        private bool pressedFlag;
        
        private GpioButton _button;

        public EnrageStockMode(Lamp[] lamps, GpioButton button)
            : base(lamps)
        {
            _button = button;
        }

        public override void Start()
        {
            foreach (var lamp in Lamps)
            {
                lamp.ToggleOn(0f);
            }
        }

        public override void Tick()
        {
            if (_button.IsPressed)
            {
                pressedFlag = true;
                
                foreach (var lamp in Lamps)
                {
                    lamp.SetBrightness(1f);
                }

                DelayHelper.DelayMilliseconds(10, true);

                foreach (var lamp in Lamps)
                {
                    lamp.SetBrightness(0f);
                }

                DelayHelper.DelayMilliseconds(5, true);
            }
            else if(pressedFlag)
            {
                pressedFlag = false;
                
                foreach (var lamp in Lamps)
                {
                    lamp.SetBrightness(0f);
                }
            }
        }
    }
}