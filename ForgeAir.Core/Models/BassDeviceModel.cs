using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    public class BassDeviceModel
    {
        public required OutputDevice TargetDevice { get; set; }

        public int Handle { get; set; }

    }
}
