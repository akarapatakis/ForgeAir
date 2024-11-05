using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.AudioEngine.Enums;
using ManagedBass;

namespace ForgeAir.Core.Shared
{
    /// <summary>
    /// Declares an Output Device so it can be easily initialized with BASS for an 'infinite' amount of Output devices for any use case
    /// </summary>
    public class OutputDevice
    {
        private static Dictionary<string, OutputDevice> instances = new Dictionary<string, OutputDevice>();
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public OutputDevice() { return; }

        /// <summary>
        /// MME, WASAPI, DirectSound, ASIO
        /// </summary>
        public DeviceOutputMethodEnum deviceOutputMethod { get; set; }

        /// <summary>
        /// deviceIndex can be obtained through GetDevices(DeviceOutputMethodEnum driver) method in the Devices class, with 'driver' argument, the driver BASS is going to be initialized.
        /// </summary>
        public int deviceIndex { get; set; } = 0;

        /// <summary>
        /// Can only be used in WaveOut/MME device initialization
        /// </summary>
        public bool useDSound { get; set; } = false;

        /// <summary>
        /// if no sampleRate is assigned, 44100hz is going to be the fallback option
        /// </summary>
        public int sampleRate { get; set; } = 44100;

        /// <summary>
        /// if no audioChannels are assigned, 2 (Stereo) are going to be the fallback option (default in most setups)
        /// </summary>
        public MMEDeviceOutputAudioChannelsEnum MMEaudioChannels { get; set; } = MMEDeviceOutputAudioChannelsEnum.Stereo;

        public WASAPIDeviceOutputAudioChannelsEnum WASAPIaudioChannels { get; set; } = WASAPIDeviceOutputAudioChannelsEnum.Stereo;
        /// <summary>
        /// if no bitDepth is assigned, 16-bit is going to be the fallback option (default in most setups)
        /// </summary>
        public DeviceOutputBitDepthEnum bitDepth { get; set; } = DeviceOutputBitDepthEnum.SixteenBit;
        
        /// <summary>
        /// Should be CPU-dependent. While most recent CPUs/Sound Cards can achieve as low as 50ms, there should be anyway a customizable bufferLength for special cases & older machines
        /// </summary>
        public int bufferLength { get; set; } = 150;

        public static OutputDevice RetreiveOrCreate(string name)
        {
            if (!instances.ContainsKey(name))
            {
                instances[name] = new OutputDevice();
            }
            return instances[name];
        }
    }
}
