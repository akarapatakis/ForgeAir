using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgePlugin;

namespace ForgeAir.Core.SDK
{
    public class PluginConfigurationManager : ForgePlugin.Helpers.IConfigurationManager
    {
        Helpers.ConfigurationManager _configurationManager;
        string? configFile;

        public PluginConfigurationManager(string? configFile)
        {
            this.configFile = configFile;

            if (configFile == null) { return; }
            if (configFile != null && !File.Exists(configFile))
            {
                File.Create(configFile);
            }
            

            _configurationManager = new Helpers.ConfigurationManager(configFile);
        }



        public bool GetBool(string key)
        {
            return _configurationManager.GetBool("", key);
        }

        public int GetInt(string key)
        {
            return _configurationManager.GetInt("", key);
        }

        public string GetString(string key)
        {
            return _configurationManager.Get("", key);
        }

        public void SetSetting(string key, string value)
        {
            _configurationManager.Set("", key, value);
        }
    }
}
