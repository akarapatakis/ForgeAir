using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models;

namespace ForgeAir.Core.Services.Scheduler.Interfaces
{
    public interface ISchedulerService
    {
        ClockItem GetClockItemFor(DateTime time);
    }

}
