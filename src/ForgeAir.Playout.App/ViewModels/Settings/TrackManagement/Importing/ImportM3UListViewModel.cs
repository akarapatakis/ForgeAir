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
using Avalonia.Platform.Storage;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ForgeAir.Playout.App.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportM3UListViewModel : Screen
    {
        private IStorageProvider openFileDialog;
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

        public string FileBox
        {
            get => _fileBox;
            set
            {
                _fileBox = value;
                NotifyOfPropertyChange(() => FileBox);
            } 
        }
        private string _fileBox;
        public double CrossFadeUpDown { get; set; }


        public ImportM3UListViewModel(IServiceProvider provider, IWindowManager windowManager) {
            _provider = provider;
            _windowManager = windowManager;
            CategoryManipulatorViewModel = _provider.GetRequiredService<CategoryManipulatorViewModel>();
            TrackTypeList = Enum.GetValues(typeof(TrackType)).Cast<TrackType>().ToList();

            TrackTypeList.Remove(TrackType.None);
            TrackTypeList.Remove(TrackType.Rebroadcast);
        }

        public async void FileSelect()
        {
            var files = await openFileDialog.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Audio Files")
                    {
                        Patterns = new[] { "*.m3u", "*.m3u8" }
                    },
                    FilePickerFileTypes.All
                }
            });
            
            if (files.Count == 0)
            {
                return;
            }
            else { FileBox = files.FirstOrDefault().Path.LocalPath; NotifyOfPropertyChange(() => FileBox); }
        }
        public void Cancel()
        {
            TryCloseAsync(true);
        }
        public async void Add()
        {
            if (string.IsNullOrWhiteSpace(FileBox))
            {
                MessageBoxManager
                    .GetMessageBoxStandard("Error", "No File Selected",
                        ButtonEnum.Ok, Icon.Error).ShowAsync();
                return;
            }
            else if (Path.GetExtension(FileBox) != ".m3u" && Path.GetExtension(FileBox) != ".m3u8")
            {
                MessageBoxManager
                    .GetMessageBoxStandard("Error", "Not an M3U Playlist",
                        ButtonEnum.Ok, Icon.Error).ShowAsync();
                return;
            }

            if (_selectedTrackType == null)
            {
                MessageBoxManager
                    .GetMessageBoxStandard("Error", "Select Track Type",
                        ButtonEnum.Ok, Icon.Error).ShowAsync();
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
