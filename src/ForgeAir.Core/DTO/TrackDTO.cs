using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Tags;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using MahApps.Metro.IconPacks;

namespace ForgeAir.Core.DTO
{
    public class TrackDTO : INotifyPropertyChanged
    {

        public int Id { get; set; }

        private string _title;
        public string Title
        {
            get => _title;
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(nameof(Title));
                }
            }
        }
        public string FilePath { get; set; }

        private string _album;
        public string Album
        {
            get => _album;
            set
            {
                if (_album != value)
                {
                    _album = value;
                    OnPropertyChanged(nameof(Album));
                }
            }
        }

        public String? ISRC { get; set; }
        public DateTime? ReleaseDate { get; set; }
        public int? Bpm { get; set; }
        public TimeSpan Duration { get; set; }
        public TimeSpan? StartPoint { get; set; }
        public TimeSpan? MixPoint { get; set; }
        public TimeSpan? EndPoint { get; set; }
        public TimeSpan? Intro { get; set; }
        public TimeSpan? Outro { get; set; }
        public TimeSpan? HookInPoint { get; set; }
        public TimeSpan? HookOutPoint { get; set; }
        public TrackType TrackType { get; set; }
        public TrackStatus TrackStatus { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateDeleted { get; set; }
        public bool IsDynamicJingleAsset { get; set; } = false; // it needs to be treated differently in the audio engine

        public ICollection<ArtistTrackDTO> TrackArtists { get; set; }

        public ICollection<CategoryDTO> Categories { get; set; }


        public string DisplayArtists
        {
            get
            {
                if (TrackArtists == null || !TrackArtists.Any())
                    return string.Empty;

                return string.Join(", ", TrackArtists.Select(ta => ta.ArtistName));
            }
        }
        public string DisplayDuration => Duration.ToString(@"hh\:mm\:ss");
        public string DisplayDateAdded => DateAdded.ToString("yyyy-MM-dd");
        public string DisplayDateModified => DateModified?.ToString("yyyy-MM-dd");
        public string DisplayAddedBy => "";
        public string? DisplayType => Enum.GetName(this.TrackType) ?? null;

        public string DisplayCategories
        {
            get
            {
                if (Categories == null || !Categories.Any())
                    return "None";

                return string.Join("/", Categories.Select(ta => ta?.Name));
            }
        }
        // differs from queue opacity
        public string Background => "#40" + Core.Helpers.TrackTypeColorGen.Generate(TrackType).Substring(1); // add 0.7 opacity to have readable foreground
        public string? Foreground
        {
            get
            {
                if (TrackType == Database.Models.Enums.TrackType.Rebroadcast || IsDynamicJingleAsset)
                {
                    return "White";
                }
                return Categories.FirstOrDefault()?.Color ?? "White";
            }
        }
        public PackIconRemixIconKind Icon => Core.Helpers.TrackTypeIconGen.Generate(TrackType);
        public MemoryStream AlbumArt
        {
            get
            {
                if (TrackType != TrackType.Rebroadcast)
                {
                    try
                    {
                        var _albumart = new TagService(this).GetPicture()?.Data.Data;
                        MemoryStream _m = new MemoryStream();
                        if (_albumart != null)
                            return new MemoryStream(_albumart);
                        else
                        {
                            ForgeAir.Core.Properties.Resources.ImageResources.DefaultAlbumArt.Save(_m, ImageFormat.Png);
                            return _m;
                        }
                    }
                    catch
                    {
                        MemoryStream _m = new MemoryStream();
                        ForgeAir.Core.Properties.Resources.ImageResources.DefaultAlbumArt.Save(_m, ImageFormat.Png);
                        return _m;
                    }
                }
                else
                {
                    MemoryStream _m = new MemoryStream();
                    ForgeAir.Core.Properties.Resources.ImageResources.DefaultRebroadcastImage.Save(_m, ImageFormat.Png);
                    return _m;
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        public static TrackDTO FromEntity(Track track)
        {
            var dto = new TrackDTO
            {
                Id = track.Id,
                TrackType = track.TrackType,
                TrackStatus = track.TrackStatus,
                Title = track.Title,
                Duration = track.Duration,
                StartPoint = track.StartPoint,
                Intro = track.Intro,
                Outro = track.Outro,
                MixPoint = track.MixPoint,
                EndPoint = track.EndPoint,
                HookInPoint = track.HookInPoint,
                HookOutPoint = track.HookOutPoint,
                ReleaseDate = track.ReleaseDate,
                Album = track.Album,
                FilePath = track.FilePath,
                Bpm = track.Bpm,
                ISRC = track.ISRC,
                DateAdded = track.DateAdded,
                DateModified = track.DateModified,
                DateDeleted = track.DateDeleted,
                TrackArtists = track.TrackArtists != null ?
                    track.TrackArtists.Select(ta => ArtistTrackDTO.FromEntity(ta)).ToList() : new(),

                Categories = track.Categories != null ?
                    track.Categories.Select(c => CategoryDTO.FromEntity(c)).ToList() : new(),
            };
            return dto;
        }

        public static Track ToEntity(TrackDTO dto)
        {
            return new Track
            {
                Id = dto.Id,
                TrackArtists = dto.TrackArtists != null ? dto.TrackArtists.Select(a => ArtistTrackDTO.ToEntity(a)).ToList() : null,
                Categories = dto.Categories != null ? dto.Categories.Select(c => CategoryDTO.ToEntity(c)).ToList() : null,
                StartPoint = dto.StartPoint,
                EndPoint = dto.EndPoint,
                MixPoint = dto.MixPoint,
                TrackType = dto.TrackType,
                TrackStatus = dto.TrackStatus,
                DateAdded = dto.DateAdded,
                DateModified = dto.DateModified,
                DateDeleted = dto.DateDeleted,
                Title = dto.Title,
                Duration = dto.Duration,
                ReleaseDate = dto.ReleaseDate,
                Album = dto.Album,
                FilePath = dto.FilePath,
                Bpm = dto.Bpm,
                ISRC = dto.ISRC,
            };
        }

        public static TrackDTO FromDynamicJingle(DynamicJingle jingle)
        {
            return new TrackDTO
            {
                Title = jingle.Name,
                FilePath = jingle.Path,
                IsDynamicJingleAsset = true
            };
        }
    }


}