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

namespace ForgeVision.Playout.UserControls
{
    /// <summary>
    /// Interaction logic for ClockControl.xaml
    /// </summary>
    public partial class ClockControl : System.Windows.Controls.UserControl
    {
        System.Timers.Timer clock = new System.Timers.Timer(1000);
        public ClockControl()
        {
            clock.Enabled = true;
            clock.Interval = 1000;
            clock.Elapsed += UpdateTime;
            clock.Start();
            InitializeComponent();
        }

        private void UpdateTime(object sender, ElapsedEventArgs e)
        {
            this.Dispatcher.Invoke(() =>
            {
                clockBlock.Text = DateTime.Now.ToString("HH:mm:ss");
                textBlock_Copy.Text = DateTime.Now.ToString("dddd dd MMMM yyyy");
            });

        }
    }
}
