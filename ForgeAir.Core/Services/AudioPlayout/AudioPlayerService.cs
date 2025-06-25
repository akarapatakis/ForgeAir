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

        public void Play()
        {
            Task.Run(() => _player.Play());
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
    }
}
