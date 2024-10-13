using ForgeAir.Core.Services.Interfaces;
using ForgeAir.Core.Services.Models;
using ForgeAir.Database;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services
{
    public class TracksService : ITracksService
    {
        private readonly IDbContextFactory<ForgeAirDbContext> dbContextFactory;

        public TracksService(IDbContextFactory<ForgeAirDbContext> dbContextFactory)
        {
            this.dbContextFactory = dbContextFactory;
        }

        public Task<Track> GetTrack(int id)
        {
            throw new NotImplementedException();
        }

        public Task<int> GetTrackCountAsync(string query = "")
        {
            throw new NotImplementedException();
        }

        public Task<IEnumerable<Track>> GetTracks(int skip, int take)
        {
            throw new NotImplementedException();
        }
    }
}
