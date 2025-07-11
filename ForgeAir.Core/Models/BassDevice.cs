using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    public class BassDevice
    {
        public required OutputDevice TargetDevice { get; set; }

        public int Handle { get; set; } = 0;

        public int vstHandle { get; set; } = 0;

    }
}
