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

namespace ForgeAir.Core.Services.AudioPlayout.Players
{
    public class NAudioPlayer : IPlayer
    {
        private IWavePlayer outputDevice;
        private MixingSampleProvider mixer;
        private List<TrackItem> activeTracks = new();
        private readonly IEventAggregator _eventAggregator;


        public NAudioPlayer(NAudioDevice device, IEventAggregator eventAggregator)
        {
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

        public async Task Play(DTO.TrackDTO? track, LinkedListQueue<TrackDTO>? list)
        {
            var reader = new AudioFileReader(track?.FilePath);
            var fader = new FadeInOutSampleProvider(reader, true);
            fader.BeginFadeIn(2000);

            mixer.AddMixerInput(new WdlResamplingSampleProvider(fader, mixer.WaveFormat.SampleRate));

            foreach (var activeTrack in activeTracks.ToList())
            {
                activeTrack.Fader.BeginFadeOut(2000);
                await Task.Delay(2000);
                mixer.RemoveMixerInput(activeTrack.Fader);
                activeTrack.Reader.Dispose();
                activeTracks.Remove(activeTrack);
            }

            activeTracks.Add(new TrackItem
            {
                Reader = reader,
                Fader = fader
            });
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

        public Task PlayNextTrack()
        {
            throw new NotImplementedException();
        }

        void IPlayer.Resume()
        {
            throw new NotImplementedException();
        }

        Task IPlayer.Stop()
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
