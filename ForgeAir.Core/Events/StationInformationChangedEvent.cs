using ForgeAir.Core.DTO;
using ForgeAir.Database.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Events
{
    public class StationInformationChangedEvent
    {
        public event Action<Station>? StationUpdated;
        public void RaiseStationUpdated(Station s)
        {
            StationUpdated?.Invoke(s);
        }
    }
}
