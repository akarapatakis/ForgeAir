using ForgeAir.Core.Helpers;
using ForgeAir.Core.Services;
using ForgeAir.Core.Services.Importers;
using ForgeAir.Core.Services.Importers.Interfaces;
using ForgeAir.Core.Services.Models;
using ForgeAir.Core.Shared;
using ForgeAir.Core.Tracks;
using ForgeAir.Core.Tracks.Enums;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Database.Models.Enums;
using HandyControl.Controls;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using MessageBox = HandyControl.Controls.MessageBox;
using Track = ForgeAir.Database.Models.Track;

namespace ForgeAir.Playout.Views
{
    /// <summary>
    /// Interaction logic for ImportTrackFileView.xaml
    /// </summary>
    public partial class ImportTrackFileView : HandyControl.Controls.GlowWindow
    {
        Microsoft.Win32.OpenFileDialog openFileDialog = new Microsoft.Win32.OpenFileDialog();
        string filename;
        private readonly TrackImporter trackImporter = new TrackImporter();
        public ImportTrackFileView()
        {
            openFileDialog.CheckFileExists = true;
            openFileDialog.Multiselect = false;
            openFileDialog.CheckPathExists = true;
            InitializeComponent();
        }


        private void dirSelect_Click(object sender, RoutedEventArgs e)
        {
            if (openFileDialog.ShowDialog() == false)
            {
                return;
            }
            else { filename = openFileDialog.FileName; fileDirBox.Text = openFileDialog.FileName; }
        }
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
        private bool IsTrackTypeSelected() =>
    songRadioButton.IsChecked == true ||
    commercialRadioButton.IsChecked == true ||
    jingleRadioButton.IsChecked == true ||
    sweeperRadioButton.IsChecked == true ||
    voicetrackRadioButton.IsChecked == true ||
    othertrackRadioButton.IsChecked == true;
        private async void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (filename == null) { return; }
            if (IsTrackTypeSelected() == false) {
                HandyControl.Controls.MessageBox.Show("Please Select Track Type.", "Error", MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var generalHelpers = new Core.Helpers.GeneralHelpers();
            if (!generalHelpers.isThisAnAudioFileSimple(filename))
                MessageBox.Show("Invalid Track.");
            double crossfade = crossFadeUpDown.Value;
            var type = getTrackType();
            Track track = await Task.Run(() => trackImporter.createTrackAsync(filename, type, TimeSpan.FromSeconds(Convert.ToDouble(crossfade))));

            DatabaseSharedData.Instance.RaiseDBModified();
        }

        
    }
}
