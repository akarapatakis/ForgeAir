using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing;

namespace ForgeAir.Playout.ViewModels.PlayoutWindows
{
    public class SettingsViewModel : Screen
    {
        private readonly IWindowManager _windowManager;

        public SettingsViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
        }

        public async void AddFx()
        {
            await _windowManager.ShowDialogAsync(new ImportFxViewModel(_windowManager));
        }

        public async void AddTrack()
        {
            await _windowManager.ShowDialogAsync(new ImportTrackFileViewModel(_windowManager));
        }

        public async void AddPlaylist()
        {
            await _windowManager.ShowDialogAsync(new ImportM3UListViewModel(_windowManager));
        }

        public async void AddDirectory()
        {
            await _windowManager.ShowDialogAsync(new ImportDirectoryViewModel(_windowManager));
        }

        public async void AddNetworkStream()
        {
            await _windowManager.ShowDialogAsync(new AddNetworkStreamViewModel(_windowManager));
        }

        public async void AddCategory()
        {
            await _windowManager.ShowDialogAsync(new AddCategoryViewModel(_windowManager));
        }
    }

}
