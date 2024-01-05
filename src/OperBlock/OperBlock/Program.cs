using System;
using System.Device;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Device.Spi;
using System.Diagnostics;
using System.Threading;
using Iot.Device.Button;
using Iot.Device.Ws28xx;
using nanoFramework.EncButton.Core;
using nanoFramework.EncButton.Enums;
using nanoFramework.Hardware.Esp32;
using OperBlock.Modes;


namespace OperBlock
{
    public class Program
    {
        private static readonly byte[] LampsPins = { 2, 4, 18, 17 };
        private static readonly byte[] LedStripsPins = { 15 };

        private static readonly DeviceFunction[] Pwms =
        {
            DeviceFunction.PWM1,
            DeviceFunction.PWM2,
            DeviceFunction.PWM3,
            DeviceFunction.PWM4,
            DeviceFunction.PWM5,
            DeviceFunction.PWM6,
            DeviceFunction.PWM7,
            DeviceFunction.PWM8,
            DeviceFunction.PWM9,
            DeviceFunction.PWM10,
            DeviceFunction.PWM11,
            DeviceFunction.PWM12,
            DeviceFunction.PWM13,
            DeviceFunction.PWM14,
            DeviceFunction.PWM15,
            DeviceFunction.PWM16,
        };

        private static int _curModeIndex = 0; // сделано временно чтобы листать режимы

        private static Timer _tickTimer;
        private static Button? _button;
        private static GpioController? _gpioController;
        private static IOperMode? _currentOperMode;

        public static Lamp[] Lamps { get; private set; } = new Lamp[LampsPins.Length];
        public static LedStrip[] LedStrips { get; private set; } = new LedStrip[LedStripsPins.Length];

        private static IOperMode[] OperModes { get; set; } = new IOperMode[0];

        public static void Main()
        {
            _gpioController = new GpioController();

            _button = new Button(21, gpioController: _gpioController);

            _tickTimer = new Timer(Tick, null, 0, 1);
            var debugTimer = new Timer((state) =>
            {
                //Debug.WriteLine($"Heartbeat [{DateTime.UtcNow:HH':'mm':'ss.fff}] | _button.IsPressed: {_button.IsPressing()} Pin: {_gpioController.Read(_button.Pin)}");
                //Debug.WriteLine($"Heartbeat [{DateTime.UtcNow:HH':'mm':'ss.fff}] | _button.IsPressed:  Pin: ");
            }, null, 0, 1000);

            InitLamps();
            InitLedStrips();

            OperModes = new IOperMode[]
            {
                new StockOperMode(Lamps, _button),
                //new EnrageStockMode(Lamps, _button, TimeSpan.FromMilliseconds(12)),
                //new ManualOperMode(Lamps),
                new PairBlinkOperMode(Lamps, LedStrips[0]),
                //new SimpleOperMode(Lamps),
            };

            Debug.WriteLine("Hello from OperBlock!");

            //WifiController.Init();
            //WebController.Init();

            SetOperMode(OperModes[0]);
            _button.ButtonAction += OnButtonAction;

            Thread.Sleep(Timeout.Infinite);
        }

        private static void InitLamps()
        {
            for (var i = 0; i < LampsPins.Length; i++)
            {
                Configuration.SetPinFunction(LampsPins[i], Pwms[i]);
                Lamps[i] = new Lamp(PwmChannel.CreateFromPin(LampsPins[i], 40000, 0f), 0.5f);
            }
        }

        private static void InitLedStrips()
        {
            Configuration.SetPinFunction(19, DeviceFunction.SPI2_MISO);
            Configuration.SetPinFunction(23, DeviceFunction.SPI2_CLOCK);

            for (var i = 0; i < LedStripsPins.Length; i++)
            {
                Configuration.SetPinFunction(LedStripsPins[i], DeviceFunction.SPI2_MOSI);

                SpiConnectionSettings settings = new(2, 22)
                {
                    ClockFrequency = 2_400_000,
                    Mode = SpiMode.Mode0,
                    DataBitLength = 8
                };
                var spi = SpiDevice.Create(settings);

                var ledStrip = new LedStrip(218, new Ws2815b(spi, 218));
                LedStrips[i] = ledStrip;
            }
        }

        private static void Tick(object state)
        {
            //if (WifiController.IsConnected == false && _currentOperMode != OperModes[0])
            //{
            //    SetOperMode(OperModes[0]);
            //}

            _currentOperMode?.Tick();
            //DelayHelper.DelayMilliseconds(0, true);
        }

        private static void OnButtonAction(object sender, EncButtonFlag encButtonFlag)
        {
            var btn = (Button)sender;

            string action = encButtonFlag switch
            {
                EncButtonFlag.Press => "Press",
                EncButtonFlag.Hold => "Hold",
                EncButtonFlag.Step => "Step",
                EncButtonFlag.Release => "Release",
                EncButtonFlag.Click => "Click",
                EncButtonFlag.Clicks => "Clicks",
                EncButtonFlag.Turn => "Turn",
                EncButtonFlag.ReleaseHold => "ReleaseHold",
                EncButtonFlag.ReleaseHoldClicks => "ReleaseHoldClicks",
                EncButtonFlag.ReleaseStep => "ReleaseStep",
                EncButtonFlag.ReleaseStepClicks => "ReleaseStepClicks",
                _ => encButtonFlag.ToString()
            };

            Debug.WriteLine($"_button_ButtonAction [{DateTime.UtcNow:HH':'mm':'ss.fff}] | {action} {btn.Clicks}");

            if (encButtonFlag == EncButtonFlag.Clicks)
            {
                if (btn.Clicks == 2)
                {
                    _curModeIndex += 1;
                    if (_curModeIndex == OperModes.Length)
                    {
                        _curModeIndex = 0;
                    }

                    Debug.WriteLine("ButtonOnDoublePress");
                    SetOperMode(OperModes[_curModeIndex]);
                }
            }
        }

        // private static void ButtonOnHolding(object sender, ButtonHoldingEventArgs e)
        // {
        //     if (_currentOperMode != OperModes[0])
        //     {
        //         SetOperMode(OperModes[0]);
        //     }
        // }


        public static void SetOperMode(IOperMode operMode)
        {
            _currentOperMode?.Stop();
            _currentOperMode = operMode;
            _currentOperMode.Start();
        }
    }
}