using ForgeAir.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services
{
    public class ArtistTracksService
    {
        public DTO.ArtistTrack GetArtist(int trackId)
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = $"SELECT * FROM Artists_Tracks WHERE TrackId={trackId}";
                return context.Database.SqlQueryRaw<DTO.ArtistTrack>(sql).FirstOrDefault();
            }
        }

        public int GetArtistCount()
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = "SELECT COUNT(*) AS Value FROM Artists_Tracks";
                return context.Database.SqlQueryRaw<int>(sql).First();
            }
        }

        public Task<IEnumerable<Database.Models.ArtistTrack>> GetArtists(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public List<DTO.ArtistTrack> GetArtists()
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = "SELECT * FROM Artists_Tracks";
                return context.Database.SqlQueryRaw<DTO.ArtistTrack>(sql).ToList();
            }
        }
    }
}
