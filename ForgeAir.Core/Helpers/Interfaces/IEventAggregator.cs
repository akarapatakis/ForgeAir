using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Helpers.Interfaces
{
    public interface IEventAggregator
    {
        void Publish<TEvent>(TEvent eventData);
        void Subscribe<TEvent>(Action<TEvent> handler);
        void Unsubscribe<TEvent>(Action<TEvent> handler);
    }
}
