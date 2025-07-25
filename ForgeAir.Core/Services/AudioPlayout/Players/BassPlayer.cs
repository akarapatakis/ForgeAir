using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Database.Models;
using ManagedBass.Mix;
using ManagedBass;
using System.Diagnostics;
using ForgeAir.Core.Models;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Events;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.Scheduler.Interfaces;
/*
    IMPORTANT!

    THE DEVELOPER IS NOT RESPONSIBLE IN ANY WAY REGARDING BASS LICENSING AS THE PLAYER IS BASED ON AN OPEN-SOURCE WRAPPER
    AND NO BASS BINARIES ARE BUNDLED WITH FORGEAIR IN ANY WAY. THE USER WHO ENABLES BASS PLAYBACK AND PROVIDES THEIR BASS LIBRARIES
    IS SOLELY RESPONSIBLE FOR ANY CHARGES DUE TO UNLICENSED SOFTWARE

    https://www.un4seen.com/bass.html#license

    - Karapatakis Angelos (https://chocolateadventurouz.rf.gd)
 
 */
namespace ForgeAir.Core.Services.AudioPlayout.Players
{
    public class BassPlayer : IPlayer, IDisposable
    {
   

        private int _trackHandle;

        private BassDevice _device;

        private CancellationTokenSource _crossfadeCts = new();
        private bool isPaused = false;
        private bool isPlaying = false;

        private TrackDTO? _currentTrack;

        private int _crossfadeDuration = 0;

        private readonly ISchedulerService _schedulerService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IQueueService _queueService;

        public BassPlayer(BassDevice device, IEventAggregator eventAggregator, IQueueService queueService, ISchedulerService schedulerService)
        {
            _device = device;
            _queueService = queueService;
            _schedulerService = schedulerService;
           // Bass.CurrentDevice = device.TargetDevice.Index; <- fix later 
            _crossfadeDuration = device.TargetDevice.BufferLength;

            _eventAggregator = eventAggregator;
        }


        public async Task Play()
        {
            if (_queueService.IsEmpty())
            {
                await _queueService.FillQueueAsync(_schedulerService.GetClockItemFor(DateTime.Now), DateTime.Now);
            }


            isPaused = false;
            if (_crossfadeCts != null)
            {
                _crossfadeCts.Cancel();
            }

            var fadeInThread = new Thread(FadeIn);
            var fadeOutThread = new Thread(FadeOut);

            if (_trackHandle != 0)
            {
                fadeOutThread.Start();
                await Task.Delay(333); // keep the stream handle untouched and alive while the crossfade is being done
            }

            _currentTrack = _queueService.Dequeue();

            if (_currentTrack == null) return;

            if (_currentTrack.TrackType != ForgeAir.Database.Models.Enums.TrackType.Rebroadcast)
            {
                _trackHandle = Bass.CreateStream(
                    _currentTrack.FilePath, 0, 0,
                    BassFlags.Decode | BassFlags.Prescan | BassFlags.AsyncFile | BassFlags.Float
                );

                if (_currentTrack.EndPoint == null && _currentTrack.MixPoint == null && _currentTrack.StartPoint == null)
                {
                    _crossfadeDuration = 333;
                }
                else { _crossfadeDuration = (int)(_currentTrack.EndPoint.Value.TotalMilliseconds - _currentTrack.MixPoint?.TotalMilliseconds ?? 0); }

                if (_crossfadeDuration == 0 || _crossfadeDuration == null)
                {
                    _crossfadeDuration = 333;
                    _currentTrack.StartPoint = TimeSpan.Zero;
                }
                Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 0);
                Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, _currentTrack.StartPoint?.TotalSeconds ?? TimeSpan.Zero.TotalSeconds), PositionFlags.Bytes); // set start position
                BassMix.MixerAddChannel(_device.Handle, _trackHandle, BassFlags.Default | BassFlags.Float);
                Bass.ChannelPlay(_device.Handle);
                fadeInThread.Start();

                isPlaying = true;
                _ = MonitorPlayback();
            }
            else
            {

                Bass.ChannelStop(_trackHandle);
                BassMix.MixerRemoveChannel(_trackHandle);
                Bass.StreamFree(_trackHandle);
                _trackHandle = 0;
                _trackHandle = Bass.CreateStream(_currentTrack.FilePath, 0, BassFlags.StreamStatus | BassFlags.Decode | BassFlags.AsyncFile | BassFlags.Float, null);
                _crossfadeDuration = 0;
                Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 0);
                BassMix.MixerAddChannel(_device.Handle, _trackHandle, BassFlags.Default | BassFlags.Float);
                Bass.ChannelPlay(_device.Handle);
                Debug.WriteLine(Bass.LastError.ToString());
                fadeInThread.Start();
                await Task.Delay(333); // being patient here to make MonitorPlayback() catchup correctly with the current track (fixes a rare issue)

                _ = MonitorPlayback();
            }

        }
        public Task<double> GetTrackLengthInSecs()
        {
            return Task.FromResult(Bass.ChannelBytes2Seconds(_trackHandle, Bass.ChannelGetLength(_trackHandle)));
        }

        public void OnTrackChanged(TrackDTO newTrack)
        {
            _eventAggregator.Publish(new TrackChangedEvent(newTrack));
        }
        public Task<int> GetRemainingMilliseconds(int channel)
        {
            long length = Bass.ChannelGetLength(channel);
            double totalSeconds = Bass.ChannelBytes2Seconds(channel, length);

            long position = Bass.ChannelGetPosition(channel);
            double currentSeconds = Bass.ChannelBytes2Seconds(channel, position);

            return Task.FromResult((int)((totalSeconds - currentSeconds) * 1000));
        }

        public Task<TimeSpan> GetElapsedTime()
        {
            if (_device.Handle == 0 ||
                Bass.ChannelIsActive(_trackHandle) != PlaybackState.Playing)
            {
                return Task.FromResult(TimeSpan.Zero);
            }

            long position = Bass.ChannelGetPosition(_trackHandle);
            return Task.FromResult(TimeSpan.FromSeconds(Bass.ChannelBytes2Seconds(_trackHandle, position)));
        }

        private async Task MonitorPlayback()
        {
            if (_currentTrack.TrackType == ForgeAir.Database.Models.Enums.TrackType.Rebroadcast)
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
                    if (remainingTime <= _crossfadeDuration)
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
        public async Task PlayFX(FX fx)
        {
            int _fxHandle = Bass.CreateStream(fx.FilePath, 0, 0, BassFlags.MusicSensitiveRamping | BassFlags.AutoFree | BassFlags.AsyncFile);


            BassMix.MixerAddChannel(_device.Handle, _fxHandle, BassFlags.Default | BassFlags.Float);
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 0.2f, (int)fx.Duration.TotalMilliseconds / 2);
            Bass.ChannelPlay(_fxHandle, false);
            await Task.Delay((int)fx.Duration.TotalMilliseconds);
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 1.0f, (int)fx.Duration.TotalMilliseconds / 2);
            Bass.ChannelStop(_fxHandle);
            BassMix.MixerRemoveChannel(_fxHandle);
            Bass.StreamFree(_fxHandle);
            return;
        }

        public async Task PlayNextTrack()
        {
            if (_queueService.IsEmpty())
            {
                var clockItem = _schedulerService.GetClockItemFor(DateTime.Now);
                await _queueService.FillQueueAsync(clockItem, DateTime.Now);
            }

            isPaused = false;

            _crossfadeCts?.Cancel();
            _crossfadeCts = null;

            var fadeInThread = new Thread(FadeIn);
            var fadeOutThread = new Thread(FadeOut);

            if (_trackHandle != 0)
            {
                fadeOutThread.Start();
                await Task.Delay(333);
            }

            _currentTrack = _queueService.Dequeue();
            if (_currentTrack == null) return;

            if (_currentTrack.TrackType != ForgeAir.Database.Models.Enums.TrackType.Rebroadcast)
            {
                _trackHandle = Bass.CreateStream(_currentTrack.FilePath, 0, 0,
                    BassFlags.Decode | BassFlags.Prescan | BassFlags.AsyncFile | BassFlags.Float);

                _crossfadeDuration = (int)(_currentTrack.EndPoint?.TotalMilliseconds - _currentTrack.MixPoint?.TotalMilliseconds ?? 333);
                if (_crossfadeDuration <= 0)
                    _crossfadeDuration = 333;

                Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 0);
                BassMix.MixerAddChannel(_device.Handle, _trackHandle, BassFlags.Default | BassFlags.Float);
                Bass.ChannelPlay(_device.Handle);

                fadeInThread.Start();
                _ = MonitorPlayback();
            }
            else
            {
                Bass.ChannelStop(_trackHandle);
                BassMix.MixerRemoveChannel(_trackHandle);
                Bass.StreamFree(_trackHandle);
                _trackHandle = 0;

                _trackHandle = Bass.CreateStream(_currentTrack.FilePath, 0,
                    BassFlags.StreamStatus | BassFlags.Decode | BassFlags.AsyncFile | BassFlags.Float, null);

                _crossfadeDuration = 0;

                Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 1);
                BassMix.MixerAddChannel(_device.Handle, _trackHandle, BassFlags.Default | BassFlags.Float);
                Bass.ChannelPlay(_device.Handle);

                fadeInThread.Start();
                _ = MonitorPlayback();
            }
        }


        public bool IsPlaying()
        {
            return Bass.ChannelIsActive(_trackHandle) == PlaybackState.Playing;
        }
        public void Pause()
        {
            Bass.ChannelPause(_device.Handle);
            isPaused = true;
        }

        public void Resume()
        {
            Bass.ChannelPlay(_device.Handle);
            isPaused = false;
        }
        private void FadeOut()
        {
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 0, _crossfadeDuration / 2);
        }

        private void FadeIn()
        {
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 1, _crossfadeDuration / 2);
        }
        public Task Stop()
        {
            throw new NotImplementedException();
        }

        public void Dispose()
        {

           BassMix.MixerRemoveChannel(_device.Handle);
            Bass.Free();
        }

        public async Task PlayFX(FxDTO fx)
        {
            int _fxHandle = Bass.CreateStream(fx.FilePath, 0, 0, BassFlags.MusicSensitiveRamping | BassFlags.AutoFree | BassFlags.AsyncFile);


            BassMix.MixerAddChannel(_device.Handle, _fxHandle, BassFlags.Default | BassFlags.Float);
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 0.2f, (int)fx.Duration.TotalMilliseconds / 2);
            Bass.ChannelPlay(_fxHandle, false);
            await Task.Delay((int)fx.Duration.TotalMilliseconds);
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 1.0f, (int)fx.Duration.TotalMilliseconds / 2);
            Bass.ChannelStop(_fxHandle);
            BassMix.MixerRemoveChannel(_fxHandle);
            Bass.StreamFree(_fxHandle);
            return;
        }


        public void OnPlaybackStopped()
        {
            throw new NotImplementedException();
        }

        public float[] GetLevels()
        {
            throw new NotImplementedException();
        }
    }
}
