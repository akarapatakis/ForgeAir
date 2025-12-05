using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Database.Models
{
    [Table("FX_Banks")]
    public class FxBank
    {
        public int FxId { get; set; }
        public virtual FX? FX { get; set; }
        
        public int BankId { get; set; }
        public virtual Bank? Bank { get; set; }

    }
}
