using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    public class DailySchedule
    {
        public DateTime Date { get; set; }
        public ScheduleTemplate Template { get; set; }
    }
}
