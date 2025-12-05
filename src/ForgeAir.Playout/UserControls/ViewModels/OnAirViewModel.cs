    using Caliburn.Micro;
    using ColorThiefDotNet;
    using ForgeAir.Core.DTO;
    using ForgeAir.Core.Events;
    using ForgeAir.Core.Services.AudioPlayout.Interfaces;
    using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
    using ForgeAir.Database.Models;
using ForgeAir.Playout.Events;
using ForgeAir.Playout.Models;
    using ForgeAir.Playout.ViewModels;
    using ForgeAir.Playout.ViewModels.Helpers;
    using m3uParser;
    using Microsoft.Extensions.DependencyInjection;
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.ComponentModel;
    using System.Drawing;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using System.Timers;
    using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using static System.Formats.Asn1.AsnWriter;

    namespace ForgeAir.Playout.UserControls.ViewModels
    {
        public class OnAirViewModel : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;
            private readonly IEventAggregator _events;
            private void OnPropertyChanged(string propertyName) =>
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

            private System.Timers.Timer _elapsedTimer;
            private System.Timers.Timer _remainingTimer;


       

            public async void OpenAutoFlyout()
            {
                 await _events.PublishOnUIThreadAsync(new OpenAutoFlyoutMessage());
            }
            private TileModel _selectorTile;
            public TileModel SelectorTile
            {
                get
                {
                    return _selectorTile;
                }
                set
                {
                    _selectorTile = value;
                    OnPropertyChanged(nameof(SelectorTile));
                }
            }
        private GradientStop _stop0;
        public GradientStop Stop0
        {
            get
            {
                return _stop0;
            }
            set
            {
                _stop0 = value;
                OnPropertyChanged(nameof(Stop0));
            }
        }
        private GradientStop _stop1;
        public GradientStop Stop1
        {
            get
            {
                return _stop1;
            }
            set
            {
                _stop1 = value;
                OnPropertyChanged(nameof(Stop1));
            }
        }
            private string _remainingTime;
            public string RemainingTime
            {
                get
                {
                    return _remainingTime;
                }
                set
                {
                    if (_remainingTime != value)
                    {
                        _remainingTime = value;
                        OnPropertyChanged(nameof(RemainingTime));

                    }
                }
            }

            private string _elapsedTime;
            public string ElapsedTime
            {
                get
                {
                    return _elapsedTime;
                }
                set
                {
                    if (_elapsedTime != value)
                    {
                        _elapsedTime = value;
                        OnPropertyChanged(nameof(ElapsedTime));

                    }
                }
            }
            private BitmapImage _currentImage;
            public BitmapImage CurrentImage
            {
                get
                {
                     return _currentImage;
                }
                set
                {
                    if (_currentImage != value)
                    {
                        _currentImage = value;
                        OnPropertyChanged(nameof(CurrentImage));

                    }
                }
            }

            private System.Windows.Media.Color _firstColor;
            public System.Windows.Media.Color FirstColor
            {
                get
                {
                    return _firstColor;
                }
                set
                {
                    if (_firstColor != value)
                    {
                        _firstColor = value;
                        OnPropertyChanged(nameof(FirstColor));

                    }
                }
            }

            private System.Windows.Media.Color _lastColor;
            public System.Windows.Media.Color LastColor
            {
                get
                {
                    return _lastColor;
                }
                set
                {
                    if (_lastColor != value)
                    {
                        _lastColor = value;
                        OnPropertyChanged(nameof(LastColor));

                    }
                }
            }

            private IServiceProvider _serviceProvider;
            private IAudioService _audioService;
            private TrackDTO _currentTrack;
            public TrackDTO CurrentTrack
            {
                get => _currentTrack;
                set
                {
                    if (_currentTrack != value)
                    {
                        _currentTrack = value;
                        OnPropertyChanged(nameof(CurrentTrack));
                    OnPropertyChanged(nameof(DisplayTrackDuration));
                }
                }
            }

        public string DisplayTrackDuration
        {
            get
            {
                if (_currentTrack == null)
                    return "";

                if (_currentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
                    return "";

                return _currentTrack.DisplayDuration;
            }
        }


        private ColorThief colorThief;
            public OnAirViewModel(IServiceProvider serviceProvider, IEventAggregator events, TrackChangedEvent trackChanged)
            {
                _serviceProvider = serviceProvider;
                _events = events;
                _audioService = _serviceProvider.GetRequiredService<IAudioService>();
                colorThief = new ColorThief();
                Stop0 = new GradientStop();
                Stop1 = new GradientStop();
                _elapsedTimer = new System.Timers.Timer()
                {
                    Enabled = true,
                    Interval = 1000
                };
                _selectorTile = new TileModel()
                {
                    Color = new SolidColorBrush() { Color = Colors.Purple },
                    Icon = MahApps.Metro.IconPacks.PackIconRemixIconKind.Robot2Fill,
                    Title = "Track Selector",
                    Command = new RelayCommand(OpenAutoFlyout)
                };
                _elapsedTimer.Elapsed += _elapsedTimer_Elapsed;
                _remainingTimer = new System.Timers.Timer()
                {
                    Enabled = true,
                
                    Interval = 1000
                };
                _remainingTimer.Elapsed += _remainingTimer_Elapsed;

                trackChanged.TrackChanged += OnTrackChanged;
                if (trackChanged.CurrentTrack != null)
                {
                    OnTrackChanged(trackChanged.CurrentTrack);
                }
            }

            private void _elapsedTimer_Elapsed(object? sender, ElapsedEventArgs e)
            {
                if (CurrentTrack == null)
                {
                    return;
                }
                ElapsedTime = Task.Run(() => _audioService.ElapsedTime()).Result.ToString("hh\\:mm\\:ss");
            }

            private void _remainingTimer_Elapsed(object? sender, ElapsedEventArgs e)
            {
                if (CurrentTrack == null)
                {
                    return;
                }
                if (CurrentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
                {
                    RemainingTime = Task.Run(() => _audioService.RemainingTime()).Result.ToString("hh\\:mm\\:ss");

                }
                else
                {
                    RemainingTime = "-" + Task.Run(() => _audioService.RemainingTime()).Result.ToString("hh\\:mm\\:ss");

                }
        }
        private void AnimateGradient(System.Windows.Media.Color newFirstColor, System.Windows.Media.Color newLastColor, double durationSeconds = 0.7)
        {
            // Ensure we are on UI thread
            App.Current.Dispatcher.Invoke(() =>
            {
                // Animate first stop
                var animation0 = new ColorAnimation
                {
                    To = newFirstColor,
                    Duration = TimeSpan.FromSeconds(durationSeconds),
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                Stop0.BeginAnimation(GradientStop.ColorProperty, animation0);

                // Animate second stop
                var animation1 = new ColorAnimation
                {
                    To = newLastColor,
                    Duration = TimeSpan.FromSeconds(durationSeconds),
                    EasingFunction = new SineEase { EasingMode = EasingMode.EaseInOut }
                };
                Stop1.BeginAnimation(GradientStop.ColorProperty, animation1);
            });
        }

        private List<System.Windows.Media.Color> UpdateBackgroundGradient()
            {
                if (CurrentImage == null)
                {
                    return new List<System.Windows.Media.Color>
                    {
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF252525")
                    };
                }
                List<QuantizedColor>? colors = colorThief.GetPalette(Helpers.ImageHelper.BitmapImageToBitmap(CurrentImage));

                if (colors == null || colors.Count == 0)
                {
                    return new List<System.Windows.Media.Color>
                    {
                        (System.Windows.Media.Color)System.Windows.Media.ColorConverter.ConvertFromString("#FF252525")
                    };
                }
                return new List<System.Windows.Media.Color>()
                {
                    ColorThiefToColor(colors.FirstOrDefault()),
                    ColorThiefToColor(colors.LastOrDefault()),
                };
            }
            private void OnTrackChanged(TrackDTO newTrack)
            {

                if (newTrack != null)
                {
                    CurrentTrack = newTrack;
                    CurrentImage = LoadAlbumArt();
                    var colors = UpdateBackgroundGradient();
                    AnimateGradient(colors.FirstOrDefault(), colors.LastOrDefault());


                _remainingTimer.Start();
                    _elapsedTimer.Start();
                }
                else
                {
                    CurrentTrack = new TrackDTO()
                    {
                        Title = "",
                        TrackArtists = new ObservableCollection<ArtistTrackDTO>() { },
                        Duration = TimeSpan.Zero

                    };
                    CurrentImage = new BitmapImage();
                    _remainingTimer.Stop();
                    _elapsedTimer.Stop();
                    ElapsedTime = "";
                    RemainingTime = "";
                }
            }
            public void PlaySkip()
            {
                
                _audioService.Play(true);
            }
            public void Stop() {
                _audioService.Stop();
            }
            public void GoToStart()
            {
                
            }
            public void GoToIntro()
            {

            }
            public void GoToHookIn()
            {

            }
            public void GoToHookOut()
            {

            }
            public void GoToOutro()
            {

            }
            public void GoToEnd()
            {

            }
            public void Pause()
            {
                _audioService.Pause();
            }
            public static System.Windows.Media.Color ColorThiefToColor(QuantizedColor color)
            {
                return System.Windows.Media.Color.FromRgb(color.Color.R, color.Color.G, color.Color.B);
            }
            private BitmapImage LoadAlbumArt()
            {
                BitmapImage albumImage = new BitmapImage();

                try
                {
                    albumImage.BeginInit();

                    double w = 150.0;
                    double h = 150.0;

                    albumImage.DecodePixelWidth = Convert.ToInt32(w);
                    albumImage.DecodePixelHeight = Convert.ToInt32(h);

                    albumImage.CacheOption = BitmapCacheOption.OnLoad;

                    if (CurrentTrack.AlbumArt != null)
                    {
                        albumImage.StreamSource = CurrentTrack.AlbumArt;
                    }
                    else
                    {
                        albumImage.UriSource = new Uri("pack://application:,,,/ForgeAir.UI.Core;component/Assets/Icons/playout/nocover_fallback.png");
                    }
                }
                catch (Exception)
                {
                    albumImage.UriSource = new Uri("pack://application:,,,/ForgeAir.UI.Core;component/Assets/Icons/playout/nocover_fallback.png");
                }
                finally
                {
                    albumImage.EndInit(); //todo: check why it is throwing exceptions
                    albumImage.Freeze(); 
                }
                return albumImage;

            }
        }
    }
