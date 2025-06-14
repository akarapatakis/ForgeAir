using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Services.Models;

namespace ForgeAir.Core.Shared
{
    public class AudioServiceShared : INotifyPropertyChanged
    {
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
                }
            }
        }

        public MemoryStream? CurrentAlbumArt { get; set; }

        public bool AutoStart { get; set; }

        public event EventHandler? OnQueueChanged;
        public event EventHandler? UpdateQueueUI;

        public void RaiseOnQueueChanged()
        {
            OnQueueChanged?.Invoke(this, EventArgs.Empty);
            UpdateQueueUI?.Invoke(this, EventArgs.Empty);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged(string propName) =>
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
    }
}
