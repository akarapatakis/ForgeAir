using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using IniParser;
using IniParser.Model;
using ForgePlugin;

namespace ForgeAir.Core.SDK
{
    public class PluginConfigurationManager
    {

        readonly bool Initialized = true;
        string? configFile;
        IniData? data;

        public string RetreiveString(string category, string key, string iniSafeFileName)
        {
            configFile = String.Format("{0}.ini", iniSafeFileName);
            if (Initialized)
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(configFile);
                if (data[category][key].ToString() == null)
                {
                    return String.Empty;
                }
                return data[category][key].ToString();
            }
            else
            {
                PluginConfigurationManager? cfgmg = new PluginConfigurationManager();
                cfgmg.RetreiveString(category, key, iniSafeFileName);
                cfgmg = null;
                return String.Empty;
            }
        }
        public void SetString(string category, string key, string value, string iniSafeFileName)
        {
            configFile = String.Format("{0}.ini", iniSafeFileName);
            if (Initialized)
            {
                var parser = new FileIniDataParser();
                IniData data = parser.ReadFile(configFile);
                data[category][key] = value;
                parser.WriteFile(configFile, data);
            }
            else
            {
                PluginConfigurationManager? cfgmg = new PluginConfigurationManager();
                cfgmg.SetString(category, key, value, iniSafeFileName);
                cfgmg = null;
            }
        }
    }
}
