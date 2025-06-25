using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers.Interfaces;
using ManagedBass.Fx;

namespace ForgeAir.Core.Services.AudioPlayout.Players.Interfaces
{
    public interface IPlayer
    {

        /// <summary>
        /// Plays either a list containing "Track" or a Track 
        /// if track is null and the list is null, then nothing will play
        /// </summary>
        /// <param name="track">The track to play</param>
        /// <returns>Nothing - passes into PlayNextInList() automatically</returns>
        Task Play();
        Task PlayNextTrack();
        void OnTrackChanged(TrackDTO newTrack);
        Task Stop();

        void Pause();

        void Resume();

        Task PlayFX(FxDTO fx);

        void OnPlaybackStopped();
        float[] GetLevels();

    }
}
