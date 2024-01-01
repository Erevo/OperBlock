using System;
using System.Collections;
using System.Device;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using nanoFramework.Hardware.Esp32;
using OperBlock.Modes;

namespace OperBlock
{
    public class Program
    {
        private static readonly byte[] LampsPins = { 0, 2 };

        private static Lamp[] _lamps = new[]
        {
            new Lamp(PwmChannel.CreateFromPin(LampsPins[0], 40000, 1f)),
            new Lamp(PwmChannel.CreateFromPin(LampsPins[1], 40000, 1f)),
        };

        public static IOperMode[] OperModes { get; } = new IOperMode[]
        {
            new ManualOperMode(_lamps),
            new PairBlinkOperMode(_lamps),
            new SimpleOperMode(_lamps),
        };

        public static IOperMode CurrentOperMode { get; set; } = OperModes[0];

        public static void Main()
        {
            Configuration.SetPinFunction(LampsPins[0], DeviceFunction.PWM1);
            Configuration.SetPinFunction(LampsPins[1], DeviceFunction.PWM2);

            Debug.WriteLine("Hello from OperBlock!");

            SetOperMode(OperModes[0]);

            new Thread(() =>
            {
                while (true)
                {
                    CurrentOperMode.Tick();
                }
            }).Start();

            while (true)
            {
                foreach (var operMode in OperModes)
                {
                    DelayHelper.DelayMilliseconds(1000, true);
                    SetOperMode(operMode);
                }
            }

            Thread.Sleep(Timeout.Infinite);
        }

        public static void SetOperMode(IOperMode operMode)
        {
            CurrentOperMode.Stop();
            CurrentOperMode = operMode;
            CurrentOperMode.Start();
        }
    }
}