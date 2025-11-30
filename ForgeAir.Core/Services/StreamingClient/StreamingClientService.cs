using DotNetEnv;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.StreamingClient.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.StreamingClient
{
    public class StreamingClientService
    {

        private readonly StreamingEncoder _streamingEncoder;
        private readonly IPlayer _player;

        private readonly BassDevice bassDevice;
        private readonly NAudioDevice naudioDevice;
        public StreamingClientService(StreamingEncoder enc, BassDevice? sourceBassDevice=null, NAudioDevice? sourceNAudioDevice=null) {
        
                _streamingEncoder = enc;
            naudioDevice = sourceNAudioDevice;
            bassDevice = sourceBassDevice;
            if (enc == null) {
                return;
            }
        }

        public IStreamingClient CreateEncoder(BassDevice? bassDevice = null, NAudioDevice? naudioDevice = null)
        {

                    return new BassStreamingEncoder(_streamingEncoder, bassDevice ?? this.bassDevice);
        }


    }
}
