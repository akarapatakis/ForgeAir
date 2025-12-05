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
    [Table("FX")]
    public class FX : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string FilePath { get; set; }
        [Required]
        public string Color { get; set; }

        [Required]
        public DateTime DateAdded { get; set; }
        public DateTime? DateModified { get; set; }
        public DateTime? DateDeleted { get; set; }

        [Required]
        public TimeSpan Duration { get; set; }

        [Required]
        public TrackStatus Status { get; set; }
    }
}
