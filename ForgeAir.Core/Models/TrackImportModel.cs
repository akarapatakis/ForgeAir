using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database.Models.Enums;


namespace ForgeAir.Core.Models
{
    public class TrackImportModel
    {
        public Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> ImportStatus { get; set; }
        public string FilePath { get; }

        public TrackType TrackType { get; }
        public string? OverrideArtistString { get; } = null;

        public string? StreamDisplayTitle { get; set; } = null;
        public TimeSpan CrossfadeTime { get; }
        public TrackImportModel(string filepath, TrackType type, TimeSpan crossfadeTime, bool isVideo = false, string? artistString = null) {
            FilePath = filepath;
            TrackType = type;
            OverrideArtistString = artistString;
            CrossfadeTime = crossfadeTime;

        }
    }
}
