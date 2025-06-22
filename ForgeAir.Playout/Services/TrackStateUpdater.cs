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
    public class TrackStateUpdater : IHandle<TrackChangedEvent>
    {
        private readonly AppState _appState;

        public TrackStateUpdater(AppState appState, IEventAggregator eventAggregator)
        {
            _appState = appState;
            eventAggregator.SubscribeOnPublishedThread(this);
        }

        public void Handle(TrackChangedEvent message)
        {
            _appState.CurrentTrack = message.CurrentTrack;
        }

        public Task HandleAsync(TrackChangedEvent message, CancellationToken cancellationToken)
        {
            _appState.CurrentTrack = message.CurrentTrack;
            return Task.CompletedTask;
        }
    }

}
