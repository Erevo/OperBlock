using System;
using System.Collections;
using System.Device;

namespace OperBlock.Modes
{
    public class PairBlinkOperMode : IOperMode
    {
        private bool _flag;

        public Lamp[] Lamps { get; private set; }

        public PairBlinkOperMode(Lamp[] lamps)
        {
            Lamps = lamps;
        }

        public void Start()
        {
            foreach (var lamp in Lamps)
            {
                lamp.ToggleOn();
            }
        }

        public void Stop()
        {
            foreach (var lamp in Lamps)
            {
                lamp.ToggleOff();
            }
        }

        public void Tick()
        {
            for (int i = 0; i < Lamps.Length; i++)
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