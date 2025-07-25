using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.DeviceManager.Interfaces;

namespace ForgeAir.Core.Services.DeviceManager
{
    public static class BassManagerFactory
    {
        public static BassManager Create(BassDevice outputDevice)
        {
            return new BassManager(outputDevice);
        }
    }
}
