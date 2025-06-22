using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.Models;

namespace ForgeAir.Core.Services.Managers
{
    public class QueueManager
    {
        private LinkedListQueue<DTO.TrackDTO> _queue;
        
        public QueueManager(LinkedListQueue<DTO.TrackDTO> targetQueue) { 
            _queue = targetQueue;
        }

        public void Add(DTO.TrackDTO item, int place)
        {
            _queue.EnqueueAt(item, place);
        }

        public void Remove(DTO.TrackDTO item) { 
            _queue.DequeueSpecificValue(item);
        }

        public void AddToTop(DTO.TrackDTO item) { 
            _queue.EnqueueAtTop(item);
        }

        public void AddToBottom(DTO.TrackDTO item) { 
            _queue.EnqueueAtBottom(item);
        }
    }
}
