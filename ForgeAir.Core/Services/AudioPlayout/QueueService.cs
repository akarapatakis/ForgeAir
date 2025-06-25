using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.TrackSelector.Interfaces;

namespace ForgeAir.Core.Services.AudioPlayout
{
    public class QueueService : IQueueService
    {
        private LinkedListQueue<TrackDTO> _queue;
        private ITrackSelectorService _selector;

        public event EventHandler QueueChanged;
        public QueueService(LinkedListQueue<TrackDTO> queue, ITrackSelectorService selector) {
            _queue = queue;
            _selector = selector;
        }
        public async Task FillQueueAsync(ClockItem item, DateTime time)
        {
            if (!_queue.IsEmpty()) return;

            while (true)
            {
                var track = await Task.Run(() => _selector.GetBestTrackAsync(item, time));
                if (track == null || !File.Exists(track.FilePath)) continue;

                _queue.EnqueueAtBottom(track);
                break;
            }
            QueueChanged?.Invoke(this, EventArgs.Empty);
        }
        public TrackDTO Dequeue()
        {
            var result = _queue.Dequeue();
            QueueChanged?.Invoke(this, EventArgs.Empty);
            return result;
        }

        public TrackDTO Peek() => _queue.Peek();
        public void EnqueueTop(TrackDTO track)
        {
            _queue.EnqueueAtTop(track);
            QueueChanged?.Invoke(this, EventArgs.Empty);
        }

        public void EnqueueBottom(TrackDTO track)
        {
            _queue.EnqueueAtBottom(track);
            QueueChanged?.Invoke(this, EventArgs.Empty);
        }

        public IReadOnlyCollection<TrackDTO> GetAll() => _queue.ToList().AsReadOnly();
        public bool IsEmpty() => _queue.IsEmpty();
        public int Count() => _queue.Count();
    }
}
