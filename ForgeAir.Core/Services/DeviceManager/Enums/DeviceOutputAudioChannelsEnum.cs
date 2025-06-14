using ManagedBass;
using ManagedBass.Wasapi;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine.Enums
{
    /// <summary>
    /// bass only
    /// </summary>
    public enum MMEDeviceOutputAudioChannelsEnum
    {
        Mono = DeviceInitFlags.Mono | 0,
        Stereo = DeviceInitFlags.Stereo | 1
    }
    /// <summary>
    /// bass only
    /// </summary>
    public enum WASAPIDeviceOutputAudioChannelsEnum
    {
        Mono = 1,
        Stereo = 2
    }
    /// <summary>
    /// for use with frontend/naudio/etc
    /// </summary>
    public enum GenericDeviceOutputAudioChannelsEnum
    {
        Mono = 1,
        Stereo = 2
    }
}
