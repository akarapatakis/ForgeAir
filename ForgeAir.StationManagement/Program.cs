using Avalonia;
using System;
using Logazmic;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Extensions.Logging;
using NLog.Config;
using NLog.Targets.Wrappers;
using NLog.Targets;
using System.Text;
namespace ForgeAir.StationManagement
{
    public class Runner
    {
        private readonly ILogger<Runner> _logger;

        public Runner(ILogger<Runner> logger)
        {
            _logger = logger;
        }

        public void DoAction(string name)
        {
            _logger.LogDebug(20, "Doing hard work! {Action}", name);
        }
    }

    internal sealed class Program
    {
        // Initialization code. Don't use any Avalonia, third-party APIs or any
        // SynchronizationContext-reliant code before AppMain is called: things aren't initialized
        // yet and stuff might break.
        [STAThread]
        public static void Main(string[] args)
        {
            var config = new LoggingConfiguration();

            #region file
            var ftXml = new FileTarget
            {
                FileName = "nlog.config",
                Layout = " ${log4jxmlevent}",
                Encoding = Encoding.UTF8,
                ArchiveEvery = FileArchivePeriod.Day,
                ArchiveNumbering = ArchiveNumberingMode.Rolling
            };

            var asXml = new AsyncTargetWrapper(ftXml);
            var ruleXml = new LoggingRule("*", NLog.LogLevel.Trace, asXml);
            config.LoggingRules.Add(ruleXml);
            #endregion

            #region tcp
            var tcpNetworkTarget = new NLogViewerTarget
            {
                Address = "tcp4://127.0.0.1:4505",
                Encoding = Encoding.UTF8,
                Name = "NLogViewer",
                IncludeNLogData = false
            };
            var tcpNetworkRule = new LoggingRule("*", NLog.LogLevel.Trace, tcpNetworkTarget);
            config.LoggingRules.Add(tcpNetworkRule);
            #endregion

            LogManager.Configuration = config;
            config.LogFactory.CreateNullLogger();

            var logger = LoggerFactory.Create(builder => builder.AddNLog()).CreateLogger<Program>();
            logger.LogInformation("Program has started.");
            BuildAvaloniaApp()


            .StartWithClassicDesktopLifetime(args);

        }

        // Avalonia configuration, don't remove; also used by visual designer.
        public static AppBuilder BuildAvaloniaApp()
            => AppBuilder.Configure<App>()
                .UsePlatformDetect()
                .WithInterFont()
                .LogToTrace();
    }
}
