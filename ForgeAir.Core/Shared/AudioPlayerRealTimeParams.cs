using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{
    public class AudioPlayerRealTimeParams
    {
        public string? audioFile { get; set; }
        public bool repeatTrack { get; set; }
        public int crossfadeTimeInMs { get; set; }
        public int fadeNextTimeInMs { get; set; }
        public int stopFadeTimeInMs { get; set; }

        private static AudioPlayerRealTimeParams? instance;
        public static AudioPlayerRealTimeParams Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new AudioPlayerRealTimeParams();
                }
                return instance;
            }
        }
    }
}
