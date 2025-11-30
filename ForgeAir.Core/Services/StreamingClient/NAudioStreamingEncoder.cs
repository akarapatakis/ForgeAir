using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.StreamingClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.StreamingClient
{
    public class NAudioStreamingEncoder : IStreamingClient
    {
        public NAudioStreamingEncoder(StreamingEncoder enc) { }
        public Task<string> StartStreaming()
        {
            throw new NotImplementedException();
        }

        public void StopStreaming()
        {
            throw new NotImplementedException();
        }

        public void UpdateNowPlaying(TrackDTO newTrack)
        {
            throw new NotImplementedException();
        }
    }
}
