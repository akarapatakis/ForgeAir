using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Caliburn.Micro;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Database.Models;
using ForgeAir.Database;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Tracks.Enums;
using Microsoft.Win32;
using System.IO;
using System.Windows;
using ForgeAir.Database.Models.Enums;
using ForgeAir.UI.Win32.Controls;
using ForgeAir.UI.Win32.Controls.Interfaces;
using ForgeAir.Core.Models;
namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportDirectoryViewModel : Screen
    {
        private readonly ForgeAirDbContextFactory factory = new();
        private readonly RepositoryService<Artist> artistsService;
        private readonly TrackImporter trackImporter = new();
        private readonly IWindowManager _windowManager;
        private ICollection<TrackImportModel> _trackImports = new List<TrackImportModel>();
        public ImportDirectoryViewModel(IWindowManager windowManager)
        {
            _windowManager = windowManager;
            artistsService = new(factory);
        }
        public string DirectoryBox { get; set; }
        public string ArtistAutoCompleteBox { get; set; }
        public bool OverrideArtistCheck { get; set; }
        public double CrossFadeUpDown { get; set; }

        // Radio Buttons (Bound by Caliburn naming convention)
        public bool SongRadioButton { get; set; }
        public bool CommercialRadioButton { get; set; }
        public bool JingleRadioButton { get; set; }
        public bool SweeperRadioButton { get; set; }
        public bool VoicetrackRadioButton { get; set; }
        public bool OtherTrackRadioButton { get; set; }

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
            if (!IsTrackTypeSelected())
            {
                MessageBox.Show("Please select track type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            string? overrideArtist = string.IsNullOrWhiteSpace(ArtistAutoCompleteBox) ? null : ArtistAutoCompleteBox;

            foreach (var file in Directory.GetFiles(DirectoryBox, "*"))
            {
                if (!GeneralHelpers.isThisAnAudioFile(file)) continue;

                _trackImports.Add(new TrackImportModel(file, GetTrackType(), TimeSpan.FromSeconds(CrossFadeUpDown), artistString: overrideArtist));
            }

            var processVM = new ImportingProcessViewModel(_trackImports);
            await _windowManager.ShowDialogAsync(processVM);
        }

        public async void ArtistAutoCompleteBoxChanged()
        {
            if (!string.IsNullOrEmpty(ArtistAutoCompleteBox))
            {
                var result = await artistsService.SearchAsync(ArtistAutoCompleteBox, ModelTypesEnum.Artist);
                // Bind to auto-complete box if needed
            }
        }

        public void Cancel()
        {
            TryCloseAsync(true);
        }

        private bool IsTrackTypeSelected() =>
            SongRadioButton || CommercialRadioButton || JingleRadioButton ||
            SweeperRadioButton || VoicetrackRadioButton || OtherTrackRadioButton;

        private TrackType GetTrackType()
        {
            if (SongRadioButton) return TrackType.Song;
            if (CommercialRadioButton) return TrackType.Commercial;
            if (JingleRadioButton) return TrackType.Jingle;
            if (SweeperRadioButton) return TrackType.Sweeper;
            if (VoicetrackRadioButton) return TrackType.Voicetrack;
            if (OtherTrackRadioButton) return TrackType.Other;
            return TrackType.Other;
        }
    }
}
