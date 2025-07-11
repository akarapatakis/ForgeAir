using ForgeAir.Core.DTO;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using MySqlConnector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Database
{
    public class RepositoryService<T> where T : class
    {
        private readonly IDbContextFactory<ForgeAirDbContext> _contextFactory;
        public RepositoryService(IDbContextFactory<ForgeAirDbContext> contextFactory)
        {
            _contextFactory = contextFactory ?? throw new ArgumentNullException(nameof(contextFactory));
        }

        public async Task<int> GetCountOf(ModelTypesEnum model)
        {
            using var _context = _contextFactory.CreateDbContext();
            return model switch
            {
                ModelTypesEnum.Track => await _context.Tracks.CountAsync(),
                ModelTypesEnum.Artist => await _context.Artists.CountAsync(),
                ModelTypesEnum.ArtistTrack => await _context.ArtistTracks.CountAsync(),
                ModelTypesEnum.Category => await _context.Category.CountAsync(),
                ModelTypesEnum.Video => await _context.Videos.CountAsync(),
                _ => 0,
            };
        }

        public async Task<bool> ArtistExists(DTO.ArtistDTO artist)
        {
            using var _context = _contextFactory.CreateDbContext();
            return await _context.Artists.AnyAsync(e => e.Name == artist.Name);
        }

        public async Task<T?> GetSpecificById(ModelTypesEnum model, int id)
        {

            using var _context = _contextFactory.CreateDbContext();
            return model switch
            {
                ModelTypesEnum.Track => await _context.Tracks
                    .Include(t => t.TrackArtists)
                        .ThenInclude(ta => ta.Artist)
                    .Include(t => t.Categories)
                    .AsNoTracking()
                    .FirstOrDefaultAsync(t => t.Id == id) as T,

                ModelTypesEnum.Artist => await _context.Artists.FindAsync(id) as T,
                ModelTypesEnum.ArtistTrack => await _context.ArtistTracks.FirstOrDefaultAsync(p => p.TrackId == id) as T,
                ModelTypesEnum.Category => await _context.Category.FindAsync(id) as T,
                ModelTypesEnum.Video => await _context.Videos.FindAsync(id) as T,
                _ => null,
            };
        }

        public async Task<T?> GetSpecificByName(ModelTypesEnum model, string name)
        {
            using var _context = _contextFactory.CreateDbContext();
            return model switch
            {
                ModelTypesEnum.Track => await _context.Tracks.FirstOrDefaultAsync(p => p.Title == name) as T,
                ModelTypesEnum.Artist => await _context.Artists.FirstOrDefaultAsync(p => p.Name == name) as T,
                ModelTypesEnum.Category => await _context.Category.FirstOrDefaultAsync(p => p.Name == name) as T,
                ModelTypesEnum.Video => await _context.Videos.FirstOrDefaultAsync(p => p.Name == name) as T,
                _ => null,
            };
        }

        public async Task<List<T>> GetAll(ModelTypesEnum model)
        {
            using var _context = _contextFactory.CreateDbContext();
            return model switch
            {
                ModelTypesEnum.Track => await _context.Tracks
                    .Include(t => t.TrackArtists)
                    .ThenInclude(ta => ta.Artist)
                    .Include(t => t.Categories)
                    .AsNoTracking()
                    .ToListAsync() as List<T>,

                ModelTypesEnum.Artist => await _context.Artists
                    .AsNoTracking()
                    .ToListAsync() as List<T>,

                ModelTypesEnum.ArtistTrack => await _context.ArtistTracks
                    .AsNoTracking()
                    .ToListAsync() as List<T>,

                ModelTypesEnum.Category => await _context.Category
                    .AsNoTracking()
                    .ToListAsync() as List<T>,

                ModelTypesEnum.FX => await _context.Fx
                    .AsNoTracking()
                    .ToListAsync() as List<T>,

                ModelTypesEnum.Video => await _context.Videos
                    .AsNoTracking()
                    .ToListAsync() as List<T>,
                _ => new List<T>(),

            };
        }

        public async Task<int> GetIdByCount(int count, ModelTypesEnum model)
        {
            using var _context = _contextFactory.CreateDbContext();
            return model switch
            {
                ModelTypesEnum.Track => await _context.Tracks.
                OrderBy(x => x.Id)
                .Skip(count - 1)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(),

                ModelTypesEnum.Artist => await _context.Artists.
                OrderBy(x => x.Id)
                .Skip(count - 1)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(),

                ModelTypesEnum.Category => await _context.Category.
                OrderBy(x => x.Id)
                .Skip(count - 1)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(),

                ModelTypesEnum.FX => await _context.Fx.
                OrderBy(x => x.Id)
                .Skip(count - 1)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(),

                ModelTypesEnum.Video => await _context.Videos.
                OrderBy(x => x.Id)
                .Skip(count - 1)
                .Select(x => x.Id)
                .FirstOrDefaultAsync(),
            };
        }

        public async Task<List<T>> SearchAsync(string name, ModelTypesEnum model)
        {
            using var _context = _contextFactory.CreateDbContext();
            try
            {
                if (Int32.TryParse(name, out int id)) // search by id
                {
                    if (await GetSpecificById(model, id) is T t && t != null)
                    {
                        return new List<T> { t };
                    }
                }

                return model switch
                {
                    ModelTypesEnum.Artist => await _context.Artists
                    .Where(x => EF.Functions.Like(x.Name, $"{name}%"))
                    .ToListAsync() as List<T>,

                    ModelTypesEnum.Track => await _context.Tracks
                    .Where(x => EF.Functions.Like(x.Title, $"{name}%") || EF.Functions.Like(x.TrackArtists.FirstOrDefault(), $"{name}%") || EF.Functions.IsMatch(x.FilePath, $"{name}%", MySqlMatchSearchMode.NaturalLanguage))
                    .ToListAsync() as List<T>,

                    ModelTypesEnum.Video => await _context.Videos
                    .Where(x => EF.Functions.Like(x.Name, $"{name}%"))
                    .ToListAsync() as List<T>
                };
            }
            catch (Exception ex)
            {
                return new List<T>();
            }
        }

        public async Task<List<TrackDTO>> GetTracksByCategoryAsync(CategoryDTO category)
        {
            using var _context = _contextFactory.CreateDbContext();
            var trackDTOs = new List<TrackDTO>();

            try
            {
                ICollection<Track> tracks = await _context.Tracks.Where(x => x.Categories.Any(c => c.Name == category.Name)).ToListAsync();
                foreach (var track in tracks)
                {
                    trackDTOs.Add(TrackDTO.FromEntity(track));
                }
                return trackDTOs;
            
            }
            catch (Exception ex)
            {
                return new List<TrackDTO>();
            }
        }
        public async Task<List<TrackDTO>> GetTracksByArtistAsync(ArtistDTO artist)
        {
            using var _context = _contextFactory.CreateDbContext();
            var trackDTOs = new List<TrackDTO>();

            try
            {
                ICollection<Track> tracks = await _context.Tracks.Where(x => x.TrackArtists.Any(c => c.Artist.Name == artist.Name)).ToListAsync();
                foreach (var track in tracks)
                {
                    trackDTOs.Add(TrackDTO.FromEntity(track));
                }
                return trackDTOs;

            }
            catch (Exception ex)
            {
                return new List<TrackDTO>();
            }
        }
    }
}
