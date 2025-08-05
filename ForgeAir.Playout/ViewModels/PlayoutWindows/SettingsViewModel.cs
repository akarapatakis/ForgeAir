using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForgeAir.Playout.Models;
using ForgeAir.Playout.ViewModels.Settings;
using ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing;
using Microsoft.Extensions.DependencyInjection;

namespace ForgeAir.Playout.ViewModels.PlayoutWindows
{
    public class SettingsViewModel : TabItemViewModelBase
    {
        private readonly IWindowManager _windowManager;
        public override string Title => "Ρυθμίσεις";
        public override bool Closeable => true;
        private readonly IServiceProvider _provider;

        public SettingsViewModel(IServiceProvider provider)
        {
            _provider = provider;
           _windowManager = provider.GetRequiredService<IWindowManager>();
        }

        public async void AboutTile()
        {
            await _windowManager.ShowDialogAsync(new AboutViewModel(_windowManager));

        }
        public async void AddFx()
        {
            await _windowManager.ShowDialogAsync(new ImportFxViewModel(_provider, _windowManager));
        }

        public async void AddTrack()
        {
          await _windowManager.ShowDialogAsync(new ImportTrackWizardViewModel(_provider, _windowManager));
        }

        public async void AddPlaylist()
        {
           await _windowManager.ShowDialogAsync(new ImportM3UListViewModel(_provider, _windowManager));
        }

        public async void AddDirectory()
        {
           await _windowManager.ShowDialogAsync(new ImportDirectoryViewModel(_provider, _windowManager));
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
