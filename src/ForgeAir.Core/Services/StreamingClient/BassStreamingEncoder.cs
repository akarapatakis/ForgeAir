using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Models;
using ForgeAir.Core.Models.Enums;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.StreamingClient.Interfaces;
using ManagedBass;
using ManagedBass.Aac;
using ManagedBass.Enc;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.StreamingClient
{
    public class BassStreamingEncoder : IStreamingClient
    {

        private readonly StreamingEncoder _encoder;
        private readonly IPlayer player;
        private readonly BassDevice source;
        private int encoderHandle;
        public BassStreamingEncoder(StreamingEncoder enc, BassDevice src) { 
            source = src;
            _encoder = enc;
        
        }
        public async Task<string> StartStreaming()
        {

            await Task.Run(() => InitEncoder());

            if (Bass.LastError != Errors.OK)
            {
                throw new Exception(BassCheatsheet.LastErrorToDetailedError(Bass.LastError));
            }
            string serverUrl = _encoder.ServerURL.TrimEnd('/');
            string mount = _encoder.Mountpoint.TrimStart('/'); 

            string fullUrl = $"{serverUrl}:{_encoder.ServerPort}/{mount}";
            bool castStatus = ManagedBass.Enc.BassEnc.CastInit(
                Handle: encoderHandle,
                Server: fullUrl,
                Url: _encoder.StreamHomepage,
                
                Genre: _encoder.Genre,
                Bitrate: _encoder.Bitrate,
                Password: _encoder.Password,
                Content: StreamingencoderFormatToMime(_encoder.Format),
                Name: _encoder.StreamName,
                Description: "ForgeAir",
                Headers: null,
                Public: true
            );
            if (castStatus != true)
            {
                throw new Exception(BassCheatsheet.LastErrorToDetailedError(Bass.LastError));
            }
            return ManagedBass.Bass.LastError.ToString();

        }

        private async Task InitEncoder()
        {
            switch (_encoder.Format)
            {
                case StreamingEncoderFormat.MP3:
                    encoderHandle = BassEnc_Mp3.Start(
                       source.Handle,
                       null,
                       EncodeFlags.UnlimitedCastDataRate | EncodeFlags.NoHeader,
                       _encoder.Bitrate.ToString());
                    if (Bass.LastError != Errors.OK)
                    {
                        throw new Exception(BassCheatsheet.LastErrorToDetailedError(Bass.LastError));
                    }
                    break;

                case StreamingEncoderFormat.OGG:
                    encoderHandle = BassEnc_Ogg.Start(
                       source.Handle,
                       null,
                       EncodeFlags.UnlimitedCastDataRate | EncodeFlags.NoHeader,
                       _encoder.Bitrate.ToString());
                    if (Bass.LastError != Errors.OK)
                    {
                        throw new Exception(BassCheatsheet.LastErrorToDetailedError(Bass.LastError));
                    }
                    break;
                
                case StreamingEncoderFormat.AAC:
                    break;
                default:
                    break;
            }

        }
        public void StopStreaming()
        {
            BassEnc.EncodeStop(encoderHandle);
        }

        private static string StreamingencoderFormatToMime(StreamingEncoderFormat f)
        {
            switch (f)
            {
                case StreamingEncoderFormat.MP3:
                    return BassEnc.MimeMp3;
                case StreamingEncoderFormat.OGG:
                    return BassEnc.MimeOgg;
                case StreamingEncoderFormat.AAC:
                    return BassEnc.MimeAac;
                default: return BassEnc.MimeMp3;
            }
        }
        public void UpdateNowPlaying(TrackDTO newTrack)
        {
            BassEnc.CastSetTitle(encoderHandle, newTrack.DisplayArtists + " - " + newTrack.Title, null);
        }
    }
}
