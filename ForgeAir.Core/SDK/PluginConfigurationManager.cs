using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAirPlugin;

namespace ForgeAir.Core.SDK
{
    public class PluginConfigurationManager
    {
        Helpers.ConfigurationManager configurationManager;
        string? configFile;

        public string RetreiveString(string category, string key, string iniSafeFileName)
        {
            configFile = String.Format("{0}.ini", iniSafeFileName);
            configurationManager = new Helpers.ConfigurationManager(configFile);

            return configurationManager.Get(category, key);
        }
        public void SetString(string category, string key, string value, string iniSafeFileName)
        {
            configFile = String.Format("{0}.ini", iniSafeFileName);
            configurationManager = new Helpers.ConfigurationManager(configFile);

            configurationManager.Set(category, key, value);
            return;
        }
    }
}
