using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Abstract;
using ForgeAir.Database.Models.Enums;

namespace ForgeAir.Database.Models
{
    [Table("Videos")]
    public class Video : BaseEntity
    {

        [Required]
        [MaxLength(400)]
        public string Name { get; set; }

        [Required]
        [MaxLength(191)]
        public string FilePath { get; set; }

        [Column(TypeName = "time(3)")]
        public TimeSpan Duration { get; set; }


        [Column(TypeName = "time(3)")]
        public TimeSpan? StartPoint { get; set; }

        [Column(TypeName = "time(3)")]
        public TimeSpan? EndPoint { get; set; }

        public TimeSpan? AdBreaks { get; set; }
        
        public TimeSpan? AdBreakLength { get; set; }


        public bool? playSubtitles { get; set; }


        [Required]
        [EnumDataType(typeof(VideoType))]
        public VideoType VideoType { get; set; }

        [Required]
        [EnumDataType(typeof(TVRating))]
        public TVRating? TVRating { get; set; }


        [Column("CustomAttributesJson")]
        public string? CustomAttributesJson { get; set; }

        [NotMapped]
        [Column(TypeName = "longtext")]
        public SortedDictionary<string, string>? customAttributes
        {
            get => string.IsNullOrWhiteSpace(CustomAttributesJson)
                ? null
                : System.Text.Json.JsonSerializer.Deserialize<SortedDictionary<string, string>>(CustomAttributesJson);
            set => CustomAttributesJson = System.Text.Json.JsonSerializer.Serialize(value);
        }


        // TO DO: ADD OVERLAY

        [Required]
        [EnumDataType(typeof(TrackStatus))]
        public TrackStatus? TrackStatus { get; set; }

        
        public DateTime DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateDeleted { get; set; }


        [Required]
        [EnumDataType(typeof(AspectRatioFlags))]
        public AspectRatioFlags? aspectRatio { get; set; }

        public string DisplayDuration => Duration.ToString(@"mm\:ss");
    }
}
