using System;
using System.Collections;
using System.Device;

namespace OperBlock.Modes
{
    public class SimpleOperMode : OperModeBase
    {
        public SimpleOperMode(Lamp[] lamps) 
            : base(lamps)
        {
        }
        
        public override void Tick()
        {
            foreach (var lamp in Lamps)
            {
                lamp.SetBrightness(1f);
            }

            DelayHelper.DelayMilliseconds(100, true);

            foreach (var lamp in Lamps)
            {
                lamp.SetBrightness(0f);
            }

            DelayHelper.DelayMilliseconds(200, true);
        }
    }
}