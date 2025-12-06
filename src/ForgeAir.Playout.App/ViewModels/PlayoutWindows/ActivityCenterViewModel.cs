using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Events;
using ForgeAir.Core.Services.AudioPlayout.Interfaces;
using ForgeAir.Core.Services.AudioPlayout.Players.Interfaces;
using ForgeAir.Core.Services.Database;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.App.Helpers;
using ForgeAir.Playout.App.Models;
using ForgeAir.Playout.UserControls;
using ForgeAir.Playout.UserControls.ViewModels;
using ForgeAir.Playout.ViewModels.Helpers;
using MahApps.Metro.IconPacks;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Media;
using Avalonia.Threading;
using ForgeAir.Playout.App.ViewModels.PlayoutWindows;

namespace ForgeAir.Playout.ViewModels.PlayoutWindows
{
    public class ActivityCenterViewModel : TabItemViewModelBase
    {
        private IList<TileModel> _dataList;
        public override string Title => "Κέντρο Έναρξης";
        private readonly IServiceProvider _provider;
        private ICommand TileClickCommand { get; }
        public override bool Closeable => false;
        private ShellViewModel _shell;
        private string _stationName;
        //todo: fix
        //public StereoVUMeterControl VUMeters;
        private IAudioService AudioService;

        private DispatcherTimer vuTimer;
        public string StationName
        {
            get { return _stationName; }
            set
            {
                _stationName = value;
                NotifyOfPropertyChange(() => StationName);
            }
        }
        private double _leftLevel;
        public double LeftLevel
        {
            get => _leftLevel;
            set => Set(ref _leftLevel, value);
        }

        private double _rightLevel;
        public double RightLevel
        {
            get => _rightLevel;
            set => Set(ref _rightLevel, value);
        }

        private string _currentPlayingText;
        public string CurrentPlayingText
        {
            get => _currentPlayingText;
            set
            {
                _currentPlayingText = value;
                NotifyOfPropertyChange(() =>  CurrentPlayingText);
            }
        }
        private void getStationName()
        {
            var dbFactory = _provider.GetRequiredService<IDbContextFactory<ForgeAirDbContext>>();
            using var context = dbFactory.CreateDbContext();

            var station = context.Stations.AsNoTracking().FirstOrDefault();
            if (station != null)
                StationName = $"{station.Name}"
                            + (!string.IsNullOrWhiteSpace(station.Slogan) ? $" - {station.Slogan}" : "")
                            + (!string.IsNullOrWhiteSpace(station.Website) ? $" ({station.Website})" : "");
        }

        private void OnTrackChanged(TrackDTO newTrack)
        {
            if (newTrack == null)
            {
                CurrentPlayingText = "Nothing On Air!";
                return;
            }
            CurrentPlayingText = "On Air: " + (!string.IsNullOrWhiteSpace(newTrack.DisplayArtists) ? $"{newTrack.DisplayArtists} - " : "") + newTrack.Title + $" (ID: {newTrack.Id})";
        }
        public ActivityCenterViewModel(IWindowManager windowManager,TestViewModel testViewModel, IServiceProvider provider, TrackChangedEvent TrackChangedEvent, StationInformationChangedEvent stationInformationChangedEvent)
        {
            _provider = provider;
            DataList = GetCardDataList();
            getStationName();
            stationInformationChangedEvent.StationUpdated += StationInformationChanged;
            TrackChangedEvent.TrackChanged += OnTrackChanged;
            if (TrackChangedEvent.CurrentTrack != null)
            {
                OnTrackChanged(TrackChangedEvent.CurrentTrack);
            }
            AudioService = _provider.GetRequiredService<IAudioService>();
            windowManager.ShowWindowAsync(new TestViewModel(AudioService));
            vuTimer = new DispatcherTimer();
            vuTimer.Interval = TimeSpan.FromMilliseconds(50);
            vuTimer.Tick += vuMeter_Update;
            vuTimer.IsEnabled = true;
            vuTimer.Start();
        }
        private void StationInformationChanged(Station newStation)
        {
            getStationName();
        }
        private void vuMeter_Update(object sender, EventArgs e)
        {
            var data = AudioService.GetLevels();

            LeftLevel = data.FirstOrDefault() * 100;
            RightLevel = data.LastOrDefault() * 100;

        }
        public void SetShellViewModel(ShellViewModel shell)
        {
            _shell = shell;
        }
        internal ObservableCollection<TileModel> GetCardDataList()
        {
            var playout = _provider.GetRequiredService<PlayoutViewModel>();
            var settings = _provider.GetRequiredService<SettingsViewModel>();
            var trackLibrary = _provider.GetRequiredService<TrackLibraryViewModel>();
            return new ObservableCollection<TileModel>
            {
                new TileModel
                {
                    Title = playout.Title,
                    Icon = PackIconRemixIconKind.RhythmFill,
                    Color = new SolidColorBrush(Colors.Orange),
                    Command = new RelayCommand(() =>
                    {
                        if (_shell == null)
                            return;


                        _shell.OpenTab(playout);
                    }),
                },
                new TileModel
                {
                    Title = settings.Title,
                    Icon = PackIconRemixIconKind.Settings2Fill,
                    Color = new SolidColorBrush(Colors.SandyBrown),
                    Command = new RelayCommand(() =>
                    {
                        if (_shell == null)
                            return;


                        _shell.OpenTab(settings);
                        
                    }),
                },
                new TileModel
                {
                    Title = "Import Track",
                    Icon = PackIconRemixIconKind.ImportFill,
                    Color = new SolidColorBrush(Colors.BlueViolet),
                    Command = new RelayCommand(() =>
                    {
                        settings.AddTrack();
                    }),
                },
                new TileModel
                {
                    Title = "Track Library",
                    Icon = PackIconRemixIconKind.Archive2Fill,
                    Color = new SolidColorBrush(Colors.DarkOliveGreen),
                    Command = new RelayCommand(() =>
                    {
                           if (_shell == null)
                            return;

                        _shell.OpenTab(trackLibrary);

                    }),
                },
                new TileModel
                {
                    Title = "Logs",
                    Icon = PackIconRemixIconKind.InfoI,
                    Color = new SolidColorBrush(Colors.RosyBrown),
                },
                new TileModel
                {
                    Title = "Web Browser",
                    Icon = PackIconRemixIconKind.GlobalFill,
                    Color = new SolidColorBrush(Colors.DarkCyan),
                },
            };
        }
        public IList<TileModel> DataList { get => _dataList; set => _dataList = value; }
    }
}
