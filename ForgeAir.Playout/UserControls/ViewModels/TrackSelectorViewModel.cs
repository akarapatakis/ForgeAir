using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Caliburn.Micro;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Models;
using ForgeAir.Core.Services.Database;
using ForgeAir.Core.Services.Managers;

namespace ForgeAir.Playout.UserControls.ViewModels
{
    public class TrackSelectorViewModel : Screen, IDisposable
    {
        private readonly AppState _appState;

        public TrackSelectorViewModel(AppState appState)
        {
            _appState = appState;
            _appState.PropertyChanged += OnAppStatePropertyChanged;
        }

        private void OnAppStatePropertyChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(AppState.CurrentTrack))
            {
                // Update ViewModel property bound to UI
                CurrentTrack = _appState.CurrentTrack;
            }
        }

        private TrackDTO _currentTrack;
        public TrackDTO CurrentTrack
        {
            get => _currentTrack;
            set
            {
                _currentTrack = value;
                NotifyOfPropertyChange(() => CurrentTrack);
            }
        }

        public void Dispose()
        {
            _appState.PropertyChanged -= OnAppStatePropertyChanged;
        }
    }


}
