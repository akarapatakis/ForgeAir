using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Models;

namespace ForgeAir.Core.DTO
{
    public class ArtistTrackDTO
    {
        public int ArtistId { get; set; }
        public string ArtistName { get; set; }
        public int TrackId { get; set; }
        public int OrderIndex { get; set; }

        public static ArtistTrackDTO FromEntity(ArtistTrack artistTrack)
        {
            return new ArtistTrackDTO
            {
                ArtistId = artistTrack.ArtistId,
                ArtistName = artistTrack.Artist.Name,
                TrackId = artistTrack.TrackId,
                OrderIndex = artistTrack.OrderIndex,
            };
        }

        public static ArtistTrack ToEntity(ArtistTrackDTO dto)
        {
            return new Database.Models.ArtistTrack
            {
                ArtistId = dto.ArtistId,
                TrackId = dto.TrackId,
                OrderIndex = dto.OrderIndex,
            };
        }
    }
}
