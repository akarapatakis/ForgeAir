using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using NAudio.Wave;

namespace ForgeAir.Core.Models
{
    public class NAudioDevice
    {

        public required OutputDevice TargetDevice { get; set; }
        public WaveOutEvent? WaveOutAPI { get; set; }

        public DirectSoundOut? DSoundAPI { get; set; }

        public WasapiOut? WasapiOutAPI { get; set; }

        public AsioOut? AsioOutAPI { get; set; }

        public ISampleProvider AudioStream { get; set; }
        public object? GetAPI() // non-static here because it needs to be used with each declared model specifically
        {
            switch (TargetDevice.API)
            {
                case AudioEngine.Enums.DeviceOutputMethodEnum.MME:
                    return WaveOutAPI;
                case AudioEngine.Enums.DeviceOutputMethodEnum.DirectSound:
                    return DSoundAPI;
                case AudioEngine.Enums.DeviceOutputMethodEnum.WASAPI:
                    return WasapiOutAPI;
                case AudioEngine.Enums.DeviceOutputMethodEnum.ASIO:
                    return AsioOutAPI;
                default: return null;
            }
        }
    }
    
}
