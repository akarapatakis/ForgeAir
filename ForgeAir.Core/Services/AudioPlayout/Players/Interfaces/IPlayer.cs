using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ManagedBass.Fx;

namespace ForgeAir.Core.Services.AudioPlayout.Players.Interfaces
{
    public interface IPlayer
    {

        /// <summary>
        /// Plays either a list containing "Track" or a Track 
        /// if track is null and the list is null, then nothing will play
        /// </summary>
        /// <param name="track">A Database.Models.Track - nullable</param>
        /// <param name="list">A LinkedListQueue by Database.Models.Track - nullable</param>
        /// <returns>Nothing - passes into PlayNextInList() automatically</returns>
        Task Play(DTO.TrackDTO? track=null, LinkedListQueue<TrackDTO>? list=null);
        Task PlayNextTrack();

        Task Stop();

        void Pause();

        void Resume();

        Task PlayFX(ForgeAir.Database.Models.FX fx);

    }
}
