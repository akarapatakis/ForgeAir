using Caliburn.Micro;
using ForgeAir.StationManagement.Components.MediaLibrary.ViewModels;
using ForgeAir.StationManagement.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace ForgeAir.StationManagement
{
    public class Bootstrapper : BootstrapperBase
    {
        public Bootstrapper()
        {
            Initialize();
            LogManager.GetLog = type => new DebugLog(type);
        }

        protected override async void OnStartup(object sender, StartupEventArgs e)
        {
            //await DisplayRootViewForAsync(typeof(LoginViewModel));
            await DisplayRootViewForAsync(typeof(MediaLibraryMainViewModel));
        }
    }
}
