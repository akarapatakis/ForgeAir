using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;

namespace ForgeAir.Core.DTO
{
    public class TrackDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string FilePath { get; set; }
        public string Album { get; set; }
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
        public string DisplayDuration => Duration.ToString(@"mm\:ss");

        public string? DisplayType => Enum.GetName(this.TrackType) ?? null;

        public string DisplayCategories
        {
            get
            {
                if (Categories == null || !Categories.Any())
                    return string.Empty;

                return string.Join("/", Categories.Select(ta => ta?.Name));
            }
        }
        // differs from queue opacity
        public string Background => "#40" + Core.Helpers.TrackTypeColorGen.Generate(TrackType).Substring(1); // add 0.7 opacity to have readable foreground because i thought that burning a person's eyes is a good idea :/
        public string? Foreground => Categories.FirstOrDefault()?.Color ?? "White";

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
