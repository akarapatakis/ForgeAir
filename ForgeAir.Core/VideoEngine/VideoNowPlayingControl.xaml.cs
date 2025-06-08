using ForgeAir.Core.Shared;
using Microsoft.EntityFrameworkCore.ValueGeneration.Internal;
using SharpDX.Mathematics.Interop;
using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unosquare.FFME;
using Unosquare.FFME.Common;

namespace ForgeAir.Core.VideoEngine
{
    /// <summary>
    /// Interaction logic for VideoNowPlayingControl.xaml
    /// </summary>
    public partial class VideoNowPlayingControl : System.Windows.Controls.UserControl
    {
        public Unosquare.FFME.MediaElement VideoPlayer => videoPlayer;
        public System.Windows.Controls.Image ChannelLogo => channelLogo;

        private string cachedSubs;

        public BitmapImage logo;
        public VideoNowPlayingControl()
        {
            InitializeComponent();
            logo = new BitmapImage();
            Unosquare.FFME.Library.EnableWpfMultiThreadedVideo = true;

            nowPlayingOverlay.DefaultBackgroundColor = System.Drawing.Color.Transparent;
            nowPlayingOverlay.Source = new Uri($"file:///{System.IO.Path.GetFullPath("overlays/flashoverlay/overlay.html").Replace("\\", "/")}");
            nowPlayingOverlay.DefaultBackgroundColor = System.Drawing.Color.FromArgb(0, 0, 0, 0);
            videoPlayer.RenderingSubtitles += VideoPlayer_RenderingSubtitles;
            if (!VideoOutputShared.Instance.showClock)
            {
                //videoClock.Visibility = Visibility.Hidden;
            }
            if (!VideoOutputShared.Instance.useLogo)
            {
                channelLogo.Visibility = Visibility.Hidden;
            } else {
                 logo = new BitmapImage(new Uri(System.IO.Path.GetFullPath(VideoOutputShared.Instance.logoPath)));
                channelLogo.Source = logo;
                channelLogo.Width = logo.Width;
                channelLogo.Height = logo.Height;
                
                channelLogo.Visibility = Visibility.Visible;
            }

            if (VideoOutputShared.Instance.stretchFourToThree)
            {
                videoPlayer.Stretch = Stretch.Fill;
            } else
            {
                videoPlayer.Stretch = Stretch.Uniform;
            }
            if (!VideoOutputShared.Instance.useOverlay)
            {
                nowPlayingOverlay.Visibility = Visibility.Hidden;
            }
            AudioPlayerShared.Instance.onStopped += StopVideo;
            VideoOutputShared.Instance.updateVideoTexts += updateTexts;
            //videoClock.textBlock_Copy.Visibility = Visibility.Hidden;
            //videoClock.Background = new SolidColorBrush(Colors.Transparent);

            Task.Run(() => { Scroll(); });
        }

        public void Scroll()
        {
            this.Dispatcher.Invoke(() => {

                double textWidth = MarqueeText.ActualWidth;
                double canvasWidth = MarqueeCanvas.ActualWidth;


                DoubleAnimation animation = new DoubleAnimation
                {
                    From = canvasWidth,
                    To = -textWidth,
                    Duration = new Duration(System.TimeSpan.FromSeconds(15)),
                    RepeatBehavior = RepeatBehavior.Forever
                };

                MarqueeText.BeginAnimation(Canvas.LeftProperty, animation);

            });
        }
        private void VideoPlayer_RenderingSubtitles(object? sender, Unosquare.FFME.Common.RenderingSubtitlesEventArgs e)
        {

            string[] originalTextSliced = e.OriginalText[0].Split(",");

            string exportedSubs = "";

            for (int i=8; 8<=originalTextSliced.Count(); i++)
            {
                if (i== originalTextSliced.Count())
                {
                    break;
                }
                if (exportedSubs == "") // τσεκ για να μη βάλει κόμμα σε κάθε γραμμή
                {
                    exportedSubs = originalTextSliced[i]; // ξέρουμε ότι είναι ένα κόμμα που μου γάμησε τη ζωή
                }
                else
                {
                    exportedSubs = exportedSubs + "," + originalTextSliced[i]; // ξέρουμε ότι είναι ένα κόμμα που μου γάμησε τη ζωή
                }

                continue;
            }
            if (exportedSubs == cachedSubs)
            {
                return;
            }
            cachedSubs = exportedSubs;
            var subtitle = exportedSubs.Split("\\N");
            int count = 0;
            foreach (var splittedSun in subtitle)
            {

                e.Text.Insert(count, splittedSun);
                count++;
            }
            


        }

        private void StopVideo(object sender, EventArgs e)
        {
            videoPlayer.Stop();
        }
        private async void UpdateNowPlaying(ForgeAir.Database.Models.Track track, string passthruArtist)
        {
            if (track != AudioPlayerShared.Instance.currentTrack)
            {
                return;
            }

            // legacy -> string script = $"updateNowPlaying('{title}', '{artist}', 10000);";
            if (track.Intro != null)
            {
                await Task.Delay(Int32.Parse(track.Intro.Value.TotalMilliseconds.ToString()));

            } else
            {
                await Task.Delay(5000);
            }

            string script = $"UpdateTrackMetadata(\"{track.Title}\", \"{passthruArtist}\",\r\n    \"{track.Album}\",\r\n    \"{track.Duration.TotalMilliseconds.ToString()}\",\r\n    \"{track.ISRC}\",\r\n    \"{track.ReleaseDate.Value.Year.ToString()}\",\r\n    \"{track.Title}\",\r\n    \"\",\r\n    5000\r\n);";
            if (track != AudioPlayerShared.Instance.currentTrack)
            {
                return;
            }


            await nowPlayingOverlay.CoreWebView2.ExecuteScriptAsync(script);
            if (track.Outro != null)
            {
                await Task.Delay((Int32.Parse(track.Outro.Value.TotalMilliseconds.ToString())) - 5000);

            }
            else
            {
                await Task.Delay((Int32.Parse(track.Duration.TotalMilliseconds.ToString())) - 10000);
            }
            if (track != AudioPlayerShared.Instance.currentTrack)
            {
                return;
            }

            await nowPlayingOverlay.CoreWebView2.ExecuteScriptAsync(script);
            return;
        }

        private void updateTexts(object sender, EventArgs e)
        {
            this.Dispatcher.Invoke(async () =>
            {
                if (VideoOutputShared.Instance.useOverlay)
                {
                    if (AudioPlayerShared.Instance.currentTrack.TrackType == Database.Models.Enums.TrackType.Song)
                    {
                        UpdateNowPlaying(AudioPlayerShared.Instance.currentTrack, AudioPlayerShared.Instance.currentTrack.DisplayArtists);
                    }

                }


            });
        }

        private void videoPlayer_MediaOpening(object sender, Unosquare.FFME.Common.MediaOpeningEventArgs e)
        {
            e.Options.IsSubtitleDisabled = false;
            e.Options.DecoderCodec.Add(0, "hwaccel_device");
        }
    }
}
