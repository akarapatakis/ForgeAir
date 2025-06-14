using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.AudioEngine.Enums;
using ManagedBass;

namespace ForgeAir.Core.Models
{
    /// <summary>
    /// Declares an Output Device so it can be easily initialized with the audio engine for an 'infinite' amount of Output devices for any use case
    /// </summary>
    public class OutputDevice
    {

        /// <summary>
        /// This boolean is for use by DeviceManager. Do not modify it elsewhere
        /// </summary>
        public bool Initialized { get; set; } = false;

        /// <summary>
        /// MME, WASAPI, DirectSound, ASIO
        /// </summary>
        public DeviceOutputMethodEnum API { get; set; }

        /// <summary>
        /// Index can be obtained through GetDevices(DeviceOutputMethodEnum driver) method in the Devices class, with 'driver' argument, the driver engine is going to be initialized.
        /// </summary>
        public int Index { get; set; } = 0;

        /// <summary>
        /// if no sampleRate is assigned, 44100hz is going to be the fallback option
        /// </summary>
        public int SampleRate { get; set; } = 44100;

        /// <summary>
        /// if no Channels are assigned, 2 (Stereo) are going to be the fallback option (default in most setups)
        /// </summary>
        public int Channels { get; set; } = 2;

        /// <summary>
        /// BASS ONLY
        /// </summary>
        public MMEDeviceOutputAudioChannelsEnum MMEaudioChannels { get; set; } = MMEDeviceOutputAudioChannelsEnum.Stereo;

        /// <summary>
        /// Should be CPU-dependent. While most recent CPUs/Sound Cards can achieve as low as 50ms, there should be anyway a customizable BufferLength for special cases & older machines
        /// </summary>
        public int BufferLength { get; set; } = 333;

        /// <summary>
        /// if no bitDepth is assigned, 16-bit is going to be the fallback option (default in most setups)
        /// </summary>
        public DeviceOutputBitDepthEnum BitDepth { get; set; } = DeviceOutputBitDepthEnum.SixteenBit;

    }
}
