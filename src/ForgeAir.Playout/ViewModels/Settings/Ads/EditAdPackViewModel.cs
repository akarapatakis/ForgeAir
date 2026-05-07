using Caliburn.Micro;
using ForgeAir.Core.Services.Database.RepositoryServices;
using ForgeAir.Core.Services.Managers;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading.Tasks;

namespace ForgeAir.Playout.ViewModels.Settings.Ads
{
    public class EditAdPackViewModel : Screen
    {
        private readonly AdPackManager _adpackmanager;
        private AdPack _adPack;
        private readonly TrackService trackService;
        private readonly IWindowManager windowManager;
        public EditAdPackViewModel(
            IDbContextFactory<ForgeAirDbContext> dbContextFactory, IWindowManager _windowManager, TrackService _trackService,
            AdPack? existingPack = null)
        {
            _adpackmanager = new AdPackManager(dbContextFactory);
             windowManager = _windowManager;
            trackService = _trackService;
            _adPack = existingPack ?? new AdPack();

            ScheduledTime = existingPack != null
                ? new ObservableCollection<DateTime>(existingPack.ScheduledDateTimes)
                : new ObservableCollection<DateTime>();

            AdTracks = existingPack?.Ads != null
                ? new ObservableCollection<AdPackItem>(existingPack.Ads.OrderBy(x => x.PlayOrder < 0).ThenBy(x => x.PlayOrder))
                : new ObservableCollection<AdPackItem>();

            if (existingPack != null)
            {
                PackName = existingPack.Name;
                IsEnabled = existingPack.IsActive;
                RepeatEveryDay = existingPack.RepeatEveryDay;
            }
        }

        #region Properties

        private string _packName;
        public string PackName
        {
            get => _packName;
            set
            {
                _packName = value;
                NotifyOfPropertyChange(() => PackName);
            }
        }

        private bool _isEnabled;
        public bool IsEnabled
        {
            get => _isEnabled;
            set
            {
                _isEnabled = value;
                NotifyOfPropertyChange(() => IsEnabled);

            }
        }

        private bool _repeatEveryDay;
        public bool RepeatEveryDay
        {
            get => _repeatEveryDay;
            set
            {
                _repeatEveryDay = value;
                NotifyOfPropertyChange(() => RepeatEveryDay);
            }
        }

        private ObservableCollection<DateTime> _scheduledTime;
        public ObservableCollection<DateTime> ScheduledTime
        {
            get => _scheduledTime;
            set
            {
                _scheduledTime = value;
                NotifyOfPropertyChange(() => ScheduledTime);
            }
        }

        private DateTime _selectedTime;
        public DateTime SelectedTime
        {
            get => _selectedTime;
            set {
                _selectedTime = value;
                NotifyOfPropertyChange(() => SelectedTime);
            }
        }

        private DateTime? _selectedScheduledTime;
        public DateTime? SelectedScheduledTime
        {
            get => _selectedScheduledTime;
            set {
                _selectedScheduledTime = value;
                NotifyOfPropertyChange(() => SelectedScheduledTime);
            }
        }

        private ObservableCollection<AdPackItem> _adTracks;
        public ObservableCollection<AdPackItem> AdTracks
        {
            get => _adTracks;
            set
            {
                _adTracks = value;
                NotifyOfPropertyChange(() => AdTracks);
            }
        }

        private AdPackItem _selectedAdTrack;
        public AdPackItem SelectedAdTrack
        {
            get => _selectedAdTrack;
            set
            {
                _selectedAdTrack = value;
                NotifyOfPropertyChange(() => SelectedAdTrack);
            }
        }

        #endregion

        #region Scheduled Time Methods

        public void AddBroadcastTime()
        {
            if (!ScheduledTime.Contains(SelectedTime))
                ScheduledTime.Add(SelectedTime);
        }

        public void RemoveBroadcastTime()
        {
            if (SelectedScheduledTime != null)
                ScheduledTime.Remove(SelectedScheduledTime.Value);
        }

        #endregion

        #region Ad Track Methods

        public async void AddAdTrack()
        {
            var selectTrackVm = new AdTracksBrowserViewModel(trackService, _adPack);
            bool? result = await windowManager.ShowDialogAsync(selectTrackVm);

            if (result == true && selectTrackVm.ResultAdPackItem != null)
            {
                var item = selectTrackVm.ResultAdPackItem;
                item.PlayOrder = AdTracks.Count; 
                AdTracks.Add(item);
            }

        }

        public void RemoveSelectedTrack()
        {
            if (SelectedAdTrack != null)
                AdTracks.Remove(SelectedAdTrack);
        }

        #endregion

        #region Save / Cancel

        public async Task SaveAdPack()
        {
            _adPack.Name = PackName;
            _adPack.IsActive = IsEnabled;
            _adPack.RepeatEveryDay = RepeatEveryDay;
            _adPack.ScheduledDateTimes = ScheduledTime;
            _adPack.Ads = AdTracks;

            await _adpackmanager.CreateOrUpdate(
                _adPack.Id,
                _adPack.Name,
                _adPack.ScheduledDateTimes,
                _adPack.IsActive,
                _adPack.RepeatEveryDay,
                _adPack.Ads);

            await TryCloseAsync();
        }

        public async Task Cancel()
        {
            await TryCloseAsync();
        }

        #endregion
    }
}
