using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.Models;
using ForgeAir.Core.RDS;
using ForgeAir.Core.Shared;
using ForgeAir.Database;
using ForgeAir.Playout.TrayIcon;
using ForgeAir.Playout.UserControls.ViewModels;
using ForgeAir.Playout.Views;
using HandyControl.Properties.Langs;
using HandyControl.Tools;
using HandyControl.Tools.Extension;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ForgeAir.Playout
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private AppBootstrapper _bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);
            _bootstrapper = new AppBootstrapper();
        }
    }
}
