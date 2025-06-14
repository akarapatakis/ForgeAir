using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Shared;
using ManagedBass;
using TagLib;
using MediaInfo;
using Microsoft.Extensions.Logging;
using System.Globalization;
using ForgeAir.Core.Services.Tags.Interfaces;
namespace ForgeAir.Core.Services.Tags
{
    public class TagService : ITagService
    {

        private DTO.TrackDTO track;

        TagLib.File _tag;

        public TagService(DTO.TrackDTO _track) {
            _tag = TagLib.File.Create(_track.FilePath);


        }

        public MediaInfo.Model.CoverInfo? GetPictureMI()
        {
            try
            {
                using ILoggerFactory factory = LoggerFactory.Create(builder => { });
                ILogger logger = factory.CreateLogger("Program");
                var media = new MediaInfoWrapper(track.FilePath, logger);

                if (media.Tags.Covers.FirstOrDefault() == null)
                {
                    return null;
                } else
                {
                    return media.Tags.Covers.FirstOrDefault();
                }
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        public IPicture? GetPicture() {
            try
            {
                var tfile = TagLib.File.Create(track.FilePath);
                if (tfile.Tag.Pictures == null)
                {
                    return null;
                }
                else
                {
                    if (tfile.Tag.Pictures.Length <= 0)
                    {
                        return null;
                    }
                    else
                    {
                        return tfile.Tag.Pictures[0];
                    }
                }
            }
            catch (UnsupportedFormatException)
            {
                return null;
            }
        }

        public string Title { get => _tag.Tag.Title ?? Path.GetFileNameWithoutExtension(track.FilePath); }




        public ForgeAir.Database.Models.Artist[] getArtists(ForgeAir.Database.Models.Track track)
        {
            var artists = new List<ForgeAir.Database.Models.Artist>();

            try
            {
                var tfile = TagLib.File.Create(track.FilePath);

                // First: Add performers from TagLib
                if (tfile.Tag.Performers != null && tfile.Tag.Performers.Length > 0)
                {
                    foreach (var performer in tfile.Tag.Performers)
                    {
                        if (!string.IsNullOrWhiteSpace(performer))
                            artists.Add(new ForgeAir.Database.Models.Artist { Name = performer.Trim() });
                    }
                }

                // Optional: Add from getArtist if needed (not clear what it does)
                var fallbackName = getArtist(track)?.Name;
                if (!string.IsNullOrWhiteSpace(fallbackName))
                {
                    foreach (var name in fallbackName.Split(";"))
                    {
                        var trimmed = name.Trim();
                        if (!string.IsNullOrEmpty(trimmed) && !artists.Any(a => a.Name == trimmed))
                        {
                            artists.Add(new ForgeAir.Database.Models.Artist { Name = trimmed });
                        }
                    }
                }

                // If still empty, add "Unknown Artist"
                if (!artists.Any())
                {
                    artists.Add(new ForgeAir.Database.Models.Artist { Name = "Unknown Artist" });
                }
            }
            catch (UnsupportedFormatException)
            {
                artists.Add(new ForgeAir.Database.Models.Artist { Name = "Unknown Artist" });
            }

            return artists.ToArray();
        }

        public ForgeAir.Database.Models.Artist getArtist(ForgeAir.Database.Models.Track track) // fuck
        {
            ForgeAir.Database.Models.Artist artist = new ForgeAir.Database.Models.Artist();
            string[] artists;


            try
            {
                var tfile = TagLib.File.Create(track.FilePath);
                if (tfile.Tag.Performers == null)
                {
                    artist.Name = "Unknown Artist";
                    return artist;
                }
                else
                {
                    artist.Name = string.Concat(tfile.Tag.Performers);
                    return artist;
                }
            }
            catch (UnsupportedFormatException)
            {
                artist.Name = "Unknown Artist";
                return artist;
            }
        }

        public DateTime? ReleaseDate
        {
            get
            {
                if (_tag.Tag.Year <= 1)
                    return null;

                if (DateTime.TryParseExact(
                    _tag.Tag.Year.ToString(),
                    "yyyy",
                    CultureInfo.InvariantCulture,
                    DateTimeStyles.None,
                    out DateTime parsedDate))
                {
                    return parsedDate;
                }

                return null;
            }
        }




        public bool containsVideoTrack()
        {
            try
            {
                if (_tag.Properties.MediaTypes == MediaTypes.Video) return true;
            }
            catch (UnsupportedFormatException)
            {
                using ILoggerFactory factory = LoggerFactory.Create(builder => { });
                ILogger logger = factory.CreateLogger("Program");
                var media = new MediaInfoWrapper(track.FilePath, logger);

                if (media.HasVideo)return true;

            }
            return false;
        }

        public string Comment { get => _tag.Tag.Comment ?? string.Empty; }

        public string Album { get => _tag.Tag.Album ?? ""; }
        public string[]? Gernes { get => _tag.Tag.Genres ?? null; }
        public string? ISRC { get => _tag.Tag.ISRC ?? string.Empty; }
        public TimeSpan AudioDuration { get => _tag.Properties.Duration; }
        public int BPM { get => (int?)_tag.Tag.BeatsPerMinute ?? 0; }

    }
}
