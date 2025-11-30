using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;

namespace ForgeAir.Core.Events
{
    public class TrackChangedEvent
    {
        public event Action<TrackDTO>? TrackChanged;
        public TrackDTO CurrentTrack { get; set; }
        public void RaiseTrackChanged(TrackDTO track)
        {
            CurrentTrack = track;
            TrackChanged?.Invoke(track);
        }
    }



}
