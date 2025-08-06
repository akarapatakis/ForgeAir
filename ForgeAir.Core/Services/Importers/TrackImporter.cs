using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Importers.Interfaces;
using ForgeAir.Core.Services.Importers.Migrators;
using ForgeAir.Core.Services.Tags;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Importers
{
    public class TrackImporter : ITrackImporter
    {
        private readonly RepositoryService<Track> _repositoryService;
        private readonly IDbContextFactory<ForgeAirDbContext> contextFactory;
        private ForgeAirDbContext _dbContext;
        public TrackImporter(IDbContextFactory<ForgeAirDbContext> _contextFactory)
        {
            contextFactory = _contextFactory;
            _repositoryService = new RepositoryService<Track>(contextFactory);
            _dbContext = contextFactory.CreateDbContext();
        }

        public async Task<Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum>> CreateNetStreamTrack(TrackImportModel stream)
        {

            _dbContext.ChangeTracker.Clear();

            var track = new TrackDTO
            {
                FilePath = stream.FilePath,
                Title = stream.StreamDisplayTitle ?? stream.FilePath,
                Album = "",
                ISRC = "",
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now,
                Bpm = 0,
                Duration = TimeSpan.Zero,
                MixPoint = TimeSpan.Zero,
                TrackStatus = TrackStatus.Enabled,
                ReleaseDate = DateTime.UtcNow,
                TrackType = TrackType.Rebroadcast,
                Intro = TimeSpan.Zero,
                Outro = TimeSpan.Zero,
                TrackArtists = new List<ArtistTrackDTO>() { }// σημαντικό
            };

            try
            {
                await _dbContext.Tracks.AddAsync(TrackDTO.ToEntity(track));
                await _dbContext.SaveChangesAsync();
                _dbContext.ChangeTracker.Clear();
                return SuccessResult();
            }
            catch
            {
                return DbErrorResult();
            }
        }
        public async Task<Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum>> CreateTrackAsync(TrackImportModel trackImport)
        {
            _dbContext.ChangeTracker.Clear();

            if (!File.Exists(trackImport.FilePath))
                return ErrorResult(ImportTrackErrorsEnum.TrackFileNotFound);

            var tagReader = new TagService(new DTO.TrackDTO { FilePath = trackImport.FilePath });

            var (trackDto, resolvedArtists) = await ImportFromTags(trackImport, tagReader);
            if (trackDto == null)
                return DbErrorResult();

            AssignIntroOutro(trackDto);
            var trackEntity = TrackDTO.ToEntity(trackDto);
            await _dbContext.Tracks.AddAsync(trackEntity);
            if (trackEntity.TrackArtists == null)
            {
                trackEntity.TrackArtists = new List<ArtistTrack>();
            }

            foreach (var artist in resolvedArtists)
            {
                bool alreadyExists = await _dbContext.ArtistTracks
                    .AnyAsync(at => at.ArtistId == artist.Id && at.TrackId == trackEntity.Id);

                if (!alreadyExists)
                {
                    trackEntity.TrackArtists.Add(new ArtistTrack
                    {
                        Artist = artist,
                        ArtistId = artist.Id,
                        Track = trackEntity,
                        TrackId = trackEntity.Id
                    });

                }
            }




            var resolvedCategories = new List<Category>();

            foreach (var categoryDto in trackImport.Categories)
            {
                var existingCategory = await _dbContext.Category
                    .FirstOrDefaultAsync(c => c.Id == categoryDto.Id || c.Name == categoryDto.Name);

                if (existingCategory == null)
                {
                    if (categoryDto == null || categoryDto.Name == null || categoryDto.Id == null)
                    {
                        continue;
                    }
                    existingCategory = new Category
                    {
                        Name = categoryDto.Name,
                        Description = categoryDto.Description,
                        Color = categoryDto.Color,
                        ParentId = categoryDto.ParentId
                    };

                    await _dbContext.Category.AddAsync(existingCategory);
                    await _dbContext.SaveChangesAsync();
                }

                resolvedCategories.Add(existingCategory);
            }

            // Finally assign
            trackEntity.Categories = resolvedCategories;



            await _dbContext.SaveChangesAsync(); // Save artist-track links
            await _dbContext.Entry(trackEntity)
            .Collection(t => t.TrackArtists)
            .Query()
            .Include(at => at.Artist)
            .LoadAsync();
            TrackDbChanged.RaiseTrackImported(TrackDTO.FromEntity(trackEntity));

            return SuccessResult();
        }


        private async Task<(TrackDTO, List<Artist>)> ImportFromJazlerInfoTag(TrackImportModel import, TagService tagReader)
        {
            return  await ImportFromTags(import, tagReader);
        }
        private async Task<(TrackDTO, List<Artist>)> ImportFromTags(TrackImportModel import, TagService tagReader)
        {
            var trackDto = new TrackDTO
            {
                FilePath = import.FilePath,
                Title = tagReader.Title,
                Album = tagReader.Album,
                ISRC = tagReader.ISRC,
                DateAdded = DateTime.Now,
                DateModified = DateTime.Now,
                Bpm = tagReader.BPM,
                StartPoint = TimeSpan.Zero,
                Duration = tagReader.AudioDuration,
                EndPoint = tagReader.AudioDuration,
                MixPoint = tagReader.AudioDuration - import.CrossfadeTime,
                TrackStatus = TrackStatus.Enabled,
                ReleaseDate = tagReader.ReleaseDate,
                TrackType = import.TrackType,
                TrackArtists = new List<ArtistTrackDTO>()
                
            };

            var resolvedArtists = new List<Artist>();

            foreach (var artistDto in tagReader.Artists ?? new())
            {
                var existing = await _dbContext.Artists.FirstOrDefaultAsync(a => a.Name.ToUpper() == artistDto.Name.ToUpper());
                if (existing == null)
                {
                    existing = new Artist { Name = artistDto.Name };
                    await _dbContext.Artists.AddAsync(existing);
                    await _dbContext.SaveChangesAsync();
                }
                    
                    resolvedArtists.Add(existing);
            }

            return (trackDto, resolvedArtists);
        }


        private void AssignIntroOutro(TrackDTO track)
        {
            if (track.TrackType != TrackType.Song || track.Duration.TotalSeconds <= 30)
            {
                track.Intro = TimeSpan.Zero;
                track.Outro = track.Duration;
            }
            else
            {
                track.Intro = TimeSpan.FromSeconds(10);
                track.Outro = track.Duration - TimeSpan.FromSeconds(10);
            }
        }

        private Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> SuccessResult() =>
            new() { { ImportTrackStatusEnum.Imported, ImportTrackErrorsEnum.NoError } };

        private Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> DbErrorResult() =>
            new() { { ImportTrackStatusEnum.Error, ImportTrackErrorsEnum.DbError } };

        private Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> ErrorResult(ImportTrackErrorsEnum error) =>
            new() { { ImportTrackStatusEnum.Error, error } };
    }
}
