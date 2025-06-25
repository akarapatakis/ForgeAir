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
using ForgeAir.Database.Models.Enums;
using ForgeAir.Core.Helpers;
using System.IO;
using System.Windows;
using Vortice.Win32;
using ForgeAir.Core.Models;
using MessageBox = HandyControl.Controls.MessageBox;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportTrackFileViewModel : Screen
    {


        Microsoft.Win32.OpenFileDialog openFileDialog = new();

        private readonly IWindowManager _windowManager;
        private ICollection<TrackImportModel> _trackImports = new List<TrackImportModel>();
        public string FileBox { get; set; }
        public double CrossFadeUpDown { get; set; }

        // Radio Buttons (Bound by Caliburn naming convention)
        public bool SongRadioButton { get; set; }
        public bool CommercialRadioButton { get; set; }
        public bool JingleRadioButton { get; set; }
        public bool SweeperRadioButton { get; set; }
        public bool VoicetrackRadioButton { get; set; }
        public bool OtherTrackRadioButton { get; set; }

        public ImportTrackFileViewModel(IWindowManager windowManager) { 
            _windowManager = windowManager;
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

            if (!IsTrackTypeSelected())
            {
                MessageBox.Show("Please select track type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            if (!GeneralHelpers.isThisAnAudioFile(FileBox))
                MessageBox.Show("Invalid Track.");

                _trackImports.Add(new TrackImportModel(FileBox, GetTrackType(), TimeSpan.FromSeconds(CrossFadeUpDown)));

            var processVM = new ImportingProcessViewModel(_trackImports);
            await _windowManager.ShowDialogAsync(processVM);
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
