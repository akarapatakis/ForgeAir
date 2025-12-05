using ForgeAir.Core.AudioEngine.Enums;
using ForgeAir.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.DeviceManager.Interfaces
{
    public interface IDeviceManager
    {
        int InitDevice();
        int FreeDevice();
        List<OutputDevice> ListDevicesByAPI(DeviceOutputMethodEnum api);
    }
}
