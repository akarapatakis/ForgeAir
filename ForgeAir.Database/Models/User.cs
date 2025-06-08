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
    [Table("Users")]
    public class User : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public String Username { get; set; }

        [Required]
        [MaxLength(100)]
        public String Password { get; set; }

        [Required]
        [MaxLength(50)]
        public String FullName { get; set; }

        [Required]
        public UserRole Role { get; set; }
    }
}
