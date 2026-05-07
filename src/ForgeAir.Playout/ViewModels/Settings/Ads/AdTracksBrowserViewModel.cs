using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Services.Database.RepositoryServices;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Reactive.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Playout.ViewModels.Settings.Ads
{
    public class AdTracksBrowserViewModel : Screen
    {
        private readonly TrackService _trackService;
        private readonly AdPack targetAdpack;
        public AdPackItem? ResultAdPackItem { get; private set; }
        private ObservableCollection<TrackDTO> _tracks;
        public ObservableCollection<TrackDTO> Tracks
        {
            get => _tracks;
            set
            {
                _tracks = value;
                NotifyOfPropertyChange(() => Tracks);
            }
        }

        private TrackDTO _selectedTrack;
        public TrackDTO SelectedTrack
        {
            get => _selectedTrack;
            set
            {
                _selectedTrack = value;
                NotifyOfPropertyChange(() => SelectedTrack);
            }
        }
        public AdTracksBrowserViewModel(TrackService trackService, AdPack pack)
        {
            targetAdpack = pack;
            _trackService = trackService;
            LoadList();
        }

        public async void Ok()
        {
            if (SelectedTrack != null)
            {
                ResultAdPackItem = new AdPackItem
                {
                    TrackId = SelectedTrack.Id,
                    Track = TrackDTO.ToEntity(SelectedTrack),
                    AdPack = targetAdpack,
                    AdPackId = targetAdpack.Id,
                    PlayOrder = 0
                };

                await TryCloseAsync(true);
            }
        }
        public void Cancel()
        {
            TryCloseAsync(false);
        }
        public async void LoadList()
        {
            Tracks = new ObservableCollection<TrackDTO>(await _trackService.GetByConditionAsync(Core.Tracks.Enums.ModelTypesEnum.Track, t => (t.TrackType != TrackType.Song)));
        }
    }
}
