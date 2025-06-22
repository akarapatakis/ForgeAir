using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.Helpers.Interfaces;

namespace ForgeAir.Core.Helpers
{
    public class SimpleEventAggregator : IEventAggregator
    {
        private readonly Dictionary<Type, List<Delegate>> _subscriptions = new();

        public void Publish<TEvent>(TEvent eventData)
        {
            if (_subscriptions.TryGetValue(typeof(TEvent), out var handlers))
            {
                foreach (Action<TEvent> handler in handlers.Cast<Action<TEvent>>())
                    handler(eventData);
            }
        }

        public void Subscribe<TEvent>(Action<TEvent> handler)
        {
            if (!_subscriptions.ContainsKey(typeof(TEvent)))
                _subscriptions[typeof(TEvent)] = new List<Delegate>();

            _subscriptions[typeof(TEvent)].Add(handler);
        }

        public void Unsubscribe<TEvent>(Action<TEvent> handler)
        {
            if (_subscriptions.TryGetValue(typeof(TEvent), out var handlers))
                handlers.Remove(handler);
        }
    }
}
