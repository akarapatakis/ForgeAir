using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgePlugin;
using ForgePlugin.Helpers;

namespace ForgeAir.Core.SDK.Interfaces
{
    public class IPluginConfigurationManager : ForgePlugin.Helpers.IConfigurationManager
    {

        IPluginEntry? pluginEntry;

        PluginConfigurationManager configurationManager = new PluginConfigurationManager();


        string IConfigurationManager.RetreiveSetting(string key, IPluginEntry plugin)
        {
            return configurationManager.RetreiveString(plugin.name, key, plugin.pluginAssemblyName).ToString();
        }

        void IConfigurationManager.SetSetting(string key, string value, IPluginEntry plugin)
        {
            configurationManager.SetString(plugin.name, key, value, plugin.pluginAssemblyName);
        }

    }
}
