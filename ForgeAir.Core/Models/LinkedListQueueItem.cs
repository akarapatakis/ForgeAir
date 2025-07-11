using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Models
{
    class LinkedListQueueItem : INotifyPropertyChanged
    {
        public required int Place { get; set; }
        public required DTO.TrackDTO Track { get; set; }

        public string DisplayTitle => Track.Title;


        private Brush _background;
        public Brush Background
        {
            get => _background;
            set { _background = value; OnPropertyChanged(nameof(Background)); }
        }

        private Brush _foreground;

        public event PropertyChangedEventHandler? PropertyChanged;

        public Brush Foreground
        {
            get => _foreground;
            set { _foreground = value; OnPropertyChanged(nameof(Foreground)); }
        }
        protected void OnPropertyChanged([CallerMemberName] string? name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }
    }
}
