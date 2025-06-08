using ForgeAir.Database.Abstract;
using ForgeAir.Database.Models.Enums;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Models
{
    [Table("Tracks")]
    public class Track : BaseEntity
    {
        [Required]
        [MaxLength(400)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(191)]
        public string? FilePath { get; set; }

        public int? Bpm { get; set; }

        [Column(TypeName = "time(3)")]
        public TimeSpan Duration { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? StartPoint { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? MixPoint { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? EndPoint { get; set; }

        [Column(TypeName = "time(3)")]
        public TimeSpan? Intro { get; set; }

        [Column(TypeName = "time(3)")]
        public TimeSpan? Outro { get; set; }

        [Column(TypeName = "time(3)")]
        public TimeSpan? HookInPoint { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? HookOutPoint { get; set; }

        public DateTime? ReleaseDate { get; set; }

        [MaxLength(200)]
        public string? Album { get; set; }

        public String? ISRC { get; set; }

        [Required]
        [EnumDataType(typeof(TrackType))]
        public TrackType? TrackType { get; set; }



        [Required]
        [EnumDataType(typeof(TrackStatus))]
        public TrackStatus? TrackStatus { get; set; }
        public DateTime DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateDeleted { get; set; }

        public ICollection<ArtistTrack>? TrackArtists { get; set; }
        public ICollection<Category>? Categories { get; set; }

        [Required]
        public bool containsVideoTrack { get; set; }


        // video tracks specific

        public bool containsSubtitles { get; set; }
        public bool externalSubtitles { get; set; }
        public string? externalSubtitlesPath { get; set; }

        public bool zoomAspectRatio { get; set; }
        public bool stretchAspectRatio { get; set; }

        public string DisplayArtists
        {
            get
            {
                if (TrackArtists == null || !TrackArtists.Any())
                    return string.Empty;

                return string.Join(", ", TrackArtists.Select(ta => ta.Artist?.Name));
            }
        }
        public string DisplayDuration => Duration.ToString(@"mm\:ss");

    }
}
