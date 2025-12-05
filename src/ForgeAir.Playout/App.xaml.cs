using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Media;

namespace ForgeAir.Playout
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private Bootstrapper _bootstrapper;

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool AllocConsole();

        [DllImport("kernel32.dll", SetLastError = true)]
        private static extern bool FreeConsole();

        protected override void OnStartup(StartupEventArgs e)
        {
            if (e.Args.FirstOrDefault() == "/update")
            {
                AllocConsole();

                Console.WriteLine("Hello from ForgeAir!");
                Console.WriteLine("Thides console is created dynamically.");

                Console.ReadLine(); // keep it open until user presses Enter

                // Optionally free it when done
                FreeConsole();
                return;
            }
            if (Process.GetProcessesByName(Process.GetCurrentProcess().ProcessName).Length > 1)
            {
                HandyControl.Controls.MessageBox.Show("ForgeAir is already running", "ForgeAir - Error", MessageBoxButton.OK, MessageBoxImage.Error);
                Application.Current.Shutdown();
            
            }
            base.OnStartup(e);
            _bootstrapper = new Bootstrapper();
        }
    }

    class Updater
    {
        private void PatchProgram(string exePath) {
            Helpers.SystemHelper.CopyDirectory(Environment.CurrentDirectory, Path.GetTempPath() + "forgeair_temp", true);



        }

        private async void DownloadUpdate()
        {
            using (var client = new HttpClient())
            {
                string filePath = Path.Combine(Path.GetTempPath(), "install_forgeair.exe");
                byte[] data = await client.GetByteArrayAsync("");
                await File.WriteAllBytesAsync(filePath, data);
            }

        }
    }
}
