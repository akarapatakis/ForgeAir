using System;
using ForgeAir.Playout.App.Models;
using ForgeAir.Playout.App.UserControls.ViewModels;
using ForgeAir.Playout.UserControls.ViewModels;
using Microsoft.Extensions.DependencyInjection;
namespace ForgeAir.Playout.App.ViewModels.PlayoutWindows
{
    public class PlayoutViewModel : TabItemViewModelBase
    {
        public override string Title => "Ροή Αναπαραγωγής";
        public override bool Closeable => true;

        public TileModel SelectorTile { get; set; }
        public TrackSelectorViewModel TrackSelectorViewModel { get; }

        private readonly Core.Helpers.Interfaces.IEventAggregator _eventAggregator;
        private readonly IServiceProvider _provider;
        public TrackQueueViewModel TrackQueueViewModel { get; }

        public OnAirViewModel OnAirViewModel { get; }
        public PlayoutViewModel(IServiceProvider provider)
        {
            _eventAggregator = provider.GetRequiredService<Core.Helpers.Interfaces.IEventAggregator>();
            _provider = provider;
            TrackSelectorViewModel = provider.GetRequiredService<TrackSelectorViewModel>();
            TrackQueueViewModel = provider.GetRequiredService<TrackQueueViewModel>();
            OnAirViewModel = provider.GetRequiredService<OnAirViewModel>();

        }

    }
}
