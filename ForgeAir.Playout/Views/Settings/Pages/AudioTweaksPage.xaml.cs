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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Shared;

namespace ForgeAir.Playout.Views.Settings.Pages
{
    /// <summary>
    /// Interaction logic for AudioTweaksPage.xaml
    /// </summary>
    public partial class AudioTweaksPage : Page
    {
        ConfigurationManager configurationManager;
        public AudioTweaksPage()
        {
            configurationManager = new ConfigurationManager("configuration.ini");
            InitializeComponent();

            untouchedRadio.IsChecked = configurationManager.GetBool("Built-In DSP", "Enabled");
            amRadio.IsChecked = configurationManager.GetBool("Built-In DSP", "AM");
            cquamRadio.IsChecked = configurationManager.GetBool("Built-In DSP", "AMStereo");
            fmRadio.IsChecked = configurationManager.GetBool("Built-In DSP", "FM");
        }

        private void radioButton_Checked(object sender, RoutedEventArgs e)
        {

        }

        private void button_Click(object sender, RoutedEventArgs e)
        {
            if (untouchedRadio.IsChecked == true)
            {
                configurationManager.Set("Built-In DSP", "Enabled", "0");
            }
            else if (cquamRadio.IsChecked == true)
            {
                configurationManager.Set("Built-In DSP", "Enabled", "1");
                configurationManager.Set("Built-In DSP", "AMStereo", "1");
            }
            else if (fmRadio.IsChecked == true)
            {
                configurationManager.Set("Built-In DSP", "Enabled", "1");
                configurationManager.Set("Built-In DSP", "FM", "1");
            }
            else if (amRadio.IsChecked == true)
            {
                configurationManager.Set("Built-In DSP", "Enabled", "1");
                configurationManager.Set("Built-In DSP", "AM", "1");
            }
            configurationManager.Save();
        }
    }
}
