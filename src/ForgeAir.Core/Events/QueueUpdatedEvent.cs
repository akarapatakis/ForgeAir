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
        public event Action? QueueChanged;
        public void RaiseQueueChanged() => QueueChanged?.Invoke();
    }

}
