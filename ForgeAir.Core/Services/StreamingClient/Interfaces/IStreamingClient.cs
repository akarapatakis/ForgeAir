using ForgeAir.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.StreamingClient.Interfaces
{
    public interface IStreamingClient
    {
        Task<string> StartStreaming();
        void StopStreaming();

        void UpdateNowPlaying(TrackDTO newTrack);
    }
}
