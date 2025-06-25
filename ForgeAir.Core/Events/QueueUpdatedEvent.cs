using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;

namespace ForgeAir.Core.Events
{
    public class QueueUpdatedEvent
    {
        public LinkedListQueue<DTO.TrackDTO> CurrentQueue { get; }

        public QueueUpdatedEvent(LinkedListQueue<TrackDTO> newQueue)
        {
            CurrentQueue = newQueue;
        }
    }
}
