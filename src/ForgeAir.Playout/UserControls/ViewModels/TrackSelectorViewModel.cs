using Caliburn.Micro;
using DynamicData;
using DynamicData.Binding;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Database.Models.Enums;
using ForgeAir.Playout.UserControls.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Reactive.Subjects;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;

namespace ForgeAir.Playout.UserControls.ViewModels
{
    public class TrackSelectorViewModel : Screen, IDisposable
    {
        private readonly SourceList<TrackDTO> _items = new();
        private readonly ITracksService _tracksService;
        private readonly TrackQueueViewModel _queueView;

        private ReadOnlyObservableCollection<TrackDTO> _filteredTracks;
        public ReadOnlyObservableCollection<TrackDTO> FilteredTracks
        {
            get => _filteredTracks;
            private set
            {
                _filteredTracks = value;
                NotifyOfPropertyChange(() => FilteredTracks);
            }
        }

        private string _searchText = string.Empty;
        private readonly Subject<string> _searchTextChanged = new();
        private readonly Subject<TrackType> _trackTypeChanged = new();

        public string SearchText
        {
            get => _searchText;
            set
            {
                _searchText = value;
                NotifyOfPropertyChange(() => SearchText);
                _searchTextChanged.OnNext(value);
            }
        }

        public ObservableCollection<TrackType> TrackTypes { get; } = new();
        private TrackType _selectedTrackType = TrackType.None;
        public TrackType SelectedTrackType
        {
            get => _selectedTrackType;
            set
            {
                _selectedTrackType = value;
                NotifyOfPropertyChange(() => SelectedTrackType);
                _trackTypeChanged.OnNext(value);
            }
        }

        public ObservableCollection<TrackType> ComboItems { get; } = new();

        // Lazy-loading fields
        private bool _isLoading = false;
        private readonly object _loadLock = new();
        private int _pageSize = 100;
        private int _currentSkip = 0;

        public TrackSelectorViewModel(ITracksService tracksService, IServiceProvider provider)
        {
            _tracksService = provider.GetRequiredService<ITracksService>();
            _queueView = provider.GetRequiredService<TrackQueueViewModel>();

            Enum.GetValues(typeof(TrackType)).Cast<TrackType>().ToObservable().Subscribe(t => TrackTypes.Add(t));

            _items
                .Connect()
                .AutoRefreshOnObservable(_ =>
                    this.WhenAnyPropertyChanged(nameof(SearchText), nameof(SelectedTrackType)))
                .Filter(FilterTrack)
                .Bind(out var filtered)
                .Subscribe();

            FilteredTracks = filtered;

            _searchTextChanged
                .Throttle(TimeSpan.FromMilliseconds(250))
                .DistinctUntilChanged()
                .ObserveOn(SynchronizationContext.Current) // run subsequent actions on UI thread
                .Subscribe(async text =>
                {
                    await ResetAndLoadFirstPageAsync();
                });

            _trackTypeChanged
                .DistinctUntilChanged()
                .ObserveOn(SynchronizationContext.Current)
                .Subscribe(async _ =>
                {
                    await ResetAndLoadFirstPageAsync();
                });

            TrackDbChanged.TrackImported += OnTrackImported;

            // Initial load
            _ = ResetAndLoadFirstPageAsync();
        }

        private async Task ResetAndLoadFirstPageAsync()
        {
            await Application.Current.Dispatcher.InvokeAsync(() =>
            {
                _currentSkip = 0;
                _items.Edit(list => list.Clear());
            });

            await LoadNextPage();
        }

        private bool FilterTrack(TrackDTO t)
        {
            bool typeMatch = _selectedTrackType == TrackType.None || t.TrackType == _selectedTrackType;
            if (string.IsNullOrWhiteSpace(SearchText))
                return typeMatch;

            string lowerSearch = _searchText.ToLowerInvariant();
            bool textMatch =
                (!string.IsNullOrEmpty(t.Title) && t.Title.ToLowerInvariant().Contains(lowerSearch)) ||
                (!string.IsNullOrEmpty(t.DisplayArtists) && t.DisplayArtists.ToLowerInvariant().Contains(lowerSearch)) ||
                (!string.IsNullOrEmpty(t.Album) && t.Album.ToLowerInvariant().Contains(lowerSearch)) ||
                (t.ISRC != null && t.ISRC.ToLowerInvariant() == (lowerSearch)) ||
                (t.Bpm != null && t.Bpm.ToString() == (lowerSearch)) ||
                (t.Id.ToString() == (lowerSearch));

            return typeMatch && textMatch;
        }

        public async Task LoadNextPage()
        {
            // quick guard to prevent overlapping loads
            if (_isLoading) return;

            lock (_loadLock)
            {
                if (_isLoading) return;
                _isLoading = true;
            }

            try
            {
                var newTracks = await _tracksService.GetTracks(_currentSkip, _pageSize);

                await Application.Current.Dispatcher.InvokeAsync(() => _items.AddRange(newTracks));

                _currentSkip += _pageSize;
            }
            finally
            {
                _isLoading = false;
            }
        }

        private void OnTrackImported(TrackDTO track)
        {
            Application.Current.Dispatcher.Invoke(() => _items.Add(track));
        }

        public void DoubleClickAdd(TrackDTO track)
        {
            _queueView?.MoveToQueue(new ForgeAir.Core.CustomCollections.LinkedListQueueItem { Track = track });
        }

        public void Dispose()
        {
            TrackDbChanged.TrackImported -= OnTrackImported;

            _items.Edit(list => list.Clear());

            _items.Dispose();

            NotifyOfPropertyChange(() => FilteredTracks);
        }

        protected override async Task OnDeactivateAsync(bool close, CancellationToken cancellationToken)
        {
            await base.OnDeactivateAsync(close, cancellationToken);

            if (close)
                Dispose();
        }
    }
}
