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

namespace ForgeAir.Core.AudioEngine
{
    public class TagReader
    {
        public TagReader() { }

        public void addMetadataAuto(Database.Models.Track track)
        {
            track.Album = getAlbum(track);
            track.Title = getTitle(track);
            //track.ReleaseDate = getYear(track);
            track.Bpm = getBPM(track);
            track.ISRC = getISRC(track);
            track.Duration = getDuration(track);
            //track.TrackArtists = getArtist(track); // todo: fix
        }

        public MediaInfo.Model.CoverInfo? GetPictureMI(Database.Models.Track track)
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
        public TagLib.IPicture? GetPicture(Database.Models.Track track) {
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
            catch (TagLib.UnsupportedFormatException)
            {
                return null;
            }
        }
        public string getTitle(Database.Models.Track track) {
            try
            {
                var tfile = TagLib.File.Create(track.FilePath);
                if (tfile.Tag.Title == null)
                {
                    return System.IO.Path.GetFileNameWithoutExtension(track.FilePath);
                }
                else
                {
                    return tfile.Tag.Title;

                }
            }
            catch (TagLib.UnsupportedFormatException)
            {
                return System.IO.Path.GetFileNameWithoutExtension(track.FilePath);
            }

        }

        public ForgeAir.Database.Models.Artist[] getArtists(Database.Models.Track track)
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
            catch (TagLib.UnsupportedFormatException)
            {
                artists.Add(new ForgeAir.Database.Models.Artist { Name = "Unknown Artist" });
            }

            return artists.ToArray();
        }

        public ForgeAir.Database.Models.Artist getArtist(Database.Models.Track track) // fuck
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
                    artist.Name = String.Concat(tfile.Tag.Performers);
                    return artist;
                }
            }
            catch (TagLib.UnsupportedFormatException)
            {
                artist.Name = "Unknown Artist";
                return artist;
            }



        }

        public string getComment(Database.Models.Track track)
        {
            try
            {
                var tfile = TagLib.File.Create(track.FilePath);
                if (tfile.Tag.Comment == null)
                {
                    return String.Empty;
                }
                else
                {
                    return tfile.Tag.Comment;
                }
            }
            catch (TagLib.UnsupportedFormatException)
            {
                return String.Empty;
            }
        }
        public string getAlbum(Database.Models.Track track)
        {
            try
            {
                var tfile = TagLib.File.Create(track.FilePath);
                if (tfile.Tag.Album == null)
                {
                    return "Unknown Album";
                }
                else
                {
                    return tfile.Tag.Album;
                }
            }
            catch (TagLib.UnsupportedFormatException)
            { return "Unknown Album"; }
        }
        public DateTime getYear(Database.Models.Track track)
        {
            try
            {
                var tfile = TagLib.File.Create(track.FilePath);
                if (tfile.Tag.Year == null || tfile.Tag.Year <= 1)
                {
                    return new DateTime(1900);
                }
                else
                {
                    DateTime dt;
                    if (DateTime.TryParseExact(tfile.Tag.Year.ToString(), tfile.Tag.Year.ToString(), CultureInfo.InvariantCulture,
                                              DateTimeStyles.None, out dt)) ;
                    return dt;
                }
            }
            catch (TagLib.UnsupportedFormatException)
            { return new DateTime(1900); } //todo: add proper date
        }

        public bool containsVideoTrack(Database.Models.Track track)
        {
            try
            {
				var tfile = TagLib.File.Create(track.FilePath);
                if (tfile.Properties.MediaTypes == MediaTypes.Video)
                {
                    return true;
                }
                else { return false; }
			}
            catch (TagLib.UnsupportedFormatException)
            {
				using ILoggerFactory factory = LoggerFactory.Create(builder => { });
				ILogger logger = factory.CreateLogger("Program");
				var media = new MediaInfoWrapper(track.FilePath, logger);

                if (media.HasVideo)
                {
                    return true;
                }
                else
                {
                    return false;
                }

            }

		}
        public string getGenre(Database.Models.Track track)
        {
            var tfile = TagLib.File.Create(track.FilePath);
            if (tfile.Tag.FirstGenre == null)
            {
                return String.Empty;
            }
            else
            {
                return tfile.Tag.FirstGenre;
            }
        }
        public string getISRC(Database.Models.Track track)
        {
            try
            {
                var tfile = TagLib.File.Create(track.FilePath);
                if (tfile.Tag.ISRC == null)
                {
                    return String.Empty;
                }
                else
                {
                    return tfile.Tag.ISRC;
                }
            }
            catch (TagLib.UnsupportedFormatException)
            { return String.Empty; }

        }
        public TimeSpan getDuration(Database.Models.Track track)
        {
            using ILoggerFactory factory = LoggerFactory.Create(builder => { });
            ILogger logger = factory.CreateLogger("Program");
            try
            {
                var tfile = TagLib.File.Create(track.FilePath);
                if (tfile.Properties.Duration == TimeSpan.Zero)
                {
                    var media = new MediaInfoWrapper(track.FilePath, logger);
                    return TimeSpan.FromMilliseconds(media.Duration);
                }
                return tfile.Properties.Duration;
            }

            catch (TagLib.UnsupportedFormatException)
            {
                var media = new MediaInfoWrapper(track.FilePath, logger);
                return TimeSpan.FromMilliseconds(media.Duration);

            }
        }
        public int getBPM(Database.Models.Track track)
        {
            var tfile = TagLib.File.Create(track.FilePath);
            if (tfile.Tag.BeatsPerMinute == null)
            {
                return 0;
            }
            else
            {
                return (int)tfile.Tag.BeatsPerMinute;
            }
        }
    }
}
