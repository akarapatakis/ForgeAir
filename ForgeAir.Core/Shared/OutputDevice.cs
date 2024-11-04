using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{
    public class OutputDevice
    {
        private static Dictionary<string, OutputDevice> instances = new Dictionary<string, OutputDevice>();
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public OutputDevice() { return; }

        public static OutputDevice RetreiveOrCreate(string name)
        {
            if (!instances.ContainsKey(name))
            {
                instances[name] = new OutputDevice();
            }
            return instances[name];
        }
    }
}
