using System;
using System.Collections;
using System.Device;
using System.Drawing;

namespace OperBlock.Modes
{
    public class PairBlinkOperMode : OperModeBase
    {
        private bool _lampsFlag;
        private bool _stripFlag;

        private long _lampsTimerMs;
        private long _stripTimerMs;

        private LedStrip? _ledStrip;

        public PairBlinkOperMode(Lamp[] lamps, LedStrip? ledStrip = null)
            : base(lamps)
        {
            _ledStrip = ledStrip;
        }

        public override void Tick()
        {
            var millis = Environment.TickCount64;

            if (_lampsTimerMs + 130 <= millis)
            {
                for (var i = 0; i < Lamps.Length; i++)
                {
                    if (i % 2 == 0)
                    {
                        Lamps[i].SetBrightness(_lampsFlag ? 1f : 0);
                    }
                    else
                    {
                        Lamps[i].SetBrightness(_lampsFlag ? 0f : 1f);
                    }
                }

                _lampsFlag = !_lampsFlag;

                _lampsTimerMs = millis;
            }

            if (_ledStrip != null)
            {
                if (_stripTimerMs + 100 <= millis)
                {
                    _ledStrip.ToggleHalf(_stripFlag, _stripFlag ? Color.Red : Color.Blue);
                    _ledStrip.ToggleHalf(!_stripFlag, Color.Black);

                    _ledStrip.Update();

                    _stripFlag = !_stripFlag;

                    _stripTimerMs = millis;
                }
            }
        }
    }
}