using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ForgeAir.Core.Helpers;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Managers;
using ForgeAir.Core.Tracks;
using ForgeAir.Database.Models.Enums;
using MessageBox = HandyControl.Controls.MessageBox;

namespace ForgeAir.Playout.ViewModels.Settings.TrackManagement.Importing
{
    public class ImportM3UListViewModel : Screen
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

        public ImportM3UListViewModel(IWindowManager windowManager) { _windowManager = windowManager; }

        public void FileSelect()
        {
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            else { FileBox = openFileDialog.FileName; NotifyOfPropertyChange(() => FileBox); }
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

        public async void Add()
        {
            if (string.IsNullOrWhiteSpace(FileBox))
            {
                MessageBox.Show("No file selected.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (Path.GetExtension(FileBox) != ".m3u" || Path.GetExtension(FileBox) != ".M3U" || !File.Exists(FileBox))
            {
                MessageBox.Show("Not an M3U File.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (!IsTrackTypeSelected())
            {
                MessageBox.Show("Please select track type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

           var tracks = Importer.M3UToTracks(FileBox);

            foreach (var track in tracks) {
                _trackImports.Add(new TrackImportModel(track.FilePath, GetTrackType(), TimeSpan.FromSeconds(CrossFadeUpDown)));
            }
            var processVM = new ImportingProcessViewModel(_trackImports);
            await _windowManager.ShowDialogAsync(processVM);

        }
    }
}
