using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players.Enums;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.Scheduler.Interfaces;
using ForgeAir.Core.Services.TrackSelector.Enums;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using NAudio.Wave;
using NAudio.Wave.SampleProviders;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using PlaybackState = NAudio.Wave.PlaybackState;

namespace ForgeAir.Core.Services.AudioPlayout.Players
{
    public class NAudioPlayer : IPlayer
    {
        private NAudioDevice _device;
        private IWavePlayer outputDevice;
        private MixingSampleProvider mixer;
        private List<TrackItem> activeTracks = new();
        private bool isPaused = false;
        private bool isPlaying = false;
        private readonly ISchedulerService _schedulerService;
        private readonly IEventAggregator _eventAggregator;
        private readonly IQueueService _queueService;
        private MeteringSampleProvider meteringSampleProvider;
        private AudioFileReader _audioFileReader;
        private CancellationTokenSource _crossfadeCts = new();
        private TrackDTO? _currentTrack;
        private int _crossfadeDuration = 0;
        private float leftVolumeLevel = 0.0f;
        private float rightVolumeLevel = 0.0f;
        private readonly Events.TrackChangedEvent _trackChangedEvent;
        private static float[] peakSmoothed;
        public NAudioPlayer(NAudioDevice device, IEventAggregator eventAggregator, IQueueService queueService, ISchedulerService schedulerService, TrackChangedEvent trackChangedEvent)
        {
            _device = device;
            object? api = device.GetAPI();

            if (api is IWavePlayer player)
            {
                outputDevice = player;
            }
            else
            {
                throw new InvalidOperationException("API is null or not an IWavePlayer");
            }

            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(device.TargetDevice.SampleRate, device.TargetDevice.Channels))
            {
                ReadFully = true
            };
            _queueService = queueService;
            _eventAggregator = eventAggregator;
            _schedulerService = schedulerService;
            _trackChangedEvent = trackChangedEvent;

            meteringSampleProvider = new MeteringSampleProvider(mixer) { SamplesPerNotification = 50 };
            meteringSampleProvider.StreamVolume += (s, a) =>
            {
                if (peakSmoothed == null || peakSmoothed.Length != a.MaxSampleValues.Length)
                    peakSmoothed = new float[a.MaxSampleValues.Length];

                // Simple smoothing: peak = 70% old + 30% new
                for (int i = 0; i < a.MaxSampleValues.Length; i++)
                {
                    peakSmoothed[i] = 0.7f * peakSmoothed[i] + 0.3f * a.MaxSampleValues[i];

                    // Convert to dBFS
                    double db = 20 * Math.Log10(peakSmoothed[i] + 1e-10);
//                    Debug.WriteLine($"Channel {i} dBFS: {db:F2}");
                    if (i == 0)
                        leftVolumeLevel = peakSmoothed[i];
                    else if (i == 1)
                        rightVolumeLevel = peakSmoothed[i];
                }


            };
            outputDevice.Init(meteringSampleProvider);
            outputDevice.Play();
        }

        public void Pause()
        {
            if (isPaused)
            {
                outputDevice.Pause();
                isPaused = false;
            }
            else
            {
                outputDevice.Pause();
                isPaused = true;
            }
        }
        public Task<double> GetPlaybackHandleDb()
        {
            // Use the higher channel level (like BASS_ChannelGetLevel)
            float peak = Math.Max(leftVolumeLevel, rightVolumeLevel);

            // Convert to dBFS
            double db = 20 * Math.Log10(peak + 1e-10);
            return Task.FromResult(db);
        }
        public async Task PopulateQueue(int times)
        {
            if (_queueService.Selector.CurrentSelector.SelectionMode == TrackSelectionMode.Manual)
            {
                return;
            }
            int absTimes = Math.Abs(times);
            for (int i = 0; i <= absTimes;)
            {
                await _queueService.FillQueueAsync(_schedulerService.GetClockItemFor(DateTime.Now), DateTime.Now); // need to do fire and forget to avoid blocking manual track selection 
                i++;
            }
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

        public void OnTrackChanged(TrackDTO newTrack)
        {
            _trackChangedEvent.RaiseTrackChanged(newTrack);
        }
        public async Task Play(bool skipToNextTrack = false)
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

            _currentTrack = _queueService.Dequeue();
            if (_currentTrack == null) return;



            if (_currentTrack.EndPoint == null && _currentTrack.MixPoint == null && _currentTrack.StartPoint == null)
            {
                _crossfadeDuration = _device.TargetDevice.BufferLength;
            }
            if (_currentTrack.MixPoint == _currentTrack.EndPoint) { 
            }
            else { _crossfadeDuration = (int)(_currentTrack.EndPoint.Value.TotalMilliseconds - _currentTrack.MixPoint?.TotalMilliseconds ?? 0); }

            if (_crossfadeDuration == 0 || _crossfadeDuration == null)
            {
                _crossfadeDuration = _device.TargetDevice.BufferLength;
                _currentTrack.StartPoint = TimeSpan.Zero;
            }
            if (_currentTrack.IsDynamicJingleAsset)
            {
                _crossfadeDuration = 0;
            }
            if (skipToNextTrack)
            {
                _crossfadeDuration = 100;
            }
            _audioFileReader = new AudioFileReader(_currentTrack?.FilePath);
            _audioFileReader.CurrentTime = _currentTrack.StartPoint.Value;

            var fader = new FadeInOutSampleProvider(_audioFileReader, true);
            fader.BeginFadeIn(333);
            mixer.AddMixerInput(new WdlResamplingSampleProvider(fader, mixer.WaveFormat.SampleRate));

            Task.Run(async () =>
            {
                foreach (var activeTrack in activeTracks.ToList())
                {
                    activeTrack.Fader.BeginFadeOut(_crossfadeDuration);
                    await Task.Delay(_crossfadeDuration);
                    mixer.RemoveMixerInput(activeTrack.Fader);
                    activeTrack.Reader.Dispose();
                    activeTracks.Remove(activeTrack);
                }
                activeTracks.Add(new TrackItem
                {
                    Reader = _audioFileReader,
                    Fader = fader
                });
            });
           


            isPlaying = true;
            OnTrackChanged(_currentTrack);
            await Task.Delay(60);

            _ = MonitorPlayback();

        }

        public Task<int> GetRemainingMilliseconds()
        {
            if (_audioFileReader == null || _currentTrack == null)
                return Task.FromResult(0);

            int remaining = (int)(_currentTrack.Duration.TotalMilliseconds - _audioFileReader.CurrentTime.TotalMilliseconds);
            if (remaining < 0) remaining = 0;
            return Task.FromResult(remaining);
        }

        private async Task MonitorPlayback()
        {
            _crossfadeCts?.Cancel();
            _crossfadeCts = new CancellationTokenSource();
            var token = _crossfadeCts.Token;

            try
            {
                while (!token.IsCancellationRequested && outputDevice.PlaybackState == PlaybackState.Playing)
                {
                    var remainingTime = await GetRemainingMilliseconds();
                    var db = await GetPlaybackHandleDb();

                    if (remainingTime <= _crossfadeDuration
                        || ((remainingTime <= 8000) &&
                            (_currentTrack.MixPoint == null || _currentTrack.MixPoint == _currentTrack.EndPoint) &&
                            db <= MixDb()))
                    {
                        if (_currentTrack.MixPoint == _currentTrack.EndPoint)
                        {
                            _crossfadeDuration = remainingTime;
                        }

                        break;
                    }

                    await Task.Delay(50, token);
                }

                if (!token.IsCancellationRequested)
                {
                    await Play();
                }
            }
            catch (TaskCanceledException)
            {
                // Expected on cancellation, safely ignore
            }
            catch (Exception ex)
            {
                // Log unexpected exceptions
                Console.WriteLine($"[MonitorPlayback] Error: {ex.Message}");
            }
        }

        public PlayerEnum GetPlayerEngine() => PlayerEnum.NAudio;
        public async Task PlayFX(FX fx)
        {
            foreach (var activeTrack in activeTracks.ToList())
            {
                activeTrack.Reader.Volume = 0.5f;
            }
    
            var reader = new AudioFileReader(fx?.FilePath);
            var fader = new FadeInOutSampleProvider(reader, true);
            fader.BeginFadeIn(50);
            mixer.AddMixerInput(fader);

            await Task.Delay(fx.Duration);
            foreach (var activeTrack in activeTracks.ToList())
            {
                activeTrack.Reader.Volume = 1.0f;
            }
        }

        void IPlayer.Resume()
        {
            throw new NotImplementedException();
        }

        Task IPlayer.Stop()
        {
            throw new NotImplementedException();
        }

        public Task PlayFX(FxDTO fx)
        {
            throw new NotImplementedException();
        }

        public void OnPlaybackStopped()
        {
            throw new NotImplementedException();
        }

        public float[] GetLevels()
        {
            return new float[] { leftVolumeLevel, rightVolumeLevel };
        }

        public Task<TimeSpan> GetElapsedTime()
        {
            return Task.FromResult(_audioFileReader.CurrentTime);
        }

        public Task<TimeSpan> GetRemainingTime()
        {
            return Task.FromResult(_currentTrack.Duration - _audioFileReader.CurrentTime);
        }

        public Task PlayNextTrack()
        {
            throw new NotImplementedException();
        }

        public float[] GetWaveformPCM(int targetPoints = 800)
        {
            throw new NotImplementedException();
        }
    }
    internal class TrackItem
    {
        public AudioFileReader Reader { get; set; }
        public FadeInOutSampleProvider Fader { get; set; }
    }
}
