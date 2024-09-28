using ForgeAir.Database.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Models
{
    public class TagValue : BaseEntity
    {
        [MaxLength(100)]
        public String? Name { get; set; }
        public int TagCategoryId { get; set; }
        public TagCategory? TagCategory { get; set; }
    }
}
