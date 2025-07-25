using ForgeAir.Core.Models;
using ForgeAir.Core.Services.DeviceManager.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.DeviceManager
{
    public class NAudioManagerFactory
    {
        public static NAudioManager Create(NAudioDevice outputDevice)
        {
            return new NAudioManager(outputDevice);
        }
    }
}
