using System;
using System.Collections.Generic;
using System.IO;
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
using ForgeAir.Core.DTO;
using ForgeAir.Core.Services;
using ForgeAir.Core.Services.Tags;
using ForgeAir.Database.Models;


namespace ForgeAir.Playout.Views.Settings
{
    /// <summary>
    /// Interaction logic for TrackEditorView.xaml
    /// </summary>
    public partial class TrackEditorView : HandyControl.Controls.Window
    {
        Track track;
        public TrackEditorView(Track trackIn)
        {
            InitializeComponent();
            if (trackIn == null || trackIn.FilePath == null || trackIn.FilePath == "") {
                return;
            }

            if (trackIn.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
            {
                isrcBox.IsEnabled = false;
                albumBox.IsEnabled = false;
                artistsBox.IsEnabled = false;
                categoriesBox.IsEnabled = false;
                enabledbox.IsEnabled = false;
            }
            track = trackIn;
            coverblur.Source = LoadAlbumArt();
            groupBox.Header = $"Edit {track.FilePath}";

            if (track.TrackArtists != null)
            {
                foreach (var artist in track.TrackArtists.ToArray())
                {
                    artistsBox.Items.Add(artist.Artist.Name     );
                }
            }
            if (track.Categories != null) {
                foreach (var category in track.Categories.ToArray())
                {
                    artistsBox.Items.Add(category.Name);
                }
            }

            titleBox.Text = track.Title;
            albumBox.Text = track.Album;
            isrcBox.Text = track.ISRC;
            switch (trackIn.TrackStatus) { 
                case Database.Models.Enums.TrackStatus.Disabled:
                    enabledbox.IsChecked = false;
                    break;
                case Database.Models.Enums.TrackStatus.Enabled:
                    enabledbox.IsChecked = true; 
                    
                    break;
            }
        }
        private BitmapImage LoadAlbumArt()
        {
            try
            {
                TagService tagReader = new TagService(TrackDTO.FromEntity(track));
                BitmapImage albumImage = new BitmapImage();
                albumImage.CacheOption = BitmapCacheOption.OnLoad;
                albumImage.BeginInit();
                var imagetag = tagReader.GetPicture();
                if (imagetag != null)
                {
                    albumImage.StreamSource = new MemoryStream(tagReader.GetPicture().Data?.Data); 
                }
                else
                {
                    var fallbackImage = new BitmapImage(new Uri("pack://application:,,,/ForgeAir.UI.Core;component/Assets/Icons/playout/nocover_fallback.png"));
                    albumImage.UriSource = fallbackImage.UriSource;
                }

                albumImage.EndInit();
                albumImage.Freeze();
                return albumImage;
            }
            catch (Exception)
            {
                var fallbackImage = new BitmapImage(new Uri("pack://application:,,,/ForgeAir.UI.Core;component/Assets/Icons/playout/nocover_fallback.png"));
                return fallbackImage;
            }
        }

        private void addButton_Click(object sender, RoutedEventArgs e)
        {
            if (track.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
            {
                track.Title = titleBox.Text;
            }
            track.DateModified = DateTime.UtcNow;
        }

        private void cancelButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
