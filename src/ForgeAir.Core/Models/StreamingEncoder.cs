using ForgeAir.Core.Models.Enums;
using NAudio.Dsp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    public class StreamingEncoder
    {
        public StreamingEncoderProtocol Protocol { get; set; }
        public int Bitrate { get; set; } = 128;
        public StreamingEncoderFormat Format { get; set; } = StreamingEncoderFormat.MP3;
        public string DisplayName { get; set; } // for ui
        public string StreamName { get; set; } = "ForgeAir Internet Stream";
        public string Genre { get; set; } = "Undefined";
        public string StreamHomepage { get; set; } = "https://akarapatakis.rf.gd/forgeair";
        public bool PullDataFromStationInfo { get; set; } = false;
        public string ServerURL { get; set; }
        public string ServerPort { get; set; }
        public string Password { get; set; }
        public string? Mountpoint { get; set; } = string.Empty;
        public bool Autoconnect { get; set; } = false;
    }
}
