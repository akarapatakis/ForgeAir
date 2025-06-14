using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Shared;
using ForgeAir.Playout.Views.VST;
using ManagedBass.Vst;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ForgeAir.Playout.Views.Settings.Pages
{
    /// <summary>
    /// Interaction logic for AudioVSTConfigPage.xaml
    /// </summary>
    public partial class AudioVSTConfigPage : Page
    {
        VSTEffectManager plugin;
        ConfigurationManager configuration;
        public AudioVSTConfigPage()
        {
            InitializeComponent();
            
            plugin = new VSTEffectManager();
            configuration = new ConfigurationManager("configuration.ini");

            //if (VSTEffect.Instance.effectInfo.Equals == null) { return; }
            useVSTCheckbox.IsChecked = Core.Shared.VSTEffect.Instance.useEffect;

            if (!Core.Shared.VSTEffect.Instance.useEffect)
            {
                openEditorVST.IsEnabled = false;
            }
            pluginPath.Text = VSTEffect.Instance.effectPath;

           // pluginAuthor.Text = VSTEffect.Instance.effectInfo.VendorName;
           // pluginVersion.Text = VSTEffect.Instance.effectInfo.EffectVersion.ToString();

        }

        private void openEditorVST_Click(object sender, RoutedEventArgs e)
        {
            if (!VSTEffect.Instance.useEffect)
            {
                HandyControl.Controls.MessageBox.Show("No VST Effect found", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
            }
            VSTPluginEditor window = new VSTPluginEditor();
            window.Show();
        }

        private void selctVSTbtn_Click(object sender, RoutedEventArgs e)
        {
            Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
            openFileDialog.Multiselect = false;
            openFileDialog.Title = "Select VST Plugin";
            openFileDialog.Filter = "VST2 Plugins|*.dll";

            if (openFileDialog.ShowDialog() == true)
            {
                if (!File.Exists(openFileDialog.FileName)) {
                    HandyControl.Controls.MessageBox.Show("No plugin selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                    return;

                }
                HandyControl.Controls.MessageBox.Show("The selected VST effect will be loaded in the next restart", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                configuration.Set("VST", "EffectPath", openFileDialog.FileName);
                return;

            }
        }

        private void useVSTCheckbox_Checked(object sender, RoutedEventArgs e)
        {
            if (Core.Shared.VSTEffect.Instance.useEffect == false)
            {
                HandyControl.Controls.MessageBox.Show("The VST effect will be enabled in the next restart", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                configuration.Set("VST", "Enabled", "1");
                Core.Shared.VSTEffect.Instance.useEffect = true;
            }
            else if (Core.Shared.VSTEffect.Instance.useEffect == true)
            {
                HandyControl.Controls.MessageBox.Show("The VST effect will be disabled in the next restart", "Information", MessageBoxButton.OK, MessageBoxImage.Information);
                configuration.Set("VST", "Enabled", "0");
                Core.Shared.VSTEffect.Instance.useEffect = false;
            }
        }

        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            configuration.Save();
            switch (useVSTCheckbox.IsChecked)
            {
                case true:
                    Core.Shared.VSTEffect.Instance.useEffect = true;
                    break;
                case false:
                    Core.Shared.VSTEffect.Instance.useEffect = false;
                    break;
            }
           
        }
    }
}
