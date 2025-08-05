using Caliburn.Micro;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using ForgeAir.Playout.UserControls.ViewModels;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using MessageBox = HandyControl.Controls.MessageBox;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportTrackFileViewModel : Screen
    {
        Microsoft.Win32.OpenFileDialog openFileDialog = new();
        private readonly IWindowManager _windowManager;
        private ICollection<TrackImportModel> _trackImports = new List<TrackImportModel>();
        public string FileBox { get; set; }
        public int CrossFadeDuration { get; set; }
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
        public ImportTrackFileViewModel(IServiceProvider provider, IWindowManager windowManager) { 
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

        public void Cancel()
        {
            TryCloseAsync(true);
        }

        public void FileSelect()
        {
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            else { FileBox = openFileDialog.FileName; NotifyOfPropertyChange(() => FileBox); }
        }
        public async void Add()
        {
            if (string.IsNullOrWhiteSpace(FileBox))
            {
                MessageBox.Show("No file selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_selectedTrackType == null)
            {
                MessageBox.Show("Please select track type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!GeneralHelpers.isThisAnAudioFile(FileBox))
                MessageBox.Show("Invalid Track.");

                _trackImports.Add(new TrackImportModel(FileBox, null, _selectedTrackType, TimeSpan.FromSeconds(CrossFadeDuration), CategoryManipulatorViewModel.SelectedCategories));

            var processVM = new ImportingProcessViewModel(_provider, _trackImports);
            await _windowManager.ShowDialogAsync(processVM);
            _trackImports.Clear();
        }


    }
}
