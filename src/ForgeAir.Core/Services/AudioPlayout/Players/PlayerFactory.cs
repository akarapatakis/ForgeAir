using ForgeAir.Core.AudioEngine.Enums;
using ForgeAir.Core.Events;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.DeviceManager;
using ForgeAir.Core.Services.DeviceManager.Interfaces;
using ForgeAir.Core.Services.Scheduler.Interfaces;
using ForgeAir.Core.Services.StreamingClient;
using ForgePlugin.Helpers;
using ManagedBass;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
        private readonly IServiceProvider _provider;
        private readonly Helpers.Interfaces.IConfigurationManager _config;
        public PlayerFactory(
            IEventAggregator events,
            IQueueService queue,
            ISchedulerService scheduler,
            IServiceProvider provider,
            Helpers.Interfaces.IConfigurationManager config)
        {
            _events = events;
            _queue = queue;
            _scheduler = scheduler;
            _provider = provider;
            _config = config;
        }

        public IPlayer CreatePlayer(DeviceTypeEnum outType)
        {
            var engine = _config.Get(DeviceManager.Converters.ToEnum.FromDeviceTypeEnum(outType), "AudioEngine");
            
            return engine switch
            {
                "Bass" => CreateBassPlayerWithDevice(outType),
                "NAudio" => CreateNAudioPlayerWithDevice(outType),
                _ => throw new InvalidOperationException("Invalid audio engine")
            };
        }

        private IPlayer CreateBassPlayerWithDevice(DeviceTypeEnum outType)
        {
            var device = new BassDevice
            {
                TargetDevice = LoadTargetDeviceFromConfig(outType)
            };

            BassManagerFactory.Create(device).InitDevice();
            Debug.WriteLine(Bass.LastError);
            var player = new BassPlayer(device, _events, _queue, _scheduler, _provider, _provider.GetRequiredService<TrackChangedEvent>());
            if (_config.GetBool("Streaming", "Autoconnect", false))
            {
                Task.Run(() => _provider.GetRequiredService<StreamingClientService>().CreateEncoder(device).StartStreaming());
            }
            return player;
        }

        private IPlayer CreateNAudioPlayerWithDevice(DeviceTypeEnum outType)
        {
            var device = new NAudioDevice
            {
                TargetDevice = LoadTargetDeviceFromConfig(outType)
            };
            #if WINDOWS
            NAudioManagerFactory.Create(device).InitDevice();
            return new NAudioPlayer(device, _events, _queue, _scheduler, _provider.GetRequiredService<TrackChangedEvent>());
            #endif
            throw new Exception("NAudio is not supported on non-Windows platform");
        }

        private OutputDevice LoadTargetDeviceFromConfig(DeviceTypeEnum type)
        {
            return new OutputDevice
            {
                Index = _config.GetInt(DeviceManager.Converters.ToEnum.FromDeviceTypeEnum(type), "ID"),
                API = DeviceManager.Converters.ToEnum.ToDeviceAPI(_config.Get(DeviceManager.Converters.ToEnum.FromDeviceTypeEnum(type), "API")),
                BitDepth = DeviceManager.Converters.ToEnum.ToDeviceOutputBitDepthEnum(_config.GetInt(DeviceManager.Converters.ToEnum.FromDeviceTypeEnum(type), "BitDepth")),
                Channels = _config.GetInt(DeviceManager.Converters.ToEnum.FromDeviceTypeEnum(type), "Channels"),
                SampleRate = _config.GetInt(DeviceManager.Converters.ToEnum.FromDeviceTypeEnum(type), "SampleRate"),
                BufferLength = _config.GetInt(DeviceManager.Converters.ToEnum.FromDeviceTypeEnum(type), "BufferLength"),
                MMEaudioChannels = MMEDeviceOutputAudioChannelsEnum.Stereo, // todo: fix later
                Type = type,
            };
        }

    }

}
