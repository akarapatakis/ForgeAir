using ForgeAir.Core.Shared;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Tracks
{
    public class Importer
    {
        public void AddTrack(Database.Models.Track inTrack) {

            using (var context = new ForgeAirDbContext()) {
                Database.Models.Track track = new Database.Models.Track
                {
                    Id = inTrack.Id,
                    TrackArtists = inTrack.TrackArtists,
                    Title = inTrack.Title,
                    Album = inTrack.Album,
                    DateAdded = inTrack.DateAdded,
                    ISRC = inTrack.ISRC,
                    Duration = inTrack.Duration,
                    FilePath = inTrack.FilePath,
                    DateDeleted = inTrack.DateDeleted,
                    ReleaseDate = inTrack.ReleaseDate,
                    Bpm = inTrack.Bpm,
                    HookInPoint = inTrack.HookInPoint,
                    HookOutPoint = inTrack.HookOutPoint,
                    MixPoint = inTrack.MixPoint,
                    StartPoint = inTrack.StartPoint,
                    TrackType = inTrack.TrackType,
                    Categories = inTrack.Categories,
                    DateModified = inTrack.DateModified,
                    TrackStatus = inTrack.TrackStatus,
                    EndPoint = inTrack.EndPoint,

                };

                    context.Tracks.Add(track);
                context.SaveChanges();
            }
        }

        public void AddArtist(Database.Models.Artist inArtist)
        {

            using (var context = new ForgeAirDbContext())
            {
                Database.Models.Artist artist = new Database.Models.Artist
                {
                    Id = inArtist.Id,
                    Name = inArtist.Name,
                    ArtistTracks = inArtist.ArtistTracks,

                };

                context.Artists.Add(artist);
                context.SaveChanges();
            }
        }

        public void AddArtistTrack(Database.Models.ArtistTrack inArtistTrack)
        {

            using (var context = new ForgeAirDbContext())
            {
                Database.Models.ArtistTrack artist = new Database.Models.ArtistTrack
                {
                    
                    TrackId = inArtistTrack.TrackId,
                    ArtistId = inArtistTrack.ArtistId,
                };

                context.ArtistTracks.Add(artist);
                context.SaveChanges();
            }
        }

        public void AddCategory(Database.Models.Category category) {
            using (var context = new ForgeAirDbContext())
            {
                Database.Models.Category cat = new Database.Models.Category
                {
                    Id = category.Id,
                    Name = category.Name,
                    Description = category.Description,
                    Color = category.Color,
                    ParentId = category.ParentId,
                };

                context.Category.Add(cat);
                context.SaveChanges();
                context.ChangeTracker.Clear();
                return;
            }


        }
    }
}
