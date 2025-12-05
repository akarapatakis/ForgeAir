using ForgeAir.Database.Models.Enums;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Helpers
{

    public static class TrackTypeIconGen
    {
        public static PackIconRemixIconKind Generate(TrackType type)
        {
            switch (type){
                case TrackType.Song:
                    return PackIconRemixIconKind.DiscFill;
                case TrackType.Jingle:
                    return PackIconRemixIconKind.AlarmWarningFill;
                case TrackType.Commercial:
                    return PackIconRemixIconKind.HandCoinFill;
                case TrackType.Rebroadcast:
                    return PackIconRemixIconKind.RadarFill;
                case TrackType.Sweeper:
                    return PackIconRemixIconKind.MicAiFill;
                default:
                    return PackIconRemixIconKind.QuestionMark;
            }
        }
    }
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
                case TrackType.Sweeper:
                    return "#00ACC1";
                case TrackType.Rebroadcast:
                    return "#8D6E63";
                default:
                    return "";
            }
        }
    }
}
