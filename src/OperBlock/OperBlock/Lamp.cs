using System;
using System.Device.Pwm;

namespace OperBlock
{
    public class Lamp
    {
        private readonly PwmChannel _pwmChannel;
        
        public Lamp(PwmChannel pwmChannel)
        {
            _pwmChannel = pwmChannel;
        }

        public bool Enabled { get; private set; }
        public float Brightness { get; set; }

        public void ToggleOn(float brightness = 1f)
        {
            if (Enabled)
            {
                return;
            }
            
            _pwmChannel.Start();
            SetBrightness(brightness);
            
            Enabled = true;
        }

        public void ToggleOff()
        {
            if (!Enabled)
            {
                return;
            }

            SetBrightness(0f);
            _pwmChannel.Stop();
            
            Enabled = false;
        }

        public void SetBrightness(float level)
        {
            Brightness = level;
            _pwmChannel.DutyCycle = level;
        }
    }
}