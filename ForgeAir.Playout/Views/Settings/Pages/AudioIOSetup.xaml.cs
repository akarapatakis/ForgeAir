using System;
using System.Collections.Generic;
using System.Configuration;
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
using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.AudioEngine.Enums;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Shared;

namespace ForgeAir.Playout.Views.Settings.Pages
{
    /// <summary>
    /// Interaction logic for AudioIOSetup.xaml
    /// </summary>
    public partial class AudioIOSetup : Page
    {

        Core.Helpers.ConfigurationManager configurationManager = new Core.Helpers.ConfigurationManager("configuration.ini");
        public AudioIOSetup()
        {
            InitializeComponent();

            DeviceManager deviceManager = new DeviceManager();

            sampleRateCombo.Items.Add("32000");
            sampleRateCombo.Items.Add("44100");
            sampleRateCombo.Items.Add("48000");
            sampleRateCombo.Items.Add("96000");
            sampleRateCombo.Items.Add("192000");

            channelsCombo.Items.Add("1");
            channelsCombo.Items.Add("2");

            foreach (var device in deviceManager.GetDevices(DeviceOutputMethodEnum.MME))
            {
                devicesCombo.Items.Add(device);
            }
            foreach (var device in deviceManager.GetDevices(DeviceOutputMethodEnum.WASAPI))
            {
                devicesCombo.Items.Add(device);
            }
            foreach (var device in deviceManager.GetDevices(DeviceOutputMethodEnum.ASIO))
            {
                devicesCombo.Items.Add(device);
            }

            devicesCombo.SelectedItem = GetDeviceString(devicesCombo.Items, configurationManager.Get("Audio", "MainOutDeviceMethod"), Int32.Parse(configurationManager.Get("Audio", "MainOutChannels", "2")));
            sampleRateCombo.SelectedItem = configurationManager.Get("Audio", "MainOutSampleRate", "48000");
            channelsCombo.SelectedItem = configurationManager.Get("Audio", "MainOutChannels", "2");

            dSoundBox.IsChecked = configurationManager.GetBool("Audio", "MainOutUseDSound", false);
        }

        private void applyBtn_Click(object sender, RoutedEventArgs e)
        {
            var selectedDevice = devicesCombo.SelectedItem as string;

            if (selectedDevice != null)
            {
                var parts = selectedDevice.Split(':');
                if (parts.Length >= 2)
                {
                    configurationManager.Set("Audio", "MainOutDevice", int.Parse(parts[1]).ToString());
                    configurationManager.Set("Audio", "MainOutDeviceMethod", parts[0].ToString());
                }
            }
            configurationManager.Set("Audio", "MainOutSampleRate", sampleRateCombo.SelectedItem.ToString());
            configurationManager.Set("Audio", "MainOutChannels", channelsCombo.SelectedItem.ToString());
            configurationManager.Set("Audio", "MainOutUseDSound", ((bool)dSoundBox.IsChecked ? 1 : 0).ToString());
            configurationManager.Save();
            HandyControl.Controls.MessageBox.Show("The settings will be applied in the next restart", "Information", MessageBoxButton.OK, MessageBoxImage.Information);

        }



        public static string GetDeviceString(ItemCollection deviceList, string method, int index)
        {
            foreach (string device in deviceList)
            {
                var parts = device.Split(':');
                if (parts.Length >= 2 &&
                    parts[0].Equals(method, StringComparison.OrdinalIgnoreCase) &&
                    int.TryParse(parts[1], out int parsedIndex) &&
                    parsedIndex == index)
                {
                    return device;
                }
            }

            return null;
        }
    }
}
