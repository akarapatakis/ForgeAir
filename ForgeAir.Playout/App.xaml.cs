using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.RDS;
using ForgeAir.Core.Shared;
using ForgeAir.Playout.TrayIcon;
using ForgeAir.Playout.Views;
using HandyControl.Properties.Langs;
using HandyControl.Tools;
using HandyControl.Tools.Extension;
using Microsoft.Win32;
using System.Configuration;
using System.Data;
using System.Windows;

namespace ForgeAir.Playout
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : System.Windows.Application
    {
        Core.InitScript initscript = new Core.InitScript();
        private void DummyEventHandler(object sender, EventArgs e)
        {

        }

        private void Application_Startup(object sender, StartupEventArgs e)
        {

            SplashScreen splashScreen = new SplashScreen();
            splashScreen.Show();

            ConfigHelper.Instance.SetLang("en");


            initscript.ExecuteForgeAir();

            splashScreen.Hide();
            Shared.SharedWindowInstances.Instance.studioWindow = new StudioWindow();
            Shared.SharedWindowInstances.Instance.studioWindow.Show();

            ForgeTrayIcon trayIcon = new ForgeTrayIcon();
            
        }

    }

}
