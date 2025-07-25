using System.Windows;

namespace ForgeAir.Playout
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Bootstrapper _bootstrapper;

        protected override void OnStartup(StartupEventArgs e)
        {
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                HandyControl.Controls.MessageBox.Show("ForgeAir is already running", "ForgeAir - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            
            }
            base.OnStartup(e);
            _bootstrapper = new Bootstrapper();
        }
    }
}
