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
    [Table("Bank")]
    public class Bank : BaseEntity
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public string Color { get; set; }

        public virtual ICollection<FX>? Tracks { get; set; }
    }
}
