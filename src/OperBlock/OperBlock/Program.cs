using System;
using System.Device;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Diagnostics;
using System.Threading;
using Iot.Device.Button;
using nanoFramework.EncButton.Core;
using nanoFramework.Hardware.Esp32;
using OperBlock.Modes;


namespace OperBlock
{
    public class Program
    {
        private static GpioController? _gpioController;
        private static readonly byte[] LampsPins = { 2, 4, 18, 17 };

        private static Button? _button;

        public static Lamp[] Lamps { get; private set; } = new Lamp[0];

        private static IOperMode[] OperModes { get; set; } = new IOperMode[0];

        private static IOperMode? _currentOperMode;

        public static void Main()
        {
            _gpioController = new GpioController();

             new GpioButton(21, TimeSpan.FromMilliseconds(400), TimeSpan.FromMilliseconds(1000), _gpioController, false, debounceTime: TimeSpan.FromMilliseconds(50));
            _button = new Button(0, gpioController: _gpioController);

            //_button.IsHoldingEnabled = true;
            ////_button.IsDoublePressEnabled = true;

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

            //_button.Holding += ButtonOnHolding;
            ////_button.DoublePress += ButtonOnDoublePress;

            WifiController.Init();
            WebController.Init();

            SetOperMode(OperModes[0]);
            while (true)
            {
                _button.tick();

                if (WifiController.IsConnected == false && _currentOperMode != OperModes[0])
                {
                    SetOperMode(OperModes[0]);
                }

                _currentOperMode?.Tick();

                Debug.WriteLine($"Heartbeat [{DateTime.UtcNow:HH':'mm':'ss.fff}] | _button.IsPressed: {_button.pressing()} {_gpioController.Read(0)}");
                //Debug.WriteLine($"Heartbeat [{DateTime.UtcNow:HH':'mm':'ss.fff}] | _button.IsPressed: {_gpioController.Read(0)}");
                DelayHelper.DelayMilliseconds(1, true);
            }

            Thread.Sleep(Timeout.Infinite);
        }

        // private static void ButtonOnHolding(object sender, ButtonHoldingEventArgs e)
        // {
        //     if (_currentOperMode != OperModes[0])
        //     {
        //         SetOperMode(OperModes[0]);
        //     }
        // }

        private static int _curModeIndex = 0;
        private static void ButtonOnDoublePress(object sender, EventArgs e)
        {
            _curModeIndex += 1;
            if (_curModeIndex == OperModes.Length)
            {
                _curModeIndex = 0;
            }

            Debug.WriteLine(nameof(ButtonOnDoublePress));
            SetOperMode(OperModes[_curModeIndex]);
        }

        public static void SetOperMode(IOperMode operMode)
        {
            _currentOperMode?.Stop();
            _currentOperMode = operMode;
            _currentOperMode.Start();
        }
    }
}