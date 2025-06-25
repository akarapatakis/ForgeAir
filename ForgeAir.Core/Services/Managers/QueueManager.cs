using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Models;

namespace ForgeAir.Core.Services.Managers
{
    public class QueueManager
    {
        private LinkedListQueue<DTO.TrackDTO> _queue;
        IEventAggregator _eventAggregator;

        public QueueManager(LinkedListQueue<DTO.TrackDTO> targetQueue, IEventAggregator eventAggregator) { 
            _queue = targetQueue;
            _eventAggregator = eventAggregator;
        }
        public void OnQueueUpdated(LinkedListQueue<TrackDTO> newQueue)
        {
            _eventAggregator.Publish(new QueueUpdatedEvent(newQueue));
        }

        public void Add(DTO.TrackDTO item, int place)
        {
            _queue.EnqueueAt(item, place);
            OnQueueUpdated(_queue);
        }

        public void Remove(DTO.TrackDTO item) { 
            _queue.DequeueSpecificValue(item);
            OnQueueUpdated(_queue);

        }

        public void AddToTop(DTO.TrackDTO item) { 
            _queue.EnqueueAtTop(item);
            OnQueueUpdated(_queue);

        }

        public void AddToBottom(DTO.TrackDTO item) { 
            _queue.EnqueueAtBottom(item);
            OnQueueUpdated(_queue);

        }
    }
}
