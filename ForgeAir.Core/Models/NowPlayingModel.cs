using ForgeAir.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    public class NowPlayingModel
    {
        public TrackDTO CurrentTrack { get; set; }
        public TrackDTO PreviousTrack { get; set; }

        public string CurrentTrackColor { get; set; } = string.Empty;
        public string PreviousTrackColor { get; set; } = string.Empty;

        public MemoryStream CurrentTrackBitmap { get; set; }
        public MemoryStream PreviousTrackBitmap { get; set; }
    }
}
