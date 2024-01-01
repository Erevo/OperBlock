using System;
using System.Collections;
using System.Device;

namespace OperBlock.Modes
{
    public class PairBlinkOperMode : OperModeBase
    {
        private bool _flag;

        public PairBlinkOperMode(Lamp[] lamps) 
            : base(lamps)
        {
        }

        public override void Tick()
        {
            for (var i = 0; i < Lamps.Length; i++)
            {
                if (i % 2 == 0)
                {
                    Lamps[i].SetBrightness(_flag ? 1f : 0);
                }
                else
                {
                    Lamps[i].SetBrightness(_flag ? 0f : 1f);
                }
            }

            _flag = !_flag;
            DelayHelper.DelayMilliseconds(100, true);
        }
    }
}