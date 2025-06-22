using ForgeAir.Core.Helpers;
using ForgeAir.Core.Services;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Shared;
using ForgeAir.Core.Tracks;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using Microsoft.EntityFrameworkCore;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for ImportFolder.xaml
    /// </summary>
    public partial class ImportFolder : HandyControl.Controls.GlowWindow
    {
        private string folderName;
        private readonly ForgeAirDbContextFactory factory = new ForgeAirDbContextFactory();
        private readonly Core.Services.Database.RepositoryService<Artist> artistsService;
        private readonly Core.Services.Database.RepositoryService<ArtistTrack> artistTracksService;
        private readonly Core.Services.Database.RepositoryService<Track> tracksService;
        private readonly TrackImporter trackImporter = new TrackImporter();

        public ImportFolder()
        {
            artistsService = new Core.Services.Database.RepositoryService<Artist>(factory);
            artistTracksService = new Core.Services.Database.RepositoryService<ArtistTrack>(factory);
            tracksService = new Core.Services.Database.RepositoryService<Track>(factory);

            InitializeComponent();
            artistAutoCompleteBox.IsEnabled = false;
        }

        private void dirSelect_Click(object sender, RoutedEventArgs e)
        {
            OpenFolderDialog dialog = new OpenFolderDialog
            {
                Multiselect = false,
                Title = "Open Folder..."

            };

            if (dialog.ShowDialog() == true)
            {
                if (System.IO.Path.Exists(dialog.FolderName))
                {
                    folderName = dialog.FolderName;
                    directoryBox.Text = folderName;
                }
            }

        }

        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            string? overrideArtist = null;
            if (!IsTrackTypeSelected())
            {
                HandyControl.Controls.MessageBox.Show("Please Select Track Type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (string filename in Directory.GetFiles(folderName, "*"))
            {
                if (filename == null) { continue; }


                if (!string.IsNullOrWhiteSpace(artistAutoCompleteBox.Text))
                {
                    overrideArtist = artistAutoCompleteBox.Text;
                }
                if (!GeneralHelpers.isThisAnAudioFile(filename))
                    continue;
                double crossfade = crossFadeUpDown.Value;
                var type = getTrackType();
                Track track = await Task.Run(() => trackImporter.createTrackAsync(filename, type, TimeSpan.FromSeconds(Convert.ToDouble(crossfade))));

                DatabaseSharedData.Instance.RaiseDBModified();

            }
            overrideArtist = null;
        }

        private async void artistAutoCompleteBox_KeyDown(object sender, KeyEventArgs e)
        {
            if (!string.IsNullOrEmpty(artistAutoCompleteBox.Text))
            {
                var result = await artistsService.SearchAsync(artistAutoCompleteBox.Text, ModelTypesEnum.Artist);
                artistAutoCompleteBox.ItemsSource = result;
            }
        }

        private void overrideArtistCheck_Checked(object sender, RoutedEventArgs e) => artistAutoCompleteBox.IsEnabled = true;
        private void overrideArtistCheck_Unchecked(object sender, RoutedEventArgs e) => artistAutoCompleteBox.IsEnabled = false;
        private void cancelButton_Click(object sender, RoutedEventArgs e) => Close();

        private bool IsTrackTypeSelected() =>
            songRadioButton.IsChecked == true ||
            commercialRadioButton.IsChecked == true ||
            jingleRadioButton.IsChecked == true ||
            sweeperRadioButton.IsChecked == true ||
            voicetrackRadioButton.IsChecked == true ||
            othertrackRadioButton.IsChecked == true;
        private TrackType getTrackType()
        {
            if (songRadioButton.IsChecked == true)
            {
                return TrackType.Song;
            }
            else if (commercialRadioButton.IsChecked == true)
            {
                return TrackType.Commercial;
            }
            if (jingleRadioButton.IsChecked == true)
            {
                return TrackType.Jingle;
            }
            if (sweeperRadioButton.IsChecked == true)
            {
                return TrackType.Sweeper;
            }
            if (voicetrackRadioButton.IsChecked == true)
            {
                return TrackType.Voicetrack;
            }
            if (othertrackRadioButton.IsChecked == true)
            {
                return TrackType.Other;
            }
            return TrackType.Other;
        }
    }
}
