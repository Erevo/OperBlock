using System;
using System.Collections;
using System.Device;

namespace OperBlock.Modes
{
    public class ManualOperMode : IOperMode
    {
        public Lamp[] Lamps { get; private set; }

        public ManualOperMode(Lamp[] lamps)
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
                lamp.SetBrightness(0);
                lamp.ToggleOff();
            }
        }

        public void Tick()
        {
            foreach (var lamp in Lamps)
            {
                lamp.SetBrightness( lamp.Brightness > 0 ? 0f : 1f);
            }

            DelayHelper.DelayMilliseconds(12, false);
        }
    }
}