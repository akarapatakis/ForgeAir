using ForgeAir.Core.Shared;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.AudioEngine
{
    public class QueueManager
    {
        public void AddToQueue(Database.Models.Track track) { 
            AudioPlayerShared.Instance.trackQueue.EnqueueAtTop(track);
        }
    }
}
