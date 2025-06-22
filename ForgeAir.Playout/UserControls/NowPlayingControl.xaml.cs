using ColorThiefDotNet;
using ForgeAir.Core.Shared;
using System;
using System.Collections.Generic;
using System.Drawing;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using ForgeAir.UI.Core;
using ManagedBass;
using Microsoft.Win32;
using System.Timers;
using Microsoft.EntityFrameworkCore.ChangeTracking.Internal;
using System.Windows.Threading;

namespace ForgeAir.Playout.UserControls
{
    /// <summary>
    /// Interaction logic for NowPlayingControl.xaml
    /// </summary>
    public partial class NowPlayingControl : System.Windows.Controls.UserControl, IDisposable
    {
        private BitmapImage bitmap;
        private DispatcherTimer updateSeekTimer;
        private TimeSpan currentTime;
        private bool _isDragging;

        private double boxRenderedW = double.NaN;
        private double boxRenderedH = double.NaN;
        public static System.Windows.Media.Color ColorThiefToColor(QuantizedColor color)
        {
            return System.Windows.Media.Color.FromRgb(color.Color.R, color.Color.G, color.Color.B);
        }

        private BitmapImage LoadAlbumArt()
        {
            try
            {
                double w = 0.0;
                double h = 0.0;
                this.Dispatcher.Invoke(() =>
                {
                    if (double.IsNaN(boxRenderedW) || double.IsNaN(boxRenderedH))
                    {
                        w = 200.0;
                        h = 200.0;
                    }
                    else
                    {
                        w = trackImage.Width;
                        h = trackImage.Height;
                    }
                });

                BitmapImage albumImage = new BitmapImage();
                albumImage.DecodePixelWidth = Convert.ToInt32(w);
                albumImage.DecodePixelHeight = Convert.ToInt32(h);
                albumImage.CacheOption = BitmapCacheOption.OnLoad;
                albumImage.BeginInit();

                if (AudioPlayerShared.Instance.currentAlbumArt != null)
                {
                    albumImage.StreamSource = AudioPlayerShared.Instance.currentAlbumArt;
                }
                else
                {
                    var fallbackImage = new BitmapImage(new Uri("pack://application:,,,/ForgeAir.UI.Core;component/Assets/Icons/playout/nocover_fallback.png"));
                    albumImage.UriSource = fallbackImage.UriSource;
                }

                albumImage.EndInit(); //todo: check why it is throwing exceptions
                albumImage.Freeze();
                return albumImage;
            }
            catch (Exception)
            {
                var fallbackImage = new BitmapImage(new Uri("pack://application:,,,/ForgeAir.UI.Core;component/Assets/Icons/playout/nocover_fallback.png"));
                return fallbackImage;
            }
        }

        public NowPlayingControl()
        {
            InitializeComponent();

            AudioPlayerShared.Instance.onTrackChangedUI += OnTrackChanged;
            updateSeekTimer = new DispatcherTimer();
            updateSeekTimer.Interval = TimeSpan.FromMilliseconds(1000);
            updateSeekTimer.Tick += UpdateElementsfromTimer;
            
        }

        public void UpdateElementsfromTimer(object? sender, EventArgs e)
        {

            Task.Run(() => _UpdateElementsfromTimer());
            this.Dispatcher.Invoke(() =>
            {
                trackslider.Value = currentTime.TotalSeconds;
                elapsedLabel.Text = currentTime.ToString(@"hh\:mm\:ss");
            });
        }

        public async Task _UpdateElementsfromTimer()
        {
           // currentTime = await AudioPlayerShared.Instance.audioPlayer.GetElapsedTime();
        }
        public void OnTrackChanged(object sender, EventArgs e)
        {
            if (AudioPlayerShared.Instance.currentTrack == null)
            {
               this.Dispose();
                return;
            }


            var colorThief = new ColorThief();
            bitmap = LoadAlbumArt();

            // Get the dominant color palette from the album art
            List<QuantizedColor> colors = colorThief.GetPalette(BitmapImageToBitmap(bitmap));

            updateSeekTimer.Start();

            // Update UI elements on the UI thread
            this.Dispatcher.Invoke(async () =>
            {
                trackImage.Source = null;
                titleLabel.Text = AudioPlayerShared.Instance.currentTrack.Title;
                artistLabel.Text = AudioPlayerShared.Instance.currentTrack.DisplayArtists;
                durationLabel.Text = AudioPlayerShared.Instance.currentTrack.Duration.ToString(@"hh\:mm\:ss");
              //  elapsedLabel.Text = AudioPlayerShared.Instance.audioPlayer.GetElapsedTime().Result.ToString(@"hh\:mm\:ss");
                durationLabel.Visibility = Visibility.Visible;
                elapsedLabel.Visibility = Visibility.Visible;
                trackImage.Source = bitmap;
                trackTypeLabel.Text = AudioPlayerShared.Instance.currentTrack.DisplayType;
                // hacky way to retreive the cover size in order to render bitmaps accordinglly
                boxRenderedW = trackImage.Width;
                boxRenderedH = trackImage.Height;
                if (AudioPlayerShared.Instance.currentTrack.TrackType != Database.Models.Enums.TrackType.Rebroadcast)
                {
                    trackslider.Visibility = Visibility.Visible;
                    trackslider.Minimum = 0;
                  //  trackslider.Maximum = await Task.Run(Core.Shared.AudioPlayerShared.Instance.audioPlayer.GetTrackLengthInSecs);
                    trackslider.Value = 0;
                }
                else if (AudioPlayerShared.Instance.currentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast || AudioPlayerShared.Instance.currentTrack.TrackType == null) { trackslider.Value = 0; trackslider.Visibility = Visibility.Hidden; }
                    this.Background = new LinearGradientBrush(
                        ColorThiefToColor(colors.FirstOrDefault()),
                        ColorThiefToColor(colors.LastOrDefault()),
                        new System.Windows.Point(0, 0),
                        new System.Windows.Point(1, 0)) // Horizontal gradient
                    {
                        Opacity = 0.7
                    };

            });
        }

        public static Bitmap BitmapImageToBitmap(BitmapImage bitmapImage)
        {
            if (bitmapImage == null) return null;

            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(outStream);
                outStream.Seek(0, SeekOrigin.Begin);

                using (var bmp = new Bitmap(outStream))
                {
                    return new Bitmap(bmp); 
                }
            }
        }

        private async void trackslider_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (!_isDragging) return;
            if (AudioPlayerShared.Instance.currentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
            {
                return;
            }
            await Task.Delay(500);
            double SliderValue = trackslider.Value;
            TimeSpan ts = TimeSpan.FromSeconds(SliderValue);

            await Task.Run(() =>
            {
                 AudioPlayerShared.Instance.audioPlayer.SeekTo(ts);
            });

        }

        private void trackslider_MouseDown(object sender, MouseButtonEventArgs e)
        { }


        private void trackslider_PreviewMouseDoubleClick(object sender, MouseButtonEventArgs e)
        {

        }

        private void trackslider_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerShared.Instance.currentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast || AudioPlayerShared.Instance.currentTrack == null)
            {
                return;
            }
            _isDragging = true;
            updateSeekTimer.Stop();
            
        }

        private void trackslider_PreviewMouseUp(object sender, MouseButtonEventArgs e)
        {
            if (AudioPlayerShared.Instance.currentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast || AudioPlayerShared.Instance.currentTrack == null)
            {
                return;
            }
            _isDragging = false;
            updateSeekTimer.Start();
        }

        public void Dispose() // fuck off
        {
            updateSeekTimer.Stop();
            this.Background = null;
            titleLabel.Text = null;
            artistLabel.Text = null;
            durationLabel.Text = null;
            elapsedLabel.Text = null;
            durationLabel.Text = null;
            trackTypeLabel.Text = null;
            trackslider.Value = 0;
            trackslider.Visibility = Visibility.Hidden;
            durationLabel.Visibility = Visibility.Hidden;
            elapsedLabel.Visibility = Visibility.Hidden;
            trackImage.Source = null;
        }
    }
}
