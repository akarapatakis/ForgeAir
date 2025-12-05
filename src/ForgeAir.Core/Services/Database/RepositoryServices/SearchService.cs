using ForgeAir.Core.Services.Database.Interfaces;
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
    public class SearchService : ISearchService
    {
        private readonly IDbContextFactory<ForgeAirDbContext> _factory;

        public SearchService(IDbContextFactory<ForgeAirDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<Artist>> SearchArtists(string text)
        {
            using var ctx = _factory.CreateDbContext();
            return await ctx.Artists
                .Where(a => EF.Functions.Like(a.Name, $"{text}%"))
                .ToListAsync();
        }

        public async Task<List<Track>> SearchTracks(string text)
        {
            using var ctx = _factory.CreateDbContext();
            return await ctx.Tracks
                .Include(t => t.TrackArtists).ThenInclude(a => a.Artist)
                .Where(t => EF.Functions.Like(t.Title, $"{text}%"))
                .ToListAsync();
        }
    }

}
