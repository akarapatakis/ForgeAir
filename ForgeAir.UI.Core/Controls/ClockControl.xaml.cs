using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace ForgeVision.Playout.UserControls
{
    /// <summary>
    /// Interaction logic for ClockControl.xaml
    /// </summary>
    public partial class ClockControl : System.Windows.Controls.UserControl
    {
        DispatcherTimer clock = new DispatcherTimer();
        public ClockControl()
        {
            clock.IsEnabled = true;
            clock.Interval = TimeSpan.FromMilliseconds(1000);
            clock.Tick += UpdateTime;
            clock.Start();
            InitializeComponent();
        }

        private void UpdateTime(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                clockBlock.Text = DateTime.Now.ToString("HH:mm:ss");
                textBlock_Copy.Text = DateTime.Now.ToString("dddd dd MMMM yyyy");
            });

        }
    }
}
