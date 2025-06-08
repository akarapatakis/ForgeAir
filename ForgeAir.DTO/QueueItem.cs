using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models.Enums;
using ForgeAir.Database.Models;
namespace ForgeAir.DTO
{
    public class QueueItem
    {
        public int Place { get; set; }
        public Database.Models.Track Track { get; set; }

        public string DisplayTitle => Track.Title;


        public string DisplayArtists
        {
            get
            {
                if (Track.TrackArtists == null || !Track.TrackArtists.Any())
                    return string.Empty;

                return string.Join(", ", Track.TrackArtists.Select(ta => ta.Artist?.Name));
            }
        }
        public string DisplayDuration => Track.Duration.ToString(@"mm\:ss");

    }
}
