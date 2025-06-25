using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ForgeAir.Core.DTO;
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
using Microsoft.EntityFrameworkCore.ChangeTracking;
using ArtistTrack = ForgeAir.Database.Models.ArtistTrack;

namespace ForgeAir.Core.Services.Importers
{
    public class TrackImporter : ITrackImporter
    {
        private TagService _tagReader;
        private readonly ForgeAirDbContext _dbContext;
        private RepositoryService<Track> RepositoryService;
        public TrackImporter() { 

            _dbContext = new ForgeAirDbContext();
            RepositoryService = new RepositoryService<Track>(new ForgeAirDbContextFactory());
        }

        public async Task<Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum>> createNetStreamTrack(TrackImportModel stream)
        {
            _dbContext.ChangeTracker.Clear(); // καθαρισμος της γαμημένης entity γιατι καυλανταει με ερρορς για διπλά entries ενώ γίνεται κανονικά το τσεκ
            TrackDTO track = new TrackDTO();

            track.FilePath = stream.FilePath;
            track.Title = stream.StreamDisplayTitle ?? stream.FilePath;
            track.Album = "";
            track.ISRC = "";
            track.DateAdded = DateTime.UtcNow;
            track.DateModified = DateTime.UtcNow;
            track.Bpm = _tagReader.BPM;
            track.Duration = TimeSpan.Zero;
            track.MixPoint = track.Duration;
            track.TrackStatus = ForgeAir.Database.Models.Enums.TrackStatus.Enabled;
            track.ReleaseDate = DateTime.UtcNow;
            track.TrackType = ForgeAir.Database.Models.Enums.TrackType.Rebroadcast;

            track.Duration = TimeSpan.Zero;
            track.Outro = track.Duration;
            track.Intro = track.Duration;

            try
            {
                await _dbContext.Tracks.AddAsync(TrackDTO.ToEntity(track));
                await _dbContext.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> { { ImportTrackStatusEnum.Error, ImportTrackErrorsEnum.DbError } };
            }


            return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> { { ImportTrackStatusEnum.Imported, ImportTrackErrorsEnum.NoError } };
        }
        public async Task<Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum>> createTrackAsync(TrackImportModel trackImport)
        {
            _dbContext.ChangeTracker.Clear(); // καθαρισμος της γαμημένης entity γιατι καυλανταει με ερρορς για διπλά entries ενώ γίνεται κανονικά το τσεκ
            TrackDTO track;
            _tagReader = new TagService(new DTO.TrackDTO() { FilePath = trackImport.FilePath });

            if (!File.Exists(trackImport.FilePath)) {
                return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> { { ImportTrackStatusEnum.Error, ImportTrackErrorsEnum.TrackFileNotFound } };
            }
            if (!_tagReader.Comment.Contains("Jazler 2.0.x InfoTag Radio Automation (www.jazler.com)")) {

                track = new TrackDTO()
                {
                    FilePath = trackImport.FilePath,
                    Title = _tagReader.Title,
                    Album = _tagReader.Album,
                    ISRC = _tagReader.ISRC,
                    DateAdded = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    Bpm = _tagReader.BPM,
                    StartPoint = TimeSpan.Zero,
                    Duration = _tagReader.AudioDuration,
                    EndPoint = _tagReader.AudioDuration,
                    MixPoint = _tagReader.AudioDuration - trackImport.CrossfadeTime,
                    TrackStatus = TrackStatus.Enabled,
                    ReleaseDate = _tagReader.ReleaseDate,
                    TrackType = trackImport.TrackType,
                    Categories = new List<CategoryDTO>(),
                    TrackArtists = new List<ArtistTrackDTO>() // σημαντικό

                };
            }
            else
            {
                var jztrack = JZRadio2TagMigrator.GenerateTrackFromInfoTag(_tagReader.Comment);

                track = new TrackDTO()
                {
                    FilePath = trackImport.FilePath,
                    Title = jztrack.Title,
                    Album = jztrack.Album,
                    ISRC = _tagReader.ISRC,
                    DateAdded = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    Bpm = _tagReader.BPM,
                    StartPoint = TimeSpan.Zero,
                    Duration = _tagReader.AudioDuration ,
                    EndPoint = _tagReader.AudioDuration,
                    MixPoint = _tagReader.AudioDuration - trackImport.CrossfadeTime,
                    TrackStatus = TrackStatus.Enabled,
                    ReleaseDate = _tagReader.ReleaseDate,
                    TrackType = trackImport.TrackType,
                    Categories = new List<CategoryDTO>(),
                    TrackArtists = new List<ArtistTrackDTO>() // σημαντικό
                };
            }


            await _dbContext.Tracks.AddAsync(TrackDTO.ToEntity(track));
            await _dbContext.SaveChangesAsync(); 

            if (trackImport.OverrideArtistString == null)
            {
                if (!_tagReader.Comment.Contains("Jazler 2.0.x InfoTag Radio Automation (www.jazler.com)"))
                {
                    foreach (ArtistDTO artistName in _tagReader.getArtists(track))
                    {
                        var existingArtist = await _dbContext.Artists
                            .FirstOrDefaultAsync(a => a.Name.ToLower() == artistName.Name.ToLower());

                        ArtistDTO artist = ArtistDTO.FromEntity(existingArtist) ?? artistName;

                        if (artist.Id == 0)
                        {
                            try
                            {
                                await _dbContext.Artists.AddAsync(ArtistDTO.ToEntity(artist));
                                await _dbContext.SaveChangesAsync();
                            }
                            catch (Exception ex)
                            {
                                return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> { { ImportTrackStatusEnum.Error, ImportTrackErrorsEnum.DbError } };
                            }
                        }

                        bool alreadyTracked = _dbContext.ChangeTracker
                            .Entries<ArtistTrack>()
                            .Any(e => e.Entity.ArtistId == artist.Id && e.Entity.TrackId == track.Id);

                        if (!alreadyTracked)
                        {
                            bool alreadyExists = await _dbContext.ArtistTracks
                                .AnyAsync(at => at.ArtistId == artist.Id && at.TrackId == track.Id);

                            if (!alreadyExists)
                            {
                                var artistTrack = new ArtistTrack
                                {
                                    ArtistId = artist.Id,
                                    TrackId = track.Id,
                                };
                                try
                                {
                                    await _dbContext.ArtistTracks.AddAsync(artistTrack);
                                }
                                catch (Exception ex)
                                {
                                    return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> { { ImportTrackStatusEnum.Error, ImportTrackErrorsEnum.DbError } };
                                }
                            }
                        }
                    }
                }
                else
                {
                    foreach (Artist artistName in JZRadio2TagMigrator.GenerateArtistFromInfoTag(_tagReader.Comment))
                    {
                        var existingArtist = await _dbContext.Artists
                            .FirstOrDefaultAsync(a => a.Name.ToLower() == artistName.Name.ToLower());

                        Artist artist = existingArtist ?? artistName;

                        if (artist.Id == 0)
                        {
                            try
                            {
                                await _dbContext.Artists.AddAsync(artist);
                                await _dbContext.SaveChangesAsync();
                            }
                            catch (Exception ex) {
                                return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> { { ImportTrackStatusEnum.Error, ImportTrackErrorsEnum.DbError } };
                            }
                        }

                        bool alreadyTracked = _dbContext.ChangeTracker
                            .Entries<ArtistTrack>()
                            .Any(e => e.Entity.ArtistId == artist.Id && e.Entity.TrackId == track.Id);

                        if (!alreadyTracked)
                        {
                            bool alreadyExists = await _dbContext.ArtistTracks
                                .AnyAsync(at => at.ArtistId == artist.Id && at.TrackId == track.Id);

                            if (!alreadyExists)
                            {
                                var artistTrack = new ArtistTrack
                                {
                                    ArtistId = artist.Id,
                                    TrackId = track.Id,
                                };
                                await _dbContext.ArtistTracks.AddAsync(artistTrack);
                            }
                        }
                    }

                    
                }
            }
            else
            {
                Artist artist = await _dbContext.Artists
    .FirstOrDefaultAsync(a => a.Name.ToLower() == trackImport.OverrideArtistString.ToLower()) as Artist ?? new Artist() { Name = trackImport.OverrideArtistString };

                if (artist.Id == 0)
                {
                    try
                    {
                        await _dbContext.Artists.AddAsync(artist);
                        await _dbContext.SaveChangesAsync();
                    }
                    catch (Exception ex) {
                        return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> { { ImportTrackStatusEnum.Error, ImportTrackErrorsEnum.DbError } };

                    }
                }

                var artistTrack = new ArtistTrack
                {
                    ArtistId = artist.Id,
                    TrackId = track.Id,
                };

                try
                {
                    await _dbContext.ArtistTracks.AddAsync(artistTrack);
                }
                catch (Exception ex)
                {
                    return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> { { ImportTrackStatusEnum.Error, ImportTrackErrorsEnum.DbError } };

                }
            }

            // Outro / Intro λογική
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
            try
            {
                await _dbContext.SaveChangesAsync(); // Αποθήκευση των artistTracks
                _dbContext.ChangeTracker.Clear(); // καθαρισμος της γαμημένης entity γιατι καυλανταει με ερρορς για διπλά entries ενώ γίνεται κανονικά το τσεκ

            }
            catch (Exception ex) {
                return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum> { { ImportTrackStatusEnum.Error, ImportTrackErrorsEnum.DbError } };

            }

            return new Dictionary<ImportTrackStatusEnum, ImportTrackErrorsEnum>{ { ImportTrackStatusEnum.Imported, ImportTrackErrorsEnum.NoError }};

        }

        public async Task<bool> LinkArtistToTrackAsync(Track track)
        {
            if (track.TrackArtists == null || track.TrackArtists.Count == 0)
                return false;

            foreach (var trackArtist in track.TrackArtists)
            {
                var existingArtist = await _dbContext.Artists
                    .FirstOrDefaultAsync(a => a.Name.ToLower() == trackArtist.Artist.Name.ToLower());

                if (existingArtist == null)
                {
                    existingArtist = trackArtist.Artist;
                    
                    try
                    {
                        await _dbContext.Artists.AddAsync(existingArtist);
                        await _dbContext.SaveChangesAsync();
                    }
                    catch
                    {
                        return false;
                    }
                }

                bool alreadyLinked = await _dbContext.ArtistTracks
                    .AnyAsync(at => at.ArtistId == existingArtist.Id && at.TrackId == track.Id);

                if (!alreadyLinked)
                {
                    bool alreadyTracked = _dbContext.ChangeTracker
                        .Entries<ArtistTrack>()
                        .Any(e => e.Entity.ArtistId == existingArtist.Id && e.Entity.TrackId == track.Id);

                    if (!alreadyTracked)
                    {
                        bool alreadyExists = await _dbContext.ArtistTracks
                            .AnyAsync(at => at.ArtistId == existingArtist.Id && at.TrackId == track.Id);

                        if (!alreadyExists)
                        {
                            var artistTrack = new ArtistTrack
                            {
                                ArtistId = existingArtist.Id,
                                TrackId = track.Id,
                            };
                            await _dbContext.ArtistTracks.AddAsync(artistTrack);
                        }
                    }

                }
            }

            return true;
        }


    }
}
