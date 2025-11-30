using ForgeAir.Database;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Database.RepositoryServices
{
    public class StationService
    {
        private readonly IDbContextFactory<ForgeAirDbContext> _factory;

        public StationService(IDbContextFactory<ForgeAirDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<Station?> GetStationByIdAsync(int id)
        {
            using var ctx = _factory.CreateDbContext();
            return await ctx.Stations.AsNoTracking().FirstOrDefaultAsync(s => s.Id == id);
        }

        public async Task UpdateStationAsync(Station station)
        {
            using var ctx = _factory.CreateDbContext();
            ctx.Stations.Update(station);
            await ctx.SaveChangesAsync();
        }
    }

}
