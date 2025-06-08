using ForgeAir.Core.AudioEngine;
using ForgeAir.Core.CustomDataTypes;
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

        public int currentMainBassMixerHandle { get; set; }

        private AudioPlayerShared() { }
        
        public AudioPlayer audioPlayer { get; set; }

        public bool repeatTrack { get; set; }
        public int crossfadeTimeInMs { get; set; }
        public int fadeNextTimeInMs { get; set; }
        public int stopFadeTimeInMs { get; set; }

        public MemoryStream? currentAlbumArt { get; set; }

        public int vstHandle { get; set; }

        public Database.Models.Track currentTrack { get; set; }

        public LinkedListQueue<Database.Models.Track> trackQueue { get; set; }

        public event EventHandler? onTrackChanged;
        public event EventHandler? onTrackChangedUI;

        public event EventHandler? onQueueChanged;
        public event EventHandler? updateQueueUI;

        public event EventHandler? onStopped;

        public void RaiseOnTrackChanged() { onTrackChanged.Invoke(this, EventArgs.Empty); onTrackChangedUI?.Invoke(this, EventArgs.Empty); updateQueueUI?.Invoke(this, EventArgs.Empty); VideoOutputShared.Instance.RaiseOnTrackChanged(); }

        public void RaiseOnQueueChanged() { onQueueChanged.Invoke(this, EventArgs.Empty); updateQueueUI?.Invoke(this, EventArgs.Empty); }

        public void RaiseOnStopped() { onStopped.Invoke(this, EventArgs.Empty);}
        public DeviceManager deviceManager { get; set; }


        public int MainleftChannel { get; set; }
        public int MainrightChannel { get; set; }


        
        public int MainChannelsCombined { get; set; }

        public bool fixClickingWorkaround { get; set; }

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
