using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players.Enums;
using ManagedBass.Fx;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
        Task Play(bool skipToNextTrack = false);
        Task PlayNextTrack();
        void OnTrackChanged(TrackDTO newTrack);
        Task Stop();
        float[] GetWaveformPCM(int targetPoints = 800);
        void Pause();

        void Resume();
        PlayerEnum GetPlayerEngine();
        Task PlayFX(FxDTO fx);

        Task<TimeSpan> GetElapsedTime();
        Task<TimeSpan> GetRemainingTime();

        void OnPlaybackStopped();
        float[] GetLevels();

    }
}
