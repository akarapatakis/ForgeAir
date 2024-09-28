using ForgeAir.Database.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Models
{
    public class Category : BaseEntity
    {
        [Required]
        [MaxLength(100)]
        public string? Name { get; set; }

        [MaxLength(1000)]
        public string? Description { get; set; }

        public int? ParentId { get; set; }

        public Category? CategoryParent { get; set; }

        [MaxLength(7)]
        public string? Color { get; set; }

        public ICollection<Category>? Subcategories { get; set; }
        public ICollection<Track>? Tracks { get; set; }
    }
}
