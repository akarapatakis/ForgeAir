using ForgeAir.Core.Services.Interfaces;
using ForgeAir.Core.Services.Models;
using ForgeAir.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services
{
    public class TracksService
    {


        public TracksService()
        {

        }

        public async Task<DTO.Track> GetTrack(int id)
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = $"SELECT * FROM Tracks WHERE Id={id}";
                return context.Database.SqlQueryRaw<DTO.Track>(sql).FirstOrDefault();
            }
        }

        public int GetTrackCount()
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = "SELECT COUNT(*) AS Value FROM Tracks";
                return context.Database.SqlQueryRaw<int>(sql).First();
            }
        }

        public Task<IEnumerable<Database.Models.Track>> GetTracks(int skip, int take)
        {
            throw new NotImplementedException();
        }

        public List<DTO.Track> GetTracks()
        {
            using (var context = new ForgeAirDbContext())
            {
                var sql = "SELECT Id, Title, Album, Duration, FilePath, StartPoint, EndPoint, MixPoint, HookInPoint, HookOutPoint FROM Tracks";
                return context.Database.SqlQueryRaw<DTO.Track>(sql).ToList();
            }
        }



    }
}
