using ManagedBass;
using ManagedBass.Wasapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine.Enums
{
    public enum MMEDeviceOutputAudioChannelsEnum
    {
        Mono = DeviceInitFlags.Mono | 0,
        Stereo = DeviceInitFlags.Stereo | 1
    }
    public enum WASAPIDeviceOutputAudioChannelsEnum
    {
        Mono = 1,
        Stereo = 2
    }
}
