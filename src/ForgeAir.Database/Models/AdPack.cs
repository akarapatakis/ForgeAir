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
    [Table("AdPacks")]
    public class AdPack : BaseEntity
    {
        public string Name { get; set; }
        public ICollection<DateTime> ScheduledDateTimes { get; set; }
        public bool IsActive { get; set; }
        public bool RepeatEveryDay { get; set; }
        [Key]
        public virtual ICollection<AdPackItem> Ads { get; set; }
    }
}
