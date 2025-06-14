using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    class LinkedListQueueItem
    {
        public required int Place { get; set; }
        public required DTO.TrackDTO Track { get; set; }

        public string DisplayTitle => Track.Title;

    }
}
