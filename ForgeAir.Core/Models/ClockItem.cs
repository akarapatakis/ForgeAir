using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models.Enums;
using ForgeAir.Database.Models.Enums;

namespace ForgeAir.Core.Models
{

    public class ClockItem
    {
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public ClockItemType Type { get; set; }
        public string Parameter { get; set; }
    }
}
