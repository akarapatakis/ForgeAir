using Caliburn.Micro;
using Microsoft.Extensions.DependencyInjection;
using ForgeAir.Core.Models;
using System;
using System.Collections.Generic;
using System.Windows;
using ForgeAir.Playout.UserControls.ViewModels;
using ForgeAir.Playout.Views;
using ForgeAir.Playout.Services;

public class AppBootstrapper : BootstrapperBase
{
    private IServiceProvider _serviceProvider;

    public AppBootstrapper()
    {
        Initialize();
    }

    protected override void Configure()
    {
        var services = new ServiceCollection();

        // Register your services and AppState
        services.AddSingleton<AppState>();

        // Register EventAggregator from Caliburn.Micro (built-in)
        services.AddSingleton<IEventAggregator, EventAggregator>();
        services.AddSingleton<TrackStateUpdater>();

        // Register your views and viewmodels
        services.AddTransient<StudioWindow>();
        services.AddTransient<TrackSelectorViewModel>();

        // Register other ForgeAir services here...

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

    protected override void OnStartup(object sender, StartupEventArgs e)
    {
        DisplayRootViewForAsync<StudioWindow>();
    }
}
