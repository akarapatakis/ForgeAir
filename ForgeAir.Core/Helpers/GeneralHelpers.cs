using ForgeAir.Core.AudioEngine.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MediaInfo;
using Microsoft.Extensions.Logging;

namespace ForgeAir.Core.Helpers
{
    public class GeneralHelpers
    {
        public DeviceOutputMethodEnum? ToDeviceOutputMethodEnum(string input)
        {
            if (input == null || input == "") { return null; }

            switch (input) {

                case "MME":
                    return DeviceOutputMethodEnum.MME;
                case "DSound":
                    return DeviceOutputMethodEnum.DirectSound;
                case "DirectSound":
                    return DeviceOutputMethodEnum.DirectSound;
                case "WASAPI":
                    return DeviceOutputMethodEnum.WASAPI;
                case "ASIO":
                    return DeviceOutputMethodEnum.ASIO;
            }
            return null;
        }

        public GenericDeviceOutputAudioChannelsEnum? ToGenericDeviceOutputAudioChannelsEnum(string input)
        {
            if (input == null || input == "") { return null; }

            switch (input) {

                case "1":
                    return GenericDeviceOutputAudioChannelsEnum.Mono;
                case "2":
                    return GenericDeviceOutputAudioChannelsEnum.Stereo;
            }
            return null;
        }

        public WASAPIDeviceOutputAudioChannelsEnum? ToWASAPIDeviceOutputAudioChannelsEnum(string input)
        {
            if (input == null || input == "") { return null; }

            switch (input)
            {

                case "1":
                    return WASAPIDeviceOutputAudioChannelsEnum.Mono;
                case "2":
                    return WASAPIDeviceOutputAudioChannelsEnum.Stereo;
            }
            return null;
        }

        public MMEDeviceOutputAudioChannelsEnum? ToMMEDeviceOutputAudioChannelsEnum(string input)
        {
            if (input == null || input == "") { return null; }

            switch (input)
            {

                case "0":
                    return MMEDeviceOutputAudioChannelsEnum.Mono;
                case "1":
                    return MMEDeviceOutputAudioChannelsEnum.Stereo;
                case "2":
                    return MMEDeviceOutputAudioChannelsEnum.Stereo;
            }
            return null;
        }

        public int MMEToMixerChans(MMEDeviceOutputAudioChannelsEnum chans)
        {
            switch (chans)
            {
                case MMEDeviceOutputAudioChannelsEnum.Mono:
                    return 1;
                case MMEDeviceOutputAudioChannelsEnum.Stereo:
                    return 2;
            }
            return 0;
        }

        
        public bool isThisAnAudioFile(string fileName)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => { });
            ILogger logger = factory.CreateLogger("Program");

            var media = new MediaInfoWrapper(fileName, logger);
            if (media.AudioStreams.Count > 0)
            {
                return true;
            }
            return false;

        }
        public Database.Models.Track DTOToTrack(DTO.Track track)
        {
            return new Database.Models.Track()
            {
                Id = track.Id,
                Title = track.Title,
                FilePath = track.FilePath,
                Album = track.Album,
                Duration = track.Duration,
            };
        }
    }
}
