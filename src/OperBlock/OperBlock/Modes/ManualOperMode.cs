using System;
using System.Collections;
using System.Device;

namespace OperBlock.Modes
{
    public class ManualOperMode : OperModeBase
    {
        public ManualOperMode(Lamp[] lamps) 
            : base(lamps)
        {
        }

        public override void Tick()
        {
            foreach (var lamp in Lamps)
            {
                lamp.SetBrightness(lamp.Brightness > 0 ? 0f : 1f);
            }

            DelayHelper.DelayMilliseconds(12, false);
        }
    }
}