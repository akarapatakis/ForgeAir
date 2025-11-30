using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models.Enums
{
    public enum StreamingEncoderFormat
    {
        MP3 = 0,
        OGG = 1,
        AAC = 2,
    }

    public enum StreamingEncoderProtocol
    {
        ShoutcastV1 = 0,
        ShoutcastV2 = 1,
        Icecast = 2,
    }
}
