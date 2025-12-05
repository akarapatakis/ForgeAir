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
#if WINDOWS

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

        /// <summary>
        /// Does the same as ListDevicesByAPI but External to allow "emegerncy" calls from places that do not have access to a DeviceManager
        /// </summary>
        /// <param name="api">Audio Driver (MME, WASAPI, DSound, ASIO)</param>
        public static List<OutputDevice> ListDevicesByAPI_Ex(DeviceOutputMethodEnum api)
        {
            switch (api)
            {
                case DeviceOutputMethodEnum.MME:
                    return getMMEOutDevices();
                case DeviceOutputMethodEnum.DirectSound:
                    return getMMEOutDevices();
                case DeviceOutputMethodEnum.WASAPI:
                    return getWASAPIOutDevices();
                case DeviceOutputMethodEnum.ASIO:
                    return getASIODevices();
            }
            return new List<OutputDevice>();
        }
        public List<OutputDevice> ListDevicesByAPI(DeviceOutputMethodEnum api)
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
            return new List<OutputDevice>();
        }

        private static List<OutputDevice> getASIODevices()
        {
            List<OutputDevice> devices = new List<OutputDevice>();

            foreach (var asio in AsioOut.GetDriverNames())
            {
              devices.Add(new OutputDevice { API = DeviceOutputMethodEnum.ASIO, Index = devices.Count, Name = asio });
            }
            return devices;
        }

        private static List<OutputDevice> getWASAPIOutDevices()
        {
            List<OutputDevice> devices = new List<OutputDevice>();
            var enumerator = new MMDeviceEnumerator();
            foreach (var wasapi in enumerator.EnumerateAudioEndPoints(DataFlow.Render, DeviceState.Active))
            {
                if (!(wasapi.State == DeviceState.Active))
                {
                    continue;
                }
                devices.Add(new OutputDevice { API = DeviceOutputMethodEnum.WASAPI, Index = devices.Count, Name = wasapi.FriendlyName });
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

        private static List<OutputDevice> getMMEOutDevices()
        {

            List<OutputDevice> devices = new List<OutputDevice>();
            for (int n = -1; n < WaveOut.DeviceCount; n++)
            {
                var caps = WaveOut.GetCapabilities(n);
                devices.Add(new OutputDevice { API = DeviceOutputMethodEnum.MME, Index = n, Name = caps.ProductName });
            }
            return devices;
        }
        private static List<OutputDevice> getDSoundOutDevices()
        {
            List<OutputDevice> devices = new List<OutputDevice>(); 

            foreach (var dev in DirectSoundOut.Devices)
            {
                devices.Add(new OutputDevice { API = DeviceOutputMethodEnum.DirectSound, Index = devices.Count, Name = dev.ModuleName, Guid = dev.Guid });
            }
            return devices;
        }

        public void Dispose()
        {
            throw new NotImplementedException();
        }
    }
#endif

}
