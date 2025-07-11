using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;

namespace ForgeAir.Core.Services.DeviceManager.Interfaces
{
    public interface IBassManagerFactory
    {
        BassManager Create(BassDevice device);
    }
}
