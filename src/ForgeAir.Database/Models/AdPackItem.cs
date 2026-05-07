using ForgeAir.Database.Abstract;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Models
{
    [Table("AdPackItem")]
    public class AdPackItem : BaseEntity
    {
        public int AdPackId { get; set; }
        public virtual AdPack AdPack { get; set; }

        public int TrackId { get; set; }
        public virtual Track Track { get; set; }

        public int PlayOrder { get; set; }
    }
}