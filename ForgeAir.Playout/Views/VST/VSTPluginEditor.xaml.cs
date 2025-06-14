using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.Shared;
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
        VSTEffectManager vstEffect = new VSTEffectManager();
        public VSTPluginEditor()
        {

            InitializeComponent();


        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {

            if (VSTEffect.Instance.effectHandle == 0 || VSTEffect.Instance.effectHandle == null || VSTEffect.Instance.useEffect == false)
            {
                this.Close();
            }

        }

        private void Window_Closed(object sender, EventArgs e)
        {
            vstEffect.SaveVSTSettings();
            
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            vstEffect.OpenVSTConfigurationPage(VSTEffect.Instance.effectHandle, (nint)null);
        }

        private void Window_ContentRendered(object sender, EventArgs e)
        {

            var wih = new WindowInteropHelper(this);
            IntPtr hWnd = wih.Handle;
            vstEffect.OpenVSTConfigurationPage(VSTEffect.Instance.effectHandle, hWnd);
            Task.Delay(1000).Wait();
        }
    }
}
