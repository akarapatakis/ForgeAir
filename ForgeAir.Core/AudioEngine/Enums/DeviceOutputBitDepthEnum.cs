using ManagedBass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine.Enums
{
    public enum DeviceOutputBitDepthEnum
    {
        [Description("8-bit")]
        EightBit = DeviceInitFlags.Byte | 0,
        [Description("16-bit")]
        SixteenBit = DeviceInitFlags.Bits16 | 1,
        [Description("24-bit")]
        TwentyfourBit = DeviceInitFlags.Default |2,
        [Description("32-bit")]
        ThirtyTwoBit = DeviceInitFlags.Default | 3
    }
}
