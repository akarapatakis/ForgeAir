using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.AudioEngine.Enums;

namespace ForgeAir.Core.Services.DeviceManager.Interfaces
{
    public interface IDevice
    {
        public DeviceTypeEnum Type { get; set; }
    }
}
