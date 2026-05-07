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
    [Table("PlaylistToWatch")]
    public class PlaylistToWatch : BaseEntity
    {
        public string? DisplayName { get; set; }

        [MaxLength(400)]
        public string FilePath { get; set; }
        public virtual ICollection<Category>? DesiredCategories { get; set; }
        public bool isWatching { get; set; }
    }
}
