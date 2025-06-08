using System;
using System.Collections.Generic;
using System.Linq;
using System.Management;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.AudioEngine.Enums;
using ForgeAir.Core.Shared;
using Unosquare.FFME;
using Unosquare.FFME.Common;

namespace ForgeAir.Core.VideoEngine
{
    class FFMEHelper
    {
        private MediaElement _mediaElement;

        public FFMEHelper(MediaElement? mediaElement) {
            _mediaElement = mediaElement;
            if (!Unosquare.FFME.Library.IsInitialized)
            {
                _init();
            }
        }


        
        private async void _init()
        {
            await Task.Run(() => initFFME());
        }
        private async Task initFFME()
        {
            await Unosquare.FFME.Library.LoadFFmpegAsync();
            return;
        }

        public static class DeviceManager
        {
            private static string[] getMMEOutDevices()
            {
                string[] devices = new string[] { };

                foreach (var device in Unosquare.FFME.Library.EnumerateLegacyAudioDevices())
                {
                    devices[device.DeviceId] = $"MME:{device.DeviceId}: {device.Name}";
                }
                Array.Resize(ref devices, Unosquare.FFME.Library.EnumerateLegacyAudioDevices().Count());
                return devices;
            }
            private static string[] getDSoundOutDevices()
            {
                string[] devices = new string[] { };
                int deviceCount = 0;

                foreach (var device in Unosquare.FFME.Library.EnumerateDirectSoundDevices())
                {
                    devices[deviceCount] = $"MME:{device.DeviceId}: {device.Name}";
                    deviceCount++;
                }
                Array.Resize(ref devices, Unosquare.FFME.Library.EnumerateLegacyAudioDevices().Count());
                return devices;
            }

            public static string[] GetDevices(DeviceOutputMethodEnum device)
            {
                switch (device)
                {
                    case DeviceOutputMethodEnum.MME:
                        return getMMEOutDevices();
                    case DeviceOutputMethodEnum.DirectSound:
                        return getMMEOutDevices();
                    case DeviceOutputMethodEnum.WASAPI:
                        break;
                    case DeviceOutputMethodEnum.ASIO:
                        break;
                }
                return Array.Empty<string>();
            }
        }
    }
}

