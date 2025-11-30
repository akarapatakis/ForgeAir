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
    [Table("Category")]
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int? ParentId { get; set; }

        public virtual Category? CategoryParent { get; set; }

        [MaxLength(20)]
        public string? Color { get; set; }

        public virtual ICollection<Category>? Subcategories { get; set; }
        public virtual ICollection<Track>? Tracks { get; set; }
    }
}
