using ForgeAir.Core.AudioEngine.Enums;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.DeviceManager.Interfaces;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.DeviceManager
{
    public class DeviceManagerFactory
    {
        private readonly IConfigurationManager _config;
        private readonly IEventAggregator _events;

        public DeviceManagerFactory(IConfigurationManager config, IEventAggregator events)
        {
            _config = config;
            _events = events;
        }

        public IDeviceManager CreateManager()
        {
            var engine = _config.Get("MainOutput", "AudioEngine");

            return engine switch
            {
                "Bass" => CreateBassManager(),
                "NAudio" => CreateNAudioManager(),
                _ => throw new InvalidOperationException("Invalid audio engine")
            };
        }

        private IDeviceManager CreateBassManager()
        {
            var device = new BassDevice
            {
                TargetDevice = new OutputDevice
                {
                    API = DeviceOutputMethodEnum.MME,
                    BitDepth = DeviceOutputBitDepthEnum.SixteenBit,
                    Channels = 2,
                    SampleRate = 44100,
                    BufferLength = 333,
                    MMEaudioChannels = MMEDeviceOutputAudioChannelsEnum.Stereo,
                    Type = DeviceTypeEnum.Main,
                    Index = -1
                }
            };

            return new BassManager(device);
        }

        private IDeviceManager CreateNAudioManager()
        {
            var device = new NAudioDevice
            {
                TargetDevice = new OutputDevice
                {
                    API = DeviceOutputMethodEnum.MME,
                    BitDepth = DeviceOutputBitDepthEnum.SixteenBit,
                    Channels = 2,
                    SampleRate = 44100,
                    BufferLength = 333,
                    MMEaudioChannels = MMEDeviceOutputAudioChannelsEnum.Stereo,
                    Type = DeviceTypeEnum.Main,
                    Index = -1
                }
            };

            return new NAudioManager(device);
        }
    }

}
