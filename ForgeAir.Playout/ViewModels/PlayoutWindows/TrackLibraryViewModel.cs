using Caliburn.Micro;
using DynamicData;
using DynamicData.Binding;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Database.Models.Enums;
using ForgeAir.Playout.Models;
using ForgeAir.Playout.UserControls.ViewModels;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;

namespace ForgeAir.Playout.ViewModels.PlayoutWindows
{
    public class TrackLibraryViewModel : TabItemViewModelBase, IDisposable
    {
        private readonly ITracksService _tracksService;
        private readonly TrackQueueViewModel _queueView;

        private readonly SourceList<TrackDTO> _items = new();
        public ReadOnlyObservableCollection<TrackDTO> Items { get; }
        public override string Title => "Βιβλιοθήκη Κομματιών";
        public override bool Closeable => true;

        private string _searchText = string.Empty;
        private TrackType _selectedTrackType = TrackType.None;

        private bool _isLoading = false;
        private int _pageSize = 100;
        private int _currentSkip = 0;

        public TrackLibraryViewModel(ITracksService tracksService, TrackQueueViewModel queueView)
        {
            _tracksService = tracksService;
            _queueView = queueView;

            // Bind SourceList -> Items (filtered)
            _items.Connect()
                  .Filter(FilterTracks)
                  .Bind(out var items)
                  .Subscribe();

            Items = items;

            TrackDbChanged.TrackImported += OnTrackImported;
        }

        // ----------------------
        //  Lazy Loading
        // ----------------------
        public async Task LoadNextPage()
        {
            if (_isLoading)
                return;

            _isLoading = true;

            var newTracks = await _tracksService.GetTracks(_currentSkip, _pageSize);

            _items.AddRange(newTracks);

            _currentSkip += _pageSize;
            _isLoading = false;
        }

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);
            _ = LoadNextPage(); // fire-and-forget async
        }

        // ----------------------
        // Filtering
        // ----------------------
        private bool FilterTracks(TrackDTO track)
        {
            // Track type filter
            bool typeMatch = _selectedTrackType == TrackType.None || track.TrackType == _selectedTrackType;

            if (string.IsNullOrWhiteSpace(_searchText))
                return typeMatch;

            // Search text
            string query = _searchText.ToLowerInvariant();

            bool textMatch =
                (!string.IsNullOrEmpty(track.Title) && track.Title.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(track.DisplayArtists) && track.DisplayArtists.ToLower().Contains(query)) ||
                (!string.IsNullOrEmpty(track.Album) && track.Album.ToLower().Contains(query)) ||
                (track.ISRC != null && track.ISRC.ToLower().Contains(query)) ||
                (track.Bpm != null && track.Bpm.ToString().Contains(query)) ||
                (track.Id != null && track.Id.ToString().Contains(query));

            return typeMatch && textMatch;
        }

        public TrackType SelectedTrackType
        {
            get => _selectedTrackType;
            set
            {
                _selectedTrackType = value;
                _items.Edit(_ => { }); // Force DynamicData to re-evaluate filter
            }
        }

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                _items.Edit(_ => { }); // Refresh filter
            }
        }

        // ----------------------
        // Track Imported from DB
        // ----------------------
        private void OnTrackImported(TrackDTO track)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                _items.Add(track);
            });
        }

        // ----------------------
        // Dispose
        // ----------------------
        public void Dispose()
        {
            TrackDbChanged.TrackImported -= OnTrackImported;
            _items.Dispose();
        }
    }
}
