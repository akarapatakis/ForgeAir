using ManagedBass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine.Enums
{
    // dev note: use on frontend please - will move to it someday
    public enum DeviceOutputTypeEnum
    {
        [Description("Main")]
        Main =  0,
        [Description("Booth")]
        Booth = 1,
        [Description("Instants")]
        Instants = 2,
        [Description("Main + Instants")]
        MainInstantsMix = 3,
        [Description("Channel A")]
        ChannelA = 4,
        [Description("Channel B")]
        ChannelB = 5
    }
}
