using System;
using System.Collections;
using System.Device;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using Iot.Device.Button;
using nanoFramework.Hardware.Esp32;
using OperBlock.Modes;

namespace OperBlock
{
    public class Program
    {
        private static readonly byte[] LampsPins = { 4, 2 };

        private static GpioButton? _button;

        private static Lamp[] _lamps = new[]
        {
            new Lamp(PwmChannel.CreateFromPin(LampsPins[0], 40000, 0f)),
            new Lamp(PwmChannel.CreateFromPin(LampsPins[1], 40000, 0f)),
        };

        private static IOperMode[] OperModes { get; set; } = new IOperMode[0];

        private static IOperMode? _currentOperMode;

        public static void Main()
        {
            _button = new GpioButton(0, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(2000));

            _button.IsHoldingEnabled = true;
            _button.IsDoublePressEnabled = true;

            OperModes = new IOperMode[]
            {
                new StockOperMode(_lamps, _button),
                new EnrageStockMode(_lamps, _button),
                new ManualOperMode(_lamps),
                new PairBlinkOperMode(_lamps),
                new SimpleOperMode(_lamps),
            };

            Configuration.SetPinFunction(LampsPins[0], DeviceFunction.PWM1);
            Configuration.SetPinFunction(LampsPins[1], DeviceFunction.PWM2);

            Debug.WriteLine("Hello from OperBlock!");

            SetOperMode(OperModes[1]);

            new Thread(() =>
            {
                while (true)
                {
                    _currentOperMode?.Tick();
                }
            }).Start();

            _button.Holding += ButtonOnHolding;
            _button.DoublePress += ButtonOnDoublePress;
            
            Thread.Sleep(Timeout.Infinite);
        }

        private static void ButtonOnHolding(object sender, ButtonHoldingEventArgs e)
        {
            if (_currentOperMode != OperModes[0])
            {
                SetOperMode(OperModes[0]);
            }
        }
        
        private static void ButtonOnDoublePress(object sender, EventArgs e)
        {
            Debug.WriteLine(nameof(ButtonOnDoublePress));
            SetOperMode(OperModes[1]);
        }

        public static void SetOperMode(IOperMode operMode)
        {
            _currentOperMode?.Stop();
            _currentOperMode = operMode;
            _currentOperMode.Start();
        }
    }
}