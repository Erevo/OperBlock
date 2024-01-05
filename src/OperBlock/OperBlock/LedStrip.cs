using System;
using System.Device;
using System.Drawing;
using System.Threading;
using Iot.Device.Ws28xx;

namespace OperBlock
{
    public class LedStrip
    {
        private bool _needUpdateFlag;
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
            var multiplayer = 0.1f;

            color = Color.FromArgb(
                (byte)(color.R * multiplayer),
                (byte)(color.G * multiplayer),
                (byte)(color.B * multiplayer)
                );
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

            Ws28xx.Update();
        }
    }
}