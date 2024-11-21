///
/// Initial Code ripped off DemoMixer
///

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Services.Models;
using ForgeAir.Database.Models;
using ManagedBass;
using ManagedBass.Vst;
using ManagedBass.Wasapi;
namespace ForgeAir.Core.AudioEngine
{
    public class AudioPlayer : Interfaces.IAudioPlayer
    {
        int stream;

        public void FadeNext()
        {
            Bass.ChannelSlideAttribute(stream, ChannelAttribute.Volume, 0, Shared.AudioPlayerRealTimeParams.Instance.fadeNextTimeInMs/2);
            stream = Bass.CreateStream(Shared.AudioPlayerRealTimeParams.Instance.audioFile, 0, 0, BassFlags.AutoFree);
            Bass.ChannelSetAttribute(stream, ChannelAttribute.Volume, 0);
            Bass.ChannelSlideAttribute(stream, ChannelAttribute.Volume, 1, Shared.AudioPlayerRealTimeParams.Instance.fadeNextTimeInMs/2);
            Bass.ChannelPlay(stream, Shared.AudioPlayerRealTimeParams.Instance.repeatTrack);
        }

        public void FadeStop()
        {
            Bass.ChannelSlideAttribute(stream, ChannelAttribute.Volume, 0, Shared.AudioPlayerRealTimeParams.Instance.stopFadeTimeInMs);
            Bass.ChannelStop(stream);
        }

        public void Next()
        {
            throw new NotImplementedException();
        }

        public void Pause()
        {
            if (Bass.ChannelIsActive(stream) == PlaybackState.Playing)
            {
                Bass.ChannelPause(stream);
            }
            else
            {
                Bass.ChannelPlay(stream, false);
            }
        }

        public static int GetRemainingMilliseconds(int channel)
        {
            // Get the total length in bytes and convert to seconds
            long length = Bass.ChannelGetLength(channel);
            double totalSeconds = Bass.ChannelBytes2Seconds(channel, length);

            // Get the current position in bytes and convert to seconds
            long position = Bass.ChannelGetPosition(channel);
            double currentSeconds = Bass.ChannelBytes2Seconds(channel, position);

            // Calculate remaining time in milliseconds and round to an integer
            double remainingSeconds = totalSeconds - currentSeconds;
            return (int)(remainingSeconds * 1000); // Convert seconds to milliseconds and cast to int
        }

        public async Task Play(Services.Models.Track track)
        {
            Thread crossfade = new Thread(() => PlayNext(track));
            
            Shared.AudioPlayerRealTimeParams.Instance.audioFile = track.FilePath;

            stream = Bass.CreateStream(Shared.AudioPlayerRealTimeParams.Instance.audioFile, 0, 0, BassFlags.AutoFree);

            Bass.ChannelPlay(stream, Shared.AudioPlayerRealTimeParams.Instance.repeatTrack);
            while (Bass.ChannelIsActive(stream) == PlaybackState.Playing)
            {
                if (GetRemainingMilliseconds(stream) == Shared.AudioPlayerRealTimeParams.Instance.crossfadeTimeInMs)
                {
                    Bass.ChannelSlideAttribute(stream, ChannelAttribute.Volume, 0, Shared.AudioPlayerRealTimeParams.Instance.crossfadeTimeInMs); // fade-out
                    crossfade.Start();
                }
            }
        }

        public void PlayNext(Services.Models.Track track) 
        {
            Shared.AudioPlayerRealTimeParams.Instance.audioFile = track.FilePath;
            stream = Bass.CreateStream(Shared.AudioPlayerRealTimeParams.Instance.audioFile, 0, 0, BassFlags.AutoFree); // create next stream
            Bass.ChannelSetAttribute(stream, ChannelAttribute.Volume, 0); // settings its volume to 0
            Bass.ChannelPlay(stream, Shared.AudioPlayerRealTimeParams.Instance.repeatTrack); // play next stream
            Bass.ChannelSlideAttribute(stream, ChannelAttribute.Volume, 1, Shared.AudioPlayerRealTimeParams.Instance.crossfadeTimeInMs); // fadein
        }

        public void Stop()
        {
            Bass.ChannelStop(stream);
        }

        public void Restart()
        {
            Bass.ChannelPlay(stream, true);
        }
    }
}
