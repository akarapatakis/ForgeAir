using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgePlugin.Enums;
using ForgePlugin.Helpers;

namespace ForgeAir.Core.SDK
{
    public class PluginLogger : ILogger
    {
        private readonly string _source;

        public PluginLogger(string source)
        {
            _source = source;
        }


        public void Log(LoggerSeverityEnum severity, string message, Exception? ex = null)
        {
            Console.WriteLine($"[INFO][{_source}] {message}");
        }
    }

}
