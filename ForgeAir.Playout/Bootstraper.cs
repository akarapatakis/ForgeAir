using Caliburn.Micro;
using Microsoft.Extensions.DependencyInjection;
using ForgeAir.Core.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using ForgeAir.Playout.UserControls.ViewModels;
using ForgeAir.Playout.Views;
using ForgeAir.Playout.Services;
using ForgeAir.Playout.UserControls;
using ForgeAir.Playout;
using ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing;
using ForgeAir.Playout.ViewModels.PlayoutWindows;
using ForgeAir.Playout.ViewModels;
using ForgeAir.Playout.TrayIcon;
using ForgeAir.Core.Services.Scheduler.Interfaces;
using ForgeAir.Core.Services.Scheduler;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using ForgeAir.Core.Services.TrackSelector;
using ForgePlugin.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players;
using ForgeAir.Core.Services.AudioPlayout;
using System.ComponentModel;

namespace ForgeAir.Playout
{
    public class Bootstrapper : BootstrapperBase
    {
        private IServiceProvider _serviceProvider;

        public Bootstrapper()
        {
            Initialize();
        }

        protected override void Configure()
        {
            var services = new ServiceCollection();

            // Registering long-lived services
            services.AddSingleton<AppState>();
            services.AddSingleton<ForgeTrayIcon>();
            services.AddSingleton<ShellViewModel>();

            services.AddSingleton<ISchedulerService, SchedulerService>();
            services.AddTransient<ITrackSelectorService, TrackSelectorService>();
            services.AddTransient<IAudioService, AudioPlayerService>();


            // Registering Event Aggregator
            services.AddSingleton<IEventAggregator, EventAggregator>();

            // Registering State updaters
            services.AddSingleton<TrackStateUpdater>();
            services.AddSingleton<QueueStateUpdater>();

            // Registering ViewModels/Views
            services.AddTransient<PlayoutPageViewModel>();
            services.AddSingleton<IWindowManager, WindowManager>();
            services.AddTransient<ImportDirectoryViewModel>();
            services.AddTransient<TrackSelectorViewModel>();





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
            await DisplayRootViewForAsync<ShellViewModel>();
        }
    }

}