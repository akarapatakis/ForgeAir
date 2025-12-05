using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    public class ScheduleTemplate
    {
        public string Name { get; set; }
        public Dictionary<int, string> HourlyClocks { get; set; }
    }
}
