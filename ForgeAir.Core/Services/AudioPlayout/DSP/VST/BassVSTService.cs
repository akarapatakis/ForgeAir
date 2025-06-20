using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST.Interfaces;
using ForgeAir.Core.Shared;
using ManagedBass;
using ManagedBass.Vst;

namespace ForgeAir.Core.Services.AudioPlayout.DSP.VST
{
    public class BassVSTService : IVSTService, IDisposable
    {
        public VSTPlugin _loadedPlugin { get; }
        private VSTConfigurationManager _configurationManager { get; }
        public BassDevice _targetDevice { get; }
        public BassVSTService(VSTPlugin plugin, BassDevice device)
        {
            _loadedPlugin = plugin;
            _targetDevice = device;
            _configurationManager = new VSTConfigurationManager(this);
        }

        public void Initialize()
        {
            Kill();
            if (!_loadedPlugin.Enabled)
            {
                return;
            }
            _targetDevice.vstHandle = 0;

            if (_targetDevice.Handle == 0)
            {
                throw new Exception("Invalid track handle: Cannot apply VST");
            }

            // Apply the VST effect to the existing stream
            int dspHandle = BassVst.ChannelSetDSP(_targetDevice.Handle, _loadedPlugin.Path, BassVstDsp.Default, 0);

            if (dspHandle == 0)
            {
                throw new Exception("Failed to apply VST DSP: " + Bass.LastError);
            }
            _configurationManager.RestoreSettings();
            _targetDevice.vstHandle = dspHandle;
        }
        public void Kill()
        {
            if (_targetDevice.vstHandle != 0)
            {
                BassVst.ChannelRemoveDSP(_targetDevice.Handle, _targetDevice.vstHandle);

                _targetDevice.vstHandle = 0;  // Clear handle to avoid further access
            }
        }
        public void Dispose()
        {
            if (_targetDevice.vstHandle != 0)
            {
                BassVst.ChannelRemoveDSP(_targetDevice.Handle, _targetDevice.vstHandle);

                _targetDevice.vstHandle = 0;  // Clear handle to avoid further access
            }
        }

        ~BassVSTService() { 
            this.Dispose();
        }
    }

    public class VSTConfigurationManager : IVSTConfigurationManager
    {
        internal protected BassVSTService _loadedService;

        public VSTConfigurationManager(BassVSTService activePlugin) { 
            _loadedService = activePlugin;
        }

        public void RestoreSettings()
        {
            if (_loadedService._loadedPlugin.Enabled == false || _loadedService._targetDevice.vstHandle == 0)
            {
                return;
            }

            if (!File.Exists(_loadedService._loadedPlugin.Path))
            {
                return;
            }

            string[] lines = File.ReadAllLines(_loadedService._loadedPlugin.Path);

            for (int i = 0; i < lines.Length; i++)
            {
                if (float.TryParse(lines[i], out float paramValue))
                {
                    BassVst.SetParam(_loadedService._targetDevice.vstHandle, i, paramValue);
                }
                else
                {
                    Console.WriteLine($"Skipping invalid parameter value: {lines[i]}");
                }
            }
        }



        public void SaveSettings()
        {
            if (_loadedService._loadedPlugin.Enabled == false || _loadedService._targetDevice.vstHandle == 0)
            {
                return;
            }

            File.Delete(_loadedService._loadedPlugin.Path);

            int paramCount = BassVst.GetParamCount(_loadedService._targetDevice.vstHandle);
            List<string> paramValues = new List<string>();

            for (int i = 0; i < paramCount; i++)
            {
                float value = BassVst.GetParam(_loadedService._targetDevice.vstHandle, i);
                paramValues.Add(value.ToString());
            }

            File.WriteAllLines(_loadedService._loadedPlugin.Path, paramValues);

            Console.WriteLine("VST parameters saved.");
        }
    }
}
