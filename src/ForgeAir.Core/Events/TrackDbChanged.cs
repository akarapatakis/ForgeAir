using ForgeAir.Core.DTO;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Events
{
    public static class TrackDbChanged
    {
        public static event Action<TrackDTO>? TrackImported;

        public static void RaiseTrackImported(TrackDTO track)
        {
            TrackImported?.Invoke(track);
        }
    }

}
