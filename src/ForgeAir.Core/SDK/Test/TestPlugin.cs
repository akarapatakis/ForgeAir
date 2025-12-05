using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgePlugin.Helpers;
using ForgePlugin.Interfaces;

namespace ForgeAir.Core.SDK.Test
{
    class TestPlugin : IPlugin
    {
        private IPluginContext _context;

        public string Name => "Test Plugin";

        public string Author => throw new NotImplementedException();

        public string Description => "Do not use it as a reference. a new one will be made available";

        public string AuthorHomepage => "about:blank";

        public string DisplayVersion => "1.0.0.0";

        public bool Initialize(IPluginContext context)
        {
            _context = context; // passing the context to the whole plugin so it can communicate with ForgeAir
            _context.Logger.Log(ForgePlugin.Enums.LoggerSeverityEnum.Debug, "Hello World!");
            return true;
        }

        public bool Quit()
        {
            _context.Logger.Log(ForgePlugin.Enums.LoggerSeverityEnum.Debug, "Bye World!");
            return true;
        }

        public void ShowAboutPage()
        {
            throw new NotImplementedException();
        }

        public void ShowConfigurationPage()
        {
            _context.Logger.Log(ForgePlugin.Enums.LoggerSeverityEnum.Error, "This test plugin does not have a configuration page. :((");
        }

        public void ThrowCrash()
        {
            _context.Logger.Log(ForgePlugin.Enums.LoggerSeverityEnum.Fatal, ";)");
        }
    }
}
