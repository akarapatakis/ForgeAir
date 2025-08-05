using Caliburn.Micro;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Managers;
using ForgeAir.Core.Tracks;
using ForgeAir.Database.Models.Enums;
using ForgeAir.Playout.UserControls.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportM3UListViewModel : Screen
    {
        Microsoft.Win32.OpenFileDialog openFileDialog = new();
        private readonly IWindowManager _windowManager;
        private ICollection<TrackImportModel> _trackImports = new List<TrackImportModel>();
        private readonly IServiceProvider _provider;
        private TrackType _selectedTrackType;
        public TrackType SelectedTrackType
        {
            get => _selectedTrackType;
            set
            {
                _selectedTrackType = value;
                NotifyOfPropertyChange(() => SelectedTrackType);
            }
        }
        private List<TrackType> trackTypeList;
        public List<TrackType> TrackTypeList
        {
            get => trackTypeList;
            set
            {
                trackTypeList = value;
                NotifyOfPropertyChange(() => TrackTypeList);
            }
        }
        public CategoryManipulatorViewModel CategoryManipulatorViewModel { get; }

        public string FileBox { get; set; }
        public double CrossFadeUpDown { get; set; }


        public ImportM3UListViewModel(IServiceProvider provider, IWindowManager windowManager) {
            _provider = provider;
            _windowManager = windowManager;
            CategoryManipulatorViewModel = _provider.GetRequiredService<CategoryManipulatorViewModel>();
            TrackTypeList = Enum.GetValues(typeof(TrackType)).Cast<TrackType>().ToList();

            // removing forgevision entries because it uses shared tracktype (fuck me)
            TrackTypeList.Remove(TrackType.None);
            TrackTypeList.Remove(TrackType.Bumper);
            TrackTypeList.Remove(TrackType.Instant);
            TrackTypeList.Remove(TrackType.Ident);
            TrackTypeList.Remove(TrackType.MusicVideo);
            TrackTypeList.Remove(TrackType.Newsreport);
            TrackTypeList.Remove(TrackType.Movie);
            TrackTypeList.Remove(TrackType.Show);
            TrackTypeList.Remove(TrackType.Rebroadcast);
        }

        public void FileSelect()
        {
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            else { FileBox = openFileDialog.FileName; NotifyOfPropertyChange(() => FileBox); }
        }
        public void Cancel()
        {
            TryCloseAsync(true);
        }
        public async void Add()
        {
            if (string.IsNullOrWhiteSpace(FileBox))
            {
                MessageBox.Show("No file selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (Path.GetExtension(FileBox) != ".m3u" && Path.GetExtension(FileBox) != ".m3u8")
            {
                MessageBox.Show("Not an M3U File.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_selectedTrackType == null)
            {
                MessageBox.Show("Please select track type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

           var tracks = Importer.M3UToTracks(FileBox);

            foreach (var track in tracks) {
               _trackImports.Add(new TrackImportModel(track.FilePath, null, _selectedTrackType, TimeSpan.FromSeconds(CrossFadeUpDown), CategoryManipulatorViewModel.SelectedCategories));
            }
            var processVM = new ImportingProcessViewModel(_provider, _trackImports);
            await _windowManager.ShowDialogAsync(processVM);
            _trackImports.Clear();

        }
    }
}
