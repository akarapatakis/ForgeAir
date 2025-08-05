using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;
using NAudio.Wave.SampleProviders;
using NAudio.Wave;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Database.Models;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Events;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using PlaybackState = NAudio.Wave.PlaybackState;
using ForgeAir.Core.Services.Scheduler.Interfaces;

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

        public NAudioPlayer(NAudioDevice device, IEventAggregator eventAggregator, IQueueService queueService, ISchedulerService schedulerService)
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
            meteringSampleProvider = new MeteringSampleProvider(mixer);
            meteringSampleProvider.StreamVolume += (s, a) =>
            {
                leftVolumeLevel = a.MaxSampleValues[0];
                rightVolumeLevel = a.MaxSampleValues[1];

            };
            outputDevice.Init(mixer);
            outputDevice.Play();
        }

        public void Pause()
        {
            throw new NotImplementedException();
        }


        public void OnTrackChanged(TrackDTO newTrack)
        {
            _eventAggregator.Publish(new TrackChangedEvent(newTrack));
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

            _currentTrack = _queueService.Dequeue();
            if (_currentTrack == null) return;



            if (_currentTrack.EndPoint == null && _currentTrack.MixPoint == null && _currentTrack.StartPoint == null)
            {
                _crossfadeDuration = _device.TargetDevice.BufferLength;
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
            _audioFileReader = new AudioFileReader(_currentTrack?.FilePath);
            _audioFileReader.CurrentTime = _currentTrack.StartPoint.Value;

            var fader = new FadeInOutSampleProvider(_audioFileReader, true);
            fader.BeginFadeIn(_crossfadeDuration / 2);

            mixer.AddMixerInput(new WdlResamplingSampleProvider(fader, mixer.WaveFormat.SampleRate));

            foreach (var activeTrack in activeTracks.ToList())
            {
                activeTrack.Fader.BeginFadeOut(_crossfadeDuration / 2);
                await Task.Delay(_crossfadeDuration / 2);
                mixer.RemoveMixerInput(activeTrack.Fader);
                activeTrack.Reader.Dispose();
                activeTracks.Remove(activeTrack);
            }
            activeTracks.Add(new TrackItem
            {
                Reader = _audioFileReader,
                Fader = fader
            });

            isPlaying = true;
            _ = MonitorPlayback();
        }

        public Task<int> GetRemainingMilliseconds()
        {
            if (_audioFileReader == null) return Task.FromResult(0);
            return Task.FromResult(_audioFileReader.CurrentTime.Milliseconds);
        }

        private async Task MonitorPlayback()
        {
            while (outputDevice.PlaybackState == PlaybackState.Playing)
            {
                var remainingTime = GetRemainingMilliseconds().Result;
                if (remainingTime < _crossfadeDuration)
                {
                    await PlayNextTrack();
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

        public async Task PlayNextTrack()
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

            _currentTrack = _queueService.Dequeue();
            if (_currentTrack == null) return;



            if (_currentTrack.EndPoint == null && _currentTrack.MixPoint == null && _currentTrack.StartPoint == null)
            {
                _crossfadeDuration = _device.TargetDevice.BufferLength;
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
            _audioFileReader = new AudioFileReader(_currentTrack?.FilePath);
            _audioFileReader.CurrentTime = _currentTrack.StartPoint.Value;

            var fader = new FadeInOutSampleProvider(_audioFileReader, true);
            fader.BeginFadeIn(_crossfadeDuration / 2);

            mixer.AddMixerInput(new WdlResamplingSampleProvider(fader, mixer.WaveFormat.SampleRate));

            foreach (var activeTrack in activeTracks.ToList())
            {
                activeTrack.Fader.BeginFadeOut(_crossfadeDuration / 2);
                await Task.Delay(_crossfadeDuration / 2);
                mixer.RemoveMixerInput(activeTrack.Fader);
                activeTrack.Reader.Dispose();
                activeTracks.Remove(activeTrack);
            }
            activeTracks.Add(new TrackItem
            {
                Reader = _audioFileReader,
                Fader = fader
            });

            isPlaying = true;
            _ = MonitorPlayback();
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

    }
    internal class TrackItem
    {
        public AudioFileReader Reader { get; set; }
        public FadeInOutSampleProvider Fader { get; set; }
    }
}
