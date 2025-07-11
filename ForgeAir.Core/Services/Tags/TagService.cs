using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ManagedBass;
using TagLib;
using MediaInfo;
using Microsoft.Extensions.Logging;
using System.Globalization;
using ForgeAir.Core.Services.Tags.Interfaces;
using ForgeAir.Core.DTO;
using static System.Net.Mime.MediaTypeNames;
namespace ForgeAir.Core.Services.Tags
{
    public class TagService : ITagService
    {

        private DTO.TrackDTO track;
        private string filePath;

        TagLib.File _tag;

        public TagService(DTO.TrackDTO _track) {
            try
            {
                filePath = Encoding.UTF8.GetString(Encoding.Default.GetBytes(_track.FilePath));
                _tag = TagLib.File.Create(Encoding.UTF8.GetString(Encoding.Default.GetBytes(_track.FilePath)));
            }

            catch
            {
                return;
            }

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

        public string Title { get => _tag.Tag.Title ?? Path.GetFileNameWithoutExtension(filePath); }




        public ObservableCollection<ArtistDTO> getArtists()
        {
            var artists = new ObservableCollection<ArtistDTO>();

            try
            {


                // First: Add performers from TagLib
                if (_tag.Tag.Performers != null && _tag.Tag.Performers.Length > 0)
                {
                    foreach (var performer in _tag.Tag.Performers)
                    {
                        if (!string.IsNullOrWhiteSpace(performer))
                            artists.Add(new ArtistDTO { Name = performer.Trim() });
                    }
                }

                // Optional: Add from getArtist if needed (not clear what it does)
                var fallbackName = getArtist()?.Name;
                if (!string.IsNullOrWhiteSpace(fallbackName))
                {
                    foreach (var name in fallbackName.Split(";"))
                    {
                        var trimmed = name.Trim();
                        if (!string.IsNullOrEmpty(trimmed) && !artists.Any(a => a.Name == trimmed))
                        {
                            artists.Add(new ArtistDTO { Name = trimmed });
                        }
                    }
                }

                // If still empty, add "Unknown Artist"
                if (!artists.Any())
                {
                    artists.Add(new ArtistDTO { Name = "Unknown Artist" });
                }
            }
            catch (UnsupportedFormatException)
            {
                artists.Add(new ArtistDTO { Name = "Unknown Artist" });
            }

            return artists;
        }

        public ArtistDTO getArtist() // fuck
        {
            ArtistDTO artist = new ArtistDTO();
            string[] artists;


            try
            {
                if (_tag.Tag.Performers == null)
                {
                    artist.Name = "Unknown Artist";
                    return artist;
                }
                else
                {
                    artist.Name = string.Concat(_tag.Tag.Performers);
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
