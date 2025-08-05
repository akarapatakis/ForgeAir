using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Managers;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using ForgeAir.Playout.ViewModels.PlayoutWindows;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeAir.Playout.UserControls.ViewModels
{
    public class TrackSelectorViewModel : Screen, IDisposable, INotifyPropertyChanged
    {
        private readonly AppState _appState;
        private readonly IServiceProvider _provider;
        public TrackDTO SelectedTrack;
        private readonly RepositoryService<Track> _trackRepository;
        private ObservableCollection<string> _comboItems = new();
        private ObservableCollection<TrackDTO> _allTracks;
        public ObservableCollection<TrackDTO> AllTracks
        {
            get => _allTracks;
            set
            {
                _allTracks = value;
                NotifyOfPropertyChange(() => AllTracks);
            }
        }
        private ObservableCollection<TrackDTO> _filteredTracks = new();
        private string _searchText = string.Empty;
        private readonly TrackQueueViewModel QueueView;
        private TrackType _selectedTrackType;
        public TrackType SelectedTrackType
        {
            get => _selectedTrackType;
            set
            {
                _selectedTrackType = value;
                ApplyFilter();
            }
        }
        public TrackSelectorViewModel(IServiceProvider provider)
        {
            _provider = provider;
            _trackRepository = _provider.GetRequiredService<RepositoryService<Track>>();
            AllTracks = _provider.GetRequiredService<ObservableCollection<TrackDTO>>();
            TrackDbChanged.TrackImported += OnTrackImported;
            QueueView = _provider.GetRequiredService<TrackQueueViewModel>();
            LoadData();
        }
        private void OnTrackImported(TrackDTO track)
        {
            Application.Current.Dispatcher.Invoke(() => {
                LoadData(); // refresh from db
            });
        }
        public ObservableCollection<TrackDTO> FilteredTracks
        {
            get => _filteredTracks;
            set
            {
                _filteredTracks = value;
                NotifyOfPropertyChange(() => FilteredTracks);
            }
        }

        public ObservableCollection<string> ComboItems
        {
            get => _comboItems;
            set
            {
                _comboItems = value;
                NotifyOfPropertyChange(() => ComboItems);
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (Set(ref _searchText, value))
                {
                    ApplyFilter();
                }
            }
        }
        private async void LoadData()
        {



            var tracks = await Task.Run(() => _trackRepository.GetAll(Core.Tracks.Enums.ModelTypesEnum.Track));


            foreach (var track in tracks)
            {
                var dto = TrackDTO.FromEntity(track);
                AllTracks.Add(dto);
            }


            FilteredTracks = new ObservableCollection<TrackDTO>(AllTracks);


            ComboItems = new ObservableCollection<string>();

            foreach (var _enum in Enum.GetNames(typeof(Database.Models.Enums.TrackType))){
                ComboItems.Add(_enum.ToString());
            }
            SelectedTrackType = TrackType.None; // init value so the combobox won't be empty

        }
        public void DoubleClickAdd(TrackDTO track)
        {
            if (QueueView != null)
            {
                QueueView.MoveToQueue(new Core.CustomCollections.LinkedListQueueItem { Track = track});
            }
        }

        private void ApplyFilter()
        {
            if (string.IsNullOrWhiteSpace(SearchText) && SelectedTrackType == TrackType.None)
            {
                FilteredTracks = new ObservableCollection<TrackDTO>(AllTracks);
            }
            else
            {
                var lowerSearch = SearchText.ToLowerInvariant();
                var filtered = AllTracks
                    .Where(t =>
                        (SelectedTrackType == TrackType.None || t.TrackType == SelectedTrackType) && 
                        (
                            (t.ISRC != null && t.ISRC.ToLowerInvariant().Contains(lowerSearch)) ||
                            (t.Bpm != null && t.Bpm.ToString().Contains(lowerSearch)) ||
                            (t.Id != null && t.Id.ToString().Contains(lowerSearch)) ||
                            (t.TrackType != null && t.TrackType.ToString().ToLowerInvariant().Contains(lowerSearch)) ||
                            (t.Title != null && t.Title.ToLowerInvariant().Contains(lowerSearch)) ||
                            (t.DisplayArtists != null && t.DisplayArtists.ToLowerInvariant().Contains(lowerSearch)) ||
                            (t.Album != null && t.Album.ToLowerInvariant().Contains(lowerSearch))
                        )
                    );

                FilteredTracks = new ObservableCollection<TrackDTO>(filtered);
            }
        }
        public void Dispose()
        {
            TrackDbChanged.TrackImported -= OnTrackImported;

        }
    }


}
