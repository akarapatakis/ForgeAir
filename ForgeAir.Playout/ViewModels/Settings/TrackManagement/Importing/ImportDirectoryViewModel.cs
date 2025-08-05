using Caliburn.Micro;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using ForgeAir.Playout.UserControls.ViewModels;
using ForgeAir.UI.Win32.Controls;
using ForgeAir.UI.Win32.Controls.Interfaces;
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
namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportDirectoryViewModel : Screen
    {
        private readonly RepositoryService<Artist> artistsService;
        private readonly IWindowManager _windowManager;
        private readonly IServiceProvider _provider;
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
        public ImportDirectoryViewModel(IServiceProvider provider, IWindowManager windowManager)
        {
            _provider = provider;
            _windowManager = windowManager;
            artistsService = new(_provider.GetRequiredService<IDbContextFactory<ForgeAirDbContext>>());
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
        public List<Artist> ArtistSearchResults { get; set; }

        public string SearchText { get; set; }
        public string DirectoryBox { get; set; }
        public string ArtistAutoCompleteBox { get; set; }
        public bool OverrideArtistCheck { get; set; }
        public double CrossFadeDuration { get; set; }

        public void DirSelect()
        {
            IDirectoryBrowser dialog = new DirectoryBrowser { RootDirectory = Environment.SpecialFolder.MyComputer };
            if (dialog.ShowDialog() == true && Path.Exists(dialog.SelectedPath))
            {
                DirectoryBox = dialog.SelectedPath;
                NotifyOfPropertyChange(() => DirectoryBox);
            }
        }

        public async void Add()
        {
            if (string.IsNullOrWhiteSpace(DirectoryBox))
            {
                MessageBox.Show("No file selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (_selectedTrackType == null)
            {
                MessageBox.Show("Please select track type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
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
                var result = await artistsService.SearchAsync(ArtistAutoCompleteBox, ModelTypesEnum.Artist);
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
