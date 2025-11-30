using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.TrackSelector;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Services.AudioPlayout.Interfaces
{
    public interface IQueueService
    {
        TrackSelectorService Selector { get; }
        event EventHandler QueueChanged;
        LinkedListQueue<TrackDTO> Queue { get; }
        Task FillQueueAsync(ClockItem item, DateTime time);
        TrackDTO Dequeue();
        TrackDTO Peek();
        
        void Remove(TrackDTO track);
        bool Contains(TrackDTO track);
        void Clear();
        void Insert(TrackDTO track, int place);
        void EnqueueTop(TrackDTO track);
        void EnqueueBottom(TrackDTO track);
        IReadOnlyCollection<TrackDTO> GetAll();
        bool IsEmpty();
        int Count();
    }

}
