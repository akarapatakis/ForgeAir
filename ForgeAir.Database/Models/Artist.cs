using ForgeAir.Database.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Models
{
    public class Artist : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        public ICollection<ArtistTrack>? ArtistTracks { get; set; }
    }
}
