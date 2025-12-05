using ForgeAir.Playout.ViewModels;
using MahApps.Metro.Controls;
using Microsoft.Toolkit.Uwp.Notifications;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for ShellView.xaml
    /// </summary>
    public partial class ShellView : MetroWindow
    {
        public ShellView()
        {
            InitializeComponent();
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            /*
            if (DataContext is ShellViewModel vm)
            {
                // now we need the window to close in order to reopen the station selector
                new ToastContentBuilder()
                .AddText("ForgeAir is minimized to the system tray.")
                .AddText("To restore, click the icon in the system tray.")
                .Show();
            }*/
        }
    }
}
