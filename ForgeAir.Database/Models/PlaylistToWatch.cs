using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Database.Abstract;

namespace ForgeAir.Database.Models
{
    [Table("PlaylistToWatch")]
    public class PlaylistToWatch : BaseEntity
    {
        public string? DisplayName { get; set; }
        public string FilePath { get; set; }
        public ICollection<Category>? DesiredCategories { get; set; }
        public bool isWatching { get; set; }
    }
}
