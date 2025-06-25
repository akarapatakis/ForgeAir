using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForgeAir.Core.Events;
using ForgeAir.Core.Models;

namespace ForgeAir.Playout.Services
{
    public class QueueStateUpdater : IHandle<QueueUpdatedEvent>
    {
        private readonly AppState _appState;

        public QueueStateUpdater(AppState appState, IEventAggregator eventAggregator)
        {
            _appState = appState;
            eventAggregator.SubscribeOnPublishedThread(this);
        }

        public void Handle(QueueUpdatedEvent message)
        {
            _appState.TrackQueue = message.CurrentQueue;
        }

        public Task HandleAsync(QueueUpdatedEvent message, CancellationToken cancellationToken)
        {
            _appState.TrackQueue = message.CurrentQueue;
            return Task.CompletedTask;
        }
    }
}
