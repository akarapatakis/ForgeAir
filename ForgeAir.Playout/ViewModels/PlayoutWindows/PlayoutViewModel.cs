using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Helpers.Interfaces;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Playout.Models;
using ForgeAir.Playout.UserControls.ViewModels;
using Microsoft.Extensions.DependencyInjection;
namespace ForgeAir.Playout.ViewModels.PlayoutWindows
{
    public class PlayoutViewModel : TabItemViewModelBase
    {
        public override string Title => "Ροή Αναπαραγωγής";
        public override bool Closeable => true;

        private readonly IAudioService? _audioService;
        public TrackSelectorViewModel TrackSelectorViewModel { get; }

        private readonly Core.Helpers.Interfaces.IEventAggregator _eventAggregator;
        private readonly IServiceProvider _provider;
        public TrackQueueViewModel TrackQueueViewModel { get; }


        public PlayoutViewModel(IServiceProvider provider)
        {
            _eventAggregator = provider.GetRequiredService<Core.Helpers.Interfaces.IEventAggregator>();
            _provider = provider;
            _audioService = provider.GetRequiredService<IAudioService>();
            TrackSelectorViewModel = provider.GetRequiredService<TrackSelectorViewModel>();
            TrackQueueViewModel = provider.GetRequiredService<TrackQueueViewModel>();

            // _audioService = audioService;
        }

        public void Play()
        {
            _audioService.Play();
        }

        public void Pause()
        {
            _audioService.Pause();
        }

        public void Stop()
        {
            _audioService.Stop();
        }
    }
}
