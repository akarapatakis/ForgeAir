using ManagedBass;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine.Enums
{

    public enum DeviceTypeEnum
    {
        [Description("Main")]
        Main =  0,
        [Description("Booth")]
        Booth = 1,
        [Description("FX")]
        Instants = 2,
        [Description("Main + FX")]
        MainInstantsMix = 3,
        [Description("MPX")]
        MPX = 4,
    }
}
