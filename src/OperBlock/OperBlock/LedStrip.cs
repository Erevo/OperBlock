using System;
using System.Device;
using System.Drawing;
using System.Threading;
using Iot.Device.Ws28xx;

namespace OperBlock
{
    public class LedStrip
    {
        private long _updateTimer;

        public LedStrip(int ledsCount, Ws28xx ws28Xx)
        {
            LedsCount = ledsCount;
            Ws28xx = ws28Xx;
        }

        public int LedsCount { get; private set; }
        public Ws28xx Ws28xx { get; private set; }

        public int Center => LedsCount / 2;

        public void ToggleHalf(bool left, Color color)
        {
            var img = Ws28xx.Image;
            if (left)
            {
                for (var i = 0; i < Center; i++)
                {
                    img.SetPixel(i, 0, color);
                }
            }
            else
            {
                for (var i = Center; i < LedsCount; i++)
                {
                    img.SetPixel(i, 0, color);
                }
            }
        }

        public void Update()
        {
            //var millis = Environment.TickCount64;

            //if (_updateTimer + 10 > millis)
            //{
            //    return;
            //}
            //_updateTimer = millis;
            Ws28xx.Update();
        }
    }
}