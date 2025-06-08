using ForgeAir.Database.Models.Enums;
using ForgeAir.Database.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.Models
{
    public class Track
    {
        public int Id { get; set; }

        public string? Title { get; }

        public string? FilePath { get; set; }

        public int? Bpm { get; set; }

        public double Duration { get; }

        public double? StartPoint { get; }

        public double? MixPoint { get; }

        public double? EndPoint { get; }

        public double? HookInPoint { get; }

        public double? HookOutPoint { get; }

        public DateTime? ReleaseDate { get; }

        public string? Album { get; }

        public String? ISRC { get; }


        public TrackType? TrackType { get; }


        public TrackStatus? TrackStatus { get; }
        public DateTime DateAdded { get; }
        public DateTime? DateModified { get; }
        public DateTime? DateDeleted { get; }

        public ICollection<ArtistTrack>? TrackArtists { get; }
        public ICollection<Category>? Categories { get; }
    }
}
