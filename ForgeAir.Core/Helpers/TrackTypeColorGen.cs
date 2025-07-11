using ForgeAir.Database.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Helpers
{
    public static class TrackTypeColorGen
    {
        public static string Generate(TrackType Type)
        {
            switch (Type)
            {
                case TrackType.None:
                    return "#252525";
                case TrackType.Song:
                    return "#2196F3";
                case TrackType.Jingle:
                    return "#00BFA5";
                case TrackType.Commercial:
                    return "#FF9800";
                case TrackType.Promo:
                    return "#252525";
                case TrackType.Voicetrack:
                    return "#AB47BC";
                case TrackType.Fx:
                    return "#CDDC39";
                case TrackType.Other:
                    return "#616161";
                case TrackType.Instant:
                    return "#CDDC39";
                case TrackType.Sweeper:
                    return "#00ACC1";
                case TrackType.Rebroadcast:
                    return "#8D6E63";
                case TrackType.Movie:
                    return "#3F51B5";
                case TrackType.Show:
                    return "#5C6BC0";
                case TrackType.Newsreport:
                    return "#D32F2F";
                case TrackType.Ident:
                    return "#4CAF50";
                case TrackType.Bumper:
                    return "#FF5722";
                case TrackType.MusicVideo:
                    return "#EC407A";
                default:
                    return "";
            }
        }
    }
}
