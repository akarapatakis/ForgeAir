using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.TrackSelector;
using ForgeAir.Core.Services.TrackSelector.Interfaces;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.AudioPlayout
{
    public class QueueService : IQueueService, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private LinkedListQueue<TrackDTO> _queue;
        public LinkedListQueue<TrackDTO> Queue { get => _queue; }

        TrackSelectorService IQueueService.Selector  { get => _selector; }
        private readonly TrackSelectorService _selector;
        private readonly Events.QueueUpdatedEvent _queueUpdatedEvent;

        public event EventHandler QueueChanged;
        public QueueService(LinkedListQueue<TrackDTO> queue, TrackSelectorService selector, Events.QueueUpdatedEvent queueUpdatedEvent)
        {
            _queue = queue;
            _selector = selector;
            _queueUpdatedEvent = queueUpdatedEvent;
        }
        public async Task FillQueueAsync(ClockItem item, DateTime time)
        {
            if (_queue.All(q => q.TrackType == ForgeAir.Database.Models.Enums.TrackType.Song))
            {
                while (true)
                {
                    var track = await Task.Run(() => _selector.CurrentSelector.GetBestTrackAsync(item, time, ForgeAir.Database.Models.Enums.TrackType.Jingle));
                    if (track == null || !File.Exists(track.FilePath)) break;

                    _queue.EnqueueAtBottom(track);
                    break;
                }
                if (!_queue.Any(q => q.TrackType == ForgeAir.Database.Models.Enums.TrackType.Jingle)) // fallback if no jingles exist
                {
                    while (true)
                    {
                        var track = await Task.Run(() => _selector.CurrentSelector.GetBestTrackAsync(item, time));
                        if (track == null || !File.Exists(track.FilePath) || (_queue.Contains(track) && track.TrackType != ForgeAir.Database.Models.Enums.TrackType.Jingle)) break; // give up

                        _queue.EnqueueAtBottom(track);
                        break;
                    }
                }
                QueueChanged?.Invoke(this, EventArgs.Empty);
                _queueUpdatedEvent.RaiseQueueChanged();
            }
            else
            {
                while (true)
                {
                    var track = await Task.Run(() => _selector.CurrentSelector.GetBestTrackAsync(item, time));
                    if (track == null || !File.Exists(track.FilePath) || (_queue.Contains(track) && track.TrackType != ForgeAir.Database.Models.Enums.TrackType.Jingle)) break;

                    _queue.EnqueueAtBottom(track);
                    break;
                }
                QueueChanged?.Invoke(this, EventArgs.Empty);
                _queueUpdatedEvent.RaiseQueueChanged();
            }

        }

        public void Insert(TrackDTO track, int place) {
            _queue.EnqueueAt(track, place);
        }
        public TrackDTO Dequeue()
        {
            var result = _queue.Dequeue();
            QueueChanged?.Invoke(this, EventArgs.Empty);
            _queueUpdatedEvent.RaiseQueueChanged();

            return result;
        }

        public TrackDTO Peek() => _queue.Peek();
        public void EnqueueTop(TrackDTO track)
        {
            _queue.EnqueueAtTop(track);
            QueueChanged?.Invoke(this, EventArgs.Empty);
            _queueUpdatedEvent.RaiseQueueChanged();

        }

        public void EnqueueBottom(TrackDTO track)
        {
            _queue.EnqueueAtBottom(track);
            QueueChanged?.Invoke(this, EventArgs.Empty);
            _queueUpdatedEvent.RaiseQueueChanged();

        }

        public IReadOnlyCollection<TrackDTO> GetAll() => _queue.ToList().AsReadOnly();
        public bool IsEmpty() => _queue.IsEmpty();
        public int Count() => _queue.Count();

        public void Remove(TrackDTO track)
        {
            Queue.Remove(track);
            QueueChanged?.Invoke(this, EventArgs.Empty);

            _queueUpdatedEvent.RaiseQueueChanged();

        }

        public bool Contains(TrackDTO track)
        {
            if (Queue.Contains(track))
            {
                return true;
            }
            return false;
        }

        public void Clear()
        {
            for (int i = 0; i < Queue.Count();)
            {
                Queue.Remove(Queue.ElementAt(i));
            }
            QueueChanged?.Invoke(this, EventArgs.Empty);
            _queueUpdatedEvent.RaiseQueueChanged();

        }
    }
}
