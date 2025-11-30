using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Models;
using ForgeAir.Core.Models.Enums;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players.Enums;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.Scheduler.Interfaces;
using ForgeAir.Core.Services.StreamingClient;
using ForgeAir.Core.Services.TrackSelector.Enums;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using ManagedBass;
using ManagedBass.Mix;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.TaskbarClock;
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
        private readonly IServiceProvider _serviceProvider;
        private readonly TrackChangedEvent _trackChangedEvent;

        public BassPlayer(BassDevice device, IEventAggregator eventAggregator, IQueueService queueService, ISchedulerService schedulerService, IServiceProvider serviceProvider, TrackChangedEvent trackChangedEvent)
        {
            _device = device;
            _queueService = queueService;
            _schedulerService = schedulerService;
            _serviceProvider = serviceProvider;
           // Bass.CurrentDevice = device.TargetDevice.Index; <- todo:fix later 
            _crossfadeDuration = device.TargetDevice.BufferLength;
            _trackChangedEvent = trackChangedEvent;
            _eventAggregator = eventAggregator;

            Bass.PluginLoad("bassflac.dll");
            Bass.PluginLoad("bassopus.dll");
            Bass.PluginLoad("bassalac.dll");
            Bass.Configure(Configuration.FloatDSP, true);


        }
        public PlayerEnum GetPlayerEngine() => PlayerEnum.Bass;

        public async Task PopulateQueue(int times)
        {
            if (_queueService.Selector.CurrentSelector.SelectionMode == TrackSelectionMode.Manual)
            {
                return;
            }
            int absTimes = Math.Abs(times);
            for (int i = 0; i <= absTimes;)
            {
                await _queueService.FillQueueAsync(_schedulerService.GetClockItemFor(DateTime.Now), DateTime.Now); 
                i++;
            }
        }
        public float[] GetWaveformPCM(int targetPoints = 800)
        {
            if (_trackHandle == 0) return Array.Empty<float>();

            int tempHandle = Bass.CreateStream(
                _currentTrack.FilePath,
                0, 0,
                BassFlags.Decode | BassFlags.Prescan | BassFlags.Float
            );

            long lengthBytes = Bass.ChannelGetLength(tempHandle, PositionFlags.Bytes);
            if (lengthBytes <= 0) return Array.Empty<float>();

            long totalSamples = lengthBytes / sizeof(float);
            long samplesPerPoint = Math.Max(1, totalSamples / targetPoints);

            float[] waveform = new float[targetPoints];
            float[] buffer = new float[samplesPerPoint];

            for (int i = 0; i < targetPoints; i++)
            {
                long posBytes = i * samplesPerPoint * sizeof(float);
                Bass.ChannelSetPosition(tempHandle, posBytes, PositionFlags.Bytes);

                // FIX HERE:
                int bytesToRead = buffer.Length * sizeof(float);
                int bytesRead = Bass.ChannelGetData(tempHandle, buffer, bytesToRead);
                int samplesRead = bytesRead / sizeof(float);

                if (samplesRead <= 0)
                    break;

                float max = 0;
                for (int j = 0; j < samplesRead; j++)
                {
                    float val = Math.Abs(buffer[j]);
                    if (val > max) max = val;
                }

                waveform[i] = max;
            }

            Bass.StreamFree(tempHandle);
            return waveform;
        }

        private double MixDb()
        {
            if (_queueService.IsEmpty())
            {
                return -18.0f;

            }
            switch (_queueService.Peek().TrackType)
            {
                case TrackType.Song:
                    if (_currentTrack.TrackType == TrackType.Jingle) return -14.0f;
                    return -6.0f;
                case TrackType.Jingle:
                    if (_currentTrack.TrackType == TrackType.Song) return -10.0f;
                    if (_currentTrack.TrackType == TrackType.Commercial) return -40.0f;
                    return -12.0f;
                case TrackType.Commercial:
                    return -40.0f;
                default:
                    return -40.0f;
            }
        }
        public async Task SeekToCue(TrackCuePositions pos)
        {
            switch (pos)
            {
                case TrackCuePositions.Start:
                    Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, _currentTrack.StartPoint.Value.TotalMilliseconds));
                    break;
                case TrackCuePositions.Intro:
                    Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, _currentTrack.Intro.Value.TotalMilliseconds));
                    break;
                case TrackCuePositions.HookIn:
                    Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, _currentTrack.HookInPoint.Value.TotalMilliseconds));
                    break;
                case TrackCuePositions.HookOut:
                    Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, _currentTrack.HookOutPoint.Value.TotalMilliseconds));
                    break;
                case TrackCuePositions.Outro:
                    Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, _currentTrack.Outro.Value.TotalMilliseconds));
                    break;
                case TrackCuePositions.End:
                    Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, _currentTrack.EndPoint.Value.TotalMilliseconds));
                    break;

            }
        }
        public async Task Play(bool skipToNextTrack = false)
        {
            try
            {
                if (_queueService.Count() < 10)
                {
                    await PopulateQueue(Math.Abs(_queueService.Count() - 10));
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
                }

                _currentTrack = _queueService.Dequeue();

                if (_currentTrack == null) return;

                if (_currentTrack.TrackType != ForgeAir.Database.Models.Enums.TrackType.Rebroadcast)
                {
                    _trackHandle = Bass.CreateStream(
                        _currentTrack.FilePath, 0, 0,
                        BassFlags.Decode | BassFlags.Prescan | BassFlags.Float
                    );
                    await Task.Delay(50);
                    if (_currentTrack.EndPoint == _currentTrack.MixPoint)
                    {
                    }
                    else
                    {
                        _crossfadeDuration = (int)(_currentTrack.EndPoint.Value.TotalMilliseconds - _currentTrack.MixPoint?.TotalMilliseconds ?? 0);
                        if (_crossfadeDuration <= 0)
                            _crossfadeDuration = 333;
                    }

                    if (_currentTrack.IsDynamicJingleAsset)
                    {
                        _crossfadeDuration = 0;
                    }
                    if (skipToNextTrack == true)
                    {
                        _crossfadeDuration = 333;
                    }
                    Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 0);
                    Bass.ChannelSetPosition(_trackHandle, Bass.ChannelSeconds2Bytes(_trackHandle, _currentTrack.StartPoint?.TotalSeconds ?? TimeSpan.Zero.TotalSeconds), PositionFlags.Bytes); // set start position
                    BassMix.MixerAddChannel(_device.Handle, _trackHandle, BassFlags.Default | BassFlags.Float);
                    Bass.ChannelPlay(_device.Handle);
                    fadeInThread.Start();
                    OnTrackChanged(_currentTrack);

                    await Task.Delay(_crossfadeDuration);
                    isPlaying = true;

                    _ = MonitorPlayback();
                }
                else
                {
                    // lets try mixing into the stream instead of cutting out and leaving a gap
                    //Bass.ChannelStop(_trackHandle);
                    //BassMix.MixerRemoveChannel(_trackHandle);
                    //Bass.StreamFree(_trackHandle);
                    //_trackHandle = 0;
                    _trackHandle = Bass.CreateStream(_currentTrack.FilePath, 0, BassFlags.StreamStatus | BassFlags.Decode | BassFlags.AsyncFile | BassFlags.Float, null);
                    _crossfadeDuration = 500; // should be enough to be at least gapless (?)
                    Bass.ChannelSetAttribute(_trackHandle, ChannelAttribute.Volume, 0);
                    BassMix.MixerAddChannel(_device.Handle, _trackHandle, BassFlags.Default | BassFlags.Float);
                    Bass.ChannelPlay(_device.Handle);
                    Debug.WriteLine(Bass.LastError.ToString());
                    fadeInThread.Start();
                    OnTrackChanged(_currentTrack);

                    await Task.Delay(333); // being patient here to make MonitorPlayback() catchup correctly with the current track (fixes an issue skipping the stream)
                    isPlaying = true;

                    _ = MonitorPlayback();
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }
        public Task<double> GetTrackLengthInSecs()
        {
            return Task.FromResult(Bass.ChannelBytes2Seconds(_trackHandle, Bass.ChannelGetLength(_trackHandle)));
        }

        public void OnTrackChanged(TrackDTO newTrack)
        {
            _trackChangedEvent.RaiseTrackChanged(newTrack);
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
            _crossfadeCts?.Cancel(); // cancel any previous monitor
            _crossfadeCts = new CancellationTokenSource();
            var token = _crossfadeCts.Token;


            try
            {
                if (_currentTrack.TrackType == TrackType.Rebroadcast)
                {
                    while (!token.IsCancellationRequested &&
                           (Bass.ChannelIsActive(_trackHandle) == PlaybackState.Playing ||
                            Bass.StreamGetFilePosition(_trackHandle) < 100))
                    {
                        await Task.Delay(50, token);
                    }

                    if (!token.IsCancellationRequested)
                    {
                        await Play();
                    }
                }
                else
                {
                    while (!token.IsCancellationRequested &&
                           Bass.ChannelIsActive(_trackHandle) == PlaybackState.Playing)
                    {
                        var remainingTime = await GetRemainingMilliseconds(_trackHandle);
                        var db = await GetPlaybackHandleDb();
                        if (remainingTime <= _crossfadeDuration
                            || ((remainingTime <= 8000) && (_currentTrack.MixPoint == null || _currentTrack.MixPoint == _currentTrack.EndPoint)
                                && (_currentTrack.Duration.TotalSeconds > 10 && db <= MixDb())))
                        {
                            if (_currentTrack.MixPoint == _currentTrack.EndPoint)
                            {
                                _crossfadeDuration = remainingTime;
                            }// do a smooth crossfade
                            break; // ready to crossfade
                        }

                        await Task.Delay(50, token);
                    }

                    if (!token.IsCancellationRequested)
                    {
                        await Play();
                    }
                }
            }
            catch (TaskCanceledException)
            {
                return;
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
            try
            {
                if (_queueService.IsEmpty())
                {
                    await _queueService.FillQueueAsync(_schedulerService.GetClockItemFor(DateTime.Now), DateTime.Now);
                }
                isPaused = false;

                _crossfadeCts?.Cancel();
                _crossfadeCts = null;

                var fadeInThread = new Thread(FadeIn);
                var fadeOutThread = new Thread(FadeOut);

                if (_trackHandle != 0)
                {
                    fadeOutThread.Start();
                }

                _currentTrack = _queueService.Dequeue();
                if (_currentTrack == null) return;

                if (_currentTrack.TrackType != ForgeAir.Database.Models.Enums.TrackType.Rebroadcast)
                {
                    _trackHandle = Bass.CreateStream(_currentTrack.FilePath, 0, 0,
                        BassFlags.Decode | BassFlags.Prescan | BassFlags.AsyncFile | BassFlags.Float);

                    _crossfadeDuration = (int)(_currentTrack.EndPoint?.TotalMilliseconds - _currentTrack.MixPoint?.TotalMilliseconds ?? 333);
                    if (_crossfadeDuration <= 0 || _currentTrack.MixPoint == _currentTrack.EndPoint)
                        _crossfadeDuration = 333;
                    if (_currentTrack.IsDynamicJingleAsset)
                    {
                        _crossfadeDuration = 0;
                    }
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
                    await Task.Delay(_crossfadeDuration / 2);

                    _ = MonitorPlayback();
                }
                OnTrackChanged(_currentTrack);

            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }

        }


        public bool IsPlaying()
        {
            return Bass.ChannelIsActive(_trackHandle) == PlaybackState.Playing;
        }
        public void Pause()
        {
            if (isPaused)
            {
                Resume();
            }
            else
            {
                Bass.ChannelPause(_device.Handle);
                isPaused = true;
            }
        }

        public void Resume()
        {
            Bass.ChannelPlay(_device.Handle);
            isPaused = false;
        }
        private void FadeOut()
        {
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 0, _crossfadeDuration);
        }

        private void FadeIn()
        {
            Bass.ChannelSlideAttribute(_trackHandle, ChannelAttribute.Volume, 1, _crossfadeDuration);
        }
        public Task Stop()
        {
            var fadeOutThread = new Thread(FadeOut);
            fadeOutThread.Start();
            Task.Delay(_crossfadeDuration);
            BassMix.MixerRemoveChannel(_trackHandle);
            OnTrackChanged(null);
            return Task.CompletedTask;
        }

        public void Dispose()
        {
               BassMix.MixerRemoveChannel(_device.Handle);
               Bass.PluginFree(_device.Handle);
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
            float minDb = -60f;
            float maxDb = -0f;

            int level = Bass.ChannelGetLevel(_device.Handle);
            if (level == -1) return new float[] {0, 0};
            int left = level & 0xFFFF;
            int right = level >> 16;

            float peakL = left / 32768f;
            float peakR = Math.Max(left, right) / 32768f;

            float leftDb = Convert.ToSingle(20 * Math.Log10(peakL + 1e-10));
            float rightDb = Convert.ToSingle(20 * Math.Log10(peakR + 1e-10));


            float normalizedL = Math.Clamp((leftDb - minDb) / (maxDb - minDb), 0f, 1f);
            float normalizedR = Math.Clamp((rightDb - minDb) / (maxDb - minDb), 0f, 1f);

            return new float[] { normalizedL, normalizedR };

        }

        public Task<double> GetPlaybackHandleDb()
        {
            int level = Bass.ChannelGetLevel(_device.Handle);
            if (level == -1)
                return Task.FromResult<double>(0.0f); // no data or channel stopped

            int left = level & 0xFFFF;
            int right = level >> 16;

            // pick the louder channel
            float peak = Math.Max(left, right) / 32768f;

            // convert to dBFS
            return Task.FromResult(20 * Math.Log10(peak + 1e-10));
        }

        public Task<TimeSpan> GetRemainingTime()
        {
            if (_device.Handle == 0 ||
                Bass.ChannelIsActive(_trackHandle) != PlaybackState.Playing)
            {
                return Task.FromResult(TimeSpan.Zero);
            }

            long position = Bass.ChannelGetPosition(_trackHandle);
            return Task.FromResult(_currentTrack.Duration - TimeSpan.FromSeconds(Bass.ChannelBytes2Seconds(_trackHandle, position)));
        }
    }
}
