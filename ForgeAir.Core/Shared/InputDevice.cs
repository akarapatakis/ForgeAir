using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{
    public class InputDevice
    {
        private static Dictionary<string, InputDevice> instances = new Dictionary<string, InputDevice>();
        private Dictionary<string, object> data = new Dictionary<string, object>();

        public InputDevice() { return; }

        public static InputDevice RetreiveOrCreate(string name)
        {
            if (!instances.ContainsKey(name))
            {
                instances[name] = new InputDevice();
            }
            return instances[name];
        }
    }
}
