using Caliburn.Micro;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Playout.App.Bootstrappers;
using ForgeAir.Playout.App.Instances;
using ForgeAir.Playout.App.Services;
using ForgeAir.Playout.App.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using NLog;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Threading;
using ForgeAir.Playout.App.Views;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ForgeAir.Playout.App
{
    public class Bootstrapper : BootstrapperBase
    {
        private IServiceProvider _serviceProvider;
        private readonly IServiceCollection services;

        private SplashScreenView splashScreen;
        public Bootstrapper()
        {
            services = new ServiceCollection();
            splashScreen = new SplashScreenView();
            Initialize();
        }

        protected override void Configure()
        {
            //services.AddSingleton<ForgeTrayIcon>();
            
            services.AddSingleton<IConfigurationManager>(provider =>
            {
                var configFile = "configuration.ini";
                return new ConfigurationManager(configFile);
            });

            services.AddSingleton<Core.Helpers.Interfaces.IEventAggregator, SimpleEventAggregator>();
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<CrashReporterService>();
            //services.AddSingleton<IHost>();

            services.AddTransient<StationSelectorViewModel>();


            _serviceProvider = services.BuildServiceProvider();
        }

        protected override object GetInstance(Type service, string key)
        {
            return _serviceProvider.GetService(service) ?? throw new Exception($"Could not locate service: {service.Name}");
        }

        protected override IEnumerable<object> GetAllInstances(Type service)
        {
            return _serviceProvider.GetServices(service);
        }

        protected override void BuildUp(object instance)
        {
            _serviceProvider.GetRequiredService(instance.GetType());
        }
        private async void LaunchWebInterface()
        {


        }
        private static readonly Logger Logger = NLog.LogManager.GetCurrentClassLogger();
        protected override async void OnStartup(object sender, ControlledApplicationLifetimeStartupEventArgs e)
        {
            Logger.Debug("Starting ForgeAir@Bootstrapper...");
            Logger.Info("Starting ForgeAir...");
            NLog.LogManager.Flush();


            splashScreen.Show();
            await Task.Delay(100); // wait for splashscreen to render
            var crash = _serviceProvider.GetRequiredService<CrashReporterService>();


#if !(DEBUG)

            System.Windows.Forms.Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            System.Windows.Forms.Application.ThreadException += (sender, args) =>
            {
                var ex = args.Exception as Exception;
                crash.Report(ex);
            };
            AppDomain.CurrentDomain.UnhandledException += (s, exArgs) =>
            {
                var ex = exArgs.ExceptionObject as Exception;
                crash.Report(ex);
            };

            Application.DispatcherUnhandledException += (s, exArgs) =>
            {
                var ex = exArgs.Exception as Exception;

                crash.Report(ex);
                exArgs.Handled = true;
            };

            TaskScheduler.UnobservedTaskException += (s, exArgs) =>
            {
                var ex = exArgs.Exception as Exception;

                crash.Report(ex);
                exArgs.SetObserved();
            };
#endif
            await Task.Run(async () =>
            {
                if (!File.Exists("configuration.ini"))
                {
                    Logger.Debug("configuration.ini not found in directory. ");
                    Logger.Fatal("Configuration file doesn't exist.");
                    await Dispatcher.UIThread.Invoke(async () =>
                    {
                        await MessageBoxManager
                            .GetMessageBoxStandard("Error", "Configuration file doesn't exist.",
                                ButtonEnum.Ok, Icon.Error).ShowAsync();
                    });
                    Environment.Exit(1);

                }
                var stationTags = _serviceProvider.GetRequiredService<IConfigurationManager>().GetAll("Stations").FirstOrDefault()?.Values.ToList();
                if (stationTags == null)
                {
                    Logger.Debug("stationTags returned null. (probably configuration.ini )");
                    Logger.Error("No Stations declared in configuration.ini. ");
                    await MessageBoxManager
                        .GetMessageBoxStandard("Error", "No stations found.",
                            ButtonEnum.Ok, Icon.Error).ShowAsync();                    
                    Environment.Exit(1);
                }
                foreach (var tag in stationTags)
                {
                    Logger.Info($"Launching station '{tag}'...");

                    string stationConfigPath = $"Stations/{tag}/{tag}.ini";

                    if (!File.Exists(stationConfigPath))
                    {
                        Logger.Warn($"Station configuration file for '{tag}' not found.");
                        continue;
                    }

                    var bootstrapper = new StationBootstrapper(stationConfigPath, _serviceProvider);
                    await bootstrapper.Initialize();
                    var player = bootstrapper.Services.GetRequiredService<IAudioService>();
                    var config = bootstrapper.Services.GetRequiredService<IConfigurationManager>();
                    if (config.GetBool("Behavior", "AutoPlay"))
                    {
                        Logger.Warn($"'{tag}': Autoplay is enabled based on configuration. Attempting to start.");

                        player.Play(true);
                    }

                    StationsInstance.Instance.Stations.Add(bootstrapper);
                }

                NLog.LogManager.Flush();


                Logger.Info($"General initialization complete.");

            });
            //todo: fix tray
           // _serviceProvider.GetRequiredService<ForgeTrayIcon>();
           // var wI = await Task.Run(() => WebInterface.Host.Start(services));

            //await Task.Run(() => wI.Run());
            //await DisplayRootViewForAsync<StationSelectorViewModel>();
            splashScreen.Close();

        }
    }

}