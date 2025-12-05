using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Models.Enums;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Scheduler.Interfaces;

namespace ForgeAir.Core.Services.Scheduler
{
    public class SchedulerService : ISchedulerService
    {
        private readonly List<ClockItem> _hourClock;

        public SchedulerService()
        {
            // Example: simple hardcoded hour structure
            _hourClock = new List<ClockItem>
            {
                new ClockItem { StartTime = TimeSpan.Zero, Type = ClockItemType.Jingle, Parameter = "TopOfHour" },
                new ClockItem { StartTime = TimeSpan.FromMinutes(0.30), Type = ClockItemType.TrackFromCategory, Parameter = "Pop" },
                new ClockItem { StartTime = TimeSpan.FromMinutes(4), Type = ClockItemType.TrackFromCategory, Parameter = "Gold" },
                // Add more
            };
        }

        public ClockItem GetClockItemFor(DateTime time)
        {
            var clockTime = time.TimeOfDay;

            ClockItem currentItem = null;

            foreach (var item in _hourClock.OrderBy(i => i.StartTime))
            {
                if (clockTime >= item.StartTime && clockTime <= item.EndTime)
                    currentItem = item;
                else
                    break;
            }

            return currentItem;
        }
    }

}
