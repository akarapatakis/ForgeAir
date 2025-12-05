using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;

namespace ForgeAir.Core.Services.AudioPlayout
{
    public class AudioPlayerService : IAudioService
    {
        IPlayer _player;


        public AudioPlayerService(IPlayer player)
        {
            _player = player;
        }

        public void Initialize()
        {

        }

        public void Stop()
        {
            _player.Stop();
        }

        public void Play(bool skipToNextTrack = false)
        {
            Task.Run(() => _player.Play(skipToNextTrack));
        }

        public void Pause()
        {
            Task.Run(() => _player.Pause());
        }

        public void Next()
        {
            Task.Run(() => _player.PlayNextTrack());
        }

        public float[] GetLevels()
        {
            return _player.GetLevels();
        }

        public Task<TimeSpan> RemainingTime() => _player.GetRemainingTime();

        public Task<TimeSpan> ElapsedTime() => _player.GetElapsedTime();

        public float[] GetWaveformPCM(int targetPoints = 800)
        {
            return _player.GetWaveformPCM(targetPoints);

        }
    }
}
