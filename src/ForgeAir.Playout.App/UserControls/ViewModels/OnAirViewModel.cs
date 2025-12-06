using Avalonia;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Threading;
using Caliburn.Micro;
using ColorThiefDotNet;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Database.Models;
using ForgeAir.Playout.ViewModels.Helpers;
using m3uParser;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Timers;
using ForgeAir.Playout.App.Events;
using ForgeAir.Playout.App.Models;
using Color = System.Drawing.Color;

namespace ForgeAir.Playout.UserControls.ViewModels
{
    public class OnAirViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private void OnPropertyChanged(string propertyName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));

        private Timer _elapsedTimer;
        private Timer _remainingTimer;

        private readonly IEventAggregator _events;
        private IServiceProvider _serviceProvider;
        private IAudioService _audioService;

        private ColorThief colorThief;

        public OnAirViewModel(IServiceProvider serviceProvider, IEventAggregator events, TrackChangedEvent trackChanged)
        {
            _serviceProvider = serviceProvider;
            _events = events;
            _audioService = _serviceProvider.GetRequiredService<IAudioService>();
            colorThief = new ColorThief();

            Stop0 = new GradientStop();
            Stop1 = new GradientStop();

            _elapsedTimer = new Timer
            {
                Enabled = true,
                Interval = 1000
            };
            _elapsedTimer.Elapsed += _elapsedTimer_Elapsed;

            _remainingTimer = new Timer
            {
                Enabled = true,
                Interval = 1000
            };
            _remainingTimer.Elapsed += _remainingTimer_Elapsed;

            _selectorTile = new TileModel()
            {
                Color = new SolidColorBrush(Colors.Purple),
                Icon = MahApps.Metro.IconPacks.PackIconRemixIconKind.Robot2Fill,
                Title = "Track Selector",
                Command = new RelayCommand(OpenAutoFlyout)
            };

            trackChanged.TrackChanged += OnTrackChanged;
            if (trackChanged.CurrentTrack != null)
            {
                OnTrackChanged(trackChanged.CurrentTrack);
            }
        }

        public async void OpenAutoFlyout()
        {
            await _events.PublishOnUIThreadAsync(new OpenAutoFlyoutMessage());
        }

        private TileModel _selectorTile;
        public TileModel SelectorTile
        {
            get => _selectorTile;
            set { _selectorTile = value; OnPropertyChanged(nameof(SelectorTile)); }
        }

        private GradientStop _stop0;
        public GradientStop Stop0
        {
            get => _stop0;
            set { _stop0 = value; OnPropertyChanged(nameof(Stop0)); }
        }

        private GradientStop _stop1;
        public GradientStop Stop1
        {
            get => _stop1;
            set { _stop1 = value; OnPropertyChanged(nameof(Stop1)); }
        }

        private string _remainingTime;
        public string RemainingTime
        {
            get => _remainingTime;
            set { if (_remainingTime != value) { _remainingTime = value; OnPropertyChanged(nameof(RemainingTime)); } }
        }

        private string _elapsedTime;
        public string ElapsedTime
        {
            get => _elapsedTime;
            set { if (_elapsedTime != value) { _elapsedTime = value; OnPropertyChanged(nameof(ElapsedTime)); } }
        }

        private Bitmap _currentImage;
        public Bitmap CurrentImage
        {
            get => _currentImage;
            set { if (_currentImage != value) { _currentImage = value; OnPropertyChanged(nameof(CurrentImage)); } }
        }

        private Color _firstColor;
        public Color FirstColor
        {
            get => _firstColor;
            set { if (_firstColor != value) { _firstColor = value; OnPropertyChanged(nameof(FirstColor)); } }
        }

        private Color _lastColor;
        public Color LastColor
        {
            get => _lastColor;
            set { if (_lastColor != value) { _lastColor = value; OnPropertyChanged(nameof(LastColor)); } }
        }

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
                if (_currentTrack == null) return "";
                if (_currentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast) return "";
                return _currentTrack.DisplayDuration;
            }
        }

        private void _elapsedTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (CurrentTrack == null) return;
            var elapsed = Task.Run(() => _audioService.ElapsedTime()).Result;
            Dispatcher.UIThread.Post(() => ElapsedTime = elapsed.ToString("hh\\:mm\\:ss"));
        }

        private void _remainingTimer_Elapsed(object? sender, ElapsedEventArgs e)
        {
            if (CurrentTrack == null) return;

            var remaining = Task.Run(() => _audioService.RemainingTime()).Result;
            if (CurrentTrack.TrackType == Database.Models.Enums.TrackType.Rebroadcast)
                Dispatcher.UIThread.Post(() => RemainingTime = remaining.ToString("hh\\:mm\\:ss"));
            else
                Dispatcher.UIThread.Post(() => RemainingTime = "-" + remaining.ToString("hh\\:mm\\:ss"));
        }

        private void AnimateGradient(Color newFirstColor, Color newLastColor, double durationSeconds = 0.7)
        {
            //todo:fix animation
            FirstColor = newFirstColor;
            LastColor = newLastColor;
        }

        private List<Color> UpdateBackgroundGradient()
        {
            //todo: modernize colorthief.net
            /*
            if (CurrentImage == null)
                return new List<Color> { System.Drawing.ColorTranslator.FromHtml("#FF252525") };

            var colors = colorThief.GetPalette(ForgeAir.Playout.App.Helpers.ImageHelper.BitmapToBitmap(CurrentImage));
            if (colors == null || colors.Count == 0)
                return new List<Color> { System.Drawing.ColorTranslator.FromHtml("#FF252525") };

            return new List<Color>
            {
                ColorThiefToColor(colors.FirstOrDefault()),
                ColorThiefToColor(colors.LastOrDefault())
            };*/
            
            return new List<Color>
            {
                System.Drawing.ColorTranslator.FromHtml("#FF252525")
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
                CurrentTrack = new TrackDTO
                {
                    Title = "",
                    TrackArtists = new ObservableCollection<ArtistTrackDTO>(),
                    Duration = TimeSpan.Zero
                };
                CurrentImage = null;
                _remainingTimer.Stop();
                _elapsedTimer.Stop();
                ElapsedTime = "";
                RemainingTime = "";
            }
        }

        public void PlaySkip() => _audioService.Play(true);
        public void Stop() => _audioService.Stop();
        public void Pause() => _audioService.Pause();

        public static Color ColorThiefToColor(QuantizedColor color) =>
            Color.FromArgb(color.Color.R, color.Color.G, color.Color.B);

        private Bitmap LoadAlbumArt()
        {
            try
            {
                if (CurrentTrack.AlbumArt != null)
                    return new Bitmap(CurrentTrack.AlbumArt);
                else
                    return new Bitmap("Assets/Icons/playout/nocover_fallback.png");
            }
            catch
            {
                return new Bitmap("Assets/Icons/playout/nocover_fallback.png");
            }
        }
    }
}
