using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgePlugin.Helpers;
using ForgePlugin.Interfaces;

namespace ForgeAir.Core.SDK
{
    public class PluginContext : IPluginContext
    {
        public IClientCalls Client { get; }
        public IConfigurationManager Config { get; }
        public ILogger Logger { get; }

        public IRDSDevice? RDSEncoder { get; set; }

        public PluginContext(IClientCalls client, IConfigurationManager config, ILogger logger)
        {
            Client = client;
            Config = config;
            Logger = logger;
        }

        public void SetRdsEncoder(IRDSDevice device)
        {
            RDSEncoder = device;
        }
    }

}
