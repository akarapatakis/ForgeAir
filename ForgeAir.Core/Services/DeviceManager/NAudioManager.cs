using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.AudioEngine.Enums;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.DeviceManager.Interfaces;
using NAudio.CoreAudioApi;
using NAudio.Wave;

namespace ForgeAir.Core.Services.DeviceManager
{
    public class NAudioManager : IDeviceManager, IDisposable
    {
        private WaveOutEvent? _wvOut;
        private DirectSoundOut? _dsOut;
        private WasapiOut? _wasapiOut;
        private AsioOut? _asioOut;
        private NAudioDevice? _device;


        public NAudioManager(NAudioDevice device)
        {
            _device = device;
        }
        public int FreeDevice()
        {
            throw new NotImplementedException();
        }

        public int InitDevice()
        {
            if (_device == null) {
                return -1;
            }
            switch (_device.TargetDevice.API)
            {
                case DeviceOutputMethodEnum.MME:
                    _device.WaveOutAPI = new WaveOutEvent { DeviceNumber = _device.TargetDevice.Index, DesiredLatency = _device.TargetDevice.BufferLength };
                    return 0;

                case DeviceOutputMethodEnum.DirectSound:
                    _device.DSoundAPI = new DirectSoundOut((Guid)GetDirectSoundDeviceGuidByIndex(_device.TargetDevice.Index));
                    
                    return 0;

                case DeviceOutputMethodEnum.WASAPI:
                    _device.WasapiOutAPI = new WasapiOut((MMDevice)GetWasapiDeviceByIndex(_device.TargetDevice.Index), AudioClientShareMode.Shared, true, _device.TargetDevice.BufferLength);
                    return 0;

                case DeviceOutputMethodEnum.ASIO:
                    _device.AsioOutAPI = new AsioOut(_device.TargetDevice.Name);
                    return 0;

                default:
                    return -1;
            }
        }

        public static string[] ListDevicesByAPI(DeviceOutputMethodEnum api)
        {
            switch (api)
            {
                case DeviceOutputMethodEnum.MME:
                    return getMMEOutDevices();
                case DeviceOutputMethodEnum.DirectSound:
                    return getDSoundOutDevices();
                case DeviceOutputMethodEnum.WASAPI:
                    return getWASAPIOutDevices();
                case DeviceOutputMethodEnum.ASIO:
                    return getASIODevices();
            }
            return Array.Empty<string>();
        }

        private static string[] getASIODevices()
        {
            string[] devices = new string[] { };
            foreach (var asio in AsioOut.GetDriverNames())
            {
                devices.Append(asio);
            }
            return devices;
        }

        private static string[] getWASAPIOutDevices()
        {
            string[] devices = new string[] { };
            var enumerator = new MMDeviceEnumerator();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.All, DeviceState.Active))
            {
                Console.WriteLine($"{wasapi.DataFlow} {wasapi.FriendlyName} {wasapi.DeviceFriendlyName} {wasapi.State}");
            }
            return devices;
        }
        private static MMDevice? GetWasapiDeviceByIndex(int index)
        {
            var devices = new MMDeviceEnumerator().EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active).ToList();
            return index >= 0 && index < devices.Count ? devices[index] : null;
        }

        private static Guid? GetDirectSoundDeviceGuidByIndex(int index)
        {
            var devices = DirectSoundOut.Devices.ToList();
            return index >= 0 && index < devices.Count ? devices[index].Guid : null;
        }

        private static string[] getMMEOutDevices()
        {
            string[] devices = new string[] { };

            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                devices.Append(caps.NameGuid.ToString());
                Console.WriteLine($"{n}: {caps.ProductName}");
            }
            return devices;
        }
        private static string[] getDSoundOutDevices()
        {
            string[] devices = new string[] { };
            foreach (var dev in DirectSoundOut.Devices)
            {
                Console.WriteLine($"{dev.ModuleName} | ({dev.Guid})");
            }
            return devices;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
}
