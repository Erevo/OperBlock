using System;
using System.Collections;
using System.Device;

namespace OperBlock.Modes
{
    public class SimpleOperMode : IOperMode
    {
        public Lamp[] Lamps { get; private set; }

        public SimpleOperMode(Lamp[] lamps)
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
                lamp.SetBrightness(1f);
            }

            DelayHelper.DelayMilliseconds(200, true);

            foreach (var lamp in Lamps)
            {
                lamp.SetBrightness(0f);
            }

            DelayHelper.DelayMilliseconds(200, true);
        }
    }
}