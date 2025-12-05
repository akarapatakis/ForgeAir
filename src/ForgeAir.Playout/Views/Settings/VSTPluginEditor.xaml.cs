using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST;
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
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ForgeAir.Playout.Views.VST
{
    /// <summary>
    /// Interaction logic for VSTPluginEditor.xaml
    /// </summary>
    public partial class VSTPluginEditor : Window
    {
        private readonly VSTService _service;
        private readonly VSTConfigurationManager _configurationManager;
        public VSTPluginEditor(VSTService service)
        {
            _service = service;
          //  _configurationManager = new VSTConfigurationManager(_service);
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
        }

        private void Window_Closed(object sender, EventArgs e)
        {
            //_service.SaveVSTSettings();
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            _service.ShowUI((nint)null);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {
            var wih = new WindowInteropHelper(this);
            IntPtr hWnd = wih.Handle;
            _service.ShowUI(hWnd);
            Task.Delay(1000).Wait();
        }
    }
}
