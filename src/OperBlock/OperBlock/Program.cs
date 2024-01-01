using System;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using Iot.Device.Button;
using nanoFramework.Hardware.Esp32;
using nanoFramework.WebServer;
using OperBlock.Modes;


namespace OperBlock
{
    public class Program
    {
        private static readonly byte[] LampsPins = { 4, 2 };

        private static GpioButton? _button;

        public static Lamp[] Lamps { get; private set; } = new Lamp[0];

        private static IOperMode[] OperModes { get; set; } = new IOperMode[0];

        private static IOperMode? _currentOperMode;

        public static void Main()
        {
            _button = new GpioButton(0, TimeSpan.FromMilliseconds(300), TimeSpan.FromMilliseconds(2000));

            _button.IsHoldingEnabled = true;
            _button.IsDoublePressEnabled = true;

            Lamps = new[]
            {
                new Lamp(PwmChannel.CreateFromPin(LampsPins[0], 40000, 0f)),
                new Lamp(PwmChannel.CreateFromPin(LampsPins[1], 40000, 0f)),
            };

            OperModes = new IOperMode[]
            {
                new StockOperMode(Lamps, _button),
                new EnrageStockMode(Lamps, _button),
                new ManualOperMode(Lamps),
                new PairBlinkOperMode(Lamps),
                new SimpleOperMode(Lamps),
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

            WifiController.Init();
            WebController.Init();

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