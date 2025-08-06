using Caliburn.Micro;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.DeviceManager;
using ForgeAir.Core.Services.DeviceManager.Interfaces;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Services.Importers.Interfaces;
using ForgeAir.Core.Services.Scheduler;
using ForgeAir.Core.Services.Scheduler.Interfaces;
using ForgeAir.Core.Services.TrackSelector;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using ForgeAir.Core.Services.Weather;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout;
using ForgeAir.Playout.Bootstrappers;
using ForgeAir.Playout.Helpers;
using ForgeAir.Playout.Instances;
using ForgeAir.Playout.Services;
using ForgeAir.Playout.TrayIcon;
using ForgeAir.Playout.UserControls;
using ForgeAir.Playout.UserControls.ViewModels;
using ForgeAir.Playout.ViewModels;
using ForgeAir.Playout.ViewModels.PlayoutWindows;
using ForgeAir.Playout.ViewModels.Settings;
using ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing;
using ForgeAir.Playout.Views;
using ForgeAir.Playout.Views.PlayoutWindows;
using ForgePlugin.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using MySqlConnector;
using NAudio.Midi;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Navigation;
using System.Windows.Threading;
using System.Xml.Linq;

namespace ForgeAir.Playout
{
    public class Bootstrapper : BootstrapperBase
    {
        private IServiceProvider _serviceProvider;
        private readonly IServiceCollection services;

        private SplashScreen splashScreen;
        public Bootstrapper()
        {
            services = new ServiceCollection();
            splashScreen = new SplashScreen();
            Initialize();
        }

        protected override void Configure()
        {
            services.AddSingleton<AppState>();
            services.AddSingleton<ForgeTrayIcon>();
            
            services.AddSingleton<IConfigurationManager>(provider =>
            {
                var configFile = "configuration.ini";
                return new ConfigurationManager(configFile);
            });

            services.AddSingleton<Core.Helpers.Interfaces.IEventAggregator, SimpleEventAggregator>();
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddSingleton<CrashReporterService>();
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

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            splashScreen.Show();
            await Task.Delay(500); // wait for splashscreen to render
            var crash = _serviceProvider.GetRequiredService<CrashReporterService>();

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
            await Task.Run(async () =>
            {
                var stationTags = _serviceProvider.GetRequiredService<IConfigurationManager>().GetAll("Stations").FirstOrDefault()?.Values.ToList();
                if (stationTags == null)
                {
                    HandyControl.Controls.MessageBox.Show("No Stations found.\nPlease add a Station.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
                foreach (var tag in stationTags)
                {
                    string stationConfigPath = $"Stations/{tag}/{tag}.ini";

                    if (!File.Exists(stationConfigPath))
                    {
                        Debug.WriteLine($" Station config for '{tag}' not found.");
                        continue;
                    }

                    var bootstrapper = new StationBootstrapper(stationConfigPath, _serviceProvider);
                    await bootstrapper.Initialize();
                    StationsInstance.Instance.Stations.Add(bootstrapper);
                }

            });
            _serviceProvider.GetRequiredService<ForgeTrayIcon>();
            await DisplayRootViewForAsync<StationSelectorViewModel>();
            splashScreen.Close();

        }
    }

}