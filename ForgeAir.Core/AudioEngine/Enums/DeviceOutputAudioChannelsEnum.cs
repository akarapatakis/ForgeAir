using ManagedBass;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine.Enums
{
    public enum DeviceOutputAudioChannelsEnum
    {
        Mono = DeviceInitFlags.Mono | 0,
        Stereo = DeviceInitFlags.Stereo | 1
    }
}
