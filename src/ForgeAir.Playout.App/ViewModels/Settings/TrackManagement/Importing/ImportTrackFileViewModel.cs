using Caliburn.Micro;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Models;
using ForgeAir.Database.Models.Enums;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Platform.Storage;
using ForgeAir.Playout.UserControls.ViewModels;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ForgeAir.Playout.App.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportTrackFileViewModel : Screen
    {
        private IStorageProvider openFileDialog;
        private readonly IWindowManager _windowManager;
        private ICollection<TrackImportModel> _trackImports = new List<TrackImportModel>();
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
        public ImportTrackFileViewModel(IServiceProvider provider, IStorageProvider storageProvider, IWindowManager windowManager) { 
            _provider = provider;
            openFileDialog = storageProvider;
            _windowManager = windowManager;
            CategoryManipulatorViewModel = _provider.GetRequiredService<CategoryManipulatorViewModel>();
            TrackTypeList = Enum.GetValues(typeof(TrackType)).Cast<TrackType>().ToList();

            TrackTypeList.Remove(TrackType.None);
            TrackTypeList.Remove(TrackType.Rebroadcast);

        }

        public void Cancel()
        {
            TryCloseAsync(true);
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
                        Patterns = new[] { "*.mp3", "*.wav", "*.flac", "*.ogg", "*.m4a", "*.fla", "*.wma", "*.opus" }
                    },
                    FilePickerFileTypes.All
                }
            });
            
            if (files.Count == 0 || files.FirstOrDefault()==null)
            {
                return;
            }
            else { FileBox = files.FirstOrDefault().Path.LocalPath; NotifyOfPropertyChange(() => FileBox); }
        }
        public async void Add()
        {
            if (string.IsNullOrWhiteSpace(FileBox))
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Error", "No File Selected",
                        ButtonEnum.Ok, Icon.Error).ShowAsync();
                return;
            }

            if (_selectedTrackType == null)
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Error", "Select Track Type",
                        ButtonEnum.Ok, Icon.Error).ShowAsync();
                return;
            }

            if (!GeneralHelpers.isThisAnAudioFile(FileBox))
            {
                await MessageBoxManager
                    .GetMessageBoxStandard("Error", "Not a valid audio file",
                        ButtonEnum.Ok, Icon.Error).ShowAsync();
                _trackImports.Add(new TrackImportModel(FileBox, null, _selectedTrackType, TimeSpan.FromSeconds(CrossFadeDuration), CategoryManipulatorViewModel.SelectedCategories));

                return;
            }
                
            var processVM = new ImportingProcessViewModel(_provider, _trackImports);
            await _windowManager.ShowDialogAsync(processVM);
            _trackImports.Clear();
        }


    }
}
