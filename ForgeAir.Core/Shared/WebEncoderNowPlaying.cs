using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{
    public class WebEncoderNowPlaying
    {
        public string PreText { get; set; } = "Now Live: ";
        public string PostText { get; set; } = " | ForgeAir Radio Automation Software";

        public string nowPlayingText { get; set; } = "ForgeAir Radio Automation Software";
        public string noTrackPlaying { get; set; } = "ForgeAir Radio Automation Software";

        public string? buttExecutable { get; set; } 

        public string? NowPlayingTXT { get; set; }
        private static WebEncoderNowPlaying? instance;
        public static WebEncoderNowPlaying Instance
        {
            get
            {
                if (instance == null)
                {
                    instance = new WebEncoderNowPlaying();
                }
                return instance;
            }
        }
    }
}
