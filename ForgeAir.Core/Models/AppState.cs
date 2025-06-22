using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using ForgeAir.Core.CustomCollections;
using ForgeAir.Core.DTO;
using ForgeAir.Core.Services.AudioPlayout.DSP.VST.Interfaces;
using ForgePlugin.Interfaces;

namespace ForgeAir.Core.Models
{
    public class AppState : INotifyPropertyChanged
    {
        public ForgeVariationsEnum Application { get; set; }

        public ICollection<OutputDevice>? InitializedDevices { get; set; }
        
        public IVSTService VSTService { get; set; }

        public LinkedListQueue<DTO.TrackDTO> TrackQueue { get; set; }

        public ICollection<IPlugin> LoadedPlugins { get; set; }

        public DatabaseCredentials DatabaseCredentials { get; set; }

        private TrackDTO _currentTrack;
        public TrackDTO CurrentTrack
        {
            get => _currentTrack;
            set
            {
                if (_currentTrack != value)
                {
                    _currentTrack = value;
                    OnPropertyChanged();
                    OnTrackChanged?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public event PropertyChangedEventHandler? PropertyChanged;


        public event EventHandler? OnTrackChanged;

        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

    }

}
