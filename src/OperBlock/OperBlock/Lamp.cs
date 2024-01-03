using System;
using System.Device.Pwm;

namespace OperBlock
{
    public class Lamp
    {
        private readonly PwmChannel _pwmChannel;
        
        public Lamp(PwmChannel pwmChannel, float maxBrightness = 1f)
        {
            _pwmChannel = pwmChannel;
            MaxBrightness = maxBrightness;
        }

        public bool Enabled { get; private set; }
        public float Brightness { get; private set; }

        public float MaxBrightness { get; set; }

        public void ToggleOn(float brightness = 1f)
        {
            if (Enabled)
            {
                return;
            }
            
            //_pwmChannel.Start();
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
            //_pwmChannel.Stop();
            
            Enabled = false;
        }

        public void SetBrightness(float level)
        {
            level = Math.Clamp(level, 0f, MaxBrightness);

            Brightness = level;
            _pwmChannel.DutyCycle = level;
        }
    }
}