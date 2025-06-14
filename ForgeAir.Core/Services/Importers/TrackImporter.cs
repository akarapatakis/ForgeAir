using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.Services.Importers.Interfaces;
using ForgeAir.Core.Services.Importers.Migrators;
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
        private readonly TagReader _tagReader;
        private readonly ForgeAirDbContext _dbContext;

        public TrackImporter() { 
            _tagReader = new TagReader();
            _dbContext = new ForgeAirDbContext();
        }
        public async Task<Track> createTrackAsync(string filename, TrackType type, TimeSpan crossfadeTime, bool isVideo = false, string? artistString = null)
        {
            _dbContext.ChangeTracker.Clear(); // καθαρισμος της γαμημένης entity γιατι καυλανταει με ερρορς για διπλά entries ενώ γίνεται κανονικά το τσεκ
            Track track;


            if (!_tagReader.getComment(new Track { FilePath = filename }).Contains("Jazler 2.0.x InfoTag Radio Automation (www.jazler.com)")) {

                track = new Track()
                {
                    FilePath = filename,
                    Title = _tagReader.getTitle(new Track { FilePath = filename }),
                    Album = _tagReader.getAlbum(new Track { FilePath = filename }),
                    ISRC = _tagReader.getISRC(new Track { FilePath = filename }),
                    DateAdded = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    Bpm = 0,
                    StartPoint = TimeSpan.Zero,
                    Duration = _tagReader.getDuration(new Track { FilePath = filename }),
                    EndPoint = _tagReader.getDuration(new Track { FilePath = filename }),
                    MixPoint = _tagReader.getDuration(new Track { FilePath = filename }) - crossfadeTime,
                    TrackStatus = TrackStatus.Enabled,
                    ReleaseDate = _tagReader.getYear(new Track { FilePath = filename }),
                    containsVideoTrack = isVideo,
                    TrackType = type,
                    Categories = new List<Category>(),
                    TrackArtists = new List<ArtistTrack>() // σημαντικό

                };
            }
            else
            {
                var jztrack = JZRadio2TagMigrator.GenerateTrackFromInfoTag(_tagReader.Comment);

                track = new Track()
                {
                    FilePath = filename,
                    Title = jztrack.Title,
                    Album = jztrack.Album,
                    ISRC = _tagReader.getISRC(new Track { FilePath = filename }),
                    DateAdded = DateTime.UtcNow,
                    DateModified = DateTime.UtcNow,
                    Bpm = 0,
                    StartPoint = TimeSpan.Zero,
                    Duration = _tagReader.getDuration(new Track { FilePath = filename }),
                    EndPoint = _tagReader.getDuration(new Track { FilePath = filename }),
                    MixPoint = _tagReader.getDuration(new Track { FilePath = filename }) - crossfadeTime,
                    TrackStatus = TrackStatus.Enabled,
                    ReleaseDate = _tagReader.getYear(new Track { FilePath = filename }),
                    TrackType = type,
                    Categories = new List<Category>(),
                    TrackArtists = new List<ArtistTrack>() // σημαντικό
                };
            }


            await _dbContext.Tracks.AddAsync(track);
            await _dbContext.SaveChangesAsync(); 

            if (artistString == null)
            {
                if (!_tagReader.getComment(new Track { FilePath = filename }).Contains("Jazler 2.0.x InfoTag Radio Automation (www.jazler.com)"))
                {
                    foreach (Artist artistName in _tagReader.getArtists(track))
                    {

                        var existingArtist = await _dbContext.Artists
                            .FirstOrDefaultAsync(a => a.Name.ToLower() == artistName.Name.ToLower());

                        Artist artist = existingArtist ?? artistName;

                        if (artist.Id == 0)
                        {
                            await _dbContext.Artists.AddAsync(artist);
                            await _dbContext.SaveChangesAsync();
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
                else
                {
                    foreach (Artist artistName in JZRadio2TagMigrator.GenerateArtistFromInfoTag(_tagReader.Comment))
                    {
                        var existingArtist = await _dbContext.Artists
                            .FirstOrDefaultAsync(a => a.Name.ToLower() == artistName.Name.ToLower());

                        Artist artist = existingArtist ?? artistName;

                        if (artist.Id == 0)
                        {
                            await _dbContext.Artists.AddAsync(artist);
                            await _dbContext.SaveChangesAsync();
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
    .FirstOrDefaultAsync(a => a.Name.ToLower() == artistString.ToLower()) as Artist ?? new Artist() { Name = artistString };

                if (artist.Id == 0)
                {
                    await _dbContext.Artists.AddAsync(artist);
                    await _dbContext.SaveChangesAsync();
                }

                var artistTrack = new ArtistTrack
                {
                    ArtistId = artist.Id,
                    TrackId = track.Id,
                };

                await _dbContext.ArtistTracks.AddAsync(artistTrack);
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

            await _dbContext.SaveChangesAsync(); // Αποθήκευση των artistTracks
            _dbContext.ChangeTracker.Clear(); // καθαρισμος της γαμημένης entity γιατι καυλανταει με ερρορς για διπλά entries ενώ γίνεται κανονικά το τσεκ

            return track;
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
                    
                    await _dbContext.Artists.AddAsync(existingArtist);
                    await _dbContext.SaveChangesAsync();
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
