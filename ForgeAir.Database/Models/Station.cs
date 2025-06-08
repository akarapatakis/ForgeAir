using ForgeAir.Database.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Models
{
    [Table("Stations")]
    public class Station : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public required string Name { get; set; } = "ForgeAir Radio";

        [MaxLength(64)]
        public string? Website { get; set; }
        [MaxLength(64)]
        public string? Telephone { get; set; }
        [MaxLength(64)]
        public string? Email { get; set; }
        [MaxLength(64)]
        public string? Genre { get; set; } = "Varied";
        [MaxLength(64)]
        public string? Slogan { get; set; } = "Radio Automation Software";

    }
}
