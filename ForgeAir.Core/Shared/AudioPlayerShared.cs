using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.Services.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ForgeAir.Core.Shared
{
    public class AudioPlayerShared
    {




        public MemoryStream? currentAlbumArt { get; set; }


        public DTO.TrackDTO currentTrack { get; set; }





        public event EventHandler? onQueueChanged;
        public event EventHandler? updateQueueUI;

 
        public void RaiseOnQueueChanged() { onQueueChanged.Invoke(this, EventArgs.Empty); updateQueueUI?.Invoke(this, EventArgs.Empty); }

        public bool autoStart { get; set; }

        private static AudioPlayerShared? _instance;
        private static readonly object _lock = new object();

        public static AudioPlayerShared Instance
        {
            get
            {
                lock (_lock)
                {
                    return _instance ??= new AudioPlayerShared();
                }
            }
        }


    }
}
