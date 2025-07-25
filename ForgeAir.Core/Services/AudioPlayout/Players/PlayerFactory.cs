using ForgeAir.Core.AudioEngine.Enums;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.DeviceManager;
using ForgeAir.Core.Services.DeviceManager.Interfaces;
using ForgeAir.Core.Services.Scheduler.Interfaces;
using ForgePlugin.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.AudioPlayout.Players
{
    public class PlayerFactory : IPlayerFactory
    {
        private readonly IEventAggregator _events;
        private readonly IQueueService _queue;
        private readonly ISchedulerService _scheduler;
        private readonly Helpers.Interfaces.IConfigurationManager _config;
        public PlayerFactory(
            IEventAggregator events,
            IQueueService queue,
            ISchedulerService scheduler,
            Helpers.Interfaces.IConfigurationManager config)
        {
            _events = events;
            _queue = queue;
            _scheduler = scheduler;
            _config = config;
        }

        public IPlayer CreatePlayer()
        {
            var engine = _config.Get("MainOutput", "AudioEngine");
            
            return engine switch
            {
                "Bass" => CreateBassPlayerWithDevice(),
                "NAudio" => CreateNAudioPlayerWithDevice(),
                _ => throw new InvalidOperationException("Invalid audio engine")
            };
        }

        private IPlayer CreateBassPlayerWithDevice()
        {
            var device = new BassDevice
            {
                TargetDevice = LoadTargetDeviceFromConfig()
            };
            BassManagerFactory.Create(device).InitDevice();
            return new BassPlayer(device, _events, _queue, _scheduler);
        }

        private IPlayer CreateNAudioPlayerWithDevice()
        {
            var device = new NAudioDevice
            {
                TargetDevice = LoadTargetDeviceFromConfig()
            };
            NAudioManagerFactory.Create(device).InitDevice();
            return new NAudioPlayer(device, _events, _queue, _scheduler);
        }

        private OutputDevice LoadTargetDeviceFromConfig()
        {
            // Read and parse config values (or fallback to defaults)
            return new OutputDevice
            {
                API = DeviceOutputMethodEnum.MME,
                BitDepth = DeviceOutputBitDepthEnum.SixteenBit,
                Channels = 2,
                SampleRate = 44100,
                BufferLength = 333,
                MMEaudioChannels = MMEDeviceOutputAudioChannelsEnum.Stereo,
                Type = DeviceTypeEnum.Main,
                Index = -1
            };
        }

    }

}
