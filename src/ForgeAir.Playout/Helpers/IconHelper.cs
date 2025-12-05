using ForgeAir.Database.Models.Enums;
using MahApps.Metro.IconPacks;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Playout.Helpers
{
    public class IconHelper
    {
        public static PackIconRemixIconKind TrackTypeToRemixIcon(TrackType trackType)
        {
            switch (trackType)
            {
                case TrackType.Song:
                    return PackIconRemixIconKind.MvFill;
                case TrackType.Jingle:
                    return PackIconRemixIconKind.BellFill;
                case TrackType.Sweeper:
                    return PackIconRemixIconKind.FlashlightFill;
                case TrackType.Rebroadcast:
                    return PackIconRemixIconKind.RadarFill;
                case TrackType.Commercial:
                    return PackIconRemixIconKind.AdvertisementFill;
                case TrackType.Fx:
                    return PackIconRemixIconKind.MagicFill;
                case TrackType.Voicetrack:
                    return PackIconRemixIconKind.MicFill;
                case TrackType.Promo:
                    return PackIconRemixIconKind.PresentationFill;
                case TrackType.Other:
                    return PackIconRemixIconKind.FileLine;
                case TrackType.None:
                    return PackIconRemixIconKind.CheckboxBlankLine;
                default:
                    return PackIconRemixIconKind.None;
            }
        }
    }
}
