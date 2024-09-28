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
    public class Track : BaseEntity
    {
        [Required]
        [MaxLength(200)]
        public string? Title { get; set; }

        [Required]
        [MaxLength(500)]
        public string? FilePath { get; set; }

        public int? Bpm { get; set; }

        [Column(TypeName = "double(11,5)")]
        public double Duration { get; set; }

        [Column(TypeName = "double(11,5)")]
        public double? StartPoint { get; set; }

        [Column(TypeName = "double(11,5)")]
        public double? MixPoint { get; set; }

        [Column(TypeName = "double(11,5)")]
        public double? EndPoint { get; set; }

        [Column(TypeName = "double(11,5)")]
        public double? HookInPoint { get; set; }

        [Column(TypeName = "double(11,5)")]
        public double? HookOutPoint { get; set; }

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

        


    }
}
