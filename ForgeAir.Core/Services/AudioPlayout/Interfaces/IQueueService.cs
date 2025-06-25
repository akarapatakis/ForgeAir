using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;

namespace ForgeAir.Core.Services.AudioPlayout.Interfaces
{
    public interface IQueueService
    {
        event EventHandler QueueChanged;

        Task FillQueueAsync(ClockItem item, DateTime time);
        TrackDTO Dequeue();
        TrackDTO Peek();
        void EnqueueTop(TrackDTO track);
        void EnqueueBottom(TrackDTO track);
        IReadOnlyCollection<TrackDTO> GetAll();
        bool IsEmpty();
        int Count();
    }

}
