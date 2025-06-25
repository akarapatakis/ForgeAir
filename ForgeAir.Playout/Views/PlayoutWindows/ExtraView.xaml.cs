using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for ExtraWindow.xaml
    /// </summary>
    public partial class ExtraWindow : HandyControl.Controls.Window
    {
        public ExtraWindow()
        {
            InitializeComponent();
            MoveToSecondMonitor();
        }
        private void MoveToSecondMonitor()
        {
            var screens = Screen.AllScreens;
            if (screens.Length > 1) // Check if a second monitor exists
            {
                var secondScreen = screens[1]; // Get the second screen
                var workingArea = secondScreen.WorkingArea;

                // Position the window on the second screen
                Left = workingArea.Left;
                Top = workingArea.Top;
                Width = workingArea.Width;
                Height = workingArea.Height;
            }
        }
    }
}
