using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Diagnostics;
using System.Net;
using System.Net.NetworkInformation;
using System.Threading;
using Iot.Device.Button;
using nanoFramework.Hardware.Esp32;
using nanoFramework.Networking;
using nanoFramework.Runtime.Native;
using nanoFramework.WebServer;
using OperBlock.Modes;


namespace OperBlock
{
    public class Program
    {
        private static GpioController? _gpioController;
        private static readonly byte[] LampsPins = { 2, 4, 18, 17 };

        private static GpioButton? _button;

        public static Lamp[] Lamps { get; private set; } = new Lamp[0];

        private static IOperMode[] OperModes { get; set; } = new IOperMode[0];

        private static IOperMode? _currentOperMode;

        public static void Main()
        {
            _gpioController = new GpioController();

            _button = new GpioButton(21, TimeSpan.FromMilliseconds(500), TimeSpan.FromMilliseconds(1000), _gpioController, false);

            _button.IsHoldingEnabled = true;
            _button.IsDoublePressEnabled = true;

            Configuration.SetPinFunction(LampsPins[0], DeviceFunction.PWM1);
            Configuration.SetPinFunction(LampsPins[1], DeviceFunction.PWM2);
            Configuration.SetPinFunction(LampsPins[2], DeviceFunction.PWM3);
            Configuration.SetPinFunction(LampsPins[3], DeviceFunction.PWM4);

            Lamps = new[]
            {
                new Lamp(PwmChannel.CreateFromPin(LampsPins[0], 40000, 0f), 0.6f),
                new Lamp(PwmChannel.CreateFromPin(LampsPins[1], 40000, 0f), 0.6f),
                new Lamp(PwmChannel.CreateFromPin(LampsPins[2], 40000, 0f), 0.6f),
                new Lamp(PwmChannel.CreateFromPin(LampsPins[3], 40000, 0f), 0.6f),

                //new Lamp(PwmChannel.CreateFromPin(LampsPins[0], 40000, 0f), 1f),
                //new Lamp(PwmChannel.CreateFromPin(LampsPins[1], 40000, 0f), 1f),
                //new Lamp(PwmChannel.CreateFromPin(LampsPins[2], 40000, 0f), 1f),
                //new Lamp(PwmChannel.CreateFromPin(LampsPins[3], 40000, 0f), 1f),
            };

            OperModes = new IOperMode[]
            {
                new StockOperMode(Lamps, _button),
                new EnrageStockMode(Lamps, _button, TimeSpan.FromMilliseconds(12)),
                new ManualOperMode(Lamps),
                new PairBlinkOperMode(Lamps),
                new SimpleOperMode(Lamps),
            };

            Debug.WriteLine("Hello from OperBlock!");

            _button.Holding += ButtonOnHolding;
            _button.DoublePress += ButtonOnDoublePress;

            WifiController.Init();
            WebController.Init();


            while (true)
            {
                if (WifiController.IsConnected == false && _currentOperMode != OperModes[0])
                {
                    SetOperMode(OperModes[0]);
                }

                _currentOperMode?.Tick();

                Debug.WriteLine($"Heartbeat [{DateTime.UtcNow:HH':'mm':'ss.fff}] | _button.IsPressed: {_button.IsPressed} {_gpioController.Read(21)}");
            }

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
            SetOperMode(OperModes[3]);
        }

        public static void SetOperMode(IOperMode operMode)
        {
            _currentOperMode?.Stop();
            _currentOperMode = operMode;
            _currentOperMode.Start();
        }
    }
}