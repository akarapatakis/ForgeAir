using System;
using System.Diagnostics;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Markup.Xaml;
using ForgeAir.Playout.App.Views;

namespace ForgeAir.Playout.App;

public partial class App : Application
{
    private Bootstrapper _bootstrapper;

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
        {
            Environment.Exit(0);
            return;
        }

        _bootstrapper = new Bootstrapper();


        base.OnFrameworkInitializationCompleted();
    }
}