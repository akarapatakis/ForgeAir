using Caliburn.Micro;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Database.Interfaces;
using ForgeAir.Core.Services.Database.RepositoryServices;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using ForgeAir.Playout.UserControls.ViewModels;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Avalonia.Platform.Storage;
using ForgeAir.Playout.App.ViewModels.Settings.TrackManagement.Importing;
using MsBox.Avalonia;
using MsBox.Avalonia.Enums;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportDirectoryViewModel : Screen
    {
        private readonly ISearchService _artistsService;
        private readonly IWindowManager _windowManager;
        private readonly IServiceProvider _provider;
        private readonly IStorageProvider _storageProvider;
        private TrackType _selectedTrackType;
        public CategoryManipulatorViewModel CategoryManipulatorViewModel { get; }

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

        private ICollection<TrackImportModel> _trackImports = new List<TrackImportModel>();
        public ImportDirectoryViewModel(ISearchService artistsService , IServiceProvider provider, IWindowManager windowManager, IStorageProvider storageProvider)
        {
            _provider = provider;
            _windowManager = windowManager;
            _artistsService = artistsService;
            _storageProvider = storageProvider;
            CategoryManipulatorViewModel = _provider.GetRequiredService<CategoryManipulatorViewModel>();
            TrackTypeList = Enum.GetValues(typeof(TrackType)).Cast<TrackType>().ToList();

            TrackTypeList.Remove(TrackType.None);
            TrackTypeList.Remove(TrackType.Rebroadcast);
        }
        public List<Artist> ArtistSearchResults { get; set; }

        public string SearchText { get; set; }
        public string DirectoryBox { get; set; }
        public string ArtistAutoCompleteBox { get; set; }
        public bool OverrideArtistCheck { get; set; }
        public double CrossFadeDuration { get; set; }

        public async void DirSelect()
        {
            var directory = await _storageProvider.OpenFolderPickerAsync(new FolderPickerOpenOptions()
            {
                AllowMultiple = false,
            });
            
            if (directory.Count == 0 || directory.FirstOrDefault()==null)
            {
                return;
            }
            
            if (Directory.Exists(directory.FirstOrDefault().Path.LocalPath))
            {
                DirectoryBox = directory.FirstOrDefault().Path.LocalPath;
                NotifyOfPropertyChange(() => DirectoryBox);
            }
        }

        public async void Add()
        {
            if (string.IsNullOrWhiteSpace(DirectoryBox))
            {
                MessageBoxManager
                    .GetMessageBoxStandard("Error", "No File Selected",
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

            if (!GeneralHelpers.isThisAnAudioFile(DirectoryBox))
            {
                MessageBoxManager
                    .GetMessageBoxStandard("Error", "Not a valid audio file",
                        ButtonEnum.Ok, Icon.Error).ShowAsync();
                _trackImports.Add(new TrackImportModel(DirectoryBox, null, _selectedTrackType, TimeSpan.FromSeconds(CrossFadeDuration), CategoryManipulatorViewModel.SelectedCategories));

                return;
            }

            string? overrideArtist = string.IsNullOrWhiteSpace(ArtistAutoCompleteBox) ? null : ArtistAutoCompleteBox;

            foreach (var file in Directory.GetFiles(DirectoryBox, "*"))
            {
                if (!GeneralHelpers.isThisAnAudioFile(file)) continue;

               _trackImports.Add(new TrackImportModel(file, null, SelectedTrackType, TimeSpan.FromSeconds(CrossFadeDuration), CategoryManipulatorViewModel.SelectedCategories));
            }

            var processVM = new ImportingProcessViewModel(_provider, _trackImports);
            await _windowManager.ShowDialogAsync(processVM);
            _trackImports.Clear();
        }
    
        public async void ArtistAutoCompleteBoxChanged()
        {
            if (!string.IsNullOrEmpty(ArtistAutoCompleteBox))
            {
                var result = await _artistsService.SearchArtists(ArtistAutoCompleteBox);
                if (result != null) { 
                    ArtistSearchResults = result;
                }
            }
        }

        public void Cancel()
        {
            TryCloseAsync(true);
        }
    }
}
