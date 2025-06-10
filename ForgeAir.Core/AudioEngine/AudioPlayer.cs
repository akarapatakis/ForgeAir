using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Security.Policy;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using FFmpeg.AutoGen;
using ForgeAir.Core.Services;
using ForgeAir.Core.Services.Models;
using ForgeAir.Core.Shared;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Core.WebEncoder;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using ManagedBass;
using ManagedBass.Hls;
using ManagedBass.Mix;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using SharpDX.Direct3D9;
using SharpDX.Win32;
using Track = ForgeAir.Database.Models.Track;

namespace ForgeAir.Core.AudioEngine
{
    public class AudioPlayer : IDisposable
    {
        private readonly ForgeAir.Core.AudioEngine.TagReader _tagReader;
        private readonly WebEncoder.NowPlaying _nowPlayingWeb;
        private readonly Pilots.RandomPilot _randomPilot;
        private readonly VSTEffectManager _vstEffect;
        private int _trackHandle;
        private CancellationTokenSource _crossfadeCts = new();
        private static readonly TaskCompletionSource<bool> _playbackCompletion = new();
        private bool willPlaySweeper = false;
        public bool isPaused = false;
        public bool isPlaying = false;
        public AudioPlayer()
        {
            _vstEffect = new VSTEffectManager();
            _randomPilot = new Pilots.RandomPilot();
            _tagReader = new TagReader();
            _nowPlayingWeb = new WebEncoder.NowPlaying();

            AudioPlayerShared.Instance.onTrackChanged += OnTrackChanged;
            AudioPlayerShared.Instance.onQueueChanged += OnQueueChanged;
            Bass.PluginLoad("basshls.dll");
            Bass.PluginLoad("bass_aac.dll");
            Bass.PluginLoad("bassalac.dll");
            Bass.PluginLoad("bass_ac3.dll");
            Bass.PluginLoad("bass_fx.dll");
            Bass.PluginLoad("bass_ffmpeg.dll");
            Bass.PluginLoad("basswv.dll");
            Bass.PluginLoad("bassdsd.dll");
            Bass.PluginLoad("bass_dshow.dll");
            Bass.PluginLoad("bass_dts.dll");
            Bass.PluginLoad("basswebm.dll");
            Bass.PluginLoad("basswma.dll");
            Bass.PluginLoad("bassopus.dll");
            Bass.Configure(Configuration.SuppressMP3ErrorCorruptionSilence, true);
            Bass.Configure(Configuration.SRCQuality, 3);

        }

        //public async Task<int> GetHandleWithSafety()
       // {
            //return handle
        //}

        public async Task FillQueue()
        {
            while (true)
            {
                var track = await Task.Run(() => _randomPilot.selectRandomTrack());
                if (track == null)
                {
                    continue;
                }
                if (!(track == AudioPlayerShared.Instance.currentTrack)) {
                    if (File.Exists(track.FilePath))
                    {
                        AudioPlayerShared.Instance.trackQueue.EnqueueAtBottom(track);
                        if (track.Intro != null && !track.containsVideoTrack)
                        {
                            Database.Models.Track sweeper;
                            while (true)
                            {
                                var newSweeper = await Task.Run(() => _randomPilot.selectRandomSweeper());
                                if (newSweeper == null || newSweeper.TrackStatus == TrackStatus.Disabled ||
                                    newSweeper.TrackType != Database.Models.Enums.TrackType.Sweeper ||
                                    newSweeper.Duration > track.Intro)
                                {

                                    SweeperShared.Instance.sweeper = null;
                                    SweeperShared.Instance.targetTrack = null;
                                    willPlaySweeper = false;
                                    continue;
                                }
                                else
                                {
                                    sweeper = newSweeper;
                                    SweeperShared.Instance.sweeper = newSweeper;
                                    SweeperShared.Instance.targetTrack = track;
                                    willPlaySweeper = true;
                                    SweeperShared.Instance.RaiseOnSweeperChanged();
                                    break;
                                }
                            }
                        }

                        if (AudioPlayerShared.Instance.trackQueue.Any())
                        {
                            AudioPlayerShared.Instance.RaiseOnQueueChanged();
                        }
                        return;
                    }
                }
            }
        }

        private async void OnQueueChanged(object sender, EventArgs e)
        {
            if (!AudioPlayerShared.Instance.trackQueue.Any())
            {
                await FillQueue();
            }
        }

        public async Task<int> GetRemainingMilliseconds(int channel)
        {
            long length = Bass.ChannelGetLength(channel);
            double totalSeconds = Bass.ChannelBytes2Seconds(channel, length);

            long position = Bass.ChannelGetPosition(channel);
            double currentSeconds = Bass.ChannelBytes2Seconds(channel, position);

            return (int)((totalSeconds - currentSeconds) * 1000);
        }

        public async Task<TimeSpan> GetElapsedTime()
        {
            if (AudioPlayerShared.Instance.currentMainBassMixerHandle == 0 ||
                Bass.ChannelIsActive(_trackHandle) != PlaybackState.Playing)
            {
                return TimeSpan.Zero;
            }

            long position = Bass.ChannelGetPosition(_trackHandle);
            return TimeSpan.FromSeconds(Bass.ChannelBytes2Seconds(_trackHandle, position));
        }
        public async Task SeekTo(TimeSpan time)
        {
            if (AudioPlayerShared.Instance.currentMainBassMixerHandle == 0)
            {
                return;
            }
            Task.Run(() => Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, time.TotalSeconds)));

            return;
        }
        public async Task<double> GetTrackLengthInSecs()
        {
            return Bass.ChannelBytes2Seconds(_trackHandle, Bass.ChannelGetLength(_trackHandle));
        }


        public bool IsPlaying()
        {
            return Bass.ChannelIsActive(_trackHandle) == PlaybackState.Playing;
        }

        public async Task PlayFX(Database.Models.FX fx)
        {
            int _fxHandle = Bass.CreateStream(fx.FilePath, 0, 0, BassFlags.MusicSensitiveRamping | BassFlags.AutoFree | BassFlags.AsyncFile);


            BassMix.MixerAddChannel(AudioPlayerShared.Instance.currentMainBassMixerHandle, _fxHandle, BassFlags.Default | BassFlags.Float);
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 0.2f, (int)fx.Duration.TotalMilliseconds / 2);
            Bass.ChannelPlay(_fxHandle, false);
            await Task.Delay((int)fx.Duration.TotalMilliseconds);
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 1.0f, (int)fx.Duration.TotalMilliseconds / 2);
            Bass.ChannelStop(_fxHandle);
            BassMix.MixerRemoveChannel(_fxHandle);
            Bass.StreamFree(_fxHandle);
            return;
        }

        public async Task Play()
        {

             isPaused = false;
             if (_crossfadeCts != null)
             {
                 _crossfadeCts.Cancel();
             }
             if (!AudioPlayerShared.Instance.trackQueue.Any())
             {
                 await FillQueue();
             }

             var fadeInThread = new Thread(FadeIn);
             var fadeOutThread = new Thread(FadeOut);

             if (_trackHandle != 0)
             {
                 fadeOutThread.Start();
                 await Task.Delay(333); // keep the stream handle untouched and alive while the crossfade is being done
             }

             AudioPlayerShared.Instance.currentTrack = AudioPlayerShared.Instance.trackQueue.Dequeue();

             if (AudioPlayerShared.Instance.currentTrack == null)
             {
                 return;
             }
             
             if (Shared.AudioPlayerShared.Instance.currentTrack.TrackType != Database.Models.Enums.TrackType.Rebroadcast)
             {
                 _trackHandle = Bass.CreateStream(
                     AudioPlayerShared.Instance.currentTrack.FilePath, 0, 0,
                     BassFlags.Decode | BassFlags.Prescan | BassFlags.AsyncFile | BassFlags.Float
                 );

                Debug.WriteLine("create stream:" + Bass.LastError.ToString());
                 AudioPlayerShared.Instance.crossfadeTimeInMs =
         (int)(AudioPlayerShared.Instance.currentTrack.EndPoint.Value.TotalMilliseconds - AudioPlayerShared.Instance.currentTrack.MixPoint?.TotalMilliseconds ?? 0);
                 if (AudioPlayerShared.Instance.crossfadeTimeInMs == 0 || AudioPlayerShared.Instance.crossfadeTimeInMs == null)
                 {
                     AudioPlayerShared.Instance.crossfadeTimeInMs = 333;
                 }
                Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 0);
                Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, AudioPlayerShared.Instance.currentTrack.StartPoint.Value.TotalSeconds), PositionFlags.Bytes); // set start position
                BassMix.MixerAddChannel(AudioPlayerShared.Instance.currentMainBassMixerHandle, _trackHandle, BassFlags.Default | BassFlags.Float);
                Debug.WriteLine("mixer:" + Bass.LastError.ToString());
                Bass.ChannelPlay(AudioPlayerShared.Instance.currentMainBassMixerHandle, Shared.AudioPlayerShared.Instance.repeatTrack);
                Debug.WriteLine("channelplay:" + Bass.LastError.ToString());

                fadeInThread.Start();
                 AudioPlayerShared.Instance.RaiseOnTrackChanged();
                 if (willPlaySweeper) { 
                     await Task.Delay((int)(AudioPlayerShared.Instance.currentTrack.Intro.Value.TotalMilliseconds - SweeperShared.Instance.sweeper.Duration.TotalMilliseconds));
                     await Task.Run(() => HookSweeperNowAsync(SweeperShared.Instance.sweeper, SweeperShared.Instance.targetTrack));
                 }
                 willPlaySweeper = false;
                 SweeperShared.Instance.sweeper = null;
                 SweeperShared.Instance.targetTrack = null;
                 SweeperShared.Instance.RaiseOnSweeperChanged();

                 if (!AudioPlayerShared.Instance.trackQueue.Any())
                 {
                     await FillQueue();
                 }
                 isPlaying = true;
                 _ = MonitorPlayback();

             }
             else
             {

                 Bass.ChannelStop(_trackHandle);
                 BassMix.MixerRemoveChannel(_trackHandle);
                 Bass.StreamFree(_trackHandle);
                 _trackHandle = 0;
                 _trackHandle = Bass.CreateStream(AudioPlayerShared.Instance.currentTrack.FilePath, 0, BassFlags.StreamStatus | BassFlags.Decode | BassFlags.AsyncFile | BassFlags.Float, null);
                 AudioPlayerShared.Instance.crossfadeTimeInMs = 0;
                 Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 0);
                 BassMix.MixerAddChannel(AudioPlayerShared.Instance.currentMainBassMixerHandle, _trackHandle, BassFlags.Default | BassFlags.Float);
                 Bass.ChannelPlay(AudioPlayerShared.Instance.currentMainBassMixerHandle, Shared.AudioPlayerShared.Instance.repeatTrack);
                 Debug.WriteLine(Bass.LastError.ToString());
                 fadeInThread.Start();
                 AudioPlayerShared.Instance.RaiseOnTrackChanged();
                 await Task.Delay(333); // being patient here to make MonitorPlayback() catchup correctly with the current track (fixes a rare issue)

                 _ = MonitorPlayback();
             }
            

        }

        public void PauseResumeHandler()
        {
            if (isPaused)
            {
                Resume();
            }
            else { Pause(); }
        }
        public void Pause()
        {
                Bass.ChannelPause(AudioPlayerShared.Instance.currentMainBassMixerHandle);
                isPaused = true;
        }

        public void Resume()
        {
            Bass.ChannelPlay(AudioPlayerShared.Instance.currentMainBassMixerHandle, Shared.AudioPlayerShared.Instance.repeatTrack);
            isPaused = false;
        }
        private async Task MonitorPlayback()
        {
            if (AudioPlayerShared.Instance.currentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
            {
                PlaybackState isBuffering = Bass.ChannelIsActive(_trackHandle);

                while (Bass.ChannelIsActive(_trackHandle) == PlaybackState.Playing || Bass.StreamGetFilePosition(_trackHandle) < 100)
                {

                    await Task.Delay(333);
                }
                _crossfadeCts = new CancellationTokenSource();
                isBuffering = Bass.ChannelIsActive(_trackHandle);
                await Task.Delay(500);
                if (_crossfadeCts != null && !_crossfadeCts.Token.IsCancellationRequested)
                {
                    _crossfadeCts.Cancel();

                    await PlayNextTrack();

                    return;
                }
                isBuffering = Bass.ChannelIsActive(_trackHandle);
            }
            else
            {
                while (Bass.ChannelIsActive(_trackHandle) == PlaybackState.Playing)
                {
                    var remainingTime = GetRemainingMilliseconds(_trackHandle).Result;
                    //Debug.WriteLine(remainingTime.ToString());
                    if (remainingTime <= Shared.AudioPlayerShared.Instance.crossfadeTimeInMs)
                    {

                        _crossfadeCts = new CancellationTokenSource();
                        break;
                    }

                    await Task.Delay(333);
                }
                await Task.Delay(500);
                if (_crossfadeCts != null && !_crossfadeCts.Token.IsCancellationRequested)
                {
                    _crossfadeCts.Cancel();

                    await PlayNextTrack();

                    return;
                }
            }
        }

        private async Task PlayNextTrack()
        {
            isPaused = false;

            if (_crossfadeCts != null)
            {
                _crossfadeCts.Cancel();

                _crossfadeCts = null; // hotfix to avoid overlapping streams since MonitorPlayback() will re-initialize it
            }

            var fadeInThread = new Thread(FadeIn);
            var fadeOutThread = new Thread(FadeOut);

            if (!AudioPlayerShared.Instance.trackQueue.Any())
            {
                await FillQueue();
            }
            if (_trackHandle != 0)
            {
                fadeOutThread.Start();
                await Task.Delay(333); // keep the stream handle untouched and alive while the crossfade is being done
            }
            AudioPlayerShared.Instance.currentTrack = AudioPlayerShared.Instance.trackQueue.Dequeue();

            if (AudioPlayerShared.Instance.currentTrack.TrackType != Database.Models.Enums.TrackType.Rebroadcast)
            {
                            _trackHandle = Bass.CreateStream(
                AudioPlayerShared.Instance.currentTrack.FilePath, 0, 0,
                BassFlags.Decode | BassFlags.Prescan | BassFlags.AsyncFile | BassFlags.Float
            );
                
                            AudioPlayerShared.Instance.crossfadeTimeInMs =
                (int)(AudioPlayerShared.Instance.currentTrack.EndPoint.Value.TotalMilliseconds - AudioPlayerShared.Instance.currentTrack.MixPoint?.TotalMilliseconds ?? 0);
                if (AudioPlayerShared.Instance.crossfadeTimeInMs == 0 || AudioPlayerShared.Instance.crossfadeTimeInMs == null)
                {
                    AudioPlayerShared.Instance.crossfadeTimeInMs = 333;
                }
                            Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 0);
                            BassMix.MixerAddChannel(AudioPlayerShared.Instance.currentMainBassMixerHandle, _trackHandle, BassFlags.Default | BassFlags.Float);
                            Bass.ChannelPlay(AudioPlayerShared.Instance.currentMainBassMixerHandle, Shared.AudioPlayerShared.Instance.repeatTrack);
                            fadeInThread.Start();
                            AudioPlayerShared.Instance.RaiseOnTrackChanged();
                if (!AudioPlayerShared.Instance.trackQueue.Any())
                {
                    await FillQueue();
                }

                _ = MonitorPlayback();
                            return;
            }
            else
            {
                Bass.ChannelStop(_trackHandle);
                BassMix.MixerRemoveChannel(_trackHandle);
                Bass.StreamFree(_trackHandle);
                _trackHandle = 0;
                Bass.ChannelStop(_trackHandle);
                _trackHandle = Bass.CreateStream(AudioPlayerShared.Instance.currentTrack.FilePath, 0, BassFlags.StreamStatus | BassFlags.Decode| BassFlags.AsyncFile | BassFlags.Float, null);
                AudioPlayerShared.Instance.crossfadeTimeInMs = 0;
                Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 1);
                BassMix.MixerAddChannel(AudioPlayerShared.Instance.currentMainBassMixerHandle, _trackHandle, BassFlags.Default | BassFlags.Float);
                Bass.ChannelPlay(AudioPlayerShared.Instance.currentMainBassMixerHandle, Shared.AudioPlayerShared.Instance.repeatTrack);

                fadeInThread.Start();
                AudioPlayerShared.Instance.RaiseOnTrackChanged();
                Debug.WriteLine(Bass.LastError.ToString());
                if (!AudioPlayerShared.Instance.trackQueue.Any())
                {
                    await FillQueue();
                }

                _ = MonitorPlayback();
                return;
            }

        }

        public void Stop()
        {
            Bass.ChannelStop(_trackHandle);
            BassMix.MixerRemoveChannel(_trackHandle);
            Bass.StreamFree(_trackHandle);
            _trackHandle = 0;

            AudioPlayerShared.Instance.currentTrack = null;
            AudioPlayerShared.Instance.RaiseOnTrackChanged();
        }

        private async Task UpdateMetadata() // add handling
        {
            if (AudioPlayerShared.Instance.currentTrack == null)
            {
                AudioPlayerShared.Instance.currentAlbumArt = null;

                WebEncoderNowPlaying.Instance.nowPlayingText = "";
                new ButtEncoder().UpdateNowPlayingText();
                return;
            }
            if (AudioPlayerShared.Instance.currentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
            {
                AudioPlayerShared.Instance.currentAlbumArt = null;

                WebEncoderNowPlaying.Instance.nowPlayingText = "";
                new ButtEncoder().UpdateNowPlayingText();
                return;
            }
            else
            {
                SendNewCover();

                WebEncoderNowPlaying.Instance.nowPlayingText = await Task.Run(() => _nowPlayingWeb.CreateString());
                // new ButtEncoder().UpdateNowPlayingText();
                new TextOutputEncoder().UpdateNowPlayingText();
                return;
            }
        }

        public async Task HookSweeperNowAsync(Database.Models.Track sweeper, Database.Models.Track targetTrack)
        {
            if (targetTrack != AudioPlayerShared.Instance.currentTrack)
            {
                return;
            }
            int _sweepHandle = Bass.CreateStream(sweeper.FilePath, 0, 0, BassFlags.MusicSensitiveRamping | BassFlags.AutoFree | BassFlags.AsyncFile);


            BassMix.MixerAddChannel(AudioPlayerShared.Instance.currentMainBassMixerHandle, _sweepHandle, BassFlags.Default | BassFlags.Float);
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 0.2f, (int)sweeper.Duration.TotalMilliseconds / 2);
            Bass.ChannelPlay(_sweepHandle, false);
            await Task.Delay((int)sweeper.Duration.TotalMilliseconds);
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 1.0f, (int)sweeper.Duration.TotalMilliseconds / 2);
            Bass.ChannelStop(_sweepHandle);
            BassMix.MixerRemoveChannel(_sweepHandle);
            Bass.StreamFree(_sweepHandle);
            return;
        }
        private void SendNewCover()
        {
            var picture = _tagReader.GetPicture(AudioPlayerShared.Instance.currentTrack)?.Data.Data;
            if (picture != null)
            {
                AudioPlayerShared.Instance.currentAlbumArt = new MemoryStream(picture);
            }
            else
            {
                AudioPlayerShared.Instance.currentAlbumArt = null;
            }
        }
        private void OnTrackChanged(object sender, EventArgs e)
        {
            _ = UpdateMetadata();
        }

        public void Dispose()
        {
            AudioPlayerShared.Instance.onTrackChanged -= OnTrackChanged;
            AudioPlayerShared.Instance.onQueueChanged -= OnQueueChanged;

            Bass.StreamFree(AudioPlayerShared.Instance.currentMainBassMixerHandle);
            Bass.StreamFree(VSTEffect.Instance.effectHandle);

            _crossfadeCts.Dispose();
        }

        private void FadeOut()
        {
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 0, Shared.AudioPlayerShared.Instance.crossfadeTimeInMs / 2);
        }

        private void FadeIn()
        {
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 1, Shared.AudioPlayerShared.Instance.crossfadeTimeInMs / 2);
        }
    }
}
