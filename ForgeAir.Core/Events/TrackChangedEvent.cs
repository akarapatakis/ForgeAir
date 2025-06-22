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
        public TrackDTO CurrentTrack { get; }

        public TrackChangedEvent(TrackDTO newTrack)
        {
            CurrentTrack = newTrack;
        }
    }

}
