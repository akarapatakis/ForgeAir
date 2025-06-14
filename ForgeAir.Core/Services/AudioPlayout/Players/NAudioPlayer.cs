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

namespace ForgeAir.Core.Services.AudioPlayout.Players
{
    class NAudioPlayer : IPlayer
    {
        private WaveOutEvent outputDevice;
        private MixingSampleProvider mixer;
        private List<TrackItem> activeTracks = new();


        public NAudioPlayer(OutputDevice device) {
            outputDevice = new WaveOutEvent();
            mixer = new MixingSampleProvider(WaveFormat.CreateIeeeFloatWaveFormat(44100, 2))
            {
                ReadFully = true
            };

            outputDevice.Init(mixer);
            outputDevice.Play();
        }


        void IPlayer.Pause()
        {
            throw new NotImplementedException();
        }

        async Task IPlayer.Play(DTO.TrackDTO? track, LinkedListQueue<TrackDTO>? list)
        {
            var reader = new AudioFileReader(track?.FilePath);
            var fader = new FadeInOutSampleProvider(reader, true);
            fader.BeginFadeIn(2000);

            mixer.AddMixerInput(fader);

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

        async Task IPlayer.PlayFX(FX fx)
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

        Task IPlayer.PlayNextTrack()
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
