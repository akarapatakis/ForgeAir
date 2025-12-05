using ForgeAir.Core.DTO;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Database.RepositoryServices
{
    public class TrackService : ITracksService
    {
        private readonly IDbContextFactory<ForgeAirDbContext> _factory;

        public TrackService(IDbContextFactory<ForgeAirDbContext> factory)
        {
            _factory = factory;
        }

        public async Task<List<TrackDTO>> GetByCategory(CategoryDTO category)
        {
            using var ctx = _factory.CreateDbContext();

            return await ctx.Tracks
                .Include(t => t.TrackArtists).ThenInclude(ta => ta.Artist)
                .Include(t => t.Categories)
                .Where(t => t.Categories.Any(c => c == CategoryDTO.ToEntity(category) || c.Name == category.Name))
                .Select(t => TrackDTO.FromEntity(t))
                .ToListAsync();
        }

        public async Task<List<TrackDTO>> GetByConditionAsync(ModelTypesEnum type, Expression<Func<Track, bool>> predicate) {
            using var ctx = _factory.CreateDbContext();
            return await ctx.Tracks.Include(t => t.TrackArtists)
                .ThenInclude(a => a.Artist).Include(t => t.Categories).Where(predicate).Select(t => TrackDTO.FromEntity(t)).ToListAsync(); 
        }
        public async Task<IEnumerable<TrackDTO>> GetTracks(int skip, int take)
        {
            using var ctx = _factory.CreateDbContext();

            return await ctx.Tracks
                .Include(t => t.TrackArtists).ThenInclude(ta => ta.Artist)
                .Include(t => t.Categories)
                .OrderBy(t => t.Id)
                .Skip(skip)
                .Take(take)
                .AsNoTracking()
                .Select(t => TrackDTO.FromEntity(t))
                .ToListAsync();
        }

        public async Task<List<TrackDTO>> GetByArtist(ArtistDTO artist)
        {
            using var ctx = _factory.CreateDbContext();

            return await ctx.Tracks
                .Include(t => t.TrackArtists).ThenInclude(ta => ta.Artist)
                .Where(t => t.TrackArtists.Any(a => a.Artist == ArtistDTO.ToEntity(artist) || a.Artist.Name == artist.Name))
                .Select(t => TrackDTO.FromEntity(t))
                .ToListAsync();
        }

        public int GetTrackCount()
        {
            using var ctx = _factory.CreateDbContext();
            return ctx.Tracks.Count();
        }

        public async Task<TrackDTO> GetTrack(int id)
        {
            using var ctx = _factory.CreateDbContext();
            return await ctx.Tracks
                .Include(t => t.TrackArtists).ThenInclude(ta => ta.Artist)
                .Include(t => t.Categories)
                .AsNoTracking()
                .Select(t => TrackDTO.FromEntity(t))
                .FirstOrDefaultAsync(t => t.Id == id);
        }
    }

}
