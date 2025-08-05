using Caliburn.Micro;
using ForgeAir.Core.Services.Database;
using ForgeAir.Database;
using ForgeAir.Database.Models;
using ForgeAir.Playout.Helpers;
using ForgeAir.Playout.Models;
using ForgeAir.Playout.Properties;
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
using System.Windows.Media;

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
        public string StationName
        {
            get { return _stationName; }
            set
            {
                _stationName = value;
                NotifyOfPropertyChange(() => StationName);
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

        public ActivityCenterViewModel(IServiceProvider provider)
        {
            _provider = provider;
            DataList = GetCardDataList();
            getStationName();
        }

        public void SetShellViewModel(ShellViewModel shell)
        {
            _shell = shell;
        }
        internal ObservableCollection<TileModel> GetCardDataList()
        {
            var playout = _provider.GetRequiredService<PlayoutViewModel>();
            var settings = _provider.GetRequiredService<SettingsViewModel>();
            
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
                },
                new TileModel
                {
                    Title = "Logout",
                    Icon = PackIconRemixIconKind.LogoutBoxFill,
                    Color = new SolidColorBrush(Colors.Red),
                },
            };
        }
        public IList<TileModel> DataList { get => _dataList; set => _dataList = value; }
    }
}
