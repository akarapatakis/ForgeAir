using Caliburn.Micro;
using ForgeAir.Core.Events;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Core.Services.DeviceManager.Interfaces;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.App.Models;
using ForgeAir.Playout.App.ViewModels.Settings;
using ForgeAir.Playout.App.ViewModels.Settings.Audio;
using ForgeAir.Playout.App.ViewModels.Settings.Generals;
using ForgeAir.Playout.App.ViewModels.Settings.TrackManagement.Importing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Platform.Storage;
using ForgeAir.Playout.ViewModels.Settings;
using ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing;

namespace ForgeAir.Playout.App.ViewModels.PlayoutWindows
{
    public class SettingsViewModel : TabItemViewModelBase
    {
        private readonly IWindowManager _windowManager;

        public override string Title => "Ρυθμίσεις";
        public override bool Closeable => true;
        private readonly IServiceProvider _provider;
        private readonly ISearchService _searchService;
        private readonly IStorageProvider _storageProvider;
        private readonly IDbContextFactory<ForgeAirDbContext> _dbFactory;
        private readonly StationInformationChangedEvent _stationInformationChangedEvent;
        public SettingsViewModel(IServiceProvider provider, IWindowManager windowManager, ISearchService searchService, IDbContextFactory<ForgeAirDbContext> dbFactory, StationInformationChangedEvent stationInformationChangedEvent, IStorageProvider storageProvider)
        {
            _provider = provider;
           _windowManager = windowManager;
            _searchService = searchService;
            _storageProvider = storageProvider;
            _stationInformationChangedEvent = stationInformationChangedEvent;
            _dbFactory = dbFactory;
        }

        private Station getStation() // temporary
        {
            var dbFactory = _provider.GetRequiredService<IDbContextFactory<ForgeAirDbContext>>();
            using var context = dbFactory.CreateDbContext();

            var station = context.Stations.AsNoTracking().FirstOrDefault();
            if (station != null)
            {
                return station;
            }
            return new Station() { NameTag = "", Name = "" };

        }
        public async void AboutTile()
        {
            await _windowManager.ShowDialogAsync(new AboutViewModel(_windowManager));

        }
        public async void AddFx()
        {
            await _windowManager.ShowDialogAsync(new ImportFxViewModel(_provider, _windowManager, _storageProvider));
        }

        public async void AddTrack()
        {
          await _windowManager.ShowDialogAsync(new ImportTrackWizardViewModel(_searchService, _provider, _windowManager, _storageProvider));
        }
        public async void EditStation()
        {
            await _windowManager.ShowDialogAsync(new StationMetadataEditorViewModel(_windowManager, _provider, getStation(), _dbFactory, _stationInformationChangedEvent, _storageProvider));
        }
        public async void AddPlaylist()
        {
           await _windowManager.ShowDialogAsync(new ImportM3UListViewModel(_provider, _windowManager));
        }
        public async void AudioConfig()
        {
            await _windowManager.ShowDialogAsync(new AudioIOViewModel(_provider, _windowManager));
        }

        public async void AddDirectory()
        {
           await _windowManager.ShowDialogAsync(new ImportDirectoryViewModel(_searchService, _provider, _windowManager, _storageProvider));
        }

        public async void AddNetworkStream()
        {
            await _windowManager.ShowDialogAsync(new AddNetworkStreamViewModel(_provider, _windowManager));
        }

        public async void AddCategory()
        {
           await _windowManager.ShowDialogAsync(new AddCategoryViewModel(_provider, _windowManager));
        }
    }

}
