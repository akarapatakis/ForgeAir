using Caliburn.Micro;
using ForgeAir.Playout.App.Bootstrappers;
using ForgeAir.Playout.App.Helpers;
using ForgeAir.Playout.App.Models;
using ForgeAir.Playout.ViewModels.Helpers;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ForgeAir.Playout.App.ViewModels
{
    public class StationSelectorViewModel : Screen, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        private readonly IServiceProvider _provider;
        private readonly IWindowManager _windowManager;

        public ICommand Command { get; }
        public StationBootstrapper SelectedStation { get; set; }

        public ObservableCollection<StationBootstrapper> Stations
        {
            get => Instances.StationsInstance.Instance.Stations;
        }


        public void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        private async void ExecuteSelectStation(StationBootstrapper station)
        {
            await station.ShowShellViewAsync();
            await this.TryCloseAsync();

        }
        public StationSelectorViewModel(IServiceProvider provider, IWindowManager windowManager) {
            _provider = provider;
            _windowManager = windowManager;
            OnPropertyChanged(nameof(Stations));
            Command = new RelayCommand<StationBootstrapper>(ExecuteSelectStation);
        }

    }
}
